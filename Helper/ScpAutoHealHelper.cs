using EasyTools.Events;
using LabApi.Features.Wrappers;
using MEC;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EasyTools.Helper
{
    public class ScpAutoHealHelper
    {
        public static IEnumerator<float> AutoReal()
        {
            while (!Round.IsRoundEnded && Round.IsRoundStarted)
            {
                foreach (var info in CustomEventHandler.PlayerManager.PlayerList)
                {
                    var p = info.Player;
                    if (p is null || !p.IsSCP) continue;

                    // 检测玩家是否受到伤害
                    float nowHealth = p.Health;
                    if (nowHealth < info.LastHealth)
                    {
                        info.LastDamageTime = DateTime.UtcNow;
                    }
                    info.LastHealth = nowHealth;

                    // 如果玩家正在移动则不回血
                    Vector3 pos = p.Position;
                    if (Vector3.Distance(pos, info.LastPosition) >= 0.1f)
                    {
                        info.LastPosition = pos;
                        info.LastMoveTime = DateTime.UtcNow;
                        continue;
                    }

                    // 距离上一次受伤时间相隔一定时间后才允许回血
                    bool recentlyDamaged = (DateTime.UtcNow - info.LastDamageTime).TotalSeconds < CustomEventHandler.Config.HealATKSecend;

                    // 满足上述条件后，还要站立一定时间才可以回血，此期间不能移动
                    bool stillLongEnough = (DateTime.UtcNow - info.LastMoveTime).TotalSeconds > CustomEventHandler.Config.HealSCPSecend;

                    if (recentlyDamaged || !stillLongEnough) continue;

                    float next = nowHealth + CustomEventHandler.Config.HealSCPQuantity;

                    if (next <= p.MaxHealth)
                    {
                        p.Health = next;
                        info.LastHealth = next;
                    }
                }
                yield return Timing.WaitForSeconds(1f);
            }
        }
    }
}
