using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace huqiang.Communication
{
    public class EnvelopeType
    {
        public const byte Mate = 0;
        public const byte AesMate = 1;
        public const byte Json = 2;
        public const byte AesJson = 3;
        public const byte DataBuffer = 4;
        public const byte AesDataBuffer = 5;
        public const byte String = 6;
        public const byte AesString = 7;
        public const byte ProtoBuf = 8;
        public const byte Lz4AesJson = 9;
        public const byte Success = 128;
        public const byte Failed = 129;
        public const byte Heart = 130;
    }
}
