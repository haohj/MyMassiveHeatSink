using HarmonyLib;

namespace MyMassiveHeatSink
{
    /// <summary>
    /// 建造菜单注入补丁：
    /// 在建筑定义加载阶段，把自定义建筑加入对应建造分类页签。
    /// </summary>
    [HarmonyPatch(typeof(GeneratedBuildings), "LoadGeneratedBuildings")]
    //建筑栏加载
    public class BuildingFence
    {
        /// <summary>
        /// Prefix：在 LoadGeneratedBuildings 前执行。
        /// 提前注册可确保建筑在菜单初始化时可见。
        /// </summary>
        public static void Prefix()
        {
            //TODO 建筑栏对应关系
            /*基地=Base，氧气=Oxygen，电力=Power,食物=Food，水管=Plumbing，通风=HVAC，精炼=Refining，
             医疗=Medical，家具=Furniture，站台=Equipment，实用=Utilities，信号=Automation，运输=Conveyance，火箭=Rocketry，帮助=HEP
               */
            //建筑栏添加反熵热量中和器
            // 分类 "Utilities" 表示“实用”栏。
            ModUtil.AddBuildingToPlanScreen("Utilities", "MyMassiveHeatSink");
        }
    }
}
