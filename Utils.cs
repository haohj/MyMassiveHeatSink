using System;
using System.IO;
using System.Reflection;

namespace MyMassiveHeatSink
{
    internal class Utils
    {
        public static Utils.ModInfo modInfo
        {
            get
            {
                bool flag = Utils._modinfo == null;
                if (flag)
                {
                    Utils._modinfo = new Utils.ModInfo();
                }
                return Utils._modinfo;
            }
        }

        public static void Localize(Type root)
        {
            ModUtil.RegisterForTranslation(root);
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            string name = executingAssembly.GetName().Name;
            string path = Path.Combine(Path.GetDirectoryName(executingAssembly.Location), "translations");
            Localization.Locale locale = Localization.GetLocale();
            bool flag = locale != null;
            if (flag)
            {
                try
                {
                    string text = Path.Combine(path, locale.Code + ".po");
                    Debug.LogWarning(name + " lang file: " + text);
                    bool flag2 = File.Exists(text);
                    if (flag2)
                    {
                        Debug.Log(name + ": Localize file found " + text);
                        Localization.OverloadStrings(Localization.LoadStringsFile(text, false));
                    }
                }
                catch
                {
                    Debug.LogWarning(name + " Failed to load localization.");
                }
            }
            LocString.CreateLocStringKeys(root, "");
        }

        private static Utils.ModInfo _modinfo;

        public class ModInfo
        {
            public ModInfo()
            {
                Assembly executingAssembly = Assembly.GetExecutingAssembly();
                this.assemblyName = executingAssembly.GetName().Name;
                this.rootDirectory = Path.GetDirectoryName(executingAssembly.Location);
                this.langDirectory = Path.Combine(this.rootDirectory, "translations");
                this.spritesDirectory = Path.Combine(this.rootDirectory, "sprites");
                this.version = executingAssembly.GetName().Version.ToString();
            }

            public readonly string assemblyName;

            public readonly string rootDirectory;

            public readonly string langDirectory;

            public readonly string spritesDirectory;

            public readonly string version;
        }
    }
}
