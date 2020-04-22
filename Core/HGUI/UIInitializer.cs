using huqiang.Data;
using huqiang.UIComposite;
using huqiang.UIEvent;
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
            feildLenth = temp.Top;
            target = null;
        }
        public override void Reset(object obj)
        {
            target = obj;
            reflections.Top = feildLenth;
        }
        public override void Initialiezd(FakeStruct fake, Component com)
        {
            if (reflections == null)
                return;
            for (int i = 0; i < reflections.Top; i++)
            {
                var m = reflections.All[i];
                if (m.name == com.name)
                {
                    if (m.FieldType == typeof(GameObject))
                        m.Value = com.gameObject;
                    else if (typeof(Component).IsAssignableFrom(m.FieldType))
                        m.Value = com.GetComponent(m.FieldType);
                    else if (typeof(Composite).IsAssignableFrom(m.FieldType))
                    {
                        var scr = com.GetComponent<UIElement>();
                        if (scr != null)
                        {
                            if (scr.composite == null)
                            {
                                var obj = Activator.CreateInstance(m.FieldType) as Composite;
                                obj.Initial(fake, scr);
                                m.Value = obj;
                            }
                            else
                                m.Value = scr.composite;
                        }
                    }
                    else if (m.FieldType == typeof(FakeStruct))
                        m.Value = fake;
                    else
                    {
                        var scr = com.GetComponent<UIElement>();
                        if (scr != null)
                        {
                            if (scr.userEvent == null)
                            {
                                scr.userEvent = Activator.CreateInstance(m.FieldType) as UserEvent;
                                scr.userEvent.Context = scr;
                                scr.userEvent.g_color = scr.MainColor;
                            }
                            m.Value = scr.userEvent;
                        }
                    }
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
        public void ReflectionEnity(object obj, Transform com)
        {
            target = obj;
            reflections.Top = feildLenth;
            ReflectionEnity(com);
            ReflectionModel[] all = reflections.All;
            for (int i = 0; i < all.Length; i++)
                all[i].field.SetValue(target, all[i].Value);
        }
        void ReflectionEnity(Transform com)
        {
            for (int i = 0; i < reflections.Top; i++)
            {
                var m = reflections.All[i];
                if (m.name == com.name)
                {
                    if (typeof(Component).IsAssignableFrom(m.FieldType))
                        m.Value = com.GetComponent(m.FieldType);
                    else if (typeof(Composite).IsAssignableFrom(m.FieldType))
                    {
                        var scr = com.GetComponent<UIElement>();
                        if (scr != null)
                            m.Value = scr.composite;
                    }
                    else
                    {
                        var scr = com.GetComponent<UIElement>();
                        if (scr != null)
                            m.Value = scr.userEvent;
                    }
                    reflections.Top--;
                    var j = reflections.Top;
                    var a = reflections.All[j];
                    reflections.All[i] = a;
                    reflections.All[j] = m;
                    break;
                }
            }
            int c = com.childCount;
            for (int i = 0; i < c; i++)
                ReflectionEnity(com.GetChild(i));
        }
    }
}
