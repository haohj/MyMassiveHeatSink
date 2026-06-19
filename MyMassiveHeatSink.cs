using PeterHan.PLib.Options;
using TUNING;
using UnityEngine;

namespace MyMassiveHeatSink
{
    /// <summary>
    /// 本文件负责“自定义反熵热量中和器”的建筑定义与组件装配。
    /// 主要职责：
    /// 1) 定义建筑基础参数（尺寸、材料、动画、建造规则等）；
    /// 2) 绑定运行组件（温度组件、功耗组件、管道输入、逻辑端口等）；
    /// 3) 统一约束关键温度参数，避免触发游戏温度模拟断言。
    /// </summary>
    public class Patches
    {//反熵热量中和器修改
        /// <summary>
        /// ONI 建筑配置入口类：
        /// 由游戏在加载建筑时回调其各生命周期方法，
        /// 用于创建 BuildingDef 并向预制体挂载运行时组件。
        /// </summary>
        public class MassiveHeatSinkConfig : IBuildingConfig
        {
            /// <summary>
            /// 安全下限：建筑主动换热功率（kW）。
            /// 值越小（越负）代表制冷越强；过强制冷会让局部单帧温度过低，可能越过模拟允许区间。
            /// </summary>
            private const float MIN_SAFE_COOLING_KW = -64f;
            /// <summary>
            /// 安全下限：允许配置的最低工作温度（摄氏度）。
            /// 对应绝对零度上方的游戏安全值（约 100 K -> -173.15 C）。
            /// </summary>
            private const float MIN_SAFE_TEMPERATURE_C = -173.15f;
            /// <summary>
            /// 安全上限：允许配置的最高工作温度（摄氏度）。
            /// 上限本身不是模拟硬限制，主要用于约束 UI 输入，防止过度配置导致玩法异常。
            /// </summary>
            private const float MAX_SAFE_TEMPERATURE_C = 200f;

            /// <summary>
            /// 创建建筑定义（BuildingDef）。
            /// 这里定义的是“静态属性”，例如大小、材质、建造位置、功耗外观等。
            /// </summary>
            public override BuildingDef CreateBuildingDef()
            {
                // ===== 基础标识与外观 =====
                string id = "MyMassiveHeatSink";
                int width = 4;
                int height = 4;
                string anim = "massiveheatsink_kanim";
                // ===== 建造与耐久 =====
                int hitpoints = 100;
                float construction_time = 120f;
                float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER5;
                string[] raw_METALS = MATERIALS.RAW_METALS;
                float melting_point = 2400f;
                BuildLocationRule build_location_rule = BuildLocationRule.OnFloor;
                // ===== 噪音与装饰 =====
                EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER5;
                BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tier, raw_METALS, melting_point, build_location_rule, BUILDINGS.DECOR.BONUS.TIER2, tier2, 0.2f);
                //建筑物活动时的功率（负值表示吸热）
                //buildingDef.ExhaustKilowattsWhenActive = -16f;
                // Clamp aggressive cooling to avoid driving simulation temperatures out of valid range.
                buildingDef.ExhaustKilowattsWhenActive = Mathf.Max(
                    SingletonOptions<Config>.Instance.ExhaustKilowattsWhenActive,
                    MIN_SAFE_COOLING_KW
                );
                //建筑物活动时的自身发热功率（负值表示冷却）
                buildingDef.SelfHeatKilowattsWhenActive = -64f;
                //是否可淹没
                buildingDef.Floodable = false;
                //是否可掩埋
                buildingDef.Entombable = false;
                // 声音类别用于调用游戏现有音效分组。
                buildingDef.AudioCategory = "Metal";
                // 气体输入口坐标（以建筑原点为参考）。
                buildingDef.UtilityInputOffset = new CellOffset(0, 0);
                buildingDef.InputConduitType = ConduitType.Gas;
                buildingDef.ShowInBuildMenu = true;
                //添加信号输入，方便自动化控制温度
                buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 0));
                return buildingDef;
            }

            /// <summary>
            /// 建筑完整配置：
            /// 此阶段向 GameObject 挂载运行组件，决定建筑实际“如何工作”。
            /// </summary>
            public override void DoPostConfigureComplete(GameObject go)
            {
                // 标记为工业机械，影响房间判定（例如某些房间类型会排斥工业设备）。
                go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery, false);
                // 挂载核心功能组件（游戏原生 MassiveHeatSink 行为脚本）。
                go.AddOrGet<MassiveHeatSink>();
                //添加信号输入，方便自动化控制温度
                go.AddOrGet<LogicOperationalController>();
                //最低温度是100k，也就是-173.15℃
                //go.AddOrGet<MinimumOperatingTemperature>().minimumTemperature = 100f;
                //改为动态调整，并且转换成摄氏度
                float minimumTemperatureC = Mathf.Clamp(
                    SingletonOptions<Config>.Instance.minimumTemperature,
                    MIN_SAFE_TEMPERATURE_C,
                    MAX_SAFE_TEMPERATURE_C
                );
                // 游戏内部温度单位是开尔文(K)，这里将摄氏度配置转换为 K。
                go.AddOrGet<MinimumOperatingTemperature>().minimumTemperature = minimumTemperatureC + 273.15f;
                PrimaryElement component = go.GetComponent<PrimaryElement>();
                // 建筑实体元素：铁。会影响导热和材质相关行为。
                component.SetElement(SimHashes.Iron, true);
                // 初始温度：294.15K（约 21℃），避免初始异常温度造成瞬态问题。
                component.Temperature = 294.15f;
                // 循环播放机械工作音效。
                go.AddOrGet<LoopingSounds>();
                // 小容量存储：满足转换器/管道消费者运行所需缓存。
                go.AddOrGet<Storage>().capacityKg = 0.099999994f;
                //设置消耗氢气
                // TODO 正式发布版本
                go.AddOrGet<ElementConverter>().consumedElements = new ElementConverter.ConsumedElement[]
                {
                    new ElementConverter.ConsumedElement(ElementLoader.FindElementByHash(SimHashes.Hydrogen).tag, 0.01f,
                        true)
                };
                // TODO 个人使用版本
                /* ElementConverter elementConverter = go.AddComponent<ElementConverter>();
                elementConverter.consumedElements = new ElementConverter.ConsumedElement[]
                {
                    // 当前版本允许任意气体作为输入，便于测试（正式发布可改回仅氢气）。
                    new ElementConverter.ConsumedElement(GameTags.Gas, 0.05f, true)
                }; */
                //管道输出消耗配置
                ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
                conduitConsumer.conduitType = ConduitType.Gas;
                // 每秒尝试从管道中拉取质量（kg/s）。
                conduitConsumer.consumptionRate = 100f;
                //设置元素，其它元素输入则会提示元素错误
                conduitConsumer.capacityTag = GameTagExtensions.Create(SimHashes.Hydrogen);
                //TODO 个人使用版本
                //conduitConsumer.capacityTag = GameTags.Gas;
                //conduitConsumer.capacityKG = 0.099999994f;
                // 生产环境应按真实供给判定，避免“无输入也持续工作”的异常行为。
                conduitConsumer.forceAlwaysSatisfied = false;
                // 错误元素处理策略：倾倒。
                conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
                // 电力激活控制：显示工作状态并参与建筑功耗逻辑。
                go.AddOrGetDef<PoweredActiveController.Def>();
                go.GetComponent<Deconstructable>().allowDeconstruction = true;
                go.AddOrGet<Demolishable>();
                go.AddOrGetDef<PoweredActiveController.Def>().showWorkingStatus = true;
                Prioritizable.AddRef(go);
            }
            //配置建筑模板

            /// <summary>
            /// 模板配置阶段：
            /// 一般用于补充存储、标签、可复制设置等“模板级”特性。
            /// </summary>
            public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
            {
                Prioritizable.AddRef(go);
                // 挂载建筑内滑条配置组件：允许每台建筑独立调整参数。
                go.AddOrGet<SetMassiveHeatSinkConfig>();
                //BuildingTemplates.CreateDefaultStorage(go, false).SetDefaultStoredItemModifiers(MassiveHeatSinkConfig.IncubatorStorage);
                //可以接受带Egg标签的物品
                go.AddOrGet<MassiveHeatSink>();
                //massiveHeatSink.AddDepositTag(GameTags.Egg);
            }

            //建造后预览
            /// <summary>
            /// 建筑预览阶段（放置蓝图时）。
            /// 保持最小逻辑，避免在预览期写入激进参数影响运行时状态。
            /// </summary>
            public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
            {
                base.DoPostConfigurePreview(def, go);
            }

            //建造后配置
            /// <summary>
            /// 建造中阶段（UnderConstruction）配置。
            /// 当前没有额外逻辑，仅保留重写点便于后续扩展。
            /// </summary>
            public override void DoPostConfigureUnderConstruction(GameObject go)
            {
                base.DoPostConfigureUnderConstruction(go);
            }

            /// <summary>
            /// 建筑 ID（必须与添加到建造栏/科技栏的 ID 保持一致）。
            /// </summary>
            public const string ID = "MyMassiveHeatSink";

            /// <summary>
            /// 预留常量：理论输入消耗速率（当前未直接使用）。
            /// </summary>
            private const float CONSUMPTION_RATE = 10f;

            /// <summary>
            /// 预留常量：内部存储容量（当前由代码直接赋值，同步保留常量便于后续统一管理）。
            /// </summary>
            private const float STORAGE_CAPACITY = 0.099999994f;
        }
    }
}
