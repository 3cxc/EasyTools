using CommandSystem.Commands.RemoteAdmin.Decontamination;
using EasyTools.Configs;
using EasyTools.Utils;
using GameCore;
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
using System.Text;
using System.Threading.Tasks;
using UnityEngine.LowLevel;
using static Broadcast;
using Log = LabApi.Features.Console.Logger;

namespace EasyTools.Events
{
    public class CustomEventHandler : CustomEventsHandler
    {
        public static Config Config;

        public static TranslateConfig TranslateConfig;

        public static BadgeConfig BadgeConfig;
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
            });
        }

        public override void OnServerRoundEnded(RoundEndedEventArgs ev)
        {
            base.OnServerRoundEnded(ev);

            if (BadgeConfig.Enable)
            {
                Timing.KillCoroutines(Badge_Coroutine);
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
                string playerIP = ev.Player.IpAddress;
                string playerInfo = $"[JOIN] Date: {DateTime.Now} | Player: {ev.Player.Nickname} | IP: {playerIP} | Steam64ID: {ev.Player.UserId}";
                Log.Info(playerInfo);

                File.AppendAllText(Config.LoggerSavePath, playerInfo + Environment.NewLine);
            }
            if (BadgeConfig.Enable)
            {
                Badge.Handler(player);
            }

        }
            if (BadgeConfig.Enable)
            {
                Badge.Remove(player);
            }
            
        }

        public override void OnPlayerHurting(PlayerHurtingEventArgs ev)
        {
            if (Config.harmless_207)
            {
                if (ev.DamageHandler is UniversalDamageHandler && ev.DamageHandler.DeathScreenText.Contains("SCP-207"))
                {
                    ev.IsAllowed = false;
                }
            }

            if (Config.harmless_1853)
            {
                if (ev.DamageHandler is UniversalDamageHandler && ev.DamageHandler.DeathScreenText.Contains("poison"))
                {
                    ev.IsAllowed = false;
                }
            }
        }

        public override void OnPlayerLeft(PlayerLeftEventArgs ev)
        {
            Player player = ev.Player;

            if (player == null || string.IsNullOrEmpty(player.UserId)) return;

            if (Config.EnableLogger)
            {
                string playerIP = ev.Player.IpAddress;
                string playerInfo = $"[EXIT] Date: {DateTime.Now} | Player: {ev.Player.Nickname} | IP: {playerIP} | Steam64ID: {ev.Player.UserId}";
                Log.Info(playerInfo);

                File.AppendAllText(Config.LoggerSavePath, playerInfo + Environment.NewLine);
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
                Log.Info(note);
                try
                {
                    if (!File.Exists(Config.AdminLogPath))
                    {
                        FileStream fs1 = new(Config.AdminLogPath, FileMode.Create, FileAccess.Write);
                        StreamWriter sw = new(fs1);
                        sw.WriteLine(note);
                        sw.Close();
                        fs1.Close();
                    }
                    else
                    {
                        FileStream fs = new(Config.AdminLogPath, FileMode.Append, FileAccess.Write);
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
    }
}
