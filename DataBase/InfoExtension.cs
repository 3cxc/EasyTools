using EasyTools.DataBase.Serialization;
using EasyTools.Events;
using LabApi.Features.Wrappers;
using LiteDB;
using MEC;
using System;
using System.Collections.Generic;

namespace EasyTools.DataBase
{
    public static class InfoExtension
    {

        /// <summary>
        /// 玩家列表
        /// 不用 Player.ReadyList 是因为它包含Dummy
        /// </summary>
        public static List<Player> PlayerList = [];

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
                };
                using LiteDatabase database = new(CustomEventHandler.DataBaseConfig.database_path);
                database.GetCollection<PlayerData>("Players").Insert(toInsert);
            }

            if (data is null)
                return toInsert;
            return data;
        }

        public static PlayerData GetData(this ReferenceHub ply)
        {
            PlayerData toInsert = null;
            if (string.IsNullOrWhiteSpace(ply.authManager.UserId))
                throw new ArgumentNullException(nameof(ply));
            if (!DataAPI.TryGetData(ply.authManager.UserId, out PlayerData data))
            {
                toInsert = new PlayerData()
                {
                    ID = ply.authManager.UserId,
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
                using LiteDatabase database = new(CustomEventHandler.DataBaseConfig.database_path);
                database.GetCollection<PlayerData>("Players").Insert(toInsert);
            }

            if (data is null)
                return toInsert;
            return data;
        }

        public static void UpdateData(this PlayerData data)
        {
            using LiteDatabase database = new(CustomEventHandler.DataBaseConfig.database_path);
            database.GetCollection<PlayerData>("Players").Update(data);
        }

        public static IEnumerator<float> CollectInfo()
        {
            while (true)
            {
                yield return Timing.WaitForSeconds(60f);

                foreach (Player Player in PlayerList)
                {
                    if (Player != null && !Player.DoNotTrack)
                    {
                        var pLog = Player.GetData();
                        pLog.PlayedTimes += 60;
                        pLog.UpdateData();
                    }
                }
                if (Round.IsRoundEnded)
                {
                    yield break;
                }
            }
        }
    }
}
