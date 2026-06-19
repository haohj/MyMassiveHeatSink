using STRINGS;

namespace MyMassiveHeatSink
{
    /// <summary>
    /// 本地化字符串定义容器。
    /// 约定与 ONI 原生 STRINGS 层级保持一致，便于自动发现与覆盖。
    /// </summary>
    internal class STRINGS
    {
        /// <summary>
        /// 手动触发字符串键注册（通常由 Localize 流程间接调用）。
        /// </summary>
        public static void DoReplacement()
        {
            LocString.CreateLocStringKeys(typeof(STRINGS), "");
        }

        /// <summary>
        /// 建筑字符串命名空间。
        /// </summary>
        public class BUILDINGS
        {
            /// <summary>
            /// 预制体字符串命名空间。
            /// </summary>
            public class PREFABS
            {
                /// <summary>
                /// 自定义建筑 MyMassiveHeatSink 的显示文本。
                /// </summary>
                public class MYMASSIVEHEATSINK
                {
                    /// <summary>
                    /// 建筑名称（可点击链接样式）。
                    /// </summary>
                    public static LocString NAME = UI.FormatAsLink("MassiveHeatSink", "MYMASSIVEHEATSINK");

                    /// <summary>
                    /// 建筑效果说明（建造菜单/信息面板显示）。
                    /// </summary>
                    public static LocString EFFECT = "A self-sustaining machine powered by what appears to be refined \nAbsorbs and neutralizes energy when provided with piped Hydrogen Gas.";
                    /// <summary>
                    /// 建筑描述（当前为空，可按发布版本补充 lore 文本）。
                    /// </summary>
                    public static LocString DESC = "";
                }
            }
        }

        /// <summary>
        /// 自定义 UI 文本分组（用于滑条标题、说明、单位等）。
        /// </summary>
        public static class K24M24GG24H0_UI
        {
            /// <summary>
            /// 滑条标题文本。
            /// </summary>
            public static LocString UI_1 = "降温效果";

            /// <summary>
            /// 滑条提示文本。
            /// </summary>
            public static LocString UI_2 = "设置反熵热量中和器的每秒降温效果";

            /// <summary>
            /// 滑条单位文本（摄氏度）。
            /// </summary>
            public static LocString UI_3 = "  ℃";
        }
    }
}
