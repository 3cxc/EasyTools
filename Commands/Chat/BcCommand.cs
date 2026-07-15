using CommandSystem;
using EasyTools.Configs;
using EasyTools.Events;
using EasyTools.Extensions;
using LabApi.Features.Wrappers;
using System;
using Log = LabApi.Features.Console.Logger;

namespace EasyTools.Commands.Chat
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class BcCommand : ICommand
    {
        public string Command => "BC";

        public string[] Aliases => [];

        public string Description => "全服聊天-PublicChat";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            TranslateConfig TranslateConfig = CustomEventHandler.TranslateConfig;
            Player player;

            if (sender is null || (player = Player.Get(sender)) is null)
            {
                response = TranslateConfig.CommandNotAllowed;
                return false;
            }

            if (arguments.Count == 0 || player.IsMuted || !CustomEventHandler.Config.EnableChatSystem)
            {
                response = TranslateConfig.ChatCommandFailed;
                return false;
            }

            player.SendHintMessage(ChatMessage.MessageType.BroadcastChat, $"<noparse>{string.Join(" ", arguments)}</noparse>");

            Log.Info(player.Nickname + " 发送了 " + arguments.At(0));
            response = TranslateConfig.ChatCommandOk;
            return true;
        }
    }
}
