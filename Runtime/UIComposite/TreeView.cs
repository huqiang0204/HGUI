using huqiang.Core.HGUI;
using huqiang.Core.UIData;
using huqiang.Data;
using huqiang.UIEvent;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace huqiang.UIComposite
{
    /// <summary>
    /// 属性框节点数据
    /// </summary>
    public class TreeViewNode
    {
        /// <summary>
        /// 展开
        /// </summary>
        public bool expand;
        /// <summary>
        /// 显示内容
        /// </summary>
        public virtual string content { get; set; }
        /// <summary>
        /// 绑定的数据,联系上下文
        /// </summary>
        public object context;
        /// <summary>
        /// 偏移位置
        /// </summary>
        public Vector2 offset;
        /// <summary>
        /// 子节点
        /// </summary>
        public List<TreeViewNode> child = new List<TreeViewNode>();
        /// <summary>
        /// 添加子节点
        /// </summary>
        /// <param name="node"></param>
        public void Add(TreeViewNode node)
        {
            if (node.parent == this)
                return;
            if (node != null)
            {
                if (node.parent != null)
                    node.parent.child.Remove(this);
                child.Add(node);
                node.parent = this;
            }
        }
        /// <summary>
        /// 设置父节点
        /// </summary>
        /// <param name="node"></param>
        public void SetParent(TreeViewNode node)
        {
            if (parent == node)
                return;
            if (parent != null)
                parent.child.Remove(this);
            if (node != null)
                node.child.Add(this);
            parent = node;
        }
        public int GetSiblingIndex()
        {
            if (parent == null)
                return 0;
            return parent.child.IndexOf(this);
        }
        public void SetSiblingIndex(int index)
        {
            if (parent == null)
                return;
            var c = parent.child;
            c.Remove(this);
            if (index >= c.Count)
                c.Add(this);
            else c.Insert(index, this);
        }
        /// <summary>
        /// 父节点
        /// </summary>
        public TreeViewNode parent { get; private set; }
        /// <summary>
        /// 获取层级
        /// </summary>
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
        /// <summary>
        /// 通过层级查找节点
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 展开子节点
        /// </summary>
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
    /// <summary>
    /// 属性框节点UI
    /// </summary>
    public class TreeViewItem
    {
        /// <summary>
        /// 主体游戏对象
        /// </summary>
        public UIElement target;
        /// <summary>
        /// 文本内容展示主体
        /// </summary>
        public HText Text;
        /// <summary>
        /// 项目事件
        /// </summary>
        public UserEvent Item;
        /// <summary>
        /// 关联的节点
        /// </summary>
        public TreeViewNode node;
    }
    /// <summary>
    /// 树形框项目构造器
    /// </summary>
    public class TVConstructor
    {
        /// <summary>
        /// UI初始化器
        /// </summary>
        public UIInitializer initializer;
        /// <summary>
        /// 创建UI实体
        /// </summary>
        /// <returns></returns>
        public virtual TreeViewItem Create() { return null; }
        /// <summary>
        /// 更新内容
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="dat"></param>
        public virtual void Update(TreeViewItem obj, TreeViewNode dat) { }
    }
    /// <summary>
    /// 属性框项目中间件
    /// </summary>
    /// <typeparam name="T">UI模型</typeparam>
    /// <typeparam name="U">数据模型</typeparam>
    public class TVMiddleware<T, U> : TVConstructor where T : TreeViewItem, new() where U : TreeViewNode, new()
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public TVMiddleware()
        {
            initializer = new UIInitializer(TempReflection.ObjectFields(typeof(T)));
        }
        /// <summary>
        /// 创建UI实体
        /// </summary>
        /// <returns></returns>
        public override TreeViewItem Create()
        {
            var t = new T();
            initializer.Reset(t);
            return t;
        }
        /// <summary>
        /// 内容更新委托
        /// </summary>
        public Action<T, U> Invoke;
        /// <summary>
        /// 更新内容
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="dat"></param>
        public override void Update(TreeViewItem obj, TreeViewNode dat)
        {
            if (Invoke != null)
                Invoke(obj as T, dat as U);
        }
    }
    /// <summary>
    /// 热更新中间件
    /// </summary>
    public class HotTVMiddleware : TVConstructor
    {
        /// <summary>
        /// 联系上下文
        /// </summary>
        public object Context;
        /// <summary>
        /// UI创建函数委托
        /// </summary>
        public Func<TreeViewItem> creator;
        /// <summary>
        /// UI内容更新委托
        /// </summary>
        public Action<TreeViewItem , TreeViewNode> caller;
        /// <summary>
        /// 创建UI实体
        /// </summary>
        /// <returns></returns>
        public override TreeViewItem Create()
        {
            if (creator != null)
                return creator();
            return base.Create();
        }
        /// <summary>
        /// 更新内容
        /// </summary>
        /// <param name="obj">ui实体</param>
        /// <param name="dat">数据实体</param>
        public override void Update(TreeViewItem obj, TreeViewNode dat)
        {
            if (caller != null)
                caller(obj, dat);
            else
                base.Update(obj, dat);
        }
    }
    /// <summary>
    /// 树形框
    /// </summary>
    public class TreeView : Composite
    {
        /// <summary>
        /// 树形框尺寸
        /// </summary>
        public Vector2 Size;
        Vector2 ContentSize;
        /// <summary>
        /// 项目节点尺寸
        /// </summary>
        public Vector2 ItemSize;
        /// <summary>
        /// 根节点
        /// </summary>
        public TreeViewNode Root;
        /// <summary>
        /// 项目高度
        /// </summary>
        public float ItemHigh = 30;
        /// <summary>
        /// 主体事件
        /// </summary>
        public UserEvent eventCall;
        /// <summary>
        /// 项目模型
        /// </summary>
        public FakeStruct ItemMod;
        Vector2 Position;
        //float m_pointY;
        //float m_pointX;
        /// <summary>
        /// 交换缓存
        /// </summary>
        public SwapBuffer<TreeViewItem, TreeViewNode> swap;
        QueueBuffer<TreeViewItem> queue;
        /// <summary>
        /// 项目选择被改变事件
        /// </summary>
        public Action<TreeView, TreeViewItem> SelectChanged;
        /// <summary>
        /// 当前选中节点
        /// </summary>
        public TreeViewNode SelectNode { get; set; }
        /// <summary>
        /// 构造函数,初始化缓存
        /// </summary>
        public TreeView()
        {
            swap = new SwapBuffer<TreeViewItem, TreeViewNode>(512);
            queue = new QueueBuffer<TreeViewItem>(256);
        }
        public override void Initial(FakeStruct fake, UIElement script,UIInitializer initializer)
        {
            base.Initial(fake, script,initializer);
            eventCall = script.RegEvent<UserEvent>();
            eventCall.PointerDown = (o, e) => { UpdateVelocity = false; };
            eventCall.Drag = (o, e, s) => { Scrolling(o, s); };
            eventCall.DragEnd = OnDragEnd;
            eventCall.MouseWheel = (o, e) => { Scrolling(o,new Vector2(0, e.MouseWheelDelta*100)); };
            eventCall.ForceEvent = true;
            eventCall.AutoColor = false;
            Size = Enity.SizeDelta;
            eventCall.CutRect = true;
            ItemMod = HGUIManager.FindChild(fake, "Item");
            if (ItemMod != null)
            {
                HGUIManager.RecycleChild(script);
                ItemSize = UIElementLoader.GetSize(ItemMod);
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
            var trans = eventCall.Context;
            v.x /= trans.localScale.x;
            v.y /= trans.localScale.y;
            LimitX(-v.x);
            LimitY(v.y);
            Refresh();
        }
        void OnDragEnd(UserEvent back, UserAction action, Vector2 v)
        {
            Scrolling(back, v);
            startVelocity.x = mVelocity.x = -back.VelocityX;
            startVelocity.y = mVelocity.y = back.VelocityY;
            UpdateVelocity = true;
        }
        float hy;
        float hx;
        /// <summary>
        /// 刷新显示
        /// </summary>
        public void Refresh()
        {
            if (Root == null)
                return;
            Size = Enity.SizeDelta;
            hy = Size.y * 0.5f;
            hx = Size.x * 0.5f;
            ContentSize.x = ItemSize.x;
            ContentSize.y = CalculHigh(Root, 0, 0);
            RecycleItem();
            if (Position.x + ItemSize.x > ContentSize.x)
                Position.x = ContentSize.x - ItemSize.x;

            for (int i = 0; i < swap.Length; i++)
            {
                var trans = swap[i].target;
                var p = trans.localPosition;
                p.x -= Position.x;
                trans.localPosition = p;
            }
        }
        /// <summary>
        /// 回收未被利用的项目
        /// </summary>
        protected void RecycleItem()
        {
            int len = swap.Length;
            for (int i = 0; i < len; i++)
            {
                var it = swap.Pop();
                it.target.activeSelf = false;
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
            if (x > ContentSize.x)
                ContentSize.x = x;
            return high;
        }
        void UpdateItem(TreeViewNode node)
        {
            float dy = node.offset.y - Position.y;
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
                    m.localPosition = new Vector3(node.offset.x, hy - dy - ItemHigh * 0.5f, 0);
                }
        }
        /// <summary>
        /// 创建项目实例,如果缓存中有则从缓存中提前
        /// </summary>
        /// <returns></returns>
        protected TreeViewItem CreateItem()
        {
            TreeViewItem it = queue.Dequeue();
            if (it != null)
            {
                it.target.activeSelf = true;
                return it;
            }
            if (creator != null)
            {
                var t = creator.Create();
                t.target = HGUIManager.Clone(ItemMod, creator.initializer);
                var trans = t.target;
                trans.SetParent(Enity);
                trans.localScale = Vector3.one;
                trans.localRotation = Quaternion.identity;
                return t;
            }
            else
            {
                var go = HGUIManager.Clone(ItemMod);
                go.SetParent(Enity);
                go.localScale = Vector3.one;
                go.localRotation = Quaternion.identity;
                TreeViewItem a = new TreeViewItem();
                a.target = go;
                a.Text = go as HText;
                a.Item = go.RegEvent<UserEvent>();
                a.Item.Click = DefultItemClick;
                a.Item.DataContext = a;
                return a;
            }

        }
        /// <summary>
        /// 限制横向滚动
        /// </summary>
        /// <param name="callBack">用户事件</param>
        /// <param name="x">参考移动距离</param>
        protected void LimitX(float x)
        {
            var size = Size;
            if (size.x > ContentSize.x)
            {
                Position.x = 0;
                return;
            }
            if (x == 0)
                return;
            float vx = Position.x + x;
            if (vx < 0)
            {
                Position.x = 0;
                mVelocity.x = 0;
                return;
            }
            else if (vx + size.x > ContentSize.x)
            {
                Position.x = ContentSize.x - size.x;
                mVelocity.x = 0;
                return;
            }
            Position.x += x;
        }
        /// <summary>
        /// 限制纵向滚动
        /// </summary>
        /// <param name="callBack">用户事件</param>
        /// <param name="x">参考移动距离</param>
        protected void LimitY(float y)
        {
            var size = Size;
            if (size.y > ContentSize.y)
            {
                Position.y = 0;
                return;
            }
            if (y == 0)
                return;
            float vy = Position.y + y;
            if (vy < 0)
            {
                Position.y = 0;
                mVelocity.y = 0;
                return;
            }
            else if (vy + size.y > ContentSize.y)
            {
                Position.y = ContentSize.y - size.y;
                mVelocity.y = 0;
                return;
            }
            Position.y += y;
        }
        /// <summary>
        /// 横向滚动百分比0-1
        /// </summary>
        public float PercentageX
        {
            get
            {
                float o = ContentSize.x - Enity.SizeDelta.x;
                if (o < 0)
                    return 0;
                o = Position.x / o;
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
                if (ContentSize.x > Enity.SizeDelta.x)
                    Position.x = value * (ContentSize.x - Enity.SizeDelta.x);
            }
        }
        /// <summary>
        /// 纵向滚动百分比0-1
        /// </summary>
        public float PercentageY
        {
            get
            {
                float o = ContentSize.y - Enity.SizeDelta.y;
                if (o < 0)
                    return 0;
                o = Position.y / o;
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
                if (ContentSize.y > Enity.SizeDelta.y)
                    Position.y = value * (ContentSize.y - Enity.SizeDelta.y);
            }
        }
        /// <summary>
        /// 项目的默认单击事件
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
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
        /// <summary>
        /// 设置项目更新函数
        /// </summary>
        /// <typeparam name="T">ui模型</typeparam>
        /// <typeparam name="U">数据模型</typeparam>
        /// <param name="action">更新函数委托</param>
        public void SetItemUpdate<T, U>(Action<T, U> action) where T : TreeViewItem, new() where U : TreeViewNode, new()
        {
            var m = new TVMiddleware<T, U>();
            m.Invoke = action;
            creator = m;
        }
        /// <summary>
        /// 设置项目更新,用于热更新
        /// </summary>
        /// <param name="m"></param>
        public void SetItemUpdate(HotTVMiddleware m)
        {
            creator = m;
        }
        /// <summary>
        /// 清除缓存资源
        /// </summary>
        public void Clear()
        {
            swap.Clear();
            queue.Clear();
            HGUIManager.RecycleChild(Enity);
        }
        /// <summary>
        /// 初始速率
        /// </summary>
        protected Vector2 startVelocity;
        Vector2 mVelocity;
        public float DecayRate = 0.997f;
        protected bool UpdateVelocity = true;
        public override void Update(float time)
        {
            if (!UpdateVelocity)
                return;
            float x = 0;
            float y = 0;
            int count = UserAction.TimeSlice;
            if (mVelocity.x != 0)
            {
                float dr = DecayRate;
                for (int i = 0; i < count; i++)
                {
                    mVelocity.x *= dr;
                    x += mVelocity.x;
                }
                if (mVelocity.x < 0.01f & mVelocity.x > -0.01f)
                {
                    mVelocity.x = 0;
                }
            }
            if (mVelocity.y != 0)
            {
                float dr = DecayRate;
                for (int i = 0; i < count; i++)
                {
                    mVelocity.y *= dr;
                    y += mVelocity.y;
                }
                if (mVelocity.y < 0.01f & mVelocity.y > -0.01f)
                {
                    mVelocity.y = 0;
                }
            }
            LimitX(x);
            LimitY(y);
            Refresh();
            //if (x != 0 | y != 0)
            //    Move(new Vector2(x, y));

            if (mVelocity.x == 0 & mVelocity.y == 0)
                UpdateVelocity = false;
        }
    }
}
