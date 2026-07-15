using System.ComponentModel;

namespace EasyTools.Configs
{
    public class LevelSystemConfig
    {
        [Description("是否启用等级系统")]
        public bool EnableLevelSystem { get; set; } = true;
        [Description("是否计入机器人")]
        public bool CountBot { get; set; } = false;
        [Description("经验值缩放系数")]
        public double XpScaleFactor { get; set; } = 1.0;
        [Description("人类阵营击杀SCP时奖励的经验")]
        public double HumanKillSCPXp { get; set; } = 100.0;
        [Description("人类阵营击杀SCP-049-2时奖励的经验")]
        public double HumanKillSCP0492Xp { get; set; } = 30.0;
        [Description("人类阵营击杀敌对人类阵营时奖励的经验")]
        public double HumanKillHumanXp { get; set; } = 5.0;
        [Description("SCP击杀人类阵营时奖励的经验")]
        public double SCPKillHumanXp { get; set; } = 5.0;
        [Description("人类阵营逃离设施时奖励的经验")]
        public double HumanEscapeXp { get; set; } = 20.0;
    }
}
