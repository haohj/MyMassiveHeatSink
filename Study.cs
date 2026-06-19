using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMassiveHeatSink
{
    /// <summary>
    /// 科技树注入补丁：
    /// 在 Db.Initialize 完成后，将自定义建筑加入指定科技解锁列表。
    /// </summary>
    [HarmonyPatch(typeof(Db))]
    [HarmonyPatch("Initialize")]
    public class Study
    {
        /// <summary>
        /// 防止重复注入科技解锁项。
        /// </summary>
        private static bool techRegistered;

        /// <summary>
        /// Postfix：原始 Db.Initialize 执行后触发。
        /// </summary>
        public static void Postfix()
        {
            if (techRegistered)
            {
                return;
            }

            //研究栏添加反熵热量中和器
            // PressureManagement: 对应原版“压力管理”科技。
            var unlocked = Db.Get().Techs.Get("PressureManagement").unlockedItemIDs;
            if (!unlocked.Contains("MyMassiveHeatSink"))
            {
                unlocked.Add("MyMassiveHeatSink");
            }
            techRegistered = true;
            global::Debug.Log("在数据初始化后执行");
        }
    }
}
