using CommandSystem;
using EasyTools.Events;
using EasyTools.Extensions;

namespace EasyTools.Commands.Chat
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class CCommand : ChatCommandBase
    {
        protected override ChatMessage.MessageType MessageType => ChatMessage.MessageType.TeamChat;
        protected override bool Enabled => CustomEventHandler.Config.EnableChatSystem;

        public override string Command => "C";

        public override string[] Aliases => ["CC"];

        public override string Description => "队伍聊天-TeamChat";
    }
}
