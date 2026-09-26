using EasyTools.Events;
using LabApi.Features.Wrappers;
using PlayerRoles;
using System;
using System.Linq;
using Log = LabApi.Features.Console.Logger;

namespace EasyTools.GamePlays
{
    /// <summary>
    /// SCP 开局交换
    /// </summary>
    public static class ScpSwapService
    {
        /// <summary>
        /// 命令 .swap 的入口
        /// </summary>
        public static bool TryRequest(Player player, RoleTypeId targetRole, out string response)
        {
            if (!CheckBase(player, out response)) return false;

            if (player.Role == targetRole)
            {
                response = "你不能与自己交换。";
                return false;
            }

            var target = Player.ReadyList.FirstOrDefault(p => p.IsSCP && p.Role == targetRole);
            if (target is null)
            {
                response = $"当前没有 {targetRole} 在线。";
                return false;
            }

            var targetInfo = CustomEventHandler.PlayerManager.Get(target);
            if (targetInfo is null)
            {
                response = "目标状态异常";
                return false;
            }

            if (targetInfo.SwapRequestFrom is not null)
            {
                response = $"已有一个交换请求发送给 {targetInfo.NickName}，请等待对方回应。";
                return false;
            }

            targetInfo.SwapRequestFrom = player;

            targetInfo.Player.SendBroadcast(
                $"<b><size=25><color=yellow>{player.Nickname}</color> 想与你交换 SCP 身份！</size></b>" +
                $"<b><size=25>输入 <color=green>.swapaccept</color> 接受，或 <color=red>.swapdeny</color> 拒绝。</size></b>", 10);

            Log.Info($"{player.Nickname} 申请与 {targetRole} 交换");
            response = "申请成功";
            return true;
        }

        /// <summary>
        /// 命令 .swapaccept 的入口
        /// </summary>
        public static bool TryAccept(Player player, out string response)
        {
            if (!CheckBase(player, out response)) return false;

            var info = CustomEventHandler.PlayerManager.Get(player);
            if (info is null)
            {
                response = "玩家状态异常";
                return false;
            }

            Player requester = info.SwapRequestFrom;
            if (requester is null)
            {
                response = CustomEventHandler.TranslateConfig.SwapCommandNoRequestBroadcastTemplate;
                return false;
            }

            if (!requester.IsSCP)
            {
                info.SwapRequestFrom = null;
                response = "请求已失效，对方已不再是 SCP";
                return false;
            }

            var tempRole = requester.Role;
            requester.Role = player.Role;
            player.Role = tempRole;

            info.SwapRequestFrom = null;

            Server.SendBroadcast(
                $"<b><size=25><color=green>{requester.Nickname} 与 {player.Nickname} 已交换 SCP 身份</color></size></b>", 5);

            Log.Info($"{player.Nickname} 与 {requester.Nickname} 交换成功");
            response = "交换成功";
            return true;
        }

        /// <summary>
        /// 命令 .swapdeny 的入口
        /// </summary>
        public static bool TryDeny(Player player, out string response)
        {
            if (!CheckBase(player, out response)) return false;

            var info = CustomEventHandler.PlayerManager.Get(player);
            if (info is null)
            {
                response = "玩家状态异常";
                return false;
            }

            Player requester = info.SwapRequestFrom;
            if (requester is null)
            {
                response = CustomEventHandler.TranslateConfig.SwapCommandNoRequestBroadcastTemplate;
                return false;
            }

            info.SwapRequestFrom = null;

            player.SendBroadcast($"你拒绝了与 {requester.Nickname} 的交换请求", 5);
            requester.SendBroadcast($"{player.Nickname} 拒绝了你的交换请求", 5);

            Log.Info($"{player.Nickname} 与 {requester.Nickname} 交换失败");
            response = "交换失败";
            return true;
        }

        /// <summary>
        /// 三个命令共用的前置检查
        /// </summary>
        private static bool CheckBase(Player player, out string response)
        {
            response = null;

            if (player is null || !player.IsSCP)
            {
                response = CustomEventHandler.TranslateConfig.CommandNotAllowed;
                return false;
            }

            if (!CustomEventHandler.Config.EnableSCPStartExchange)
            {
                response = CustomEventHandler.TranslateConfig.CommandNotEnabled;
                return false;
            }

            if ((DateTime.Now - CustomEventHandler.RoundStartTime).TotalSeconds
                > CustomEventHandler.Config.SCPStartExchangeTime)
            {
                response = CustomEventHandler.TranslateConfig.SwapCommandTimeLimitBroadcastTemplate;
                return false;
            }

            return true;
        }
    }
}