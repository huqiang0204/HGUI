using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huqiang.Data
{
    /// <summary>
    /// 通信用的消息标准
    /// </summary>
    [Serializable]
    public class Msg
    {
        /// <summary>
        /// 消息类型
        /// </summary>
        public Int32 Type;
        /// <summary>
        /// 消息指令
        /// </summary>
        public Int32 Cmd;
        /// <summary>
        /// 错误代码,0为无错误
        /// </summary>
        public Int32 Error;
        /// <summary>
        /// 附带参数
        /// </summary>
        public string Args;
    }
}
