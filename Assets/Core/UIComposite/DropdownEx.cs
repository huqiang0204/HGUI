using huqiang.Core.HGUI;
using huqiang.Data;
using huqiang.UIEvent;
using System;
using System.Collections;
using UnityEngine;

namespace huqiang.UIComposite
{
    public class DropdownEx : Composite
    {
        public class PopItemMod
        {
            public UserEvent Item;
            public HText Label;
            public Transform check;
            [NonSerialized]
            public object data;
            [NonSerialized]
            public int Index;
        }
        HText Label;
        ScrollY m_scroll;
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
        public FakeStruct ItemMod;
        IList DataList;
        public object BindingData { get { return DataList; } set { DataList = value as IList; } }
        public bool down = true;
        public float MaxHeight = 300;
        public float PopOffset = 0;
        public Vector2 ItemSize;
        int s_index;
        public UserEvent callBack;
        public int SelectIndex
        {
            get { return s_index; }
            set
            {
                if (BindingData == null)
                    return;
                if (value < 0)
                {
                    s_index = -1;
                    if (Label != null)
                        Label.Text = "";
                    return;
                }
                if (value >= DataList.Count)
                    value = DataList.Count - 1;
                s_index = value;
                if (Label != null)
                {
                    var dat = DataList[s_index];
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
                    m_scroll = ui.composite as ScrollY;
            }
        }
        void Show(UserEvent back, UserAction action)
        {
            if (m_scroll != null)
            {
                if (ItemMod != null)
                    m_scroll.ItemMod = ItemMod;
                m_scroll.BindingData = BindingData;
                m_scroll.SetItemUpdate<PopItemMod,object>(ItemUpdate);
                m_scroll.eventCall.LostFocus = LostFocus;
                m_scroll.eventCall.DataContext = this;

                action.AddFocus(m_scroll.eventCall);
            }
        }

        public Action<DropdownEx, object> OnSelectChanged;

        void LostFocus(UserEvent eve, UserAction action)
        {
            m_scroll.Enity.gameObject.SetActive(false);
        }
        GameObject Checked;
        void ItemUpdate(PopItemMod g,object o, int index)
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
            if (item.Label != null)
            {
                if (o is string)
                    item.Label.Text = o as string;
                else item.Label.Text = o.ToString();
            }
            if (item.check != null)
            {
                if (index == SelectIndex)
                {
                    item.check.gameObject.SetActive(true);
                    Checked = item.check.gameObject;
                }
                else item.check.gameObject.SetActive(false);
            }
        }
        void ItemClick(UserEvent eventCall, UserAction action)
        {
            if (Checked != null)
                Checked.SetActive(false);
            PopItemMod mod = eventCall.DataContext as PopItemMod;
            if (mod == null)
                return;
            if (mod.check != null)
                mod.check.gameObject.SetActive(true);
            SelectIndex = mod.Index;
            if (Label != null)
            {
                if (mod.data is string)
                    Label.Text = mod.data as string;
                else Label.Text = mod.data.ToString();
            }
            if (OnSelectChanged != null)
                OnSelectChanged(this, mod.data);
            scrollY.Enity.gameObject.SetActive(false);
        }
    }
}
