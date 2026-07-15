using CommandSystem;
using EasyTools.Events;
using LabApi.Features.Wrappers;
using PlayerRoles;
using System;
using System.Linq;
using Log = LabApi.Features.Console.Logger;

namespace EasyTools.Commands.Scp
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class SwapScpCommand : ICommand
    {
        public string Command => "swap";

        public string[] Aliases => ["sp"];

        public string Description => "申请与其他SCP交换";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player;

            if (sender is null || (player = Player.Get(sender)) is null || !(player = Player.Get(sender)).IsSCP)
            {
                response = "失败，可能指令未启用或者身份不允许等";
                return false;
            }

            if ((DateTime.Now - CustomEventHandler.RoundStartTime).TotalSeconds > 300)
            {
                response = "失败，交换指令只能在开局三分钟内使用";
                return false;
            }

            if (arguments.Count == 0)
            {
                response = "失败，未指定你要交换的目标(Scp079|Scp096|Scp106|Scp173|Scp049|Scp3114)";
                return false;
            }

            string input = arguments.At(0);
            if (!Enum.TryParse(input, out RoleTypeId targetRole))
            {
                response = $"无效的 SCP 编号：{input}。可用的有 Scp079|Scp096|Scp106|Scp173|Scp049|Scp3114 （必须带Scp）";
                return false;
            }

            // 防止与自己交换
            if (player.Role == targetRole)
            {
                response = "你不能与自己交换。";
                return false;
            }

            // 确保目标SCP存在
            Player target = Player.List.FirstOrDefault(p => p.IsSCP && p.Role == targetRole);
            if (target == null)
            {
                response = $"当前没有 {targetRole} 在线。";
                return false;
            }

            // 检查是否已存在请求
            if (CustomEventHandler.SwapRequests.ContainsKey(target))
            {
                response = $"已有一个交换请求发送给 {target.Nickname}，请等待对方回应。";
                return false;
            }

            // 保存请求
            CustomEventHandler.SwapRequests[target] = player;

            // 通知目标玩家
            target.SendBroadcast($"<color=yellow>{player.Nickname}</color> 想与你交换 SCP 身份！输入 <color=green>.swapaccept</color> 接受，或 <color=red>.swapdeny</color> 拒绝。", 10);

            Log.Info($"{player.Nickname} 申请与 {arguments.At(0)} 交换");

            response = "申请成功";
            return true;
        }
    }
}
