using Newtonsoft.Json;
using PeterHan.PLib.Options;

namespace MyMassiveHeatSink
{
    [JsonObject(MemberSerialization.OptIn)]
    [ConfigFile()]
    [RestartRequired]

    public class Config : SingletonOptions<Config>
    {
        //设置降温效果
        [Option("降温效果", "设置反熵热量中和器的每秒降温效果", "反熵热量中和器", Format = "F0")]
        [Limit(-500.0, 500.0)]
        [JsonProperty]
        public float ExhaustKilowattsWhenActive { get; set; }
        //设置最低工作温度
        [Option("最低工作温度", "设置反熵热量中和器的最低工作温度，单位是℃", "反熵热量中和器", Format = "F0")]
        [Limit(-200.0, 200.0)]
        [JsonProperty]
        public float minimumTemperature { get; set; }

        public Config()
        {
            //默认选项内容
            this.ExhaustKilowattsWhenActive = -16f;
            this.minimumTemperature = 100f;
        }
    }
}
