using huqiang.Core.HGUI;
using huqiang.UIEvent;
using System;
using System.Collections.Generic;

namespace huqiang.UIComposite
{
    public enum OptionsType
    {
        /// <summary>
        /// 单选
        /// </summary>
        Radio,
        /// <summary>
        /// 多选
        /// </summary>
        MultiChoice
    }
    /// <summary>
    /// 选项组,用于单选和多选按钮
    /// </summary>
    public class OptionGroup
    {
        /// <summary>
        /// 选择类型
        /// </summary>
        public OptionsType options;
        UserEvent m_select;
        UserEvent m_last;
        List<UserEvent> userEvents = new List<UserEvent>();
        /// <summary>
        /// 多选列表
        /// </summary>
        public List<UserEvent> MultiSelect;
        /// <summary>
        /// 事件添加到当前组
        /// </summary>
        /// <param name="user"></param>
        public void AddEvent(UserEvent user)
        {
            if (user == null)
                return;
            user.Click = Click;
            user.AutoColor = false;
            userEvents.Add(user);
        }
        /// <summary>
        /// 添加UI元素到当前组
        /// </summary>
        /// <param name="element"></param>
        public void AddEvent(UIElement element)
        {
            if (element.userEvent == null)
            {
                element.eventType = huqiang.Core.HGUI.EventType.UserEvent;
                element.RegEvent<UserEvent>();
            }
            AddEvent(element.userEvent);
        }
        void Click(UserEvent user, UserAction action)
        {
            switch (options)
            {
                case OptionsType.Radio:
                    Radio(user, action);
                    break;
                case OptionsType.MultiChoice:
                    MultiChoice(user, action);
                    break;
            }
        }
        void Radio(UserEvent user, UserAction action)
        {
            if (m_select == user)
                return;
            m_last = m_select;
            m_select = user;
            if (SelectChanged != null)
                SelectChanged(this, action);
        }
        void MultiChoice(UserEvent user, UserAction action)
        {
            if (MultiSelect == null)
                MultiSelect = new List<UserEvent>();
            if (MultiSelect.Contains(user))
            {
                m_last = user;
                m_select = null;
                MultiSelect.Remove(user);
            }
            else
            {
                m_last = null;
                m_select = user;
                MultiSelect.Add(user);
            }
            if (SelectChanged != null)
                SelectChanged(this, action);
        }
        /// <summary>
        /// 选中项被改变事件
        /// </summary>
        public Action<OptionGroup, UserAction> SelectChanged;
        /// <summary>
        /// 最后选中的事件
        /// </summary>
        public UserEvent LastSelect { get => m_last; }
        /// <summary>
        /// 设置或获取选中事件
        /// </summary>
        public UserEvent Selecet
        {
            get => m_select;
            set
            {
                if (userEvents.Contains(value))
                {
                    switch (options)
                    {
                        case OptionsType.Radio:
                            Radio(value, null);
                            break;
                        case OptionsType.MultiChoice:
                            MultiChoice(value, null);
                            break;
                    }
                }
            }
        }
    }
}
