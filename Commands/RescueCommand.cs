using CommandSystem;
using EasyTools.Configs;
using EasyTools.Events;
using LabApi.Features.Wrappers;
using RelativePositioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyTools.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class RescueCommand : ICommand
    {
        public string Command => "killme";

        public string[] Aliases => ["suicide"];

        public string Description => "防卡死命令";

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
