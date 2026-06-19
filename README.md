# MyMassiveHeatSink

一个用于《Oxygen Not Included（缺氧）》的反熵热量中和器增强 Mod。

本 Mod 的目标是：在保留原版核心玩法的前提下，让建筑更可控、更稳定，并提供可配置能力。

## 功能概览

- 将 `MyMassiveHeatSink` 注入研究树（`PressureManagement`）和建造栏（`Utilities`）。
- 支持配置工作时吸/放热强度（`ExhaustKilowattsWhenActive`）。
- 支持配置最低工作温度（摄氏度）。
- 反熵热量中和器设置为：
  - 不可淹没（`Floodable = false`）
  - 不可掩埋（`Entombable = false`）
- 输入气体限制为**氢气**（Hydrogen only）。
- 支持建筑内参数调节（每台建筑实例独立设置，可复制设置）。

## 与原版差异

- 原版反熵热量中和器无法直接像普通建筑那样通过该 Mod 的方式进行可配置扩展。
- 本 Mod 提供了可调参数，并在实现中加入温度边界保护，避免极端配置导致模拟异常。

## 配置项说明

配置类：`Config.cs`

- `ExhaustKilowattsWhenActive`
  - 含义：建筑工作时额外吸/放热功率（kW）。
  - 负值表示吸热（制冷），正值表示放热。
  - 当前限制：`[-64.0, 500.0]`（代码中仍会做安全钳制）。
- `MinimumTemperatureC`（兼容旧配置键：`minimumTemperature`）
  - 含义：最低工作温度（单位：摄氏度）。
  - 低于该温度时建筑停止工作。
  - 当前限制：`[-173.0, 200.0]`。

## 建筑内调节（实例级）

组件：`SetMassiveHeatSinkConfig.cs`

- 通过滑条实现**每台建筑独立**的最低工作温度调节。
- 滑条范围：`-173` 到 `200`（摄氏度）。
- 调整结果直接写入当前建筑实例的 `MinimumOperatingTemperature`，不会污染全局配置。
- 支持“复制建筑设置”。

## 运行安全策略

为避免出现模拟温度断言（如 `final_temperature` 越界）：

- 对最低工作温度进行钳制（`-173.15` 到 `200` 摄氏度）。
- 对吸热下限进行保护（避免过强负值导致瞬时越界）。
- 建筑输入不再强制“永远满足”（`forceAlwaysSatisfied = false`），防止无输入也持续运行。

## 常见问题（FAQ）

### 1) 报错：`SimMessage: Assert failed: 0 <= final_temperature && final_temperature <= SIM_MAX_TEMPERATURE`

通常由极端配置导致（例如最低温度过低或热量参数过激）。

建议将配置调整到安全范围，例如：

```json
{"ExhaustKilowattsWhenActive":-64.0,"minimumTemperature":-173.0}
```

更保守配置：

```json
{"ExhaustKilowattsWhenActive":-16.0,"minimumTemperature":100.0}
```

### 2) 输入气体到底是不是只允许氢气？

当前实现是**只允许氢气**：

- `ElementConverter` 消耗项为 Hydrogen。
- `ConduitConsumer.capacityTag` 设为 Hydrogen。

### 3) 为什么我改了全局配置，但某台建筑表现不同？

因为启用了建筑内实例级调节。已经放置并单独调过参数的建筑，会优先使用该实例值。

## 项目结构（主要文件）

- `MyMassiveHeatSink.cs`：建筑定义与组件装配（核心逻辑）
- `Config.cs`：Mod 配置项定义
- `SetMassiveHeatSinkConfig.cs`：建筑内滑条与实例级参数同步
- `Patch.cs`：Mod 入口、PLib 初始化、本地化注册
- `Study.cs`：科技树注入
- `BuildingFence.cs`：建造栏注入
- `STRINGS.cs`：本地化字符串键定义
- `Utils.cs`：翻译文件加载工具
- `SetConsumerTags.cs`：可配置消耗项的预留实现

## 安装方式

1. 在 Steam 创意工坊搜索并订阅本 Mod（关键词可用“反熵热量中和器”或项目名）。
2. 启动游戏并在 Mod 列表中启用。
3. 进入游戏后在研究与建造栏中确认建筑已注入。

## 兼容与注意事项

- 建议与会修改同一建筑定义（`MassiveHeatSink`）的 Mod 错峰排查冲突。
- 若遇到异常，优先检查配置值是否超出合理范围、是否有其他温度系统改动 Mod 并存。

## 致谢

感谢 ONI 社区与 PLib/Harmony 生态提供的工具支持。
