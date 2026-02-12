using System.ComponentModel;

namespace EasyTools.Configs
{
    public class HUDInfoConfig
    {
        [Description("是否显示SCP914提示信息")]
        public bool info_914 { get; set; } = true;
        [Description("是否显示电梯提示信息")]
        public bool info_elevator { get; set; } = true;

        /// /////////////////////////////////////////////////
        [Description("914显示,X轴坐标(0为正中,-为左,+为右):")]
        public float _914_x { get; set; } = 0;
        [Description("914显示,Y轴坐标(屏幕最上端大概100,最下端大概1000):")]
        public float _914_y { get; set; } = 80;
        [Description("914显示,字体大小:")]
        public int _914_font { get; set; } = 20;

        /// /////////////////////////////////////////////////
        [Description("电梯显示,X轴坐标(0为正中,-为左,+为右):")]
        public float _elev_x { get; set; } = 0;
        [Description("电梯显示,Y轴坐标(屏幕最上端大概100,最下端大概1000):")]
        public float _elev_y { get; set; } = 800;
        [Description("电梯显示,字体大小:")]
        public int _elev_font { get; set; } = 20;

        [Description("电梯提示显示可见范围（操作者为中心）:")]
        public float elev_range { get; set; } = 10f;
    }
}
