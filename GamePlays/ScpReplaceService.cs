using EasyTools.DataStructures;
using EasyTools.Events;
using LabApi.Features.Wrappers;
using MEC;
using PlayerRoles;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Log = LabApi.Features.Console.Logger;

namespace EasyTools.GamePlays
{
    /// <summary>
    /// SCP 掉线补位
    /// </summary>
    public static class ScpReplaceService
    {
        private class ReplacementEntry
        {
            public readonly List<PlayerInfo> Applicants = new();
            public float ExpireTime;
        }

        private static readonly Dictionary<RoleTypeId, ReplacementEntry> _slots = new();

        /// <summary>
        /// 玩家作为 SCP 掉线时调用，开放一个补位名额
        /// </summary>
        public static void OpenSlot(Player player)
        {
            if (player is null || !player.IsSCP) return;

            var role = player.Role;
            _slots[role] = new ReplacementEntry
            {
                ExpireTime = Time.time + CustomEventHandler.Config.SCPReplaceTime
            };

            Server.SendBroadcast(
                $"\n<b><size=25><color=#00CC00>{role} 掉线，输入 .replace 以补位！</color></size></b>", 3);

            Timing.CallDelayed(CustomEventHandler.Config.SCPReplaceTime, () => Execute(role));
        }

        /// <summary>
        /// 玩家离开时清理其在所有补位列表中的记录
        /// </summary>
        public static void RemoveApplicant(PlayerInfo info)
        {
            if (info is null) return;

            foreach (var entry in _slots.Values)
                entry.Applicants.Remove(info);
        }

        /// <summary>
        /// 回合开始时清空
        /// </summary>
        public static void ClearAll() => _slots.Clear();

        /// <summary>
        /// 玩家执行补位命令时的入口
        /// </summary>
        public static bool TryApply(Player player, RoleTypeId role, out string response)
        {
            if (!_slots.TryGetValue(role, out var entry) || Time.time > entry.ExpireTime)
            {
                response = $"当前没有 {role} 的补位名额或已过期。";
                return false;
            }

            var info = CustomEventHandler.PlayerManager.Get(player);
            if (info is null)
            {
                response = "玩家状态异常";
                return false;
            }

            if (entry.Applicants.Contains(info))
            {
                response = "你已经申请过该角色了，请等待系统分配。";
                return false;
            }

            entry.Applicants.Add(info);
            response = $"你已申请补位 {role}，等待 {entry.ExpireTime - Time.time:0.0} 秒后系统随机选择。";
            return true;
        }

        private static void Execute(RoleTypeId role)
        {
            if (!_slots.TryGetValue(role, out var entry)) return;

            // 无论结果如何都移除，避免过期残留
            _slots.Remove(role);

            var valid = entry.Applicants
                .Where(i => i.Player is not null && i.Player.IsHuman)
                .ToList();

            if (valid.Count == 0)
            {
                Server.SendBroadcast($"<b><size=25><color=orange>{role} 补位无人申请，该角色空缺。</color></size></b>", 5);
                return;
            }

            var chosen = valid[Random.Range(0, valid.Count)];
            chosen.Player.Role = role;

            Server.SendBroadcast($"<b><size=25><color=green>补位成功！{chosen.NickName} 成为了 {role}。</color></size></b>", 10);
            Log.Info($"{chosen.NickName} 补位成为 {role}");
        }
    }
}