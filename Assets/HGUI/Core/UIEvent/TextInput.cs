using huqiang.Core.HGUI;
using huqiang.Data;
using huqiang.Data2D;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace huqiang.UIEvent
{
    public enum ContentType
    {
        Standard,
        Autocorrected,
        IntegerNumber,
        DecimalNumber,
        Alphanumeric,
        Name,
        NumberAndName,
        EmailAddress,
        Password,
        Pin,
        Custom
    }
    public enum InputType
    {
        Standard,
        AutoCorrect,
        Password,
    }
    public enum LineType
    {
        SingleLine,
        MultiLineSubmit,
        MultiLineNewline
    }
    public unsafe struct TextInputData
    {
        public Color inputColor;
        public Color tipColor;
        public Color pointColor;
        public Color selectColor;
        public Int32 inputString;
        public Int32 tipString;
        public int CharacterLimit;
        public bool ReadyOnly;
        public ContentType contentType;
        public LineType lineType;
        public static int Size = sizeof(TextInputData);
        public static int ElementSize = Size / 4;
    }
}