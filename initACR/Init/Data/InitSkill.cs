namespace Init.Data;

/// <summary>
/// 技能 ID 常量
///
/// TODO: 替换为你的职业的技能 ID
/// 获取方式:
///   1. 游戏中通过 Dalamud 插件 (如 SimpleTweaks / DataCenter) 查看
///   2. 查 XIVAPI: https://xivapi.com/
///   3. 查 Teamcraft: https://ffxivteamcraft.com/
///   4. Lumina 导出: GameData.GetExcelSheet<Action>() 遍历
/// </summary>
public static class InitSkill
{
    // TODO: 替换为你的职业 GCD 技能 ID
    // 示例格式 (以下是 MCH 的技能，请替换):
    // public const uint 基础连击1 = 2866;
    // public const uint 基础连击2 = 2868;
    // public const uint 基础连击3 = 2873;

    // === GCD 技能 ===
    // TODO: 添加你的 GCD 技能

    // === 能力技 (oGCD) ===
    // TODO: 添加你的 oGCD 技能

    // === 职能技能（通用，通常不变） ===
    /// <summary>内丹</summary>
    public const uint 内丹 = 7541;

    /// <summary>伤头</summary>
    public const uint 伤头 = 7551;

    /// <summary>速行</summary>
    public const uint 速行 = 7557;

    /// <summary>冲刺</summary>
    public const uint 冲刺 = 3;

    /// <summary>防击退</summary>
    public const uint 防击退 = 7548;

    // TODO: 根据职业选择减伤技能
    // 坦克: 铁壁/大减伤/小减伤
    // 近战: 牵制/血仇
    // 远程: 策动/武装解除
    // 法系: 昏乱
    // 治疗: 复活/康复
}
