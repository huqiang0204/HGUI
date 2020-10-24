using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace huqiang
{
    /// <summary>
    /// 封包数据类型,可以自行添加
    /// </summary>
    public class EnvelopeType
    {
        /// <summary>
        /// 元数据
        /// </summary>
        public const byte Mate = 0;
        /// <summary>
        /// aes加密的元数据
        /// </summary>
        public const byte AesMate = 1;
        /// <summary>
        /// Json数据
        /// </summary>
        public const byte Json = 2;
        /// <summary>
        /// aes加密的接送数据
        /// </summary>
        public const byte AesJson = 3;
        /// <summary>
        /// DataBuffer类型数据
        /// </summary>
        public const byte DataBuffer = 4;
        /// <summary>
        /// aes加密过的DataBuffer数据
        /// </summary>
        public const byte AesDataBuffer = 5;
        /// <summary>
        /// 字符串
        /// </summary>
        public const byte String = 6;
        /// <summary>
        /// aes加密过的字符串
        /// </summary>
        public const byte AesString = 7;
        /// <summary>
        /// ProtoBuf数据
        /// </summary>
        public const byte ProtoBuf = 8;
        /// <summary>
        /// 使用Lz4压缩过并用aes加密的Json数据
        /// </summary>
        public const byte Lz4AesJson = 9;
        /// <summary>
        /// 这是一个消息成功的回执
        /// </summary>
        public const byte Success = 128;
        /// <summary>
        /// 这是一个失败的消息
        /// </summary>
        public const byte Failed = 129;
        /// <summary>
        /// 这是一个心跳包
        /// </summary>
        public const byte Heart = 130;
    }
}
