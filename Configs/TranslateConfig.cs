using EasyTools.Utils;
using PlayerRoles;
using Scp914;
using System.Collections.Generic;
using System.ComponentModel;

namespace EasyTools.Configs
{
    public class TranslateConfig
    {
        [Description("自救指令_错误")]
        public string RescueCommandError { get; set; } = "执行指令时发生错误，请稍后再试";
        [Description("自救指令_失败")]
        public string RescueCommandFailed { get; set; } = "失败，可能指令未启用或者身份不允许等";
        [Description("自救指令_成功")]
        public string RescueCommandOk { get; set; } = "成功";
        [Description("聊天指令_错误")]
        public string ChatCommandError { get; set; } = "发送消息时出现错误，请稍后重试";
        [Description("聊天指令_失败")]
        public string ChatCommandFailed { get; set; } = "发送失败，你被禁言或者信息为空或者聊天系统未启用";
        [Description("聊天指令_成功")]
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
        public string elev_template { get; set; } = "[Elevator] 电梯使用者: <color=#B952FA>{p_operator}</color>";
        [Description("SCP914显示模板(HEX color写死, {mode}表示操作模式, {p_operator}表示操作人, 没有的话自动为未知):")]
        public string scp914_template { get; set; } = "[Scp914] 已启动! 模式: <color=#F7C73E>{mode}</color>, 操作人: <color=#0080FF>{p_operator}</color>";
        [Description("SCP914, Rough模式翻译:")]
        public Dictionary<Scp914KnobSetting, string> scp914_trans { get; set; } = new Dictionary<Scp914KnobSetting, string>()
        {
            { Scp914KnobSetting.Rough, "粗加" },
            { Scp914KnobSetting.Coarse, "半粗" },
            { Scp914KnobSetting.OneToOne, "1:1" },
            { Scp914KnobSetting.Fine, "精加" },
            { Scp914KnobSetting.VeryFine, "超精加工" }
        };
    }
}
