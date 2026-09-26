using CommandSystem;
using EasyTools.Events;
using EasyTools.Extensions;
using LabApi.Features.Wrappers;
using System;
using Log = LabApi.Features.Console.Logger;

namespace EasyTools.Commands.Chat
{
    public abstract class ChatCommandBase : ICommand
    {
        protected abstract ChatMessage.MessageType MessageType { get; }
        protected abstract bool Enabled { get; }

        public abstract string Command { get; }
        public abstract string[] Aliases { get; }
        public abstract string Description { get; }

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            var cfg = CustomEventHandler.TranslateConfig;

            if (sender is null || Player.Get(sender) is not { } player)
            {
                response = cfg.CommandNotAllowed;
                return false;
            }

            if (arguments.Count == 0 || player.IsMuted || !Enabled)
            {
                response = cfg.ChatCommandFailed;
                return false;
            }

            string msg = string.Join(" ", arguments);
            player.SendHintMessage(MessageType, $"<noparse>{msg}</noparse>");

            Log.Info($"{player.Nickname} 发送了 {msg}");
            response = cfg.ChatCommandOk;
            return true;
        }
    }
}
