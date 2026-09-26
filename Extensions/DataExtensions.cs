using EasyTools.API;
using EasyTools.DataStructures;
using EasyTools.Events;
using LabApi.Features.Wrappers;
using LiteDB;
using MEC;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace EasyTools.Extensions
{
    public static class DataExtensions
    {
        private static readonly Dictionary<string, PlayerData> _cache = new();

        public static PlayerData GetData(this Player player) => GetOrCreate(player.UserId, player.Nickname);

        public static PlayerData GetData(string userId) => GetOrCreate(userId, null);

        private static PlayerData GetOrCreate(string userId, string nickname)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentNullException(nameof(userId));

            if (!Regex.IsMatch(userId, @"^\d+@steam$", RegexOptions.IgnoreCase))
                throw new FormatException($"无效的 Steam ID 格式: {userId}");

            if (_cache.TryGetValue(userId, out var cached))
            {
                if (!string.IsNullOrEmpty(nickname))
                    cached.NickName = nickname;
                return cached;
            }

            using var db = new LiteDatabase(CustomEventHandler.Config.DataBasePath);
            var col = db.GetCollection<PlayerData>("Players");
            var data = col.FindById(userId);

            if (data is null)
            {
                data = new()
                {
                    ID = userId,
                    NickName = nickname ?? "",
                    LastJoinedTime = DateTime.Now,
                    LastLeftTime = DateTime.Now,
                    PlayerXp = 0.0,
                    PlayerLevel = 0.0,
                    PermissionLevel = PermissionLevel.Player,
                    Badge = "",
                    BadgeColor = "rainbow"
                };
                col.Insert(data);
            }

            _cache[userId] = data;
            return data;
        }

        public static void UpdateData(this PlayerData data)
        {
            using LiteDatabase database = new(CustomEventHandler.Config.DataBasePath);
            database.GetCollection<PlayerData>("Players").Update(data);
        }

        public static void RemoveFromCache(string userId)
        {
            if (!string.IsNullOrEmpty(userId))
                _cache.Remove(userId);
        }

        public static IEnumerator<float> CollectInfo()
        {
            while (true)
            {
                yield return Timing.WaitForSeconds(60f);

                foreach (PlayerInfo info in CustomEventHandler.PlayerManager.PlayerList)
                {
                    var player = info.Player;
                    if (player is null || player.DoNotTrack) continue;

                    var data = player.GetData();
                    data.PlayedTimes += 60;
                    data.UpdateData();
                }
                if (Round.IsRoundEnded)
                {
                    yield break;
                }
            }
        }
    }
}
