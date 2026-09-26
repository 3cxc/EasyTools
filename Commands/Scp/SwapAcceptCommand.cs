using CommandSystem;
using EasyTools.Events;
using EasyTools.GamePlays;
using LabApi.Features.Wrappers;
using System;

namespace EasyTools.Commands.Scp
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class SwapAcceptCommand : ICommand
    {
        public string Command => "swapaccept";

        public string[] Aliases => ["spa"];

        public string Description => "同意与其他SCP的交换请求";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (sender is null || Player.Get(sender) is not { } player)
            {
                response = CustomEventHandler.TranslateConfig.CommandNotAllowed;
                return false;
            }

            return ScpSwapService.TryAccept(player, out response);
        }
    }
}
