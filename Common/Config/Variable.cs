using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Config
{
    /// <summary>
    /// 变量
    /// </summary>
    public class Variable
    {
        //配置属性

        /// <summary>
        /// 变量名称
        /// </summary>
        public string VarName { get; set; }

        /// <summary>
        /// 起始索引
        /// </summary>
        public ushort Start { get; set; }

        /// <summary>
        /// 数据类型
        /// </summary>
        public string DataType { get; set; }

        /// <summary>
        /// 偏移量或长度
        /// </summary>
        public int OffsetOrLength { get; set; }

        /// <summary>
        /// 通信组名称
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// 备注说明
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 上升沿报警
        /// </summary>
        public string PosAlarmValue { get; set; }
        private bool posAlarm;

        public bool PosAlarm
        {
            get { return posAlarm; }
            set
            {
                if (value)
                {
                    this.PosAlarmValue = "启用";
                    posAlarm = true;
                }
                else
                {
                    this.PosAlarmValue = "禁用";
                    posAlarm = false;
                }
            }
        }
        /// <summary>
        /// 下降沿报警
        /// </summary>
        public string NegAlarmValue { get; set; }
        private bool negAlarm;

        public bool NegAlarm
        {
            get { return negAlarm; }
            set
            {
                if (value)
                {
                    this.NegAlarmValue = "启用";
                    negAlarm = true;
                }
                else
                {
                    this.NegAlarmValue = "禁用";
                    negAlarm = false;
                }
            }
        }
        /// <summary>
        /// 转换系数
        /// </summary>
        public float Scale { get; set; } = 1.0f;

        /// <summary>
        /// 偏移系数
        /// </summary>
        public float Offset { get; set; } = 0.0f;


        //上升沿逻辑属性
        public bool PosCacheValue { get; set; } = false;
        //下降沿缓存
        public bool NegCacheValue { get; set; } = true;
        //参数值
        public object VarValue { get; set; }
    }
}
