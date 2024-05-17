using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PuppetMaster
{
    public class Utilities
    {
        public static object RunReflectiveCommand(Type type, String method_name, Type[] args, object instance, object[] parameters)
        {
            var methodInfo = type.GetMethod(method_name, args);
            if (methodInfo == null)
            {
                throw new Exception($"No such method {method_name} exists. ");
            }

            object r = methodInfo.Invoke(instance, parameters);

            return r;

        }
        public static void LogToFile(String Text)
        {
            using (StreamWriter outputFile = new StreamWriter(Path.Combine("C:/Users/jake_", "Injected.txt"), true))
            {
                outputFile.WriteLine(Text);
            }
        }

        public static Assembly GetAssemblyByName(string name)
        {
            return AppDomain.CurrentDomain.GetAssemblies().
                   SingleOrDefault(assembly => assembly.GetName().Name == name);
        }

        public static Type GetTypeFromAssembly(string assemblyName, string typeName)
        {
            return GetAssemblyByName(assemblyName).GetType(typeName);
        }
    }
}
