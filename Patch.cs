using HarmonyLib;
using KMod;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;

namespace MyMassiveHeatSink
{
    /// <summary>
    /// Mod 总入口：
    /// 1) 初始化 PLib；
    /// 2) 注册配置菜单；
    /// 3) 保存命名空间信息；
    /// 4) 通过 Harmony Patch 挂接本地化初始化流程。
    /// </summary>
    public class Patch : UserMod2
    {
        /// <summary>
        /// 当前 Mod 的命名空间缓存，便于其他位置读取。
        /// </summary>
        public static string Namespace { get; private set; }

        /// <summary>
        /// Mod 加载时执行。
        /// 该函数是 UserMod2 生命周期入口之一。
        /// </summary>
        /// <param name="harmony">Harmony 实例，由游戏框架传入。</param>
        public override void OnLoad(Harmony harmony)
        {
            // 先执行基类加载逻辑，确保基础初始化完成。
            base.OnLoad(harmony);
            // 初始化 PLib（参数 true 表示启用调试日志等增强行为）。
            PUtil.InitLibrary(true);
            // 注册配置页面（对应 Config 类型）。
            new POptions().RegisterOptions(this, typeof(Config));
            Debug.Log("菜单配置加载！");
            // 缓存命名空间，避免后续反复反射读取。
            Patch.Namespace = base.GetType().Namespace;
        }

        /// <summary>
        /// 本地化初始化补丁：
        /// 当游戏 Localization.Initialize 执行完成后，注入自定义字符串。
        /// </summary>
        [HarmonyPatch(typeof(Localization), "Initialize")]
        private class Fan_Yi
        {
            /// <summary>
            /// Postfix 在原始函数执行后触发。
            /// </summary>
            public static void Postfix()
            {
                global::Debug.Log("加载翻译");
                // 将 STRINGS 类型中的 LocString 键注册并加载对应语言包。
                Utils.Localize(typeof(STRINGS));
            }
        }
    }
}
