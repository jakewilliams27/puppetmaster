using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace PuppetMaster
{
    public class Modloader
    {

        public static object patchedAtlas = null;
        public static object patchedMenuAtlas = null;
        public static object patchedBGAtlas = null;
        public static object patchedBossAtlas = null;

        public static bool PATCHED = false;
        public static List<Type> allMods = new List<Type>();

        public static Assembly GetTowerFallAssembly()
        {
            return Utilities.GetAssemblyByName("TowerFall");
        }

        public static void Inject()
        {
            Assembly.LoadFile("C:\\\\Users\\\\jake_\\\\Documents\\\\RE\\\\towerfall-modding\\\\bin\\\\Release\\\\net400\\\\0Harmony.dll");
            

            // Find the Harmony Dependency in Loaded Assemblies
            Utilities.LogToFile("---------------------");
            
            Utilities.LogToFile("Injected successfully!");

            HarmonyBridge b = new HarmonyBridge("com.jakesnake.modloader");

            Type MonocleEngineType = GetTowerFallAssembly().GetType("Monocle.Engine");


            Type GameTime = Utilities.GetTypeFromAssembly("Microsoft.Xna.Framework.Game", "Microsoft.Xna.Framework.GameTime");


            // Runtime Texture Patching

            Type MonocleAtlasType = GetTowerFallAssembly().GetType("Monocle.Atlas");
            patchedAtlas = Activator.CreateInstance(MonocleAtlasType, new object[] { "Atlas/atlas.xml", "Atlas/atlas_patched.png", true });

            patchedMenuAtlas = Activator.CreateInstance(MonocleAtlasType, new object[] { "Atlas/menuAtlas.xml", "Atlas/menuAtlas_patched.png", true });

            patchedBGAtlas = Activator.CreateInstance(MonocleAtlasType, new object[] { "Atlas/bgAtlas.xml", "Atlas/bgAtlas_patched.png", true });

            patchedBossAtlas = Activator.CreateInstance(MonocleAtlasType, new object[] { "Atlas/bossAtlas.xml", "Atlas/bossAtlas_patched.png", true });

            object get_AtlasOriginal = AccessTools.Method(GetTowerFallAssembly().GetType("TowerFall.TFGame"), "get_Atlas", new Type[] { }, null);

            MethodInfo get_AtlasPrefix = typeof(Modloader).GetMethod("get_Atlas", BindingFlags.NonPublic | BindingFlags.Static);           

            b.Patch(get_AtlasOriginal, HarmonyMethod.New(get_AtlasPrefix), null);

            object get_MenuAtlasOriginal = AccessTools.Method(GetTowerFallAssembly().GetType("TowerFall.TFGame"), "get_MenuAtlas", new Type[] { }, null);

            MethodInfo get_MenuAtlasPrefix = typeof(Modloader).GetMethod("get_MenuAtlas", BindingFlags.NonPublic | BindingFlags.Static);

            b.Patch(get_MenuAtlasOriginal, HarmonyMethod.New(get_MenuAtlasPrefix), null);

            object get_BGAtlasOriginal = AccessTools.Method(GetTowerFallAssembly().GetType("TowerFall.TFGame"), "get_BGAtlas", new Type[] { }, null);

            MethodInfo get_BGAtlasPrefix = typeof(Modloader).GetMethod("get_BGAtlas", BindingFlags.NonPublic | BindingFlags.Static);

            b.Patch(get_BGAtlasOriginal, HarmonyMethod.New(get_BGAtlasPrefix), null);

            object get_BossAtlasOriginal = AccessTools.Method(GetTowerFallAssembly().GetType("TowerFall.TFGame"), "get_BossAtlas", new Type[] { }, null);

            MethodInfo get_BossAtlasPrefix = typeof(Modloader).GetMethod("get_BossAtlas", BindingFlags.NonPublic | BindingFlags.Static);

            b.Patch(get_BossAtlasOriginal, HarmonyMethod.New(get_BossAtlasPrefix), null);


            // Patch Update function. We can guarantee that will run.

            object mOriginal = AccessTools.Method(MonocleEngineType, "Update", new Type[] { GameTime }, null);

            object mPrefix = SymbolExtensions.GetMethodInfo(() => ApplyPatches());
            object mPostfix = SymbolExtensions.GetMethodInfo(() => AfterApplyPatches());

            Utilities.LogToFile("Got method infos...");

            b.Patch(mOriginal, HarmonyMethod.New(mPrefix), HarmonyMethod.New(mPostfix));


            Utilities.LogToFile("Loading mods...");

            LoadMods();

            Utilities.LogToFile("Loaded mods!");


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
                string path = Path.GetDirectoryName("C:\\GOG Games\\Towerfall - Ascension\\mods\\");

                foreach (string dll in Directory.EnumerateFiles(path, "*.dll"))
                {
                    Assembly assembly = Assembly.LoadFrom(dll);
                    Utilities.LogToFile($"Loaded assembly {assembly.FullName}");
                    
                    foreach (Type type in assembly.GetTypes())
                    {
                        if (type.GetMethod("Entry") != null)
                            allMods.Add(type);
                    };
                }
            }
            catch (ReflectionTypeLoadException e)
            {
                foreach (Exception ex in e.LoaderExceptions)
                {
                    Utilities.LogToFile(ex.ToString());
                }
            }
        }

        // Currently unused but will allow some post-update patching once I build it out more
        static void AfterApplyPatches()
        {
            return;
        }

        static void ApplyPatches()
        {

            if (PATCHED) return;

            // Load each mod by calling Entry() function inside the mod

            foreach (Type mod in allMods.ToArray())
            {
                if (mod.FullName != "PuppetMasterTools")
                {
                    Utilities.RunReflectiveCommand(mod, "Entry", new Type[] { }, null, new Type[] { });
                }
            }

            try
            {

                Type MonocleEngineType = GetTowerFallAssembly().GetType("Monocle.Engine");
                Type TowerFallEngineType = GetTowerFallAssembly().GetType("TowerFall.TFGame");
                object EngineInstance = Utilities.RunReflectiveCommand(MonocleEngineType, "get_Instance", new Type[] { }, null, new object[] { });

                // Create a new command that indicates the modloader is loaded

                CommandTools.RegisterCommand("modloader_info", delegate (string[] args)
                {
                    CommandTools.Trace("V1.0 of the PuppetMaster Modloader");
                    CommandTools.Trace("Comes with modding tools and new commands");
                    CommandTools.Trace($"Loaded {allMods.Count} mods");
                });

            }
            catch (Exception ex)
            {
                Utilities.LogToFile(ex.ToString());
            }
            
            PATCHED = true;
            Utilities.LogToFile("Patched commands");
        }
    }

}