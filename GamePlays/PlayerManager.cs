using EasyTools.DataStructures;
using LabApi.Features.Wrappers;
using System.Collections.Generic;

namespace EasyTools.GamePlays
{
    public sealed class PlayerManager
    {
        private readonly List<PlayerInfo> Players = new();
        private readonly Dictionary<Player, PlayerInfo> PlayerToInfo = new();

        public IReadOnlyList<PlayerInfo> PlayerList => Players;
        public int PlayerCount => Players.Count;

        public PlayerInfo AddPlayer(Player player, PlayerData playerData, HintData hud914, HintData hudElevator)
        {
            if (PlayerToInfo.TryGetValue(player, out var info)) return info;

            info = new PlayerInfo(player, playerData, hud914, hudElevator);
            Players.Add(info);
            PlayerToInfo[player] = info;
            return info;
        }

        public bool RemovePlayer(Player player)
        {
            if (player is null) return false;
            if (!PlayerToInfo.TryGetValue(player, out var info)) return false;

            Players.Remove(info);
            PlayerToInfo.Remove(player);
            info.Dispose();
            return true;
        }

        public bool TryGet(Player player, out PlayerInfo info) => PlayerToInfo.TryGetValue(player, out info);
        public PlayerInfo Get(Player player) => player is not null && PlayerToInfo.TryGetValue(player, out var info) ? info : null;

        /// <summary>
        /// 玩家退出时，清除所有指向他的 SwapRequestFrom
        /// </summary>
        public void ClearSwapRequestsTo(Player player)
        {
            foreach (var info in Players)
                if (info.SwapRequestFrom == player)
                    info.SwapRequestFrom = null;
        }

        public void Clear()
        {
            foreach (var info in Players) info.Dispose();
            Players.Clear();
            PlayerToInfo.Clear();
        }
    }
}
