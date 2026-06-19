using KSerialization;
using PeterHan.PLib.Options;
using System;
using UnityEngine;

namespace MyMassiveHeatSink
{
    /// <summary>
    /// 建筑“滑条配置组件”：
    /// 通过实现 ISliderControl/ISingleSliderControl，
    /// 将温度参数暴露给游戏 UI 滑条，并同步回 Config。
    /// </summary>
    public class SetMassiveHeatSinkConfig : KMonoBehaviour, ISingleSliderControl, ISliderControl
    {
        /// <summary>
        /// ONI 内部“复制建筑设置”事件 ID。
        /// </summary>
        private const int CopySettingsEventId = -905833192;

        /// <summary>
        /// 建筑安全温度下限（摄氏度）。
        /// </summary>
        private const float MinTemperatureC = -173f;

        /// <summary>
        /// 建筑安全温度上限（摄氏度）。
        /// </summary>
        private const float MaxTemperatureC = 200f;

        /// <summary>
        /// 摄氏度转开尔文的偏移值。
        /// </summary>
        private const float CelsiusToKelvin = 273.15f;

        /// <summary>
        /// 滑条小数位数（0 表示整数步进）。
        /// </summary>
        public int SliderDecimalPlaces(int index)
        {
            return 0;
        }

        /// <summary>
        /// 滑条最小值（摄氏度）。
        /// 这里与配置安全下限保持一致，防止 UI 直接设置危险值。
        /// </summary>
        public float GetSliderMin(int index)
        {
            return MinTemperatureC;
        }

        /// <summary>
        /// 滑条最大值（摄氏度）。
        /// </summary>
        public float GetSliderMax(int index)
        {
            // 与 Config 上限保持一致，避免 UI 可选值与实际生效值不一致。
            return MaxTemperatureC;
        }

        /// <summary>
        /// 滑条标题（本地化键）。
        /// </summary>
        public string SliderTitleKey
        {
            get
            {
                return STRINGS.K24M24GG24H0_UI.UI_1;
            }
        }

        /// <summary>
        /// 滑条提示文本（本地化键）。
        /// </summary>
        public string GetSliderTooltip(int index)
        {
            return STRINGS.K24M24GG24H0_UI.UI_2;
        }

        /// <summary>
        /// 滑条单位后缀（例如 “℃”）。
        /// </summary>
        public string SliderUnits
        {
            get
            {
                return STRINGS.K24M24GG24H0_UI.UI_3;
            }
        }

        /// <summary>
        /// 将当前滑条值同步到当前建筑实例。
        /// 注意：本方法是组件内部同步入口，不是 Unity 的 MonoBehaviour.Update 帧回调。
        /// </summary>
        internal void SyncInstanceTemperature()
        {
            float clampedC = Mathf.Clamp(this.AA, MinTemperatureC, MaxTemperatureC);
            this.AA = clampedC;
            // 建筑内可调：直接作用于当前建筑实例，不修改全局配置。
            this.minimumOperatingTemperature.minimumTemperature = clampedC + CelsiusToKelvin;
        }

        /// <summary>
        /// 获取当前滑条值。
        /// </summary>
        public float GetSliderValue(int index)
        {
            return this.AA;
        }

        /// <summary>
        /// 可选：返回 tooltip 的本地化键。
        /// 当前直接返回空串，使用 GetSliderTooltip 返回的文本。
        /// </summary>
        public string GetSliderTooltipKey(int index)
        {
            return "";
        }

        /// <summary>
        /// 组件生成时回调。
        /// 这里主动执行一次同步，确保 UI 初始值与配置一致。
        /// </summary>
        protected override void OnSpawn()
        {
            base.OnSpawn();
            // 新建建筑时，用当前全局配置作为初始值；存档加载时保留序列化值。
            if (this.AA < MinTemperatureC || this.AA > MaxTemperatureC)
            {
                this.AA = SingletonOptions<Config>.Instance.minimumTemperature;
            }
            this.SyncInstanceTemperature();
        }

        /// <summary>
        /// 当玩家拖动滑条时调用：
        /// 先写入本地字段，再同步到 Config。
        /// </summary>
        public void SetSliderValue(float value, int index)
        {
            this.AA = value;
            this.SyncInstanceTemperature();
        }
        /// <summary>
        /// 预制体初始化阶段：
        /// 订阅“复制建筑设置”事件，使同类建筑可复制此参数。
        /// </summary>
        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            base.Subscribe(CopySettingsEventId, new Action<object>(this.OnCopySettings));
        }

        /// <summary>
        /// 处理复制设置事件。
        /// 从来源建筑读取 AA 值并同步到当前建筑。
        /// </summary>
        /// <param name="data">来源对象（通常是 GameObject）。</param>
        internal void OnCopySettings(object data)
        {
            SetMassiveHeatSinkConfig component = ((GameObject)data).GetComponent<SetMassiveHeatSinkConfig>();
            if (component != null)
            {
                this.AA = component.AA;
                this.SyncInstanceTemperature();
            }
        }

        //[MyCmpReq]
        //public SingletonOptions<Config>.Instance.minimumTemperature conduitConsumer;

        /// <summary>
        /// 当前滑条值（序列化字段）：
        /// 用于保存建筑实例自身设置，支持存档/读档恢复。
        /// </summary>
        [Serialize]
        public float AA = 100f;

        /// <summary>
        /// 当前建筑的最低工作温度组件（实例级生效目标）。
        /// </summary>
        [MyCmpReq]
        public MinimumOperatingTemperature minimumOperatingTemperature;

        /// <summary>
        /// 自动添加复制设置组件引用，确保“复制建筑设置”按钮可用。
        /// </summary>
        [MyCmpAdd]
        public CopyBuildingSettings copyBuildingSettings;
    }
}
