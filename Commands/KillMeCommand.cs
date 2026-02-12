using CommandSystem;
using EasyTools.Configs;
using EasyTools.Events;
using LabApi.Features.Wrappers;
using RelativePositioning;
using System;

namespace EasyTools.Commands
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
                response = TranslateConfig.RescueCommandError;
                return false;
            }

            WaypointBase.GetRelativePosition(player.Position, out byte id, out _);

            if (!player.IsAlive || !CustomEventHandler.Config.KillMeCommand)
            {
                response = TranslateConfig.RescueCommandFailed;
                return false;
            }

            player.Kill();

            response = TranslateConfig.RescueCommandOk;
            return true;
        }
    }
}
