using huqiang.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace huqiang.Core.HGUI
{
    public class UIInitializer:Initializer
    {
        TempReflection reflections;
        object target;
        int feildLenth;
        public UIInitializer(object obj)
        {
            target = obj;
            reflections = TempReflection.ObjectFields(obj);
            feildLenth = reflections.Top;
        }
        public UIInitializer(TempReflection temp)
        {
            reflections = temp;
            target = null;
        }
        public void Reset(object obj)
        {
            target = obj;
            reflections.Top = feildLenth;
        }
        public override void Initialiezd(FakeStruct fake, Component com)
        {
            if (reflections == null)
                return;
            var scr = com.GetComponent<UIElement>();
            if (scr != null)
                scr.Initial(fake);
            for (int i = 0; i < reflections.Top; i++)
            {
                var m = reflections.All[i];
                if (m.name == com.name)
                {
                    if (typeof(Component).IsAssignableFrom(m.FieldType))
                        m.Value = com.GetComponent(m.FieldType);
                    reflections.Top--;
                    var j = reflections.Top;
                    var a = reflections.All[j];
                    reflections.All[i] = a;
                    reflections.All[j] = m;
                    break;
                }
            }
        }
        public override void Done()
        {
            if (target == null)
                return;
            ReflectionModel[] all = reflections.All;
            for (int i = 0; i < all.Length; i++)
                all[i].field.SetValue(target, all[i].Value);
        }
    }
}
