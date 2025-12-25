using CommandSystem;
using EasyTools.Configs;
using EasyTools.Events;
using EasyTools.Utils;
using LabApi.Features.Wrappers;
using System;
using Log = LabApi.Features.Console.Logger;

namespace EasyTools.Commands.Chat
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class AcCommand : ICommand
    {
        public string Command => "ac";

        public string[] Aliases => [];

        public string Description => "私聊管理-Talk to Admin";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            TranslateConfig TranslateConfig = CustomEventHandler.TranslateConfig;
            Player player;

            if (sender is null || (player = Player.Get(sender)) is null)
            {
                response = TranslateConfig.ChatCommandError;
                return false;
            }

            if (arguments.Count == 0 || player.IsMuted || !CustomEventHandler.Config.EnableAcSystem)
            {
                response = TranslateConfig.ChatCommandFailed;
                return false;
            }

            ChatUtils.SendMessage(player, ChatMessage.MessageType.AdminPrivateChat, $"<noparse>{string.Join(" ", arguments)}</noparse>");

            Log.Info(player.Nickname + " 发送了 " + arguments.At(0));
            response = TranslateConfig.ChatCommandOk;
            return true;
        }
    }
}
