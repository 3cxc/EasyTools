using CommandSystem;
using EasyTools.Events;
using EasyTools.Extensions;

namespace EasyTools.Commands.Chat
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class BcCommand : ChatCommandBase
    {
        protected override ChatMessage.MessageType MessageType => ChatMessage.MessageType.BroadcastChat;
        protected override bool Enabled => CustomEventHandler.Config.EnableChatSystem;

        public override string Command => "BC";
        public override string[] Aliases => [];
        public override string Description => "全服聊天-PublicChat";
    }
}
