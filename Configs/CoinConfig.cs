using System.Collections.Generic;
using System.ComponentModel;

namespace EasyTools.Configs
{
    public class CoinConfig
    {
        [Description("抽卡奖励列表，权重总和为 100（自动归一化也可），支持修改概率和奖励内容")]
        public List<RewardSetting> Rewards { get; set; } = new List<RewardSetting>
        {
        // ---- 变成 SCP（每种0.075%，总和0.3%） ----
        new RewardSetting { Weight = 0.075f, Type = RewardType.ChangeRole, Param = "Scp939", Name = "SCP-939" },
        new RewardSetting { Weight = 0.075f, Type = RewardType.ChangeRole, Param = "Scp3114", Name = "SCP-3114" },
        new RewardSetting { Weight = 0.075f, Type = RewardType.ChangeRole, Param = "Scp106", Name = "SCP-106" },
        new RewardSetting { Weight = 0.075f, Type = RewardType.ChangeRole, Param = "Scp049", Name = "SCP-049" },

        // ---- 0.7% 特殊武器 ----
        new RewardSetting { Weight = 0.7f, Type = RewardType.GiveItem, Param = "GunSCP127", Name = "SCP-127" },

        // ---- 4% 指挥官卡 ----
        new RewardSetting { Weight = 4.0f, Type = RewardType.GiveItem, Param = "KeycardMTFCaptain", Name = "指挥官卡" },

        // ---- 5% 可乐 ----
        new RewardSetting { Weight = 5.0f, Type = RewardType.GiveItem, Param = "SCP207", Name = "可乐" },

        // ---- 15% 随机枪械（两种枪各7.5%） ----
        new RewardSetting { Weight = 7.5f, Type = RewardType.GiveItem, Param = "GunE11SR", Name = "狗官枪" },
        new RewardSetting { Weight = 7.5f, Type = RewardType.GiveItem, Param = "GunAK", Name = "AK-47" },

        // ---- 10% 中士卡 ----
        new RewardSetting { Weight = 10.0f, Type = RewardType.GiveItem, Param = "KeycardMTFOperative", Name = "中士卡" },

        // ---- 10% 再来一次 ----
        new RewardSetting { Weight = 10.0f, Type = RewardType.ExtraCoin, Name = "又获得了一个硬币" },

        // ---- 10% 随机传送 ----
        new RewardSetting { Weight = 10.0f, Type = RewardType.TeleportToSCP, Name = "被传送到SCP旁边" },

        // ---- 20% 医疗物品（SCP-500 和医疗包各10%） ----
        new RewardSetting { Weight = 10.0f, Type = RewardType.GiveItem, Param = "SCP500", Name = "SCP-500" },
        new RewardSetting { Weight = 10.0f, Type = RewardType.GiveItem, Param = "Medkit", Name = "医疗物品" },

        // ---- 25% 什么都没有 ----
        new RewardSetting { Weight = 25.0f, Type = RewardType.None, Name = "损失了一个硬币" },
        };
    }

    public class RewardSetting
    {
        [Description("显示名称")]
        public string Name { get; set; } = "未知奖励";

        [Description("相对权重")]
        public float Weight { get; set; } = 10f;

        [Description("奖励类型")]
        public RewardType Type { get; set; } = RewardType.None;

        [Description("额外参数")]
        public string Param { get; set; } = string.Empty;
    }

    public enum RewardType
    {
        None,           // 什么都没发生
        ChangeRole,     // 变身SCP（Param = 角色ID）
        GiveItem,       // 给物品（Param = 物品ID）
        GiveRandomGun,  // 随机给一把枪（Param = 逗号分隔的枪械ID）
        TeleportToSCP,  // 传送到随机SCP旁（Param留空）
        ExtraCoin,      // 返还硬币（Param留空）
        GiveMedical     // 随机给医疗包或SCP500（Param留空）
    }
}
