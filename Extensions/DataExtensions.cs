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
        public static PlayerData GetData(this Player ply)
        {
            PlayerData toInsert = null;
            if (!DataAPI.TryGetData(ply.UserId, out PlayerData data))
            {
                toInsert = new PlayerData()
                {
                    ID = ply.UserId,
                    NickName = "",
                    LastJoinedTime = DateTime.Now,
                    LastLeftTime = DateTime.Now,
                    PlayedTimes = 0,
                    PlayerKills = 0,
                    PlayerDeath = 0,
                    PlayerSCPKills = 0,
                    PlayerDamage = 0,
                    RolePlayed = 0,
                    PlayerShot = 0,
                    PlayerXp = 0.0,
                    PlayerLevel = 0.0
                };
                using LiteDatabase database = new(CustomEventHandler.Config.DataBasePath);
                database.GetCollection<PlayerData>("Players").Insert(toInsert);
            }

            if (data is null)
                return toInsert;
            return data;
        }

        public static PlayerData GetData(string userId)
        {
            PlayerData toInsert = null;
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentNullException(nameof(userId));
            // 验证 Steam ID 格式：纯数字+@steam
            if (!Regex.IsMatch(userId, @"^\d+@steam$", RegexOptions.IgnoreCase))
                throw new FormatException("无效的 Steam ID 格式，应为 数字@steam");
            if (!DataAPI.TryGetData(userId, out PlayerData data))
            {
                toInsert = new PlayerData()
                {
                    ID = userId,
                    NickName = "",
                    LastJoinedTime = DateTime.Now,
                    LastLeftTime = DateTime.Now,
                    PlayedTimes = 0,
                    PlayerKills = 0,
                    PlayerDeath = 0,
                    PlayerSCPKills = 0,
                    PlayerDamage = 0,
                    RolePlayed = 0,
                    PlayerShot = 0,
                };
                using LiteDatabase database = new(CustomEventHandler.Config.DataBasePath);
                database.GetCollection<PlayerData>("Players").Insert(toInsert);
            }

            if (data is null)
                return toInsert;
            return data;
        }

        public static void UpdateData(this PlayerData data)
        {
            using LiteDatabase database = new(CustomEventHandler.Config.DataBasePath);
            database.GetCollection<PlayerData>("Players").Update(data);
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
