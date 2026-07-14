using EasyTools.Configs;
using LabApi.Features.Wrappers;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EasyTools.Extensions
{
    public static class RewardExtensions
    {
        public static RewardSetting PickReward(this List<RewardSetting> rewards)
        {
            float totalWeight = rewards.Sum(r => r.Weight);
            float roll = UnityEngine.Random.Range(0f, totalWeight);
            float cumulative = 0f;
            foreach (var reward in rewards)
            {
                cumulative += reward.Weight;
                if (roll < cumulative)
                    return reward;
            }
            return rewards.LastOrDefault();
        }

        public static string ApplyReward(this Player player, RewardSetting reward)
        {
            switch (reward.Type)
            {
                case RewardType.ChangeRole:
                    if (Enum.TryParse(reward.Param, out RoleTypeId role))
                    {
                        player.Role = role;
                        return $"变成了 {reward.Name}";
                    }
                    return "变身失败（配置错误）";

                case RewardType.GiveItem:
                    if (Enum.TryParse(reward.Param, out ItemType item))
                    {
                        player.AddItem(item);
                        return $"获得了 {reward.Name}";
                    }
                    return "获得物品失败（配置错误）";

                case RewardType.GiveRandomGun:
                    string[] guns = reward.Param.Split(',');
                    string chosen = guns[UnityEngine.Random.Range(0, guns.Length)];
                    if (Enum.TryParse(chosen.Trim(), out ItemType gun))
                    {
                        player.AddItem(gun);
                        return $"获得了 {chosen}";
                    }
                    return "获得武器失败（配置错误）";

                case RewardType.TeleportToSCP:
                    var scps = Player.ReadyList.Where(p => p.IsSCP && p.Role != RoleTypeId.Scp079).ToList();
                    if (scps.Any())
                    {
                        var target = scps[UnityEngine.Random.Range(0, scps.Count)];
                        player.Position = target.Position + Vector3.right;
                        player.Rotation = target.Rotation;
                        return "被传送到SCP旁边";
                    }
                    // 没有SCP时返还硬币（防止纯损失）
                    player.AddItem(ItemType.Coin);
                    return "...SCP都不在了，返还硬币";

                case RewardType.ExtraCoin:
                    player.AddItem(ItemType.Coin);
                    return "又获得了一个硬币";

                case RewardType.GiveMedical:
                    bool give500 = UnityEngine.Random.Range(0, 2) == 0;
                    player.AddItem(give500 ? ItemType.SCP500 : ItemType.Medkit);
                    return "获得了医疗物品";

                default:
                    return "损失了一个硬币";
            }
        }
    }
}
