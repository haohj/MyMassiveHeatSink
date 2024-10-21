using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMassiveHeatSink
{
    [HarmonyPatch(typeof(Db))]
    [HarmonyPatch("Initialize")]
    public class Study
    {
        public static void Postfix()
        {
            //研究栏添加反熵热量中和器
            Db.Get().Techs.Get("PressureManagement").unlockedItemIDs.Add("MyMassiveHeatSink");
            global::Debug.Log("在数据初始化后执行");
        }
    }
}
