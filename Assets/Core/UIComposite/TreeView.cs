using huqiang.Core.HGUI;
using huqiang.Data;
using huqiang.UI;
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
        Vector2 aSize;
        public Vector2 ItemSize;
        public TreeViewNode nodes;
        public float ItemHigh = 16;
        public UserEvent eventCall;//scrollY自己的按钮
        public FakeStruct ItemMod;
        float m_point;
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
            v.y /= eventCall.Context.transform.localScale.y;
            Limit(back, v.y);
            Refresh();
        }
        float hy;
        public void Refresh()
        {
            if (nodes == null)
                return;
            hy = Size.y * 0.5f;
            aSize.y = CalculHigh(nodes, 0, 0);
            RecycleItem();
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
            node.offset.x = level * ItemHigh;
            node.offset.y = high;
            UpdateItem(node);
            level++;
            high += ItemHigh;
            if (node.extand)
                for (int i = 0; i < node.child.Count; i++)
                    high = CalculHigh(node.child[i], level, high);
            return high;
        }
        void UpdateItem(TreeViewNode node)
        {
            float dy = node.offset.y - m_point;
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
            TreeViewItem a = new TreeViewItem();
            a.target = go;
            a.text = go.GetComponent<HText>();
            a.callBack = go.GetComponent<UIElement>().RegEvent<UserEvent>();
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
        protected void Limit(UserEvent callBack, float y)
        {
            var size = Size;
            if (size.y > aSize.y)
            {
                m_point = 0;
                return;
            }
            if (y == 0)
                return;
            float vy = m_point + y;
            if (vy < 0)
            {
                m_point = 0;
                eventCall.VelocityY = 0;
                return;
            }
            else if (vy + size.y > aSize.y)
            {
                m_point = aSize.y - size.y;
                eventCall.VelocityY = 0;
                return;
            }
            m_point += y;
        }
    }
}
