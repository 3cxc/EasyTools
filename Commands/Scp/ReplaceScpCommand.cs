using CommandSystem;
using EasyTools.Events;
using EasyTools.GamePlays;
using LabApi.Features.Wrappers;
using PlayerRoles;
using System;
using UnityEngine;

namespace EasyTools.Commands.Scp
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class ReplaceScpCommand : ICommand
    {
        public string Command => "replace";

        public string[] Aliases => ["rscp"];

        public string Description => "申请补位一个断线SCP";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (sender is null || Player.Get(sender) is not { } player || player.IsSCP)
            {
                response = CustomEventHandler.TranslateConfig.CommandNotAllowed;
                return false;
            }

            if (!CustomEventHandler.Config.EnableSCPReplace)
            {
                response = CustomEventHandler.TranslateConfig.CommandNotEnabled;
                return false;
            }

            if (arguments.Count == 0)
            {
                response = "请指定要补位的 SCP 编号(Scp079|Scp096|Scp106|Scp173|Scp049|Scp3114)";
                return false;
            }

            string input = arguments.At(0);
            if (!Enum.TryParse(input, out RoleTypeId targetRole))
            {
                response = $"无效的 SCP 编号：{input}。可用的有 Scp079|Scp096|Scp106|Scp173|Scp049|Scp3114 （必须带Scp）";
                return false;
            }

            return ScpReplaceService.TryApply(player, targetRole, out response);
        }
    }
}
