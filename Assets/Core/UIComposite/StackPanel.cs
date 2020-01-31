using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using huqiang.Core.HGUI;
using huqiang.Data;
using huqiang.UIEvent;
using UnityEngine;

namespace huqiang.UIComposite
{
    public enum Direction
    {
        Horizontal,
        Vertical
    }
    public class StackPanel:Composite
    {
        int c = 0;
        public override void Initial(FakeStruct mod, UIElement script)
        {
            base.Initial(mod, script);
            script.SizeChanged = (o) => Order();
        }
        public Direction direction = Direction.Horizontal;
        void Order()
        {
            switch (direction)
            {
                case Direction.Horizontal:
                    OrderHorizontal();
                    break;
                case Direction.Vertical:
                    OrderVertical();
                    break;
            }
        }
        void OrderHorizontal()
        {
            if (Enity != null)
            {
                var sx =Enity.SizeDelta.x * -0.5f;
                var y = Enity.SizeDelta.y;
                var trans = Enity.transform;
                var c = trans.childCount;
                for (int i = 0; i < c; i++)
                {
                    var son = trans.GetChild(i);
                    var ss = son.GetComponent<UIElement>();
                    float w = 0;
                    if (ss != null)
                    {
                        w = ss.SizeDelta.x;
                    }
                    float os = sx + w * 0.5f;
                    son.localPosition = new Vector3(os, 0, 0);
                    son.localScale = Vector3.one;
                    sx += w;
                }
            }
        }
        void OrderVertical()
        {
            if (Enity != null)
            {
                var sy = Enity.SizeDelta.y * 0.5f;
                var x = Enity.SizeDelta.x;
                var trans = Enity.transform;
                var c = trans.childCount;
                for (int i = 0; i < c; i++)
                {
                    var son = trans.GetChild(i);
                    var ss = son.GetComponent<UIElement>();
                    float h = 0;
                    if (ss != null)
                    {
                        h = ss.SizeDelta.y;
                    }
                    float os = sy - h * 0.5f;
                    son.localPosition = new Vector3(0, os, 0);
                    son.localScale = Vector3.one;
                    sy -= h;
                }
            }
        }
        public override void Update(float time)
        {
            var a = Enity.transform.childCount;
            if (a != c)
            {
                c = a;
                Order();
            }
        }
    }
    public enum OptionsType
    {
        Radio,
        MultiChoice
    }
    public class OptionGroup
    {
        public OptionsType options;
        UserEvent m_select;
        UserEvent m_last;
        List<UserEvent> userEvents = new List<UserEvent>();
        public List<UserEvent> MultiSelect;
        public void AddEvent(UserEvent user)
        {
            if (user == null)
                return;
            user.Click = Click;
            user.AutoColor = false;
            userEvents.Add(user);
        }
        public void AddEvent(UIElement element)
        {
            if(element.userEvent==null)
            {
                element.eventType = huqiang.Core.HGUI.EventType.UserEvent;
                element.RegEvent<UserEvent>();
            }
            AddEvent(element.userEvent);
        }
        void Click(UserEvent user,UserAction action)
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
        void Radio(UserEvent user,UserAction action)
        {
            if (m_select == user)
                return;
            m_last = m_select;
            m_select = user;
            if (SelectChanged != null)
                SelectChanged(this,action);
        }
        void MultiChoice(UserEvent user,UserAction action)
        {
            if (MultiSelect == null)
                MultiSelect = new List<UserEvent>();
            if(MultiSelect.Contains(user))
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
        public Action<OptionGroup,UserAction> SelectChanged;
        public UserEvent LastSelect { get => m_last; }
        public UserEvent Selecet
        {
            get => m_select;
            set {
                if(userEvents.Contains(value))
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
