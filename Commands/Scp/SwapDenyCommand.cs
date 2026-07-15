using CommandSystem;
using EasyTools.Events;
using LabApi.Features.Wrappers;
using System;
using Log = LabApi.Features.Console.Logger;

namespace EasyTools.Commands.Scp
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class SwapDenyCommand : ICommand
    {
        public string Command => "swapdeny";

        public string[] Aliases => ["spd"];

        public string Description => "拒绝与其他SCP的交换请求";

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

            // 清理请求
            CustomEventHandler.SwapRequests.Remove(player);

            // 通知玩家
            player.SendBroadcast($"你拒绝了与 {requester.Nickname} 的交换请求", 5);
            requester.SendBroadcast($"{player.Nickname} 拒绝了你的交换请求", 5);


            Log.Info($"{player.Nickname} 与 {requester.Nickname} 交换失败");

            response = "交换失败";
            return true;
        }
    }
}
