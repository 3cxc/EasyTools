using EasyTools.Configs;
using EasyTools.Events;
using GameCore;
using LabApi.Features.Wrappers;
using MEC;
using NewXp.IniApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Log = LabApi.Features.Console.Logger;

namespace EasyTools.BadgeSystem
{
    public class Badge
    {
        public static string[] FMoreColo = new string[]
        {
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
        };

        public static List<Player> rainbw = new List<Player>();

        public static IEnumerator<float> Rainbw()
        {
            while (true)
            {
                foreach (var item in rainbw)
                {
                    item.GroupColor = FMoreColo.RandomItem();
                }
                yield return Timing.WaitForSeconds(CustomEventHandler.BadgeConfig.each);
            }

        }

        public static void Handler(Player player)
        {
            string filePath = Path.Combine(CustomEventHandler.BadgeConfig.Pach, player.UserId + ".ini");

            if (!File.Exists(filePath))
            {
                IniFile iniFile = new IniFile();
                iniFile.Section("DIR").Set("称号", "空", "称号");
                iniFile.Section("DIR").Set("称号颜色", "空");
                iniFile.Section("DIR").Set("管理权限组", "空");
                iniFile.Save(filePath);
            }
            else
            {

                IniFile iniFile = new IniFile(filePath);
                iniFile.Save(filePath);
                if (iniFile.Section("DIR").Get("管理权限组") == "空")
                {
                    if (iniFile.Section("DIR").Get("称号") != "空")
                    {
                        player.GroupName = iniFile.Section("DIR").Get("称号");
                        switch (iniFile.Section("DIR").Get("称号颜色"))
                        {
                            case "rainbow":
                                rainbw.Add(player);
                                break;
                            default:
                                player.GroupColor = iniFile.Section("DIR").Get("称号颜色");
                                break;
                        }
                    }

                }
                else
                {
                    Server.RunCommand($"/setgroup {player.PlayerId} {iniFile.Section("DIR").Get("管理权限组")}");
                    Log.Info($"已经给予{player.Nickname}-{player.UserId}==={iniFile.Section("DIR").Get("管理权限组")}权限");
                    if (iniFile.Section("DIR").Get("称号") != "空")
                    {
                        player.GroupName = iniFile.Section("DIR").Get("称号");
                        switch (iniFile.Section("DIR").Get("称号颜色"))
                        {
                            case "rainbow":
                                rainbw.Add(player);
                                break;
                            default:
                                player.GroupColor = iniFile.Section("DIR").Get("称号颜色");
                                break;
                        }
                    }
                }
            }
        }

        public static void Remove(Player player)
        {
            if (Badge.rainbw.Contains(player))
            {
                Badge.rainbw.Remove(player);
            }
        }

    }
}
