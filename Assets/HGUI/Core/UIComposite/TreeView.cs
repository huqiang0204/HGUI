using huqiang.Core.HGUI;
using huqiang.Data;
using huqiang.UIEvent;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace huqiang.UIComposite
{
    public class TreeViewNode
    {
        /// <summary>
        /// 展开
        /// </summary>
        public bool expand;
        public string content;
        /// <summary>
        /// 绑定的数据,联系上下文
        /// </summary>
        public object context;
        public Vector2 offset;
        public List<TreeViewNode> child = new List<TreeViewNode>();
        public void Add(TreeViewNode node)
        {
            if (node != null)
            {
                child.Add(node);
                node.parent = this;
            }
        }
        public void SetParent(TreeViewNode node)
        {
            if (parent != node)
                parent.child.Remove(this);
            if (node != null)
                node.child.Add(this);
        }
        public TreeViewNode parent { get; private set; }
        public int[] Level
        {
            get
            {
                List<int> tmp = new List<int>();
                var p = parent;
                var s = this;
                for (int i = 0; i < 1024; i++)
                {
                    if (p != null)
                    {
                        tmp.Add(p.child.IndexOf(s));
                        s = p;
                        p = p.parent;
                    }
                    else break;
                }
                if (tmp.Count > 0)
                {
                    int c = tmp.Count;
                    int e = c - 1;
                    int[] buf = new int[c];
                    for (int i = 0; i < buf.Length; i++)
                    {
                        buf[i] = tmp[e];
                        e--;
                    }
                    return buf;
                }
                else return new int[1];

            }
        }
        public TreeViewNode Find(int[] level)
        {
            int l = level.Length;
            TreeViewNode p = this;
            for (int i = 0; i < l; i++)
            {
                int c = level[i];
                if (c < 0)
                    return null;
                else if (c >= p.child.Count)
                    return null;
                p = p.child[c];
            }
            return p;
        }
        public void Expand()
        {
            var p = parent;
            for (int i = 0; i < 1024; i++)
            {
                if (p == null)
                    return;
                p.expand = true;
                p = p.parent;
            }
        }
    }
    public class TreeViewItem
    {
        public GameObject target;
        public HText Text;
        public UserEvent Item;
        public TreeViewNode node;
    }
    public class TVConstructor
    {
        public UIInitializer initializer;
        public virtual TreeViewItem Create() { return null; }
        public virtual void Update(TreeViewItem obj, TreeViewNode dat) { }
    }
    public class TVMiddleware<T, U> : TVConstructor where T : TreeViewItem, new() where U : TreeViewNode, new()
    {
        public TVMiddleware()
        {
            initializer = new UIInitializer(TempReflection.ObjectFields(typeof(T)));
        }
        public override TreeViewItem Create()
        {
            var t = new T();
            initializer.Reset(t);
            return t;
        }
        public Action<T, U> Invoke;
        public override void Update(TreeViewItem obj, TreeViewNode dat)
        {
            if (Invoke != null)
                Invoke(obj as T, dat as U);
        }
    }
    public class TreeView : Composite
    {
        public Vector2 Size;//scrollView的尺寸
        Vector2 contentSize;
        public Vector2 ItemSize;
        public TreeViewNode nodes;
        public float ItemHigh = 30;
        public UserEvent eventCall;//scrollY自己的按钮
        public FakeStruct ItemMod;
        float m_pointY;
        float m_pointX;
        public SwapBuffer<TreeViewItem, TreeViewNode> swap;
        QueueBuffer<TreeViewItem> queue;
        public Action<TreeView, TreeViewItem> SelectChanged;
        public TreeViewNode SelectNode { get; set; }
        public TreeView()
        {
            swap = new SwapBuffer<TreeViewItem, TreeViewNode>(512);
            queue = new QueueBuffer<TreeViewItem>(256);
        }
        public override void Initial(FakeStruct fake, UIElement script)
        {
            base.Initial(fake, script);
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
            Enity.SizeChanged = (o) => { Refresh(); };
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
            Size = Enity.SizeDelta;
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
            if (node.expand)
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
                    }
                    if (creator != null)
                        creator.Update(item, node);
                    if (item.Text != null)
                    {
                        if (node.child.Count > 0)
                        {
                            if (node.expand)
                                item.Text.Text = "▼ " + node.content;
                            else
                                item.Text.Text = "► " + node.content;
                        }
                        else item.Text.Text = node.content;
                    }
                    var m = item.Item.Context;
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
            if (creator != null)
            {
                var t = creator.Create();
                t.target = HGUIManager.GameBuffer.Clone(ItemMod, creator.initializer);
                var trans = t.target.transform;
                trans.SetParent(Enity.transform);
                trans.localScale = Vector3.one;
                trans.localRotation = Quaternion.identity;
                return t;
            }
            else
            {
                var go = HGUIManager.GameBuffer.Clone(ItemMod);
                var trans = go.transform;
                trans.SetParent(Enity.transform);
                trans.localScale = Vector3.one;
                trans.localRotation = Quaternion.identity;
                TreeViewItem a = new TreeViewItem();
                a.target = go;
                a.Text = go.GetComponent<HText>();
                a.Item = a.Text.RegEvent<UserEvent>();
                a.Item.Click = DefultItemClick;
                a.Item.DataContext = a;
                return a;
            }

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
        public float PercentageX
        {
            get
            {
                float o = contentSize.x - Enity.SizeDelta.x;
                if (o < 0)
                    return 0;
                o = m_pointX / o;
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
                if (contentSize.x > Enity.SizeDelta.x)
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
        public void DefultItemClick(UserEvent o, UserAction e)
        {
            var item = o.DataContext as TreeViewItem;
            if (item.node != null)
            {
                item.node.expand = !item.node.expand;
                Refresh();
                SelectNode = item.node;
            }
            if (SelectChanged != null)
                SelectChanged(this, item);
        }
        TVConstructor creator;
        public void SetItemUpdate<T, U>(Action<T, U> action) where T : TreeViewItem, new() where U : TreeViewNode, new()
        {
            var m = new TVMiddleware<T, U>();
            m.Invoke = action;
            creator = m;
        }
    }
}
