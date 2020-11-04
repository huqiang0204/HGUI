using huqiang.Data;
using huqiang.UIComposite;
using huqiang.UIEvent;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace huqiang.Core.HGUI
{
    /// <summary>
    /// ui初始化器
    /// </summary>
    public class UIInitializer:Initializer
    {
        TempReflection reflections;
        object target;
        int feildLenth;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="obj">对象实例</param>
        public UIInitializer(object obj)
        {
            target = obj;
            reflections = TempReflection.ObjectFields(obj);
            feildLenth = reflections.Top;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="temp">对象反射信息</param>
        public UIInitializer(TempReflection temp)
        {
            reflections = temp;
            feildLenth = temp.Top;
            target = null;
        }
        /// <summary>
        /// 设置一个新的目标实例
        /// </summary>
        /// <param name="obj">对象实例</param>
        public override void Reset(object obj)
        {
            target = obj;
            reflections.Top = feildLenth;
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="fake">模型数据</param>
        /// <param name="com">组件实体</param>
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
                                obj.Initial(fake, scr, this);
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
                                scr.userEvent.Initial(fake);
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
        /// <summary>
        /// 初始化完毕
        /// </summary>
        public override void Done()
        {
            base.Done();
            if (target == null)
                return;
            ReflectionModel[] all = reflections.All;
            for (int i = 0; i < all.Length; i++)
                all[i].field.SetValue(target, all[i].Value);
        }
        /// <summary>
        /// 反射实体到目标载体
        /// </summary>
        /// <param name="obj">载体实例对象</param>
        /// <param name="com">ui组件实例</param>
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
        /// <summary>
        /// 更换使用语言,将配置文件中的语言反射到UI组件上
        /// </summary>
        /// <param name="section"></param>
        public void ChangeLanguage(INISection section)
        {
            if (reflections == null)
                return;
            var all = reflections.All;
            for (int i = 0; i < all.Length; i++)
            {
                HText txt = all[i].Value as HText;
                if (txt != null)
                {
                    var str = section.GetValue(all[i].name);
                    if (str != null)
                    {
                        if (str != "")
                            txt.Text = str.Replace("\\n", "\n");
                        else txt.Text = str;
                    }
                }
                else
                {
                    InputBox box = all[i].Value as InputBox;
                    if(box!=null)
                    {
                        var str = section.GetValue(all[i].name);
                        if (str != null)
                        {
                            if (str != "")
                                box.TipString = str.Replace("\\n", "\n");
                            else box.TipString = str;
                        }
                    }
                }
            }
        }

    }
}
