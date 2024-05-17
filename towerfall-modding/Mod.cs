using PuppetMaster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace modloader
{
    internal class Mod
    {
        private string name;
        private Assembly assembly;
        private Type type;
        private bool HasTextures;

        public Mod (string name, Assembly assembly)
        {
            this.name = name;
            this.assembly = assembly;
            foreach (Type t in assembly.GetTypes())
            {
                if (t.GetMethod("Entry") != null)
                    this.type = t;
            };
        }

        public void callEntry()
        {
            UtilitiesInternal.RunReflectiveCommand(this.type, "Entry", new Type[] { }, null, new Type[] { });
            return;
        }

        internal string GetName()
        {
            return this.name;
        }
    }
}
