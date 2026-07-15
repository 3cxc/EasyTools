using CommandSystem;
using EasyTools.Events;
using LabApi.Features.Wrappers;
using PlayerRoles;
using System;
using UnityEngine;

namespace EasyTools.Commands.Scp
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class ReplaceScpCommand : ICommand
    {
        public string Command => "replace";

        public string[] Aliases => ["rscp"];

        public string Description => "申请补位一个断线SCP";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player;

            if (sender is null || (player = Player.Get(sender)) is null || !(player = Player.Get(sender)).IsSCP)
            {
                response = CustomEventHandler.TranslateConfig.CommandNotAllowed;
                return false;
            }

            if (CustomEventHandler.Config.EnableSCPReplace)
            {
                response = CustomEventHandler.TranslateConfig.CommandNotEnabled;
                return false;
            }

            if (arguments.Count == 0)
            {
                response = "请指定要补位的 SCP 编号(Scp079|Scp096|Scp106|Scp173|Scp049|Scp3114)";
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

            if (!CustomEventHandler.Replacements.TryGetValue(targetRole, out var entry) || Time.time > entry.ExpireTime)
            {
                response = $"当前没有 {targetRole} 的补位名额或已过期。";
                return false;
            }

            if (entry.Applicants.Contains(player))
            {
                response = "你已经申请过该角色了，请等待系统分配。";
                return false;
            }

            entry.Applicants.Add(player);
            float remaining = entry.ExpireTime - Time.time;
            response = $"你已申请补位 {targetRole}，等待 {remaining:0.0} 秒后系统随机选择。";
            return true;
        }
    }
}
