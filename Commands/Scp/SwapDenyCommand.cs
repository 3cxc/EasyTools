using CommandSystem;
using EasyTools.Events;
using EasyTools.GamePlays;
using LabApi.Features.Wrappers;
using System;

namespace EasyTools.Commands.Scp
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class SwapDenyCommand : ICommand
    {
        public string Command => "swapdeny";

        public string[] Aliases => ["spd"];

        public string Description => "拒绝与其他SCP的交换请求";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (sender is null || Player.Get(sender) is not { } player)
            {
                response = CustomEventHandler.TranslateConfig.CommandNotAllowed;
                return false;
            }

            return ScpSwapService.TryDeny(player, out response);
        }
    }
}
