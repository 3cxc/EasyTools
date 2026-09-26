using LabApi.Features.Wrappers;
using PlayerRoles;
using System;
using UnityEngine;

namespace EasyTools.DataStructures
{
    /// <summary>
    /// 代表单个玩家的运行时状态
    /// </summary>
    public sealed class PlayerInfo : IDisposable
    {
        public Player Player { get; init; }
        public PlayerData PlayerData { get; init; }
        public string NickName { get; init; }
        public string UserId { get; init; }
        public RoleTypeId Role { get; set; }

        /// <summary>
        /// 玩家 HUD
        /// </summary>
        public PlayerHint Hud { get; }

        /// <summary>
        /// 玩家是否拥有彩虹称号
        /// </summary>
        public bool RainbowBadge { get; set; }

        // SCP 站立回血
        public Vector3 LastPosition { get; set; }
        public DateTime LastMoveTime { get; set; }
        public float LastHealth { get; set; }
        public DateTime LastDamageTime { get; set; }

        /// <summary>
        /// 收到来自谁的交换请求，null 表示没有
        /// </summary>
        public Player SwapRequestFrom { get; set; }

        public PlayerInfo(Player player, PlayerData playerData, HintData hud914, HintData hudElevator)
        {
            Player = player;
            PlayerData = playerData;
            NickName = player.Nickname;
            UserId = player.UserId;
            Role = player.Role;
            Hud = new PlayerHint(player, hud914, hudElevator);
            LastPosition = player.Position;
            LastMoveTime = DateTime.UtcNow;
            LastHealth = player.Health;
            LastDamageTime = DateTime.MinValue;
        }

        public void Dispose()
        {
            Hud.Dispose();
            SwapRequestFrom = null;
        }
    }
}
