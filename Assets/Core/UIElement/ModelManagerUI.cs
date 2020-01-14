using huqiang.Core.HGUI;
using huqiang.Data;
using huqiang.Pool;
using huqiang.UIComposite;
using huqiang.UIEvent;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;


namespace huqiang.UI
{
    public class ReflectionModel
    {
        public string name;
        public FieldInfo field;
        public Type FieldType;
        public object Value;
    }
    public class TempReflection
    {
        public int Top;
        public ReflectionModel[] All;
    }
}
