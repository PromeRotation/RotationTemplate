namespace Init.Data;

/// <summary>
/// 战斗缓存数据（单场战斗内有效，脱战/切地图时 Reset）
///
/// TODO: 添加你的职业需要的战斗内临时状态
/// 示例: 是否已触发某机制、是否已放某技能、连击计数等
/// </summary>
public class InitBattleData
{
    public static InitBattleData Instance { get; set; } = new();

    // TODO: 添加战斗内临时字段
    // 示例:
    // public bool SomeFlag = false;
    // public int ComboCounter = 0;
    // public DateTime LastSomeActionTime = DateTime.MinValue;

    /// <summary>重置所有战斗数据（脱战/切地图时调用）</summary>
    public void Reset()
    {
        // TODO: 重置所有字段到默认值
        // SomeFlag = false;
        // ComboCounter = 0;
        // LastSomeActionTime = DateTime.MinValue;
    }
}
