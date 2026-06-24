using System.Collections.Generic;
using Init.Data;
using PromeRotation.Data;
using PromeRotation.Rotation;

namespace Init.Opener;

/// <summary>
/// 起手模板 — 复制此文件创建更多起手
///
/// TODO: 替换为你的职业起手序列
///
/// 起手由两部分组成:
///   1. InitializeCountdown — 倒计时阶段预放技能
///   2. InCombatSequence  — 开怪后的固定序列
/// </summary>
public class OpenerTemplate : IOpener
{
    public string OpenerName => "Init_起手模板";  // TODO: 修改起手名称

    /// <summary>
    /// 倒计时阶段技能登记
    /// </summary>
    public void InitializeCountdown(CountDownHandler countdownHandler)
    {
        // TODO: 倒计时阶段预放技能
        // 伪代码:
        // countdownHandler.AddAction(3500, O(InitSkill.某技能, ActionTargetType.Self));  // 剩余3.5秒放
        // if (InitSettings.Instance.UsePotionInOpener)
        //     countdownHandler.AddAction(GrabItLimit + 1000,
        //         new PAction(ApiHelper.最佳爆发药, ActionType.Item, ActionTargetType.Self));
        // countdownHandler.AddAction(GrabItLimit, G(InitSkill.起手GCD));  // 抢开
    }

    /// <summary>
    /// 开怪后起手序列（框架按顺序执行）
    /// </summary>
    public List<PAction> InCombatSequence
    {
        get
        {
            // TODO: 检查起手条件（技能是否就绪等）
            // if (!InitOpenerChecker.CheckOpenerReady(InitSkill.某技能)) return new List<PAction>();

            var s = new List<PAction>();

            // TODO: 编排起手 GCD + oGCD 序列
            // 伪代码:
            // s.Add(G(InitSkill.起手GCD));           // GCD
            // s.Add(O(InitSkill.能力技1));           // 插 oGCD
            // s.Add(O(InitSkill.能力技2));           // 插 oGCD
            // s.Add(G(InitSkill.第二个GCD));          // GCD
            // s.Add(O(InitSkill.能力技3));           // 插 oGCD
            // ...

            return s;
        }
    }

    // === 快捷方法 ===
    private static PAction G(uint id) => new(id, ActionType.Gcd, ActionTargetType.Target) { RequiresVerification = true };
    private static PAction O(uint id, ActionTargetType t = ActionTargetType.Target) => new(id, ActionType.OffGcd, t);
}
