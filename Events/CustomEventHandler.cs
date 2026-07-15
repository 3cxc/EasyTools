using EasyTools.BadgeSystem;
﻿using EasyTools.Configs;
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

        public static CoroutineHandle Badge_Coroutine;

        public static readonly Dictionary<Player, PlayerHint> _huds = new();

        public static HintData data_914, data_elevator;

        public static DateTime RoundStartTime { get; private set; }

        // SCP交换列表
        public static volatile Dictionary<Player, Player> SwapRequests = new Dictionary<Player, Player>();
        public override void OnServerWaitingForPlayers()
        {
            base.OnServerWaitingForPlayers();

            if (BadgeConfig.Enable)
            {
                Badge.rainbw.Clear();
                Badge_Coroutine = Timing.RunCoroutine(Badge.Rainbw());
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
                _huds.Values.ToList().ForEach(h => h.Start());
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
                Timing.KillCoroutines(Badge_Coroutine);
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

            if (BadgeConfig.Enable)
            {
                Badge.Handler(player);
            }

            _huds[player] = new PlayerHint(player, data_914, data_elevator);

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

            if (Config.EnableLogger)
            {
                string playerInfo = $"[EXIT] Date: {DateTime.Now} | Player: {nickName} | Steam64ID: {userId}";
                string path = Path.Combine(CustomEventHandler.Config.PlayerLogPath, $"{Server.Port}.log");
                Log.Info(playerInfo);

                File.AppendAllText(path, playerInfo + Environment.NewLine);
            }

            if (BadgeConfig.Enable)
            {
                Badge.Remove(player);
            }

            if (_huds.ContainsKey(player))
            {
                _huds.Remove(player);
            }
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

            if (Config.EnableRoundCoin)
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
            if (!ev.IsAllowed || !Config.Coin) return;

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
            if (HUDInfoConfig.info_914 == false) return;

            Scp914.Scp914KnobSetting knob = ev.KnobSetting;
            string mode = TranslateConfig.scp914_trans[knob];

            var p_operator = ev.Player.Nickname ?? "未知";

            string msg = TranslateConfig.scp914_template.Replace("{mode}", mode)
                                   .Replace("{p_operator}", p_operator);

            foreach (var p in Player.List)
            {
                if (p.IsAlive && p != null && p.Room.Name == RoomName.Lcz914)// 检测914附近玩家，然后告诉他们914正在运行
                {
                    _huds[p].Show914(msg);
                }
            }
        }

        public override void OnPlayerInteractingElevator(PlayerInteractingElevatorEventArgs ev)
        {
            if (HUDInfoConfig.info_elevator == false) return;

            IEnumerable<Player> near = Player.List.Where(p =>
                Vector3.Distance(p.Position, ev.Player.Position) <= HUDInfoConfig.elev_range);

            var p_operator = ev.Player.Nickname ?? "未知";
            string text = TranslateConfig.elev_template.Replace("{p_operator}", p_operator);
            foreach (var p in near)
            {
                _huds[p].ShowElevator(text);
            }
        }
    }
}
