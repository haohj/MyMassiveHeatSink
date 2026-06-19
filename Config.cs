using Newtonsoft.Json;
using PeterHan.PLib.Options;

namespace MyMassiveHeatSink
{
    /// <summary>
    /// Mod 全局配置对象。
    /// 由 PLib 自动序列化到配置文件，并在游戏内“Mod 选项”菜单中显示。
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    [ConfigFile()]
    [RestartRequired]

    public class Config : SingletonOptions<Config>
    {
        /// <summary>
        /// 建筑的换热功率（kW）。
        /// 负值代表“吸热/制冷”，正值代表“放热”。
        /// 
        /// 注意：
        /// 过小（过于负）的值会造成局部温度变化过激，
        /// 因此下限限制在 -64，避免触发模拟温度断言。
        /// </summary>
        [Option("降温效果", "设置反熵热量中和器的每秒降温效果", "反熵热量中和器", Format = "F0")]
        [Limit(-64.0, 500.0)]
        [JsonProperty]
        public float ExhaustKilowattsWhenActive { get; set; }

        /// <summary>
        /// 建筑最低工作温度（摄氏度）。
        /// 当环境低于该温度时，建筑将不再继续工作（由 MinimumOperatingTemperature 控制）。
        /// </summary>
        [Option("最低工作温度", "设置反熵热量中和器的最低工作温度，单位是℃", "反熵热量中和器", Format = "F0")]
        [Limit(-173.0, 200.0)]
        [JsonProperty]
        public float minimumTemperature { get; set; }

        /// <summary>
        /// 默认配置构造函数。
        /// 当用户还未生成配置文件时，首次加载将使用这些默认值。
        /// </summary>
        public Config()
        {
            //默认选项内容
            // 默认换热功率：-16kW（相对温和，接近原版平衡）。
            this.ExhaustKilowattsWhenActive = -16f;
            // 默认最低工作温度：100℃（现有项目逻辑保持一致）。
            this.minimumTemperature = 100f;
        }
    }
}
