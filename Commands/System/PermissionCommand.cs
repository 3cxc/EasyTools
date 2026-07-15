using CommandSystem;
using EasyTools.API;
using EasyTools.DataStructures;
using EasyTools.Events;
using EasyTools.Extensions;
using LabApi.Features.Wrappers;
using System;

namespace EasyTools.Commands.System
{
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class PermissionCommand : ICommand
    {
        public string Command => "eperm";

        public string[] Aliases => [];

        public string Description => "玩家权限管理";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!CustomEventHandler.Config.EnableAdmin)
            {
                response = CustomEventHandler.TranslateConfig.CommandNotEnabled;
                return false;
            }

            if (arguments.Count == 0)
            {
                response = "请输入要给予权限的 SteamId";
                return false;
            }

            if (!DataAPI.TryGetData(arguments.At(0), out PlayerData data))
            {
                response = "无法查找到玩家";
                return false;
            }

            if (arguments.Count == 1)
            {
                response = "请输入要给予的权限组 (moderator|admin|owner)";
                return false;
            }

            if (!Enum.TryParse(arguments.At(1), true, out PermissionLevel level))
            {
                response = "权限组无效，请输入 (moderator|admin|owner)";
                return false;
            }

            data.PermissionLevel = level;
            data.Badge = level switch
            {
                PermissionLevel.Moderator => "(Lv1.MODERATOR)",
                PermissionLevel.Admin => "(Lv2.ADMIN)",
                PermissionLevel.Owner => "(Lv3.OWNER)",
                _ => data.Badge
            };
            data.BadgeColor = "rainbow";
            data.UpdateData();

            response = $"设置成功";
            return true;
        }
    }
}
