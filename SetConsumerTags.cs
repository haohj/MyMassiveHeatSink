using System;
using STRINGS;
using UnityEngine;

namespace MyMassiveHeatSink
{
    /// <summary>
    /// 可配置消耗物选项（预留实现）：
    /// 目标是向 IConfigurableConsumer 提供可选配方/标签集合。
    /// 
    /// 当前文件为“接口占位”状态，方法尚未实现；
    /// 保留该类可避免后续扩展时重新搭骨架。
    /// </summary>
    public class SetConsumerTags : KMonoBehaviour, IConfigurableConsumerOption, IConfigurableConsumerIngredient
    {
        /// <summary>
        /// 默认允许标签：氢气。
        /// </summary>
        private static readonly Tag HydrogenTag = GameTagExtensions.Create(SimHashes.Hydrogen);

        /// <summary>
        /// 默认消耗量（kg/s）。
        /// </summary>
        private const float DefaultAmount = 0.01f;

        /// <summary>
        /// 预制体初始化回调（当前无额外逻辑）。
        /// </summary>
        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
        }

        /// <summary>
        /// 生成时回调（当前无额外逻辑）。
        /// </summary>
        protected override void OnSpawn()
        {
            base.OnSpawn();

        }

        /// <summary>
        /// 返回选项唯一标识。
        /// 未实现：后续可返回如 SimHashes.Hydrogen 对应标签。
        /// </summary>
        Tag IConfigurableConsumerOption.GetID()
        {
            return HydrogenTag;
        }

        /// <summary>
        /// 返回选项名称（展示在 UI）。
        /// </summary>
        string IConfigurableConsumerOption.GetName()
        {
            return "Hydrogen";
        }

        /// <summary>
        /// 返回选项详细描述（长文本）。
        /// </summary>
        string IConfigurableConsumerOption.GetDetailedDescription()
        {
            return "Only accepts Hydrogen gas from the input conduit.";
        }

        /// <summary>
        /// 返回选项简介（短文本）。
        /// </summary>
        string IConfigurableConsumerOption.GetDescription()
        {
            return "Hydrogen only";
        }

        /// <summary>
        /// 返回选项图标。
        /// </summary>
        Sprite IConfigurableConsumerOption.GetIcon()
        {
            // 占位实现：后续可接入元素图标。
            return null;
        }

        /// <summary>
        /// 返回该选项下的“配方原料集合”。
        /// </summary>
        IConfigurableConsumerIngredient[] IConfigurableConsumerOption.GetIngredients()
        {
            // 当前选项本身即为唯一原料定义。
            return new IConfigurableConsumerIngredient[] { this };
        }

        /// <summary>
        /// 返回允许的标签集合（可多标签兼容）。
        /// </summary>
        Tag[] IConfigurableConsumerIngredient.GetIDSets()
        {
            return new Tag[] { HydrogenTag };
        }

        /// <summary>
        /// 返回该原料需求量。
        /// </summary>
        float IConfigurableConsumerIngredient.GetAmount()
        {
            return DefaultAmount;
        }
    }
}
