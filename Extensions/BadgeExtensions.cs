using EasyTools.DataStructures;
using EasyTools.Events;
using LabApi.Features.Wrappers;
using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using Log = LabApi.Features.Console.Logger;

namespace EasyTools.Extensions
{
    public static class BadgeExtensions
    {
        public static string[] FMoreColo =
        [
            "pink",
            "silver",
            "cyan",
            "aqua",
            "tomato",
            "yellow",
            "magenta",
            "orange",
            "lime",
            "green",
            "red",
            "brown",
            "red",
            "orange",
            "yellow",
            "green",
            "blue_green",
            "magenta",
            "pink",
            "brown",
            "silver",
            "light_green",
            "crimson",
            "cyan",
            "aqua",
            "deep_pink",
            "tomato",
            "blue_green",
            "lime",
            "emerald",
            "carmine",
            "nickel",
            "mint",
            "army_green",
            "pumpkin"
        ];

        public static IEnumerator<float> Rainbw()
        {
            while (true)
            {
                foreach (var item in CustomEventHandler.PlayerManager.PlayerList.Where(info => info.RainbowBadge))
                {
                    item.Player.GroupColor = FMoreColo.RandomItem();
                }
                yield return Timing.WaitForSeconds(CustomEventHandler.BadgeConfig.Each);
            }
        }

        public static void ApplyBadge(this Player player)
        {
            PlayerData data = player.GetData();
            if (data.Badge == "") return;

            player.GroupName = data.Badge;

            if (string.IsNullOrEmpty(data.BadgeColor)) return;

            if (data.BadgeColor == "rainbow")
            {
                if (CustomEventHandler.PlayerManager.TryGet(player, out var info))
                    info.RainbowBadge = true;
            }
            else
            {
                player.GroupColor = data.BadgeColor;
            }
        }

        public static void ApplyPermission(this Player player)
        {
            PlayerData data = player.GetData();
            if (data.PermissionLevel != PermissionLevel.Player)
            {
                string group;
                switch (data.PermissionLevel)
                {
                    case PermissionLevel.Moderator:
                        group = "moderator";
                        break;
                    case PermissionLevel.Admin:
                        group = "admin";
                        break;
                    case PermissionLevel.Owner:
                        group = "owner";
                        break;
                    default:
                        group = "moderator";
                        break;
                };

                Server.RunCommand($"/setgroup {player.PlayerId} {group}");

                if (CustomEventHandler.Config.EnableAdminLogger)
                {
                    Log.Info($"[AC] Date: {DateTime.Now} | Player: {player.Nickname} | Perm: {group} | Steam64ID: {player.UserId}");
                }
            }
        }
    }
}
