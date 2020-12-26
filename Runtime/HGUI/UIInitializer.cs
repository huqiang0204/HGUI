using huqiang.Data;
using huqiang.UIComposite;
using huqiang.UIEvent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huqiang.Core.HGUI
{
    /// <summary>
    /// ui初始化器
    /// </summary>
    public class UIInitializer : Initializer
    {
        protected struct ContextUIAction
        {
            public Action<UIElement> CallBack;
            public int InsID;
        }
        protected struct ContextUI
        {
            public UIElement Ins;
            public int InsID;
        }
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
            uicontexts.Clear();
            uiobjects.Clear();
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="fake">模型数据</param>
        /// <param name="com">组件实体</param>
        public void Initialiezd(FakeStruct fake, UIElement com)
        {
            if (reflections == null)
                return;
            for (int i = 0; i < reflections.Top; i++)
            {
                var m = reflections.All[i];
                if (m.name == com.name)
                {
                    if (typeof(UIElement).IsAssignableFrom(m.FieldType))
                        m.Value = com;
                    else if (typeof(Composite).IsAssignableFrom(m.FieldType))
                    {
                        if (com.composite == null)
                        {
                            var obj = Activator.CreateInstance(m.FieldType) as Composite;
                            obj.Initial(fake, com, this);
                            m.Value = obj;
                        }
                        else
                            m.Value = com.composite;
                    }
                    else if (m.FieldType == typeof(FakeStruct))
                        m.Value = fake;
                    else if (typeof(UserEvent).IsAssignableFrom(m.FieldType))
                    {
                        if (com.userEvent == null)
                        {
                            com.userEvent = Activator.CreateInstance(m.FieldType) as UserEvent;
                            com.userEvent.Context = com;
                            com.userEvent.g_color = com.MainColor;
                            com.userEvent.Initial(fake);
                        }
                        m.Value = com.userEvent;
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
            int c = uicontexts.Count;
            int m = uiobjects.Count;
            for (int i = 0; i < c; i++)
            {
                var act = uicontexts[i].CallBack;
                int id = uicontexts[i].InsID;
                if (act != null)
                {
                    for (int j = 0; j < m; j++)
                    {
                        if (uiobjects[j].InsID == id)
                        {
                            act(uiobjects[j].Ins);
                            break;
                        }
                    }
                }
            }
            uiobjects.Clear();
            uicontexts.Clear();
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
        public void ReflectionEnity(object obj,UIElement com)
        {
            target = obj;
            reflections.Top = feildLenth;
            ReflectionEnity(com);
            ReflectionModel[] all = reflections.All;
            for (int i = 0; i < all.Length; i++)
                all[i].field.SetValue(target, all[i].Value);
        }
        void ReflectionEnity(UIElement com)
        {
            for (int i = 0; i < reflections.Top; i++)
            {
                var m = reflections.All[i];
                if (m.name == com.name)
                {
                    if (typeof(UIElement).IsAssignableFrom(m.FieldType))
                        m.Value = com;
                    else if (typeof(Composite).IsAssignableFrom(m.FieldType))
                    {
                            m.Value = com.composite;
                    }
                    else
                    {
                            m.Value = com.userEvent;
                    }
                    reflections.Top--;
                    var j = reflections.Top;
                    var a = reflections.All[j];
                    reflections.All[i] = a;
                    reflections.All[j] = m;
                    break;
                }
            }
            int c = com.child.Count;
            for (int i = 0; i < c; i++)
                ReflectionEnity(com.child[i]);
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
                    if (box != null)
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
        /// <summary>
        /// 添加联系上下文
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="insID"></param>
        public void AddContext(UIElement trans, int insID)
        {
            ContextUI co = new ContextUI();
            co.Ins = trans;
            co.InsID = insID;
            uiobjects.Add(co);
        }
        protected List<ContextUI> uiobjects = new List<ContextUI>();
        protected List<ContextUIAction> uicontexts = new List<ContextUIAction>();
        public void AddContextAction(Action<UIElement> action, int insID)
        {
            ContextUIAction ca = new ContextUIAction();
            ca.CallBack = action;
            ca.InsID = insID;
            uicontexts.Add(ca);
        }
    }
}
