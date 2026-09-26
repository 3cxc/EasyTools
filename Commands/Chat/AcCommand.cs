using CommandSystem;
using EasyTools.Events;
using EasyTools.Extensions;

namespace EasyTools.Commands.Chat
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class AcCommand : ChatCommandBase
    {
        protected override ChatMessage.MessageType MessageType => ChatMessage.MessageType.AdminPrivateChat;
        protected override bool Enabled => CustomEventHandler.Config.EnableAcSystem;

        public override string Command => "ac";

        public override string[] Aliases => [];

        public override string Description => "私聊管理-Talk to Admin";
    }
}
