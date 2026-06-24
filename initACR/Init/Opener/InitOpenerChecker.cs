using Init.Data;

namespace Init.Opener;

/// <summary>
/// 起手条件校验 — 判断起手所需技能是否全部就绪
///
/// TODO: 根据你的起手需求实现校验逻辑
/// </summary>
public static class InitOpenerChecker
{
    /// <summary>
    /// 校验起手爆发技能是否全部就绪
    /// </summary>
    /// <param name="skipSkillId">可选跳过的技能 ID（如果起手第一个 GCD 不同）</param>
    /// <returns>所有必要技能是否就绪</returns>
    public static bool CheckOpenerReady(uint skipSkillId)
    {
        // TODO: 实现你的起手条件校验
        // 伪代码:
        // var me = ApiHelper.玩家;
        // if (me == null) return false;
        // var lv = me.Level;
        //
        // // 检查核心技能是否就绪
        // if (!ApiHelper.技能已解锁(InitSkill.某技能) || !ApiHelper.技能可用(InitSkill.某技能)) return false;
        // if (skipSkillId != InitSkill.某技能)
        // {
        //     if (!ApiHelper.技能已解锁(InitSkill.另一个技能) || !ApiHelper.技能可用(InitSkill.另一个技能))
        //         return false;
        // }
        // // ... 更多检查
        //
        // return true;

        return false; // TODO
    }
}
