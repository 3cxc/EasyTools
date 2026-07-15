using LabApi.Loader.Features.Paths;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace EasyTools.Configs
{
    public class Config
    {
        [Description("数据库存储路径")]
        public string DataBasePath { get; set; } = Path.Combine(PathManager.Configs.FullName ?? Environment.CurrentDirectory, @"EasyTools.db");

        /// /////////////////////////////////////////////////
        [Description("是否打开开局给D级人员一张卡")]
        public bool EnableRoundSupplies { get; set; } = true;

        [Description("给D级人员什么卡？")]
        public ItemType ClassDCard { get; set; } = ItemType.KeycardJanitor;

        /// /////////////////////////////////////////////////
        [Description("启用服务器定时广播")]
        public bool EnableAutoServerMessage { get; set; } = true;

        [Description("服务器定时广播多久一次")]
        public int AutoServerMessageTime { get; set; } = 5;

        [Description("服务器定时广播显示多久")]
        public ushort AutoServerMessageTimer { get; set; } = 5;

        [Description("服务器广播文本")]
        public List<string> AutoServerMessageText { get; set; } = ["服务器广播1", "服务器广播2"];

        /// /////////////////////////////////////////////////
        [Description("启用.bc和.c聊天系统")]
        public bool EnableChatSystem { get; set; } = true;

        [Description("聊天系统大小，默认20")]
        public int ChatSystemSize { get; set; } = 20;

        [Description("启用玩家反馈管理系统")]
        public bool EnableAcSystem { get; set; } = true;

        [Description("一个人的聊天信息显示多久，单位为秒")]
        public int MessageTime { get; set; } = 10;

        [Description("聊天消息的格式，可用的标签为{Message}，{MessageType}, {MessageTypeColor}, {SenderNickname}，{SenderTeam}，{SenderRole} ，{SenderTeamColor}, {CountDown}")]
        public string MessageTemplate { get; set; } = "[{CountDown}]<color={{SenderTeamColor}}>[{SenderTeam}][{SenderRole}]</color><color={MessageTypeColor}>[{MessageType}]</color>{SenderNickname}: {Message}";
        
        /// /////////////////////////////////////////////////
        [Description("是否启用管理权限系统")]
        public bool EnableAdmin { get; set; } = true;

        // /////////////////////////////////////////////////
        [Description("是否启用玩家在线时长统计")]
        public bool EnablePlayTime { get; set; } = true;

        /// /////////////////////////////////////////////////
        [Description("防卡死命令")]
        public bool KillMeCommand { get; set; } = true;

        /// /////////////////////////////////////////////////
        [Description("启用修改SCP血量系统")]
        public bool EnableChangeSCPHPSystem { get; set; } = true;

        [Description("SCP173,SCP939,SCP049,SCP049-2,SCP096,SCP106血量")]
        public List<float> SCPsHP { get; set; } = [5200, 3000, 2800, 600, 3100, 2800];

        /// /////////////////////////////////////////////////
        [Description("是否开启保安下班")]
        public bool GuardsCanEscape { get; set; } = true;

        [Description("选择保安下班变成的角色 (NtfSergeant, NtfCaptain, NtfPrivate, NtfSpecialist)")]
        public string EscapedGuardRole { get; set; } = "NtfCaptain";

        /// /////////////////////////////////////////////////
        [Description("是否开启粉糖")]
        public bool EnablePinkCandy { get; set; } = false;

        [Description("粉糖生成概率(默认50%)")]
        public int PinkCandyWeight { get; set; } = 2;

        /// /////////////////////////////////////////////////
        [Description("是否记录玩家进出日志")]
        public bool EnablePlayerLogger { get; set; } = true;

        [Description("是否记录管理员操作相关日志")]
        public bool EnableAdminLogger { get; set; } = true;

        [Description("Player logger settings / 玩家日志保存路径")]
        public string PlayerLogPath { get; set; } = Path.Combine(PathManager.Configs.FullName ?? Environment.CurrentDirectory, "JoinLogs");

        [Description("Management logger settings / 管理日志保存路径")]
        public string AdminLogPath { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "AdminLogs");

        /// /////////////////////////////////////////////////
        [Description("Is 207 harmless? / 是否开启207(可乐)无害?")]
        public bool Harmless207 { get; set; } = false;

        [Description("Is 1853 harmless? / 是否开启1853(洗手液)无害?")]
        public bool Harmless1853 { get; set; } = true;

        /// /////////////////////////////////////////////////
        [Description("SCP静止回血？")]
        public bool EnableHealSCP { get; set; } = true;

        [Description("等待多少秒后开始回血")]
        public float HealSCPSecend { get; set; } = 6;

        [Description("受伤检测")]
        public float HealATKSecend { get; set; } = 2;

        [Description("回血量")]
        public int HealSCPQuantity { get; set; } = 2;

        /// /////////////////////////////////////////////////
        [Description("开启回合结束友伤?")]
        public bool EnableFriendFire { get; set; } = true;

        /// /////////////////////////////////////////////////

        [Description("开启SCP开局交换?")]
        public bool EnableSCPStartExchange { get; set; } = true;

        [Description("允许SCP开局交换的时间(以秒为单位)")]
        public int SCPStartExchangeTime { get; set; } = 300;

        [Description("开启SCP掉线时补位?")]
        public bool EnableSCPReplace { get; set; } = true;

        [Description("允许玩家输入补位指令的时间(以秒为单位)")]
        public float SCPReplaceTime { get; set; } = 15f;

        /// /////////////////////////////////////////////////
        [Description("开启开局生成 SCP-3114?")]
        public bool EnableSCP3114 { get; set; } = false;

        [Description("当服务器有多少玩家时才会生成 SCP-3114?")]
        public int SCP3114Limit { get; set; } = 8;

        [Description("SCP-3114的生成概率(默认50%)")]
        public float SCP3114Weight { get; set; } = 0.1f;
    }
}
