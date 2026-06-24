using Init.Data;
using PromeRotation.Data;
using PromeRotation.Resolvers;

namespace Init.Action.OffGcd;

/// <summary>
/// oGCD Resolver 模板 — 复制此文件创建你的 oGCD 技能
///
/// TODO: 替换为你的职业技能逻辑
///
/// Check() 流程:
///   1. 标准前检查链（玩家/读条/GCD窗口）
///   2. 技能特定条件
///   3. 返回 CheckResult(成功?, "原因")
/// </summary>
public class OffGcdTemplate : IDecisionResolver
{
    public CheckResult Check()
    {
        if (ApiHelper.玩家 == null) return new(false, "玩家未加载");
        if (ApiHelper.读条中) return new(false, "读条中");

        // TODO: GCD 窗口检查（能力技需要在 GCD 后半插）
        // if (ApiHelper.GCD剩余 * 1000f < 600f) return new(false, "GCD窗口短");

        // TODO: 设置项检查
        // if (!InitSettings.Instance.某设置) return new(false, "设置关闭");

        // TODO: 技能可用性检查
        // if (!ApiHelper.技能已解锁(InitSkill.某技能)) return new(false, "未解锁");
        // if (!ApiHelper.技能可用(InitSkill.某技能)) return new(false, "冷却中");
        // if (ApiHelper.最近用过(InitSkill.某技能, 1000)) return new(false, "刚用过");

        // TODO: 触发条件
        // var hp = ApiHelper.玩家血量;
        // if (hp > InitSettings.Instance.血量阈值) return new(false, $"血量({hp:F0}%)>{阈值}%");

        // return new(true, "就绪");

        return new(false, "TODO: 实现 oGCD 逻辑");
    }

    public PAction GetAction()
    {
        // TODO: 返回实际要放的技能
        // return ApiHelper.自我能力(InitSkill.某技能);
        return new(0, ActionType.OffGcd, ActionTargetType.Self);
    }
}
