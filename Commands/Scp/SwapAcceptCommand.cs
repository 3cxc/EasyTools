using CommandSystem;
using EasyTools.Events;
using LabApi.Features.Wrappers;
using System;
using Log = LabApi.Features.Console.Logger;

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
            Player player;

            if (sender is null || (player = Player.Get(sender)) is null || !(player = Player.Get(sender)).IsSCP)
            {
                response = CustomEventHandler.TranslateConfig.CommandNotAllowed;
                return false;
            }

            if (CustomEventHandler.Config.EnableSCPStartExchange)
            {
                response = CustomEventHandler.TranslateConfig.CommandNotEnabled;
                return false;
            }

            if ((DateTime.Now - CustomEventHandler.RoundStartTime).TotalSeconds > CustomEventHandler.Config.SCPStartExchangeTime)
            {
                response = CustomEventHandler.TranslateConfig.SwapCommandTimeLimitBroadcastTemplate;
                return false;
            }

            // 检查是否有发给自己的请求
            if (!CustomEventHandler.SwapRequests.TryGetValue(player, out Player requester))
            {
                response = CustomEventHandler.TranslateConfig.SwapCommandNoRequestBroadcastTemplate;
                return false;
            }

            // 检查发起请求的玩家是否仍然在线且为 SCP
            if (requester == null || !requester.IsSCP)
            {
                CustomEventHandler.SwapRequests.Remove(player);
                response = "请求已失效，对方已离线或不再是 SCP。";
                return false;
            }

            // 交换并从列表中移除
            var tempRole = requester.Role;
            requester.Role = player.Role;
            player.Role = tempRole;

            CustomEventHandler.SwapRequests.Remove(player);

            // 通知玩家
            string msg = $"<color=green>交换成功！</color> {requester.Nickname} 与 {player.Nickname} 已交换 SCP 身份。";
            requester.SendBroadcast(msg, 5);
            player.SendBroadcast(msg, 5);

            Log.Info($"{player.Nickname} 与 {requester.Nickname} 交换成功");

            response = "交换成功";
            return true;
        }
    }
}
