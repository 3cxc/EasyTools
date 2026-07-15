using CommandSystem;
using EasyTools.API;
using EasyTools.DataStructures;
using EasyTools.Events;
using EasyTools.Extensions;
using LabApi.Features.Wrappers;
using System;

namespace EasyTools.Commands.System
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class BadgeCommand : ICommand
    {
        public string Command => "badge";

        public string[] Aliases => [];

        public string Description => "设置玩家称号";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player;

            if (sender is null || (player = Player.Get(sender)) is null)
            {
                response = CustomEventHandler.TranslateConfig.CommandFailed;
                return false;
            }

            PlayerData data = player.GetData();

            if (data.PermissionLevel == PermissionLevel.Player)
            {
                response = CustomEventHandler.TranslateConfig.CommandNotAllowed;
                return false;
            }

            if (arguments.Count == 0)
            {
                response = "请输入要给予称号的 SteamId";
                return false;
            }

            if (!DataAPI.TryGetData(arguments.At(0), out PlayerData targetData))
            {
                response = "无法查找到玩家";
                return false;
            }

            if (arguments.Count < 2)
            {
                response = "请输入要设置的称号";
                return false;
            }

            if (targetData.PermissionLevel != PermissionLevel.Player)
            {
                switch (targetData.PermissionLevel)
                {
                    case PermissionLevel.Moderator:
                        targetData.Badge = $"{arguments.At(1)}(Lv{(int)PermissionLevel.Moderator}.MODERATOR)";
                        break;
                    case PermissionLevel.Admin:
                        targetData.Badge = $"{arguments.At(1)}(Lv{(int)PermissionLevel.Admin}.ADMIN)";
                        break;
                    case PermissionLevel.Owner:
                        targetData.Badge = $"{arguments.At(1)}(Lv{(int)PermissionLevel.Owner}.OWNER)";
                        break;
                    default:
                        targetData.Badge = $"{arguments.At(1)}(Lv{(int)PermissionLevel.Moderator}.MODERATOR)";
                        break;
                }
            }
            else targetData.Badge = arguments.At(1);

            // 无颜色参数时默认为 rainbow
            string color = arguments.Count >= 3 ? arguments.At(2) : "rainbow";
            targetData.BadgeColor = color;

            targetData.UpdateData();

            string colorDisplay = color == "rainbow" ? "彩虹色" : color;
            response = $"已将 {arguments.At(0)} 的称号设置为 {targetData.Badge}，颜色为 {colorDisplay}";
            return true;
        }
    }
}
