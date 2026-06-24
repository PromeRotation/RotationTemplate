namespace Init.Data;

/// <summary>
/// Buff / Debuff / Aura 状态 ID 常量
///
/// TODO: 替换为你的职业的 Buff ID
/// 获取方式: 在游戏中通过 Dalamud 插件查看或查 XIVAPI/Teamcraft
/// </summary>
public static class InitBuff
{
    // TODO: 替换为你的职业 Buff ID
    // 示例格式:
    // public const uint 某增益Buff = 851;
    // public const uint 某Debuff = 1946;

    // === 通用 Buff（大多数职业可用） ===
    /// <summary>冲刺</summary>
    public const uint 冲刺 = 50;

    // TODO: 添加你职业专属的 Buff/Debuff ID
    // public const uint 职业Buff1 = TODO;
    // public const uint 职业Buff2 = TODO;
}
