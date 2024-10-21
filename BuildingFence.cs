using HarmonyLib;

namespace MyMassiveHeatSink
{
    [HarmonyPatch(typeof(GeneratedBuildings), "LoadGeneratedBuildings")]
    //建筑栏加载
    public class BuildingFence
    {
        public static void Prefix()
        {
            //TODO 建筑栏对应关系
            /*基地=Base，氧气=Oxygen，电力=Power,食物=Food，水管=Plumbing，通风=HVAC，精炼=Refining，
             医疗=Medical，家具=Furniture，站台=Equipment，实用=Utilities，信号=Automation，运输=Conveyance，火箭=Rocketry，帮助=HEP
               */
            //建筑栏添加反熵热量中和器
            ModUtil.AddBuildingToPlanScreen("Utilities", "MyMassiveHeatSink");
        }
    }
}
