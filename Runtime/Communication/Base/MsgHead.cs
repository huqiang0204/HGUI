using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace huqiang.Communication
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct MsgHead
    {
        /// <summary>
        /// 数据压缩类型
        /// </summary>
        public byte Type;//1
        /// <summary>
        /// 此消息的id
        /// </summary>
        public UInt16 MsgID;//2
        /// <summary>
        /// 此消息的某个分卷
        /// </summary>
        public UInt16 CurPart;//2
        /// <summary>
        /// 此消息总计分卷
        /// </summary>
        public UInt16 AllPart;//2
        /// <summary>
        /// 此消息分卷长度
        /// </summary>
        public UInt16 PartLen;//2
        /// <summary>
        /// 此消息总计长度
        /// </summary>
        public UInt32 Lenth;//4
        /// <summary>
        /// 此消息发送的时间戳
        /// </summary>
        public Int16 Time;//2
        public static unsafe int Size = sizeof(MsgHead);
    }
}
