using PuppetMaster;

namespace SampleMod
{
    public class Mod
    {
        public static void Entry()
        {
            
            CommandTools.RegisterCommand("test_sample_mod", delegate (string[] args) 
            {
                CommandTools.Trace("It did load!");
            });
        }
    }
}