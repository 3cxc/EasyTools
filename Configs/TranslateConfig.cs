using EasyTools.Extensions;
using PlayerRoles;
using Scp914;
using System.Collections.Generic;
using System.ComponentModel;

namespace EasyTools.Configs
{
    public class TranslateConfig
    {
        [Description("命令未启用时的提示")]
        public string CommandNotEnabled { get; set; } = "该命令未启用";
        [Description("当前身份不允许执行命令时的提示")]
        public string CommandNotAllowed { get; set; } = "当前身份不允许执行该命令";
        [Description("命令执行失败时的提示")]
        public string CommandFailed { get; set; } = "执行失败，可能是参数错误等";
        [Description("命令执行成功时的提示")]
        public string CommandOk { get; set; } = "执行成功";
        [Description("聊天信息发送失败时提示")]
        public string ChatCommandFailed { get; set; } = "发送失败，你被禁言或者信息为空或者聊天系统未启用";
        [Description("聊天信息发送成功时提示")]
        public string ChatCommandOk { get; set; } = "发送成功";
        /// <summary>
        /// 
        /// </summary>
        [Description("聊天中消息列表的标题")]
        public string ChatMessageTitle { get; set; } = "消息列表:";

        [Description("聊天中每种消息的名字")]
        public Dictionary<ChatMessage.MessageType, string> MessageTypeName { get; set; } = new()
        {
            { ChatMessage.MessageType.AdminPrivateChat, "管理私聊" },
            { ChatMessage.MessageType.BroadcastChat, "公共消息" },
            { ChatMessage.MessageType.TeamChat, "队友消息" },
        };
        /// <summary>
        /// 
        /// </summary>
        [Description("聊天系统自定义玩家角色名称")]
        public Dictionary<RoleTypeId, string> ChatSystemRoleTranslation { get; set; } = new Dictionary<RoleTypeId, string>
        {
            {RoleTypeId.ClassD , "D级人员"},
            {RoleTypeId.FacilityGuard , "保安" },
            {RoleTypeId.ChaosConscript , "混沌征召兵"},
            {RoleTypeId.ChaosMarauder , "混沌掠夺者"},
            {RoleTypeId.ChaosRepressor , "混沌压制者"},
            {RoleTypeId.ChaosRifleman , "混沌步枪兵"},
            {RoleTypeId.NtfCaptain , "九尾狐指挥官"},
            {RoleTypeId.NtfPrivate , "九尾狐列兵"},
            {RoleTypeId.NtfSergeant , "九尾狐中士"},
            {RoleTypeId.NtfSpecialist , "九尾狐收容专家"},
            {RoleTypeId.Scientist , "科学家"},
            {RoleTypeId.Tutorial , "教程角色"},
            {RoleTypeId.Scp096 , "SCP-096" },
            {RoleTypeId.Scp049 , "SCP-049" },
            {RoleTypeId.Scp173 , "SCP-173" },
            {RoleTypeId.Scp939 , "SCP-939" },
            {RoleTypeId.Scp079 , "SCP-079" },
            {RoleTypeId.Scp0492 , "SCP-049-2" },
            {RoleTypeId.Scp106 , "SCP-106" },
            {RoleTypeId.Scp3114 , "SCP-3114" },
            {RoleTypeId.Spectator , "观察者" },
            {RoleTypeId.Overwatch , "监管" },
            {RoleTypeId.Filmmaker , "导演" }
        };
        [Description("聊天系统自定义玩家团队名称")]
        public Dictionary<Team, string> ChatSystemTeamTranslation { get; set; } = new Dictionary<Team, string>
        {
            {Team.Dead , "旁观者" },
            {Team.ClassD , "DD阵营" },
            {Team.OtherAlive , "神秘阵营" },
            {Team.Scientists , "博士阵营" },
            {Team.SCPs , "SCP阵营" },
            {Team.ChaosInsurgency , "混沌阵营" },
            {Team.FoundationForces , "九尾狐阵营" },
        };
        /// <summary>
        /// 
        /// </summary>
        [Description("电梯显示模板(HEX color写死, {p_operator}表示操作人, 没有的话自动为未知):")]
        public string ElevatorTemplate { get; set; } = "[Elevator] 电梯使用者: <color=#B952FA>{p_operator}</color>";
        [Description("SCP-914 显示模板(HEX color写死, {mode}表示操作模式, {p_operator}表示操作人, 没有的话自动为未知):")]
        public string Scp914Template { get; set; } = "[Scp914] 已启动! 模式: <color=#F7C73E>{mode}</color>, 操作人: <color=#0080FF>{p_operator}</color>";
        [Description("SCP-914 模式翻译:")]
        public Dictionary<Scp914KnobSetting, string> Scp914ModeTranslations { get; set; } = new Dictionary<Scp914KnobSetting, string>()
        {
            { Scp914KnobSetting.Rough, "粗加" },
            { Scp914KnobSetting.Coarse, "半粗" },
            { Scp914KnobSetting.OneToOne, "1:1" },
            { Scp914KnobSetting.Fine, "精加" },
            { Scp914KnobSetting.VeryFine, "超精加工" }
        };
        /// <summary>
        /// 
        /// </summary>
        [Description("等级系统_查询失败")]
        public string LevelCommandFailed { get; set; } = "查询失败，等级系统未启用";
        /// <summary>
        /// 
        /// </summary>
        [Description("使用硬币抽卡时背包空间不足提示")]
        public string RewardFailedBroadcastTemplate { get; set; } = "\n<b><size=25><color=#00CC00>你的背包空间不足，无法继续抽卡！</color></size></b>";
        [Description("使用硬币抽卡成功提示")]
        public string RewardOkBroadcastTemplate { get; set; } = "\n<b><size=25><color=#00CC00>🎉 恭喜！玩家 {nickName} 通过抛硬币{result}！</color></size></b>";
        /// <summary>
        /// 
        /// </summary>
        [Description("开局三分钟后使用交换指令的禁止提示")]
        public string SwapCommandTimeLimitBroadcastTemplate { get; set; } = "交换指令只能在开局三分钟内使用";
        [Description("没有待处理交换请求时提示")]
        public string SwapCommandNoRequestBroadcastTemplate { get; set; } = "没有待处理的交换请求";
    }
}
