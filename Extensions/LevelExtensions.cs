using LabApi.Features.Wrappers;
using System;

namespace EasyTools.Extensions
{
    public static class LevelExtensions
    {
        /// <summary>
        /// 根据经验值计算当前等级
        /// </summary>
        public static int GetLevelFromXp(double xp, double a)
        {
            if (xp < 0) xp = 0;
            double level = -50.0 + Math.Sqrt((4.0 * xp / a) + 9800.0) / 2.0;
            return (int)Math.Ceiling(level);
        }

        /// <summary>
        /// 从0级升到指定等级所需的总经验
        /// </summary>
        public static double GetTotalXpForLevel(int level, double a)
        {
            if (level < 0) level = 0;
            return Math.Ceiling((level * level + 100.0 * level + 50.0) * a);
        }

        /// <summary>
        /// 当前经验值下，升级到下一级还需要的经验
        /// </summary>
        public static double GetXpUntilNextLevel(double currentXp, double a)
        {
            int currentLevel = GetLevelFromXp(currentXp, a);
            if (currentLevel < 0) currentLevel = 0;
            double nextLevelTotal = GetTotalXpForLevel(currentLevel + 1, a);
            return Math.Max(0, nextLevelTotal - currentXp);
        }

        // 玩家名称更新方法
        public static void UpdatePlayerNameWithLevelPrefix(this Player player)
        {
            if (player == null || player.IsNpc) return;

            string originalName = player.Nickname;

            // 设置新名称（前缀+原始名称）
            string newName = $"[Level.{player.GetData().PlayerLevel}]{originalName}";
            player.DisplayName = newName;
        }
    }
}
