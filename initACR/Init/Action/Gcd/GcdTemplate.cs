using Init.Data;
using PromeRotation.Data;
using PromeRotation.Extensions;
using PromeRotation.Resolvers;

namespace Init.Action.Gcd;

/// <summary>
/// GCD Resolver 模板 — 复制此文件创建你的 GCD 技能
///
/// TODO: 替换为你的职业技能逻辑
///
/// Check() 流程:
///   1. 标准前检查链（玩家/目标/距离/读条/QT）
///   2. 技能特定条件（buff/资源/冷却）
///   3. 返回 CheckResult(成功?, "原因")
///
/// GetAction() 返回要放的 PAction
/// </summary>
public class GcdTemplate : IDecisionResolver
{
    public CheckResult Check()
    {
        // === 标准前检查链（根据你的职业调整） ===
        var r = ApiHelper.攻击距离(25);                    // TODO: 调整攻击距离
        var p = ApiHelper.玩家;
        if (p == null) return new(false, "玩家未加载");
        if (ApiHelper.目标 == null) return new(false, "无目标");
        if (ApiHelper.目标!.EntityId == p.EntityId) return new(false, "目标为自己");
        if (ApiHelper.目标.IsPlayer()) return new(false, "目标为玩家");
        if (p.DistanceToMe() > r) return new(false, $"过远(>{r}m)");
        if (ApiHelper.读条中) return new(false, "读条中");
        if (ApiHelper.获取QT(InitQT.停手)) return new(false, "停手");

        // === TODO: 技能特定条件 ===
        // 示例:
        // if (!ApiHelper.获取QT(InitQT.某技能开关)) return new(false, "QT未开");
        // if (!ApiHelper.技能已解锁(InitSkill.某技能)) return new(false, "未解锁");
        // if (!ApiHelper.技能可用(InitSkill.某技能)) return new(false, "冷却中");
        // if (!ApiHelper.玩家有状态(InitBuff.某状态)) return new(false, "缺少buff");

        // return new(true, "就绪");

        return new(false, "TODO: 实现技能逻辑");
    }

    public PAction GetAction()
    {
        // TODO: 返回实际要放的技能
        // return new(InitSkill.某技能, ActionType.Gcd, ActionTargetType.Target);
        return new(0, ActionType.Gcd, ActionTargetType.Target);
    }
}
