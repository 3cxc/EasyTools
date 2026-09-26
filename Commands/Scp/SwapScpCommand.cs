using CommandSystem;
using EasyTools.Events;
using EasyTools.GamePlays;
using LabApi.Features.Wrappers;
using PlayerRoles;
using System;

namespace EasyTools.Commands.Scp
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class SwapScpCommand : ICommand
    {
        public string Command => "swap";

        public string[] Aliases => ["sp"];

        public string Description => "申请与其他SCP交换";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (sender is null || Player.Get(sender) is not { } player)
            {
                response = CustomEventHandler.TranslateConfig.CommandNotAllowed;
                return false;
            }

            if (arguments.Count == 0)
            {
                response = "失败，未指定你要交换的目标(Scp079|Scp096|Scp106|Scp173|Scp049|Scp3114)";
                return false;
            }

            string input = arguments.At(0);
            if (!Enum.TryParse(input, out RoleTypeId targetRole))
            {
                response = $"无效的 SCP 编号：{input}。可用的有 Scp079|Scp096|Scp106|Scp173|Scp049|Scp3114 （必须带Scp）";
                return false;
            }

            return ScpSwapService.TryRequest(player, targetRole, out response);
        }
    }
}
