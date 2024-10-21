using HarmonyLib;
using PeterHan.PLib.Options;
using TUNING;
using UnityEngine;
using static ElementConverter;
using static Klei.SimUtil;

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
                buildingDef.ExhaustKilowattsWhenActive = -16f;
                buildingDef.SelfHeatKilowattsWhenActive = -64f;
                buildingDef.Floodable = false;
                buildingDef.Entombable = false;
                buildingDef.AudioCategory = "Metal";
                buildingDef.UtilityInputOffset = new CellOffset(0, 0);
                buildingDef.InputConduitType = ConduitType.Gas;
                buildingDef.ShowInBuildMenu = true;

                return buildingDef;
            }
            public override void DoPostConfigureComplete(GameObject go)
            {
                go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery, false);
                go.AddOrGet<MassiveHeatSink>();
                go.AddOrGet<MinimumOperatingTemperature>().minimumTemperature = 100f;
                PrimaryElement component = go.GetComponent<PrimaryElement>();
                component.SetElement(SimHashes.Iron, true);
                component.Temperature = 294.15f;
                go.AddOrGet<LoopingSounds>();
                go.AddOrGet<Storage>().capacityKg = 0.099999994f;
                ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
                conduitConsumer.conduitType = ConduitType.Gas;
                conduitConsumer.consumptionRate = 1f;
                conduitConsumer.capacityTag = GameTagExtensions.Create(SimHashes.Hydrogen);
                conduitConsumer.capacityKG = 0.099999994f;
                conduitConsumer.forceAlwaysSatisfied = true;
                conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
                go.AddOrGet<ElementConverter>().consumedElements = new ElementConverter.ConsumedElement[]
                {
                    new ElementConverter.ConsumedElement(ElementLoader.FindElementByHash(SimHashes.Hydrogen).tag, 0.01f, true)
                };
                go.AddOrGetDef<PoweredActiveController.Def>();
                go.GetComponent<Deconstructable>().allowDeconstruction = true;
                go.AddOrGet<Demolishable>();
                go.AddOrGetDef<PoweredActiveController.Def>().showWorkingStatus = true;
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
            private const float CONSUMPTION_RATE = 1f;

            // Token: 0x04000822 RID: 2082
            private const float STORAGE_CAPACITY = 0.099999994f;
        }
    }
}
