using EasyTools.Configs;
using EasyTools.DataStructures;
using EasyTools.Extensions;
using EasyTools.Helper;
using InventorySystem.Items;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.Scp914Events;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.CustomHandlers;
using LabApi.Features.Wrappers;
using MapGeneration;
using MEC;
using PlayerRoles;
using PlayerStatsSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Log = LabApi.Features.Console.Logger;

namespace EasyTools.Events
{
    public class CustomEventHandler : CustomEventsHandler
    {
        public static Config Config;

        public static TranslateConfig TranslateConfig;

        public static BadgeConfig BadgeConfig;

        public static CustomRoleConfig CustomRoleConfig;

        public static DataBaseConfig DataBaseConfig;

        public static HUDInfoConfig HUDInfoConfig;

        public static CoinConfig CoinConfig;

        public static CoroutineHandle BadgeCoroutine;

        public static readonly Dictionary<Player, PlayerHint> PlayerHuds = new();

        public static HintData Scp914HintData, ElevatorHintData;

        public static DateTime RoundStartTime { get; private set; }

        // SCP交换列表
        public static volatile Dictionary<Player, Player> SwapRequests = new Dictionary<Player, Player>();

        // SCP补位列表
        public static readonly Dictionary<RoleTypeId, ReplacementEntry> Replacements = new();

        public class ReplacementEntry
        {
            public List<Player> Applicants = new();
            public float ExpireTime; // Time.time + 10f
        }

        public override void OnServerWaitingForPlayers()
        {
            base.OnServerWaitingForPlayers();

            if (BadgeConfig.Enable)
            {
                BadgeExtensions.rainbw.Clear();
                BadgeCoroutine = Timing.RunCoroutine(BadgeExtensions.Rainbw());
            }

            Server.FriendlyFire = false;
        }

        public override void OnServerRoundStarted()
        {
            RoundStartTime = DateTime.Now;

            Timing.CallDelayed(10f, () =>
            {
                if (Config.EnableAutoServerMessage)
                {
                    Timing.RunCoroutine(ServerBroadcastHelper.AutoServerBroadcast());
                }

                if (Config.EnableHealSCP)
                {
                    Timing.RunCoroutine(ScpAutoHealHelper.AutoReal());
                }
                PlayerHuds.Values.ToList().ForEach(h => h.Start());
            });

            if (DataBaseConfig.database_enable)
            {
                Timing.RunCoroutine(DataExtensions.CollectInfo());
            }
        }

        public override void OnServerRoundEnded(RoundEndedEventArgs ev)
        {
            base.OnServerRoundEnded(ev);

            if (BadgeConfig.Enable)
            {
                Timing.KillCoroutines(BadgeCoroutine);
            }

            if (Config.EnableFriendFire)
            {
                Server.FriendlyFire = true;
                Server.SendBroadcast($"\n<b><size=25><color=#00CC00>🎉 回合结束，友伤已开启~</color></size></b>", 5);
            }
        }

        public override void OnPlayerJoined(PlayerJoinedEventArgs ev)
        {
            base.OnPlayerJoined(ev);

            Player player = ev.Player;

            if (player == null || string.IsNullOrEmpty(player.UserId)) return;

            player.InitChatHint();

            if (DataBaseConfig.database_enable)
            {
                DataExtensions.PlayerList.Add(player);
                PlayerData data = player.GetData();
                data.NickName = player.Nickname;
                data.LastJoinedTime = DateTime.Now;
                data.UpdateData();
                player.UpdatePlayerNameWithLevelPrefix();
                player.ApplyPermission();
                player.ApplyBadge();
            }

            if (Config.EnableLogger)
            {
                string playerInfo = $"[JOIN] Date: {DateTime.Now} | Player: {ev.Player.Nickname} | IP: {ev.Player.IpAddress} | Steam64ID: {ev.Player.UserId}";
                string path = Path.Combine(CustomEventHandler.Config.PlayerLogPath, $"{Server.Port}.log");
                Log.Info(playerInfo);

                try
                {
                    // 递归创建目录
                    string dir = Path.GetDirectoryName(path);
                    if (!string.IsNullOrEmpty(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }

                    File.AppendAllText(path, playerInfo + Environment.NewLine);
                }
                catch (Exception e)
                {
                    Log.Error(e.Message);
                }
            }

            PlayerHuds[player] = new PlayerHint(player, Scp914HintData, ElevatorHintData);

        }

        public override void OnPlayerLeft(PlayerLeftEventArgs ev)
        {
            Player player = ev.Player;
            string nickName = player.Nickname;
            string userId = player.UserId;

            if (player == null || string.IsNullOrEmpty(player.UserId)) return;

            if (DataBaseConfig.database_enable)
            {
                DataExtensions.PlayerList.Remove(player);
                PlayerData data = player.GetData();
                data.LastJoinedTime = DateTime.Now;
                data.UpdateData();
            }

            if (Config.EnablePlayerLogger)
            {
                string playerInfo = $"[EXIT] Date: {DateTime.Now} | Player: {nickName} | Steam64ID: {userId}";
                string path = Path.Combine(CustomEventHandler.Config.PlayerLogPath, $"{Server.Port}.log");
                Log.Info(playerInfo);

                File.AppendAllText(path, playerInfo + Environment.NewLine);
            }

            if (BadgeConfig.Enable)
            {
                if (BadgeExtensions.rainbw.Contains(player))
                {
                    BadgeExtensions.rainbw.Remove(player);
                }
            }

            if (PlayerHuds.ContainsKey(player))
            {
                PlayerHuds.Remove(player);
            }

            // 清理该玩家在所有补位申请中的记录（防止幽灵申请）
            foreach (var entry in Replacements.Values)
                entry.Applicants.Remove(player);

            if (player.IsSCP && player.Health > 0)
            {
                var role = player.Role;

                Replacements[role] = new ReplacementEntry
                {
                    ExpireTime = Time.time + 10f
                };

                Timing.CallDelayed(10f, () => ExecuteReplacement(role));
            }
        }

        private void ExecuteReplacement(RoleTypeId role)
        {
            if (!Replacements.TryGetValue(role, out var entry))
                return;

            // 从补位名单中筛选仍在线的人类玩家
            var valid = entry.Applicants.Where(p => p != null && p.IsHuman).ToList();

            if (valid.Count == 0)
            {
                Server.SendBroadcast($"<color=orange>{role} 补位无人申请，该角色空缺。</color>", 5);
                return;
            }

            // 随机选择
            var chosen = valid[UnityEngine.Random.Range(0, valid.Count)];
            chosen.Role = role;

            Server.SendBroadcast($"<color=green>补位成功！{chosen.Nickname} 成为了 {role}。</color>", 10);
            Log.Info($"{chosen.Nickname} 补位成为 {role}");
        }

        private static volatile bool allow_spawn_scp_3114 = true; //用以确保不会重复生成SCP-3114

        public override void OnPlayerSpawning(PlayerSpawningEventArgs ev)
        {
            if (CustomRoleConfig.spawn_scp_3114 && allow_spawn_scp_3114)
            {
                if (Player.ReadyList.Count() >= CustomRoleConfig.spawn_scp_3114_limit)
                {
                    foreach (Player p in Player.ReadyList)
                    {
                        if ((UnityEngine.Random.Range(0, 10) == 3))
                        {
                            Timing.CallDelayed(0.5f, () =>
                            {
                                ev.Player.Role = RoleTypeId.Scp3114;
                                ev.IsAllowed = true;
                                allow_spawn_scp_3114 = false;
                            });
                        }
                    }
                }
                allow_spawn_scp_3114 = false;
            }
        }

        public override void OnPlayerSpawned(PlayerSpawnedEventArgs ev)
        {
            Player Player = ev.Player;
            RoleTypeId Role = ev.Role.RoleTypeId;

            Dictionary<RoleTypeId, float> healthDict = new()
            {
                [RoleTypeId.Scp173] = CustomEventHandler.Config.SCPsHP[0],
                [RoleTypeId.Scp939] = CustomEventHandler.Config.SCPsHP[1],
                [RoleTypeId.Scp049] = CustomEventHandler.Config.SCPsHP[2],
                [RoleTypeId.Scp0492] = CustomEventHandler.Config.SCPsHP[3],
                [RoleTypeId.Scp096] = CustomEventHandler.Config.SCPsHP[4],
                [RoleTypeId.Scp106] = CustomEventHandler.Config.SCPsHP[5]
            };

            if (Config.EnableRoundSupplies)
            {
                if (Role is RoleTypeId.ClassD)
                {
                    Timing.CallDelayed(0.5f, () =>
                    {
                        Player.AddItem(Config.ClassDCard, ItemAddReason.AdminCommand);
                    });
                }
            }

            if (Config.EnableChangeSCPHPSystem && Player.IsSCP)
            {
                Timing.CallDelayed(0.5f, () =>
                {
                    if (healthDict.TryGetValue(Role, out var health))
                    {
                        Player.Health = health;
                        Player.MaxHealth = health;
                    }
                });
            }

            if (CoinConfig.EnableRoundCoin)
            {
                if (Player.Role == RoleTypeId.ClassD || Player.Role == RoleTypeId.FacilityGuard || Player.Role == RoleTypeId.Scientist)
                {
                    Timing.CallDelayed(0.5f, () =>
                    {
                        Player.AddItem(ItemType.Coin);
                    });
                }
            }
        }

        public override void OnPlayerDying(PlayerDyingEventArgs ev)
        {
            // 避免处理非玩家击杀或无效情况
            //if (ev.Attacker == null || ev.Player == null)
            //    return;

            // 给击杀者添加经验
            PlayerData data = ev.Attacker.GetData();

            if (ev.Attacker.IsSCP)
            {
                data.PlayerXp += 5.0;
            }
            else if (ev.Player.IsSCP)
            {
                if (ev.Player.Role == RoleTypeId.Scp0492)
                {
                    data.PlayerXp += 30.0;
                }
                else
                {
                    data.PlayerXp += 50.0;
                }
            }
            else
            {
                data.PlayerXp += 5.0;
            }

            data.PlayerLevel = LevelExtensions.GetLevelFromXp(data.PlayerXp, 1);
            ev.Attacker.UpdatePlayerNameWithLevelPrefix();
        }

        public override void OnPlayerHurting(PlayerHurtingEventArgs ev)
        {
            if (Config.Harmless207)
            {
                if (ev.DamageHandler is UniversalDamageHandler && ev.DamageHandler.DeathScreenText.Contains("SCP-207"))
                {
                    ev.IsAllowed = false;
                }
            }

            if (Config.Harmless1853)
            {
                if (ev.DamageHandler is UniversalDamageHandler && ev.DamageHandler.DeathScreenText.Contains("poison"))
                {
                    ev.IsAllowed = false;
                }
            }
        }

        public override void OnPlayerEscaping(PlayerEscapingEventArgs ev)
        {
            if (Config.GuardsCanEscape)
            {
                RoleTypeId id;
                switch (Config.EscapedGuardRole)
                {
                    case "NtfSergeant":
                        id = RoleTypeId.NtfSergeant;
                        break;
                    case "NtfPrivate":
                        id = RoleTypeId.NtfPrivate;
                        break;
                    case "NtfSpecialist":
                        id = RoleTypeId.NtfSpecialist;
                        break;
                    default:
                        id = RoleTypeId.NtfCaptain;
                        break;

                }

                if (ev.Player.Role == RoleTypeId.FacilityGuard)
                {
                    ev.Player.SetRole(id);
                    ev.IsAllowed = true;
                }
            }

            PlayerData data = ev.Player.GetData();
            data.PlayerXp += 20.0;

            data.PlayerLevel = LevelExtensions.GetLevelFromXp(data.PlayerXp, 1);
            ev.Player.UpdatePlayerNameWithLevelPrefix();

            // 通知玩家
            ev.Player.SendBroadcast("\n<b><size=25><color=#00CC00>逃脱成功，获得20经验值</color></size></b>", 3);
        }

        public override void OnPlayerInteractingScp330(PlayerInteractingScp330EventArgs ev)
        {
            if (Config.EnablePinkCandy)
            {
                if (new System.Random().Next(1, Config.PinkCandyWeight + 1) == 1)
                {
                    ev.CandyType = InventorySystem.Items.Usables.Scp330.CandyKindID.Pink;
                    ev.IsAllowed = true;
                }
            }
        }

        public override void OnServerCommandExecuted(CommandExecutedEventArgs ev)
        {
            var sender = ev.Sender;
            var command = ev.Command.Command;

            Player player = Player.Get(sender);

            if (player != null && !string.IsNullOrEmpty(command) && Config.EnableLogger)
            {
                if (!player.RemoteAdminAccess) return;


                string note = $"[AC] Date: {DateTime.Now} | Player: {player.Nickname} | Command: {command} | Steam64ID: {player.UserId}";
                string path = Path.Combine(CustomEventHandler.Config.AdminLogPath, $"{Server.Port}.log");
                Log.Info(note);
                try
                {
                    // 递归创建目录
                    string dir = Path.GetDirectoryName(path);
                    if (!string.IsNullOrEmpty(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }

                    File.AppendAllText(path, note + Environment.NewLine);
                }
                catch (Exception e)
                {
                    Log.Error(e.Message);
                }
            }
        }

        public override void OnPlayerFlippingCoin(PlayerFlippingCoinEventArgs ev)
        {
            if (!ev.IsAllowed || !CoinConfig.Enable) return;

            if (ev.Player.Items.Count() == 8)
            {
                ev.Player.SendBroadcast(TranslateConfig.RewardFailedBroadcastTemplate, 2);
                ev.IsAllowed = false;
                return;
            }

            Player player = ev.Player;

            player.RemoveItem(ItemType.Coin);

            // 按权重随机选择奖励
            RewardSetting reward = CoinConfig.Rewards.PickReward();
            if (reward == null) return;

            // 执行奖励
            string result = player.ApplyReward(reward);

            // 通知玩家
            Server.SendBroadcast(TranslateConfig.RewardOkBroadcastTemplate.Replace("{nickName}", player.Nickname).Replace("{result}", result), 3);
        }

        public override void OnScp914Activating(Scp914ActivatingEventArgs ev)
        {
            if (HUDInfoConfig.EnableScp914Info == false) return;

            Scp914.Scp914KnobSetting knob = ev.KnobSetting;
            string mode = TranslateConfig.Scp914ModeTranslations[knob];

            var p_operator = ev.Player.Nickname ?? "未知";

            string msg = TranslateConfig.Scp914Template.Replace("{mode}", mode)
                                   .Replace("{p_operator}", p_operator);

            foreach (var p in Player.List)
            {
                if (p.IsAlive && p != null && p.Room.Name == RoomName.Lcz914)// 检测914附近玩家，然后告诉他们914正在运行
                {
                    PlayerHuds[p].Show914(msg);
                }
            }
        }

        public override void OnPlayerInteractingElevator(PlayerInteractingElevatorEventArgs ev)
        {
            if (HUDInfoConfig.EnableElevatorInfo == false) return;

            IEnumerable<Player> near = Player.List.Where(p =>
                Vector3.Distance(p.Position, ev.Player.Position) <= HUDInfoConfig.ElevatorHintRange);

            var p_operator = ev.Player.Nickname ?? "未知";
            string text = TranslateConfig.ElevatorTemplate.Replace("{p_operator}", p_operator);
            foreach (var p in near)
            {
                PlayerHuds[p].ShowElevator(text);
            }
        }
    }
}
