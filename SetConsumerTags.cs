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
            throw new NotImplementedException();
        }

        /// <summary>
        /// 返回选项名称（展示在 UI）。
        /// </summary>
        string IConfigurableConsumerOption.GetName()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 返回选项详细描述（长文本）。
        /// </summary>
        string IConfigurableConsumerOption.GetDetailedDescription()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 返回选项简介（短文本）。
        /// </summary>
        string IConfigurableConsumerOption.GetDescription()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 返回选项图标。
        /// </summary>
        Sprite IConfigurableConsumerOption.GetIcon()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 返回该选项下的“配方原料集合”。
        /// </summary>
        IConfigurableConsumerIngredient[] IConfigurableConsumerOption.GetIngredients()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 返回允许的标签集合（可多标签兼容）。
        /// </summary>
        Tag[] IConfigurableConsumerIngredient.GetIDSets()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 返回该原料需求量。
        /// </summary>
        float IConfigurableConsumerIngredient.GetAmount()
        {
            throw new NotImplementedException();
        }
    }
}
