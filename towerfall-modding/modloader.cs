using modloader;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace PuppetMaster
{
    public class Modloader
    {

        private static string cwd = Directory.GetCurrentDirectory();

        private static HarmonyBridge b = null;

        public static object patchedAtlas = null;
        public static object patchedMenuAtlas = null;
        public static object patchedBGAtlas = null;
        public static object patchedBossAtlas = null;

        public static bool PATCHED = false;
        public static List<Type> allMods = new List<Type>();
        private static List<Mod> mods = new List<Mod>();

        public static Assembly GetTowerFallAssembly()
        {
            return UtilitiesInternal.GetAssemblyByName("TowerFall");
        }

        public static int Inject(string modloaderpath)
        {

            var client = new NamedPipeClientStream("tfmods");
            client.Connect();
            StreamWriter writer = new StreamWriter(client);

            writer.WriteLine("Process stream started...");
            writer.WriteLine("Trying to load harmony");
            Assembly.LoadFile($"{Directory.GetCurrentDirectory()}\\0Harmony.dll");
            
            // Find the Harmony Dependency in Loaded Assemblies

            b = new HarmonyBridge("com.jakesnake.modloader");

            Type MonocleEngineType = GetTowerFallAssembly().GetType("Monocle.Engine");


            Type GameTime = UtilitiesInternal.GetTypeFromAssembly("Microsoft.Xna.Framework.Game", "Microsoft.Xna.Framework.GameTime");

            // Load mods

            writer.WriteLine("Loading mods...");

            LoadMods();

            writer.WriteLine("Loaded mods!");


            // Runtime Texture Patching
            patch_textures();
            // Patch Update function. We can guarantee that will run.

            object mOriginal = AccessTools.Method(MonocleEngineType, "Update", new Type[] { GameTime }, null);

            object mPrefix = SymbolExtensions.GetMethodInfo(() => ApplyPatches());
            object mPostfix = SymbolExtensions.GetMethodInfo(() => AfterApplyPatches());

            b.Patch(mOriginal, HarmonyMethod.New(mPrefix), HarmonyMethod.New(mPostfix));
            
            writer.Flush();
            writer.Close();

            return 0;

        }

        private static void patch_textures()
        {

            if (patchedAtlas != null)
            {
                object get_AtlasOriginal = AccessTools.Method(GetTowerFallAssembly().GetType("TowerFall.TFGame"), "get_Atlas", new Type[] { }, null);
                MethodInfo get_AtlasPrefix = typeof(Modloader).GetMethod("get_Atlas", BindingFlags.NonPublic | BindingFlags.Static);
                b.Patch(get_AtlasOriginal, HarmonyMethod.New(get_AtlasPrefix), null);
            }
            if (patchedMenuAtlas != null)
            {
                object get_MenuAtlasOriginal = AccessTools.Method(GetTowerFallAssembly().GetType("TowerFall.TFGame"), "get_MenuAtlas", new Type[] { }, null);
                MethodInfo get_MenuAtlasPrefix = typeof(Modloader).GetMethod("get_MenuAtlas", BindingFlags.NonPublic | BindingFlags.Static);
                b.Patch(get_MenuAtlasOriginal, HarmonyMethod.New(get_MenuAtlasPrefix), null);
            }
            if (patchedBGAtlas != null)
            {
                object get_BGAtlasOriginal = AccessTools.Method(GetTowerFallAssembly().GetType("TowerFall.TFGame"), "get_BGAtlas", new Type[] { }, null);
                MethodInfo get_BGAtlasPrefix = typeof(Modloader).GetMethod("get_BGAtlas", BindingFlags.NonPublic | BindingFlags.Static);
                b.Patch(get_BGAtlasOriginal, HarmonyMethod.New(get_BGAtlasPrefix), null);
            }
            if (patchedAtlas != null)
            {
                object get_BossAtlasOriginal = AccessTools.Method(GetTowerFallAssembly().GetType("TowerFall.TFGame"), "get_BossAtlas", new Type[] { }, null);
                MethodInfo get_BossAtlasPrefix = typeof(Modloader).GetMethod("get_BossAtlas", BindingFlags.NonPublic | BindingFlags.Static);
                b.Patch(get_BossAtlasOriginal, HarmonyMethod.New(get_BossAtlasPrefix), null);
            }

        }

        // These next 4 functions patch the atlases to provide new textures
        private static bool get_Atlas(ref object __result)
        {

            __result = patchedAtlas;

            return false;
        }

        private static bool get_MenuAtlas(ref object __result)
        {

            __result = patchedMenuAtlas;

            return false;
        }

        private static bool get_BGAtlas(ref object __result)
        {
            __result = patchedBGAtlas;

            return false;
        }

        private static bool get_BossAtlas(ref object __result)
        {
            __result = patchedBossAtlas;

            return false;
        }

        private static void LoadMods()
        {
            try
            {
                string path = Path.GetDirectoryName($"{cwd}\\mods\\");
                Type MonocleAtlasType = GetTowerFallAssembly().GetType("Monocle.Atlas");


                foreach (string modDir in Directory.GetDirectories(path))
                {
                    if (File.Exists(modDir + "/mod.json")) {
                        try
                        {
                            var jsonReader = JsonReaderWriterFactory.CreateJsonReader(File.ReadAllBytes(modDir + "/mod.json"), XmlDictionaryReaderQuotas.Max);

                            var root = XElement.Load(jsonReader);

                            string name = root.XPathSelectElement("//name").Value;

                            Assembly assembly = Assembly.LoadFrom(modDir + "/mod.dll");

                            Mod mod = new Mod(name, assembly);

                            Directory.CreateDirectory($"{cwd}//Content//Runtime");

                            var dirName = new DirectoryInfo(modDir).Name;

                            if (root.XPathSelectElement("//atlas_image") != null)
                            {
                                patchedAtlas = Activator.CreateInstance(MonocleAtlasType, new object[] { "Atlas/atlas.xml", $"../mods/{dirName}/{root.XPathSelectElement("//atlas_image").Value}", true });
                            }
                            if (root.XPathSelectElement("//menuatlas_image") != null)
                            {
                                patchedMenuAtlas = Activator.CreateInstance(MonocleAtlasType, new object[] { "Atlas/menuAtlas.xml", $"../mods/{dirName}/{root.XPathSelectElement("//menuatlas_image").Value}", true });
                            }
                            if (root.XPathSelectElement("//bgatlas_image") != null)
                            {
                                patchedBGAtlas = Activator.CreateInstance(MonocleAtlasType, new object[] { "Atlas/bgAtlas.xml", $"../mods/{dirName}/{root.XPathSelectElement("//bgatlas_image").Value}", true });
                            }
                            if (root.XPathSelectElement("//bossatlas_image") != null)
                            {
                                patchedBossAtlas = Activator.CreateInstance(MonocleAtlasType, new object[] { "Atlas/bossAtlas.xml", $"../mods/{dirName}/{root.XPathSelectElement("//bossatlas_image").Value}", true });
                            }

                            mods.Add(mod);
                        } catch (Exception ex)
                        {
                            UtilitiesInternal.LogToFile(ex.ToString()); 
                        }
                    } else
                    {
                        UtilitiesInternal.LogToFile(path + "/mod.json");
                    }
                }
            }
            catch (ReflectionTypeLoadException e)
            {
                foreach (Exception ex in e.LoaderExceptions)
                {
                    UtilitiesInternal.LogToFile(ex.ToString());
                }
            }
        }

        // Currently unused but will allow some post-update patching once I build it out more
        
        static void DoAtlasPatches(Type __instance)
        {
            UtilitiesInternal.LogToFile(__instance.ToString());
        }
        static void AfterApplyPatches()
        {
            return;
        }

        static void ApplyPatches()
        {

            if (PATCHED) return;

            // Load each mod by calling Entry() function inside the mod

            foreach (Mod mod in mods.ToArray())
            {
                mod.callEntry();
            }

            try
            {

                Type MonocleEngineType = GetTowerFallAssembly().GetType("Monocle.Engine");
                Type TowerFallEngineType = GetTowerFallAssembly().GetType("TowerFall.TFGame");
                object EngineInstance = UtilitiesInternal.RunReflectiveCommand(MonocleEngineType, "get_Instance", new Type[] { }, null, new object[] { });

                // Create a new command that indicates the modloader is loaded

                CommandToolsInternal.RegisterCommand("modloader_info", delegate (string[] args)
                {
                    CommandToolsInternal.Trace("V1.0 of the PuppetMaster Modloader");
                    CommandToolsInternal.Trace("Comes with modding tools and new commands");
                    CommandToolsInternal.Trace($"Loaded {mods.Count} mods");
                });
                CommandToolsInternal.RegisterCommand("modloader_info", delegate (string[] args)
                {
                    CommandToolsInternal.Trace("V1.0 of the PuppetMaster Modloader");
                    CommandToolsInternal.Trace("Comes with modding tools and new commands");
                    CommandToolsInternal.Trace($"Loaded {mods.Count} mods");
                });

                CommandToolsInternal.RegisterCommand("mods", delegate (string[] args)
                {
                    foreach (Mod mod in mods)
                    {
                        CommandToolsInternal.Trace($"{mod.GetName()}");
                    }
                });

            }
            catch (Exception ex)
            {
                UtilitiesInternal.LogToFile(ex.ToString());
            }
            
            PATCHED = true;
            UtilitiesInternal.LogToFile("Patched commands");
        }
    }

}