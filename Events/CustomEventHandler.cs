using EasyTools.BadgeSystem;
using EasyTools.Configs;
using EasyTools.Utils;
using InventorySystem.Items;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.CustomHandlers;
using LabApi.Features.Wrappers;
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

        public static CoroutineHandle Badge_Coroutine;

        public override void OnServerWaitingForPlayers()
        {
            base.OnServerWaitingForPlayers();

            if (BadgeConfig.Enable)
            {
                Badge.rainbw.Clear();
                Badge_Coroutine = Timing.RunCoroutine(Badge.Rainbw());
            }
        }

        public override void OnServerRoundStarted()
        {
            Timing.CallDelayed(10f, () =>
            {
                if (Config.EnableAutoServerMessage)
                {
                    Timing.RunCoroutine(Util.AutoServerBroadcast());
                }

                if (Config.EnableHealSCP)
                {
                    Timing.RunCoroutine(ScpReal.AutoReal());
                }
            });
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

            ChatUtils.InitForPlayer(player);

            if (Config.EnableLogger)
            {
                string playerInfo = $"[JOIN] Date: {DateTime.Now} | Player: {player.Nickname} | IP: {player.IpAddress} | Steam64ID: {player.UserId}";
                string path = Path.Combine(CustomEventHandler.Config.PlayerLogPath, $"{Server.Port}.ini");
                Log.Info(playerInfo);

                File.AppendAllText(path, playerInfo + Environment.NewLine);
            }
            if (BadgeConfig.Enable)
            {
                Badge.Handler(player);
            }

        }

        public override void OnPlayerLeft(PlayerLeftEventArgs ev)
        {
            Player player = ev.Player;

            if (player == null || string.IsNullOrEmpty(player.UserId)) return;

            if (Config.EnableLogger)
            {
                string playerInfo = $"[EXIT] Date: {DateTime.Now} | Player: {player.Nickname} | IP: {player.IpAddress} | Steam64ID: {player.UserId}";
                string path = Path.Combine(CustomEventHandler.Config.PlayerLogPath, $"{Server.Port}.ini");
                Log.Info(playerInfo);

                File.AppendAllText(path, playerInfo + Environment.NewLine);
            }

            if (BadgeConfig.Enable)
            {
                Badge.Remove(player);
            }

            {
                _huds.Remove(player);
            }
        }

        public static bool scp_3114_spawned = false; //用以确保不会重复生成SCP-3114

        public override void OnPlayerSpawning(PlayerSpawningEventArgs ev)
        {
            if (CustomRoleConfig.spawn_scp_3114 && Player.ReadyList.Count() >= CustomRoleConfig.spawn_scp_3114_limit && !scp_3114_spawned)
            {
                foreach (Player p in Player.ReadyList)
                {
                    bool weaponIndex = UnityEngine.Random.Range(0, 10) == 3;
                    if (weaponIndex)
                    {
                        Timing.CallDelayed(0.5f, () =>
                        {
                            ev.Player.Role = RoleTypeId.Scp3114;
                            ev.IsAllowed = true;
                            scp_3114_spawned = true;
                        });
                    }
                }
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

            if (Config.PinkCandyRespawn && Player.IsHuman)
            {
                Timing.CallDelayed(0.5f, () =>
                {
                    Player.GiveCandy(InventorySystem.Items.Usables.Scp330.CandyKindID.Pink, ItemAddReason.AdminCommand);
                });
            }

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


                string note = $"Date: {DateTime.Now} | Player: {player.Nickname} | Command: {command} | Steam64ID: {player.UserId}";
                string path = Path.Combine(CustomEventHandler.Config.AdminLogPath, $"{Server.Port}.ini");
                Log.Info(note);
                try
                {
                    if (!File.Exists(path))
                    {
                        FileStream fs1 = new(path, FileMode.Create, FileAccess.Write);
                        StreamWriter sw = new(fs1);
                        sw.WriteLine(note);
                        sw.Close();
                        fs1.Close();
                    }
                    else
                    {
                        FileStream fs = new(path, FileMode.Append, FileAccess.Write);
                        StreamWriter sr = new(fs);
                        sr.WriteLine(note);
                        sr.Close();
                        fs.Close();
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e.Message);
                }
            }
        }

        public override void OnPlayerFlippingCoin(PlayerFlippingCoinEventArgs ev)
        {
            if (!ev.IsAllowed) return;
            if (!Config.Coin) return;
            if (ev.Player.Items.Count() == 8)
            {
                ev.Player.SendBroadcast($"\n<b><size=25><color=#00CC00>你的背包空间不足，无法继续抽卡！</color></size></b>", 2);
                return;
            }

            Player player = ev.Player;

            player.RemoveItem(ItemType.Coin);

            // 生成0-100的随机数
            float randomValue = UnityEngine.Random.Range(0f, 100f);

            // 初始化奖励变量
            string rewardName = "";
            bool success = false;

            // 概率判断（从高到低）
            if (randomValue < 0.3f) // 0.3% 变成SCP
            {
                int scpType = UnityEngine.Random.Range(0, 4);
                switch (scpType)
                {
                    case 0:
                        player.Role = RoleTypeId.Scp939;
                        rewardName = "变成了狗子";
                        break;
                    case 1:
                        player.Role = RoleTypeId.Scp3114;
                        rewardName = "变成了3114";
                        break;
                    case 2:
                        player.Role = RoleTypeId.Scp106;
                        rewardName = "变成了老头";
                        break;
                    case 3:
                        player.Role = RoleTypeId.Scp049;
                        rewardName = "变成了49";
                        break;
                    default:
                        player.Role = RoleTypeId.Scp939;
                        rewardName = "变成了狗子";
                        break;
                }
                success = true;
            }
            else if (randomValue < 1f && !success) // 0.7% 特殊武器
            {
                int weaponIndex = UnityEngine.Random.Range(0, 3);

                switch (weaponIndex)
                {
                    case 0: // 127
                        player.AddItem(ItemType.GunSCP127);
                        rewardName = "获得了127";
                        break;
                    case 1: // 3X
                        player.AddItem(ItemType.ParticleDisruptor);
                        rewardName = "获得了3X";
                        break;
                    case 2: // 电炮
                        player.AddItem(ItemType.MicroHID);
                        rewardName = "获得了电炮";
                        break;
                    default:
                        player.AddItem(ItemType.GunSCP127);
                        rewardName = "获得了127";
                        break;
                }
                success = true;
            }
            else if (randomValue < 5f && !success) // 4% 黑卡
            {
                player.AddItem(ItemType.KeycardO5);
                rewardName = "获得了O5卡";
                success = true;
            }
            else if (randomValue < 10f && !success) // 5% 可乐
            {
                player.AddItem(ItemType.SCP207);
                rewardName = "获得了可乐";
                success = true;
            }
            else if (randomValue < 15f && !success) // 5% 粉糖
            {
                player.GiveCandy(InventorySystem.Items.Usables.Scp330.CandyKindID.Pink, ItemAddReason.Undefined);
                rewardName = "获得了粉糖";
                success = true;
            }
            else if (randomValue < 25f && !success) // 10% 枪
            {
                bool weaponIndex = UnityEngine.Random.Range(0, 2) == 0;
                if (weaponIndex)
                {
                    player.AddItem(ItemType.GunFRMG0);
                    rewardName = "获得了狗官枪";
                }
                else
                {
                    player.AddItem(ItemType.GunLogicer);
                    rewardName = "获得了大机枪";
                }
                success = true;
            }
            else if (randomValue < 35f && !success) // 10% 医疗
            {
                bool healthIndex = UnityEngine.Random.Range(0, 2) == 0;

                if (healthIndex)
                {
                    player.AddItem(ItemType.SCP500);
                }
                else
                {
                    player.AddItem(ItemType.Medkit);
                }
                rewardName = "获得了医疗物品";
                success = true;
            }
            else if (randomValue < 45f && !success) // 10% 红卡
            {
                player.AddItem(ItemType.KeycardFacilityManager);
                rewardName = "获得了设施总监卡";
                success = true;
            }
            else if (randomValue < 60f && !success) // 15% 随机传送
            {
                foreach (Player p in Player.ReadyList)
                {
                    if (p.IsSCP)
                    {
                        player.Position = p.Position + Vector3.right;
                        player.Rotation = p.Rotation;
                    }
                }
                rewardName = "被传送到SCP旁边";
                success = true;
            }
            else if (randomValue < 80f && !success) // 20% 手雷
            {
                player.AddItem(ItemType.GrenadeHE);
                rewardName = "获得了手雷";
                success = true;
            }
            else if (!success) // 20% 老头空间
            {
                if (!PocketDimension.IsPlayerInside(player))
                {
                    PocketDimension.ForceInside(player);
                    rewardName = "传送到老头空间";
                }
                else
                {
                    PocketDimension.ForceExit(player);
                    rewardName = "离开了老头空间";
                }
                success = true;
            }

            // 通知玩家
            if (success)
            {
                Server.SendBroadcast($"\n<b><size=25><color=#00CC00>🎉 恭喜！玩家 {player.Nickname} 通过抛硬币{rewardName}！</color></size></b>", 3);
            }

        }
    }
}
