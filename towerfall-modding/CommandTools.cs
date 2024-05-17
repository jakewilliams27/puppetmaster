using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PuppetMaster
{
    public class CommandToolsInternal
    {
        /*
         *
         * This is a class for interfacing with the Commands of Towerfall. Allows for adding new commands through modding.
         * 
         */
        public static Assembly GetTowerFallAssembly()
        {
            return UtilitiesInternal.GetAssemblyByName("TowerFall");
        }
        
        // Outputs to the command console
        public static void Trace(string message)
        {
            // Intermediary to Monocle.Commands.Trace()

            Type MonocleEngineType = GetTowerFallAssembly().GetType("Monocle.Engine");
            object EngineInstance = UtilitiesInternal.RunReflectiveCommand(MonocleEngineType, "get_Instance", new Type[] { }, null, new object[] { });


            Type MonocleCommandsType = GetTowerFallAssembly().GetType("Monocle.Commands");
            object Commands = UtilitiesInternal.RunReflectiveCommand(MonocleEngineType, "get_Commands", new Type[] { }, EngineInstance, new object[] { });

            UtilitiesInternal.RunReflectiveCommand(MonocleCommandsType, "Trace", new Type[] { typeof(String) }, Commands, new object[] { message });
        }

        // Registers a command with a callback function to execute
        public static void RegisterCommand(string command_string, Action<string[]> action)
        {
            // Intermediary to Monocle.Commands.RegisterCommand(string command, Action<string[]> action)

            Type MonocleEngineType = GetTowerFallAssembly().GetType("Monocle.Engine");
            Type MonocleCommandsType = GetTowerFallAssembly().GetType("Monocle.Commands");

            object EngineInstance = UtilitiesInternal.RunReflectiveCommand(MonocleEngineType, "get_Instance", new Type[] { }, null, new object[] { });


            object Commands = UtilitiesInternal.RunReflectiveCommand(MonocleEngineType, "get_Commands", new Type[] { }, EngineInstance, new object[] { });

            UtilitiesInternal.RunReflectiveCommand(MonocleCommandsType, "RegisterCommand", new Type[] { typeof(String), typeof(Action<string[]>) }, Commands, 
                new object[] { command_string, action});
        }
    }
}
