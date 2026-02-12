using EasyTools.Events;
using LabApi.Features.Wrappers;
using MEC;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EasyTools.Utils
{
    public class ScpReal
    {

        private static readonly Dictionary<Player, (Vector3 pos, DateTime time)> _lastMove = [];

        private static readonly Dictionary<Player, float> _lastHealth = [];
        private static readonly Dictionary<Player, DateTime> _lastDamageTime = [];

        public static IEnumerator<float> AutoReal()
        {
            while (true)
            {
                if (Round.IsRoundEnded || !Round.IsRoundStarted)
                {
                    yield break;
                }

                foreach (Player p in Player.ReadyList)
                {
                    if (p.IsSCP)
                    {

                        // 先检测玩家是否正在受到伤害
                        if (_lastHealth.TryGetValue(p, out var lastHealth))
                        {
                            if (p.Health < lastHealth)
                            {
                                _lastDamageTime[p] = DateTime.UtcNow;
                            }
                        }

                        _lastHealth[p] = lastHealth;

                        Vector3 pos = p.Position;
                        if (_lastMove.TryGetValue(p, out var last))
                        {
                            if (Vector3.Distance(pos, last.pos) < 0.1f)
                            {

                                bool canceled = false;
                                // 检测是否正在受伤
                                if (_lastDamageTime.TryGetValue(p, out var lastDamageTime))
                                {
                                    if (DateTime.UtcNow - lastDamageTime < TimeSpan.FromSeconds(CustomEventHandler.Config.heal_atk_secend)) { canceled = true; }
                                }

                                if (!canceled && DateTime.UtcNow - last.time > TimeSpan.FromSeconds(CustomEventHandler.Config.heal_scp_secend))
                                {
                                    float old_health = p.Health;
                                    float new_health = old_health + CustomEventHandler.Config.heal_scp_x;
                                    if (new_health <= p.MaxHealth)
                                    {
                                        p.Health = new_health;
                                    }
                                }
                            }
                            else { _lastMove[p] = (pos, DateTime.UtcNow); }
                        }
                        else { _lastMove[p] = (pos, DateTime.UtcNow); }
                    }
                    else if (_lastMove.ContainsKey(p)) { _lastMove.Remove(p); }
                }
                yield return Timing.WaitForSeconds(1f);
            }
        }
    }
}
