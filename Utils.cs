using System;
using System.IO;
using System.Reflection;

namespace MyMassiveHeatSink
{
    /// <summary>
    /// 通用工具类：
    /// 当前主要提供本地化加载、Mod 元信息缓存。
    /// </summary>
    internal class Utils
    {
        /// <summary>
        /// 懒加载缓存的 ModInfo 对象。
        /// 仅首次访问时创建，后续复用。
        /// </summary>
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

        /// <summary>
        /// 加载并注册本地化字符串：
        /// 1) 注册类型中的 LocString 键；
        /// 2) 读取 translations 目录中对应语言的 .po 文件；
        /// 3) 覆盖游戏字符串表。
        /// </summary>
        /// <param name="root">包含 LocString 字段的根类型（通常是 STRINGS）。</param>
        public static void Localize(Type root)
        {
            // 向游戏注册待翻译字符串键。
            ModUtil.RegisterForTranslation(root);
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            string name = executingAssembly.GetName().Name;
            // 约定目录：Mod 根目录/translations
            string path = Path.Combine(Path.GetDirectoryName(executingAssembly.Location), "translations");
            Localization.Locale locale = Localization.GetLocale();
            bool flag = locale != null;
            if (flag)
            {
                try
                {
                    // 根据当前语言代码拼装文件名，如 zh.po / en.po。
                    string text = Path.Combine(path, locale.Code + ".po");
                    Debug.LogWarning(name + " lang file: " + text);
                    bool flag2 = File.Exists(text);
                    if (flag2)
                    {
                        // 如果文件存在，载入并覆盖对应字符串。
                        Debug.Log(name + ": Localize file found " + text);
                        Localization.OverloadStrings(Localization.LoadStringsFile(text, false));
                    }
                }
                catch
                {
                    // 本地化失败时仅告警，不阻断 Mod 主流程。
                    Debug.LogWarning(name + " Failed to load localization.");
                }
            }
            // 根据 root 类型生成 LocString 键，确保代码侧引用有效。
            LocString.CreateLocStringKeys(root, "");
        }

        /// <summary>
        /// ModInfo 单例缓存字段。
        /// </summary>
        private static Utils.ModInfo _modinfo;

        /// <summary>
        /// Mod 元信息对象：
        /// 用于集中保存程序集名、路径、版本等信息，便于日志与资源定位。
        /// </summary>
        public class ModInfo
        {
            /// <summary>
            /// 构造并采集当前执行程序集的基础信息。
            /// </summary>
            public ModInfo()
            {
                Assembly executingAssembly = Assembly.GetExecutingAssembly();
                this.assemblyName = executingAssembly.GetName().Name;
                this.rootDirectory = Path.GetDirectoryName(executingAssembly.Location);
                this.langDirectory = Path.Combine(this.rootDirectory, "translations");
                this.spritesDirectory = Path.Combine(this.rootDirectory, "sprites");
                this.version = executingAssembly.GetName().Version.ToString();
            }

            /// <summary>
            /// 程序集名称（通常与 Mod DLL 名一致）。
            /// </summary>
            public readonly string assemblyName;

            /// <summary>
            /// Mod 根目录（DLL 所在目录）。
            /// </summary>
            public readonly string rootDirectory;

            /// <summary>
            /// 翻译目录路径（root/translations）。
            /// </summary>
            public readonly string langDirectory;

            /// <summary>
            /// 精灵资源目录路径（root/sprites）。
            /// </summary>
            public readonly string spritesDirectory;

            /// <summary>
            /// 程序集版本号。
            /// </summary>
            public readonly string version;
        }
    }
}
