using huqiang.Core.HGUI;
using huqiang.Data;
using huqiang.UIEvent;
using System;
using System.Collections;
using UnityEngine;

namespace huqiang.UIComposite
{
    public class PopItemMod
    {
        public UserEvent Item;
        public HText Text;
        public Transform Check;
        [NonSerialized]
        public object data;
        [NonSerialized]
        public int Index;
    }
    public class DropdownEx : Composite
    {
        HText Label;
        ScrollY m_scroll;
        /// <summary>
        /// 用于更新Item的方法推荐使用SetItemUpdate<T, U>()where T:PopItemMod,new()
        /// </summary>
        public ScrollY scrollY
        {
            get { return m_scroll; }
            set
            {
                m_scroll = value;
                if (value == null)
                    return;
                ItemSize = m_scroll.ItemSize;
                MaxHeight = m_scroll.Enity.SizeDelta.y;
                m_scroll.Enity.gameObject.SetActive(false);
            }
        }
        public bool down = true;
        public float MaxHeight = 300;
        public float PopOffset = 0;
        public Vector2 ItemSize;
        int s_index = -1;
        public UserEvent callBack;
        public int SelectIndex
        {
            get { return s_index; }
            set
            {
                if (m_scroll == null)
                    return;
                int len = m_scroll.DataLength;
                if (value < 0)
                {
                    s_index = -1;
                    if (Label != null)
                        Label.Text = "";
                    return;
                }
                if (value >= len)
                    value = len - 1;
                s_index = value;
                if (Label != null)
                {
                    var dat = m_scroll.GetData(s_index);
                    if (dat is string)
                        Label.Text = dat as string;
                    else Label.Text = dat.ToString();
                }
            }
        }
        public override void Initial(FakeStruct mod, UIElement script)
        {
            base.Initial(mod,script);
            var trans = Enity.transform;
            Label = trans.Find("Label").GetComponent<HText>();
            callBack = Enity.RegEvent<UserEvent>();
            callBack.Click = Show;
            var scroll = trans.Find("Scroll");
            if(scroll!=null)
            {
                scroll.gameObject.SetActive(false);
                var ui = scroll.GetComponent<UIElement>();
                if (ui != null)
                {
                    m_scroll = ui.composite as ScrollY;
                    if (m_scroll != null)
                    {
                        m_scroll.SetItemUpdate<PopItemMod, object>(ItemUpdate);
                        m_scroll.eventCall.LostFocus = LostFocus;
                        m_scroll.eventCall.DataContext = this;
                    }
                }  
            }
        }
        int showAni;
        float showTime;
        Vector2 size;
        void Show(UserEvent back, UserAction action)
        {
            if (m_scroll != null)
            {
                if(!m_scroll.Enity.gameObject.activeSelf)
                {
                    m_scroll.Enity.gameObject.SetActive(true);
                    action.AddFocus(m_scroll.eventCall);
                    showAni = 1;
                    showTime = 0;
                    size = m_scroll.Enity.SizeDelta;
                    m_scroll.Enity.SizeDelta = new Vector2(size.x,0);
                }
            }
        }

        public Action<DropdownEx, object> OnSelectChanged;

        void LostFocus(UserEvent eve, UserAction action)
        {
            if (action.ExistFocus(callBack))
                action.AddFocus(eve);
            else {
                //m_scroll.Enity.gameObject.SetActive(false); 
                showAni = -1;
                showTime = 0;
            }
        }
        GameObject Checked;
        public void ItemUpdate(PopItemMod g,object o, int index)
        {
            PopItemMod item = g as PopItemMod;
            if (item == null)
                return;

            item.Index = index;
            item.data = o;
            if (item.Item != null)
            {
                item.Item.DataContext = item;
                item.Item.Click = ItemClick;
            }
            if (item.Text != null)
            {
                if (o is string)
                    item.Text.Text = o as string;
                else item.Text.Text = o.ToString();
            }
            if (item.Check != null)
            {
                if (index == SelectIndex)
                {
                    item.Check.gameObject.SetActive(true);
                    Checked = item.Check.gameObject;
                }
                else item.Check.gameObject.SetActive(false);
            }
        }
        void ItemClick(UserEvent eventCall, UserAction action)
        {
            if (Checked != null)
                Checked.SetActive(false);
            PopItemMod mod = eventCall.DataContext as PopItemMod;
            if (mod == null)
                return;
            if (mod.Check != null)
            { 
                mod.Check.gameObject.SetActive(true);
                Checked = mod.Check.gameObject;
            }
            SelectIndex = mod.Index;
            if (Label != null)
            {
                if (mod.data is string)
                    Label.Text = mod.data as string;
                else Label.Text = mod.data.ToString();
            }
            if (OnSelectChanged != null)
                OnSelectChanged(this, mod.data);
            showAni = -1;
            showTime = 0;
        }
        public override void Update(float time)
        {
            if(showAni>0)
            {
                showTime += time;
                if (showTime > 300)
                {
                    showTime = 300;
                    showAni = 0;
                }
                float y = showTime / 300;
                y *= size.y;
                m_scroll.Enity.SizeDelta = new Vector2(size.x, y);
            }else if(showAni<0)
            {
                showTime += time;
                if (showTime >= 300)
                {
                    showAni = 0;
                    m_scroll.Enity.gameObject.SetActive(false);
                    m_scroll.Enity.SizeDelta = size;
                }
                else
                {
                    float y = 1 - showTime / 300;
                    y *= size.y;
                    m_scroll.Enity.SizeDelta = new Vector2(size.x, y);
                }
            }
        }
    }
}
