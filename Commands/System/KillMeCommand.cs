using CommandSystem;
using EasyTools.Configs;
using EasyTools.Events;
using LabApi.Features.Wrappers;
using RelativePositioning;
using System;

namespace EasyTools.Commands.System
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class KillMeCommand : ICommand
    {
        public string Command => "killme";

        public string[] Aliases => ["suicide"];

        public string Description => "自救命令";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player;
            TranslateConfig TranslateConfig = CustomEventHandler.TranslateConfig;

            if (sender is null || (player = Player.Get(sender)) is null)
            {
                response = TranslateConfig.CommandFailed;
                return false;
            }

            WaypointBase.GetRelativePosition(player.Position, out byte id, out _);

            if (!CustomEventHandler.Config.KillMeCommand)
            {
                response = TranslateConfig.CommandNotEnabled;
                return false;
            }

            if (!player.IsAlive)
            {
                response = TranslateConfig.CommandNotAllowed;
                return false;
            }

            player.Kill();

            response = TranslateConfig.CommandOk;
            return true;
        }
    }
}
