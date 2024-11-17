using PeterHan.PLib.Options;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

namespace MyMassiveHeatSink
{
    public class Patches
    {//反熵热量中和器修改
        public class MassiveHeatSinkConfig : IBuildingConfig
        {
            public override BuildingDef CreateBuildingDef()
            {
                string id = "MyMassiveHeatSink";
                int width = 4;
                int height = 4;
                string anim = "massiveheatsink_kanim";
                int hitpoints = 100;
                float construction_time = 120f;
                float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER5;
                string[] raw_METALS = MATERIALS.RAW_METALS;
                float melting_point = 2400f;
                BuildLocationRule build_location_rule = BuildLocationRule.OnFloor;
                EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER5;
                BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tier, raw_METALS, melting_point, build_location_rule, BUILDINGS.DECOR.BONUS.TIER2, tier2, 0.2f);
                //建筑物活动时的功率（负值表示吸热）
                //buildingDef.ExhaustKilowattsWhenActive = -16f;
                buildingDef.ExhaustKilowattsWhenActive = SingletonOptions<Config>.Instance.ExhaustKilowattsWhenActive;
                //建筑物活动时的自身发热功率（负值表示冷却）
                buildingDef.SelfHeatKilowattsWhenActive = -64f;
                //是否可淹没
                buildingDef.Floodable = false;
                //是否可掩埋
                buildingDef.Entombable = false;
                buildingDef.AudioCategory = "Metal";
                buildingDef.UtilityInputOffset = new CellOffset(0, 0);
                buildingDef.InputConduitType = ConduitType.Gas;
                buildingDef.ShowInBuildMenu = true;
                //添加信号输入，方便自动化控制温度
                buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 0));
                return buildingDef;
            }
            public override void DoPostConfigureComplete(GameObject go)
            {
                go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery, false);
                go.AddOrGet<MassiveHeatSink>();
                //添加信号输入，方便自动化控制温度
                go.AddOrGet<LogicOperationalController>();
                //最低温度是100k，也就是-173.15℃
                //go.AddOrGet<MinimumOperatingTemperature>().minimumTemperature = 100f;
                //改为动态调整，并且转换成摄氏度
                go.AddOrGet<MinimumOperatingTemperature>().minimumTemperature = SingletonOptions<Config>.Instance.minimumTemperature + 273.15f; 
                PrimaryElement component = go.GetComponent<PrimaryElement>();
                component.SetElement(SimHashes.Iron, true);
                component.Temperature = 294.15f;
                go.AddOrGet<LoopingSounds>();
                go.AddOrGet<Storage>().capacityKg = 0.099999994f;
                //设置消耗氢气
                /*go.AddOrGet<ElementConverter>().consumedElements = new ElementConverter.ConsumedElement[]
                {
                    new ElementConverter.ConsumedElement(ElementLoader.FindElementByHash(SimHashes.Hydrogen).tag, 0.01f,
                        true)
                };*/
                ElementConverter elementConverter = go.AddComponent<ElementConverter>();
                elementConverter.consumedElements = new ElementConverter.ConsumedElement[]
                {
                    new ElementConverter.ConsumedElement(GameTags.Gas, 0.05f, true)
                };
                //管道输出消耗配置
                ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
                conduitConsumer.conduitType = ConduitType.Gas;
                conduitConsumer.consumptionRate = 100f;
                //设置元素，其它元素输入则会提示元素错误
                //conduitConsumer.capacityTag = GameTagExtensions.Create(SimHashes.Hydrogen);
                conduitConsumer.capacityTag = GameTags.Gas;
                conduitConsumer.capacityKG = 0.099999994f;
                conduitConsumer.forceAlwaysSatisfied = true;
                conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
                go.AddOrGetDef<PoweredActiveController.Def>();
                go.GetComponent<Deconstructable>().allowDeconstruction = true;
                go.AddOrGet<Demolishable>();
                go.AddOrGetDef<PoweredActiveController.Def>().showWorkingStatus = true;
                Prioritizable.AddRef(go);
            }

            //建造后预览
            public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
            {
                base.DoPostConfigurePreview(def, go);
            }

            //建造后配置
            public override void DoPostConfigureUnderConstruction(GameObject go)
            {
                base.DoPostConfigureUnderConstruction(go);
            }

            public const string ID = "MyMassiveHeatSink";

            // Token: 0x04000821 RID: 2081
            private const float CONSUMPTION_RATE = 10f;

            // Token: 0x04000822 RID: 2082
            private const float STORAGE_CAPACITY = 0.099999994f;
        }
    }
}
