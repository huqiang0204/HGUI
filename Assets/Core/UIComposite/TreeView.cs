﻿using huqiang.Core.HGUI;
using huqiang.Data;
using huqiang.UIEvent;
using System.Collections.Generic;
using UnityEngine;

namespace huqiang.UIComposite
{
    public class TreeViewNode
    {
        public bool extand;
        public string content;
        public Vector2 offset;
        public List<TreeViewNode> child = new List<TreeViewNode>();
    }
    public class TreeViewItem
    {
        public GameObject target;
        public HText text;
        public UserEvent callBack;
        public TreeViewNode node;
    }
    public class TreeView : Composite
    {
        public Vector2 Size;//scrollView的尺寸
        Vector2 contentSize;
        public Vector2 ItemSize;
        public TreeViewNode nodes;
        public float ItemHigh = 16;
        public UserEvent eventCall;//scrollY自己的按钮
        public FakeStruct ItemMod;
        float m_pointY;
        float m_pointX;
        public SwapBuffer<TreeViewItem, TreeViewNode> swap;
        QueueBuffer<TreeViewItem> queue;
        public TreeView()
        {
            swap = new SwapBuffer<TreeViewItem, TreeViewNode>(512);
            queue = new QueueBuffer<TreeViewItem>(256);
        }
        public override void Initial(FakeStruct fake,UIElement script)
        {
            base.Initial(fake,script);
            eventCall = script.RegEvent<UserEvent>();
            eventCall.Drag = (o, e, s) => { Scrolling(o, s); };
            eventCall.DragEnd = (o, e, s) => { Scrolling(o, s); };
            eventCall.Scrolling = Scrolling;
            eventCall.ForceEvent = true;
            eventCall.AutoColor = false;
            Size = Enity.SizeDelta;
            eventCall.CutRect = true;
            ItemMod = HGUIManager.FindChild(fake, "Item");
            if (ItemMod != null)
            {
                HGUIManager.GameBuffer.RecycleChild(script.gameObject);
                unsafe { ItemSize = ((TransfromData*)ItemMod.ip)->size; }
                ItemHigh = ItemSize.y;
            }
        }
        void Draging(UserEvent back, UserAction action, Vector2 v)
        {
            back.DecayRateY = 0.998f;
            Scrolling(back, v);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="back"></param>
        /// <param name="v">移动的实际像素位移</param>
        void Scrolling(UserEvent back, Vector2 v)
        {
            if (Enity == null)
                return;
            var trans = eventCall.Context.transform;
            v.x /= trans.localScale.x;
            v.y /= trans.localScale.y;
            LimitX(back, -v.x);
            LimitY(back, v.y);
            Refresh();
        }
        float hy;
        float hx;
        public void Refresh()
        {
            if (nodes == null)
                return;
            hy = Size.y * 0.5f;
            hx = Size.x * 0.5f;
            contentSize.x = ItemSize.x;
            contentSize.y = CalculHigh(nodes, 0, 0);
            RecycleItem();
            if (m_pointX + ItemSize.x > contentSize.x)
                m_pointX = contentSize.x - ItemSize.x;

            for (int i = 0; i < swap.Length; i++)
            {
                var trans = swap[i].target.transform;
                var p = trans.localPosition;
                p.x -= m_pointX;
                trans.localPosition = p;
            }
        }
        protected void RecycleItem()
        {
            int len = swap.Length;
            for (int i = 0; i < len; i++)
            {
                var it = swap.Pop();
                it.target.SetActive(false);
                queue.Enqueue(it);
            }
            swap.Done();
        }

        float CalculHigh(TreeViewNode node, int level, float high)
        {
            float sx = level * ItemHigh + ItemSize.x * 0.5f - hx;
            node.offset.x = sx;
            node.offset.y = high;
            UpdateItem(node);
            level++;
            high += ItemHigh;
            if (node.extand)
                for (int i = 0; i < node.child.Count; i++)
                    high = CalculHigh(node.child[i], level, high);
            float x = level * ItemHigh + ItemSize.x;
            if (x > contentSize.x)
                contentSize.x = x;
            return high;
        }
        void UpdateItem(TreeViewNode node)
        {
            float dy = node.offset.y - m_pointY;
            if (dy <= Size.y)
                if (dy + ItemHigh > 0)
                {
                    var item = swap.Exchange((o, e) => { return o.node == e; }, node);
                    if (item == null)
                    {
                        item = CreateItem();
                        swap.Push(item);
                        item.node = node;
                        if (item.text != null)
                        {
                            if (node.child.Count > 0)
                                item.text.Text = "▷ " + node.content;
                            else item.text.Text = node.content;
                        }
                    }
                    var m = item.callBack.Context;
                    m.transform.localPosition = new Vector3(node.offset.x, hy - dy - ItemHigh * 0.5f, 0);
                }
        }
        protected TreeViewItem CreateItem()
        {
            TreeViewItem it = queue.Dequeue();
            if (it != null)
            {
                it.target.SetActive(true);
                return it;
            }
            var go = HGUIManager.GameBuffer.Clone(ItemMod);
            var trans = go.transform;
            trans.SetParent(Enity.transform);
            trans.localScale = Vector3.one;
            trans.localRotation = Quaternion.identity;
            TreeViewItem a = new TreeViewItem();
            a.target = go;
            a.text = go.GetComponent<HText>();
            a.callBack = a.text.RegEvent<UserEvent>();
            a.callBack.Click = (o, e) => {
                var item = o.DataContext as TreeViewItem;
                if (item.node != null)
                {
                    item.node.extand = !item.node.extand;
                    Refresh();
                }
            };
            a.callBack.DataContext = a;
            return a;
        }
        protected void LimitX(UserEvent callBack, float x)
        {
            var size = Size;
            if (size.x > contentSize.x)
            {
                m_pointX = 0;
                return;
            }
            if (x == 0)
                return;
            float vx = m_pointX + x;
            if (vx < 0)
            {
                m_pointX = 0;
                eventCall.VelocityX = 0;
                return;
            }
            else if (vx + size.x > contentSize.x)
            {
                m_pointX = contentSize.x - size.x;
                eventCall.VelocityX = 0;
                return;
            }
            m_pointX += x;
        }
        protected void LimitY(UserEvent callBack, float y)
        {
            var size = Size;
            if (size.y > contentSize.y)
            {
                m_pointY = 0;
                return;
            }
            if (y == 0)
                return;
            float vy = m_pointY + y;
            if (vy < 0)
            {
                m_pointY = 0;
                eventCall.VelocityY = 0;
                return;
            }
            else if (vy + size.y > contentSize.y)
            {
                m_pointY = contentSize.y - size.y;
                eventCall.VelocityY = 0;
                return;
            }
            m_pointY += y;
        }
        public float PercentageX {
            get {
                float o = contentSize.x - Enity.SizeDelta.x;
                if (o < 0)
                    return 0;
                o = m_pointX / o;
                if (o > 1)
                    o = 1;
                return o;
            }
            set {
                if (value < 0)
                    value = 0;
                else if (value > 1)
                    value = 1;
                if(contentSize.x>Enity.SizeDelta.x)
                    m_pointX = value * (contentSize.x - Enity.SizeDelta.x);
            }
        }
        public float PercentageY
        {
            get
            {
                float o = contentSize.y - Enity.SizeDelta.y;
                if (o < 0)
                    return 0;
                o = m_pointY / o;
                if (o > 1)
                    o = 1;
                return o;
            }
            set
            {
                if (value < 0)
                    value = 0;
                else if (value > 1)
                    value = 1;
                if (contentSize.y > Enity.SizeDelta.y)
                    m_pointY = value * (contentSize.y - Enity.SizeDelta.y);
            }
        }
    }
}
