using huqiang.Core.HGUI;
using huqiang.Core.Line;
using huqiang.Data;
using huqiang.UIEvent;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace huqiang.UIComposite
{
    public class DataBaseItem
    {
        public int index =-1;
        public GameObject target;
    }
    public class DataGridHead:DataBaseItem
    {
        public UIElement Head;
        public HText Text;
        public DataGridColumn Context;
    }
    public class DataGridItem:DataBaseItem
    {
        public UIElement Item;
        public HText Text;
        public DataGridItemContext Context;
        public DataGridColumn GridColumn;
        public int Row = -1;
        public int Column = -1;
    }
    public class DataGridItemContext
    {
        public string Text;
    }
    public class DataGridColumn
    {
        /// <summary>
        /// 宽度
        /// </summary>
        public float width = 80;
        /// <summary>
        /// 标题
        /// </summary>
        public string Head;
        /// <summary>
        /// ui布局模型
        /// </summary>
        public FakeStruct mod;
        /// <summary>
        /// 此列绑定的数据列表
        /// </summary>
        public List<DataGridItemContext> datas = new List<DataGridItemContext>();
        /// <summary>
        /// 此列的UI模型初始化创建器
        /// </summary>
        public ModelConstructor itemCreator;
        /// <summary>
        /// 此列的重复回收利用缓存
        /// </summary>
        public QueueBuffer<DataGridItem> buf=new QueueBuffer<DataGridItem>(128);
        /// <summary>
        /// 设置此列的Item更新模板和回调函数
        /// </summary>
        /// <typeparam name="T">UI反射模板</typeparam>
        /// <typeparam name="U">数据模板</typeparam>
        /// <param name="action">回调函数</param>
        public void SetItemUpdate<T, U>(Action<T, U> action)
            where T : DataGridItem, new() where U : DataGridItemContext, new()
        {
            var m = new ModelMiddleware<T, U>();
            m.Invoke = action;
            itemCreator = m;
        }
        /// <summary>
        /// 创建一个实例,如果回收缓存中有就从缓存中提取
        /// </summary>
        /// <returns></returns>
        public DataGridItem CreateEnity()
        {
            DataGridItem it = buf.Dequeue();
            if (it != null)
            {
                it.target.SetActive(true);
                return it;
            }
            var t = itemCreator.Create() as DataGridItem;
            t.target = HGUIManager.GameBuffer.Clone(mod, itemCreator.initializer);
            return t;
        }
    }
    /// <summary>
    /// 模型构造器
    /// </summary>
    public class ModelConstructor
    {
        /// <summary>
        /// UI初始化器
        /// </summary>
        public UIInitializer initializer;
        /// <summary>
        /// 创建一个UI实体
        /// </summary>
        /// <returns></returns>
        public virtual object Create() { return null; }
        /// <summary>
        /// 更新项目
        /// </summary>
        /// <param name="obj">UI实体</param>
        /// <param name="dat">数据实体</param>
        public virtual void Update(object obj, object dat) { }
    }
    /// <summary>
    /// 模型中间件
    /// </summary>
    /// <typeparam name="T">UI模型</typeparam>
    /// <typeparam name="U">数据模型</typeparam>
    public class ModelMiddleware<T, U> : ModelConstructor where T :class, new() where U : class, new()
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ModelMiddleware()
        {
            initializer = new UIInitializer(TempReflection.ObjectFields(typeof(T)));
        }
        /// <summary>
        /// 创建UI实例
        /// </summary>
        /// <returns></returns>
        public override object Create()
        {
            var t = new T();
            initializer.Reset(t);
            return t;
        }
        /// <summary>
        /// 项目更新委托
        /// </summary>
        public Action<T, U> Invoke;
        /// <summary>
        /// 项目更新
        /// </summary>
        /// <param name="obj">ui实例</param>
        /// <param name="dat">数据实例</param>
        public override void Update(object obj, object dat)
        {
            if (Invoke != null)
                Invoke(obj as T, dat as U);
        }
    }
    /// <summary>
    /// 网格数据滚动框
    /// </summary>
    public class DataGrid:Composite
    {
        /// <summary>
        /// 拖动列宽度时,光标显示的Icon
        /// </summary>
        public static Texture2D CursorX;
        /// <summary>
        /// 拖动行高时,光标显示的Icon,暂未开发
        /// </summary>
        public static Texture2D CursorY;
        /// <summary>
        /// 当前滚动的位置
        /// </summary>
        public Vector2 Position;
        FakeStruct ItemMod;
        FakeStruct HeadMod;
        FakeStruct DragMod;
        Transform Heads;
        Transform Grid;
        Transform Items;
        Transform Drags;
        HLine lines;
        Vector2 contentSize;
        Vector2 headSize;
        float itemY;
        float m_pointY;
        float m_pointX;
        float lineWidth;
        float lineHigh;
        List<DataGridColumn> columns = new List<DataGridColumn>();
        /// <summary>
        /// 所有列
        /// </summary>
        public List<DataGridColumn> BindingData { get => columns; }
        SwapBuffer<DataGridHead, DataGridColumn> HeadSwap;
        QueueBuffer<DataGridHead> headQueue;
        SwapBuffer<DataGridItem, DataGridItemContext> ItemSwap;
        QueueBuffer<DataGridItem> itemQueue;
        SwapBuffer<UserEvent, DataGridHead> dragSwap;
        //List<HImage> lines;
        List<HImage> temp;
        public UserEvent eventCall;
        /// <summary>
        /// 当用户改变列宽时调用此委托
        /// </summary>
        public Action<DataGrid,DataGridColumn> ColumnResized;
        /// <summary>
        /// 构造函数
        /// </summary>
        public DataGrid()
        {
            HeadSwap = new SwapBuffer<DataGridHead, DataGridColumn>(128);
            headQueue = new QueueBuffer<DataGridHead>(32);
            ItemSwap = new SwapBuffer<DataGridItem, DataGridItemContext>(1024);
            itemQueue = new QueueBuffer<DataGridItem>(256);
            SetHeadUpdate<DataGridHead, DataGridColumn>(DefHeadUpdate);
            SetItemUpdate<DataGridItem, DataGridItemContext>(DefItemUpdate);
            dragSwap = new SwapBuffer<UserEvent, DataGridHead>(128);
            temp = new List<HImage>();
        }
        /// <summary>
        /// 初始化布局
        /// </summary>
        /// <param name="mod">数据模型</param>
        /// <param name="element">UI元素主体</param>
        public override void Initial(FakeStruct mod, UIElement element,Initializer initializer)
        {
            base.Initial(mod, element,initializer);
            HeadMod = HGUIManager.FindChild(mod, "Head");
            ItemMod = HGUIManager.FindChild(mod, "Item");
            DragMod = HGUIManager.FindChild(mod, "Drag");
            var trans = element.transform;
            Heads = trans.Find("Heads");
            Grid = trans.Find("Grid");
            Items = Grid.Find("Items");
            Drags = trans.Find("Drags");
            lines = Grid.Find("Line").GetComponent<HLine>();
            unsafe
            {
                headSize = ((UITransfromData*)HeadMod.ip)->size;
                itemY = ((UITransfromData*)ItemMod.ip)->size.y;
            }
            HGUIManager.GameBuffer.RecycleGameObject(trans.Find("Head").gameObject);
            HGUIManager.GameBuffer.RecycleGameObject(trans.Find("Item").gameObject);
            HGUIManager.GameBuffer.RecycleGameObject(trans.Find("Drag").gameObject);
            eventCall = element.RegEvent<UserEvent>();
            eventCall.CutRect = true;
            eventCall.Drag = (o, e, s) => { Scrolling(o, s); };
            eventCall.DragEnd = (o, e, s) => { Scrolling(o, s); };
            eventCall.MouseWheel = (o, e) => { Scrolling(o, new Vector2(0, e.MouseWheelDelta * 100)); };
            eventCall.Scrolling = Scrolling;
            eventCall.ForceEvent = true;
            eventCall.AutoColor = false;
            var ue = Grid.GetComponent<UIElement>().RegEvent<UserEvent>();
            ue.CutRect = true;
            ue.Penetrate = true;
            Enity.SizeChanged = (o) => { Refresh(); };
        }
        void Scrolling(UserEvent back, Vector2 v)
        {
            if (Enity == null)
                return;
            var trans = back.Context.transform;
            v.x /= trans.localScale.x;
            v.y /= trans.localScale.y;
            LimitX(back, -v.x);
            LimitY(back, v.y);
            Refresh();
        }
        /// <summary>
        /// 初始化设置和绑定数据后调用此函数,刷新显示内容
        /// </summary>
        public void Refresh()
        {
            float x = 0;
            for (int i = 0; i < columns.Count; i++)
                x += columns[i].width;
            contentSize.x = x;
            if (columns.Count > 0)
                contentSize.y = itemY * columns[0].datas.Count;
            if (contentSize.x > Enity.m_sizeDelta.x)
                lineWidth = Enity.m_sizeDelta.x;
            else lineWidth = contentSize.x;
            if (contentSize.y > Enity.m_sizeDelta.y)
                lineHigh = Enity.m_sizeDelta.y;
            else lineHigh = contentSize.y;
            Order();
        }
        T CreateEnity<T>(QueueBuffer<T> buf, FakeStruct mod, ModelConstructor creator,Transform parent)
            where T:DataBaseItem,new()
        {
            T it = buf.Dequeue();
            if (it != null)
            {
                it.target.SetActive(true);
                return it;
            }
            var t = creator.Create() as T;
            t.target = HGUIManager.GameBuffer.Clone(mod, creator.initializer);
            var trans = t.target.transform;
            trans.SetParent(parent);
            trans.localScale = Vector3.one;
            trans.localRotation = Quaternion.identity;
            return t;
        }
        void RecycleEnity<T,U>(SwapBuffer<T,U> swap, QueueBuffer<T> queue) where T:DataBaseItem
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
        void RecycleItemEnity()
        {
            int len = ItemSwap.Length;
            for(int i=0;i<len;i++)
            {
                var it = ItemSwap.Pop();
                var col = it.GridColumn;
                it.target.SetActive(false);
                if (col == null)
                {
                    HGUIManager.GameBuffer.RecycleGameObject(it.target);
                }
                else col.buf.Enqueue(it);
            }
            ItemSwap.Done();
        }
        void UpdateHead(Vector3 pos, DataGridColumn col)
        {
            var item = HeadSwap.Exchange((o, e) => { return o.Context == e; }, col);
            if (item == null)
            {
                item = CreateEnity<DataGridHead>(headQueue, HeadMod, headCreator,Heads);
                item.Context = col;
                HeadSwap.Push(item);
            }
            item.target.transform.localPosition = pos;
            pos.x += col.width;
            UpdateDrag(pos,item);
            UpdateVerticalLine(pos.x);
            if (item.Head != null)
                if (item.Head.m_sizeDelta.x != col.width)
                {
                    item.Head.m_sizeDelta.x = col.width;
                    UIElement.ResizeChild(item.Head);
                }
            headCreator.Update(item, col);
        }
        void UpdateItem(Vector3 pos, DataGridItemContext data,int row, int col)
        {
            var item = ItemSwap.Exchange((o, e) => { return o.Context == e; }, data);
            var colunn = columns[col];
            if (item == null)
            {
                if(colunn.itemCreator!=null)
                {
                    item = colunn.CreateEnity();
                    item.index = -1;
                    item.Row = -1;
                    item.Column = -1;
                    if (item.Item == null)
                        item.Item = item.target.GetComponent<UIElement>();
                    var trans = item.target.transform;
                    trans.SetParent(Items);
                    trans.localScale = Vector3.one;
                    trans.localRotation = Quaternion.identity;
                    ItemSwap.Push(item);
                }
                else
                {
                    item = CreateEnity<DataGridItem>(itemQueue, ItemMod, itemCreator, Items);
                    ItemSwap.Push(item);
                }
            }
            pos.y = -pos.y;
            item.GridColumn = colunn;
            float w = columns[col].width;
            var size = item.Item.SizeDelta;
            if(size.x!=w)
            {
                size.x = w;
                item.Item.SizeDelta = size;
                UIElement.ResizeChild(item.Item);
            }
            item.target.transform.localPosition = pos;
            if (item.Row != row | item.Column != col)
            {
                item.Row = row;
                item.Column = col;
                item.Context = data;
                if (colunn.itemCreator != null)
                    colunn.itemCreator.Update(item, data);
                else itemCreator.Update(item, data);
            }
        }
        void UpdateDrag(Vector3 pos,DataGridHead col)
        {
            var item = dragSwap.Exchange((o, e) => { return (o.DataContext as DataGridHead) == e; }, col);
            if(item==null)
            {
                var go = HGUIManager.GameBuffer.Clone(DragMod);
                var ele = go.GetComponent<UIElement>();
                item = ele.RegEvent<UserEvent>();
                item.DataContext = col;
                dragSwap.Push(item);
                item.Drag = Draging;
                item.DragEnd = DragEnd;
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
                item.PointerEntry = (o, e) =>
                {
                    Cursor.SetCursor(CursorX, new Vector2(64, 64), CursorMode.Auto);
                };
                item.PointerLeave = (o, e) => {
                    if (!o.Pressed)
                        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                };
#endif
                var trans = go.transform;
                trans.SetParent(Drags);
                trans.localScale = Vector3.one;
                trans.localRotation = Quaternion.identity;
            }
            pos.y = -0.5f * headSize.y;
            item.Context.transform.localPosition = pos;
        }
        void Draging(UserEvent back,UserAction action,Vector2 v)
        {
            var head = back.DataContext as DataGridHead;
            var col = head.Context;
            var trans = back.Context.transform;
            v.x /= trans.localScale.x;
            col.width += v.x;
            if (col.width < 80)
                col.width = 80;
            head.Head.m_sizeDelta.x = col.width;
            UIElement.ResizeChild(head.Head);
            eventCall.RemoveFocus();
            Refresh();

        }
        void DragEnd(UserEvent back, UserAction action, Vector2 v)
        {
            var head = back.DataContext as DataGridHead;
            var col = head.Context;
            var trans = back.Context.transform;
            v.x /= trans.localScale.x;
            col.width += v.x;
            if (col.width < 80)
                col.width = 80;
            head.Head.m_sizeDelta.x = col.width;
            UIElement.ResizeChild(head.Head);
            Refresh();
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
#endif
            if (ColumnResized != null)
                ColumnResized(this, col);
        }
        void UpdateVerticalLine(float x)
        {
            float ox =x - Enity.m_sizeDelta.x * 0.5f;
            float oy = Enity.m_sizeDelta.y *0.5f-itemY*0.5f;
            Beeline beeline = new Beeline();
            beeline.lineBase.Width = 2;
            beeline.lineBase.Color = lines.MainColor;
            beeline.Start.x = ox;
            beeline.Start.y = oy;
            beeline.End.x = ox;
            beeline.End.y = oy - lineHigh;
            lines.AddLine(ref beeline);
        }
        void UpdateHorizontalLine(float y)
        {
            float ox = Enity.m_sizeDelta.x * -0.5f;
            float oy = Enity.m_sizeDelta.y * 0.5f - y ;
            Beeline beeline = new Beeline();
            beeline.lineBase.Width = 2;
            beeline.lineBase.Color = lines.MainColor;
            beeline.Start.x = ox;
            beeline.Start.y = oy;
            beeline.End.x = ox+lineWidth;
            beeline.End.y = oy;
            lines.AddLine(ref beeline);
        }
        void Order()
        {
            lines.Clear();
            float x = m_pointX;
            float os  = -x;
            for (int i = 0; i < columns.Count; i++)
            {
                var col = columns[i];
                if (os + col.width > 0)
                { 
                    UpdateHead(new Vector3(os, 0, 0), col);
                    OrderItem(col,os,i);
                }
                os += col.width;
                if (os > Enity.SizeDelta.x)
                    break;
            }
            RecycleEnity(HeadSwap,headQueue);
            RecycleItemEnity();
            int len = dragSwap.Length;
            for (int i = 0; i < len; i++)
            {
                var it = dragSwap.Pop();
                HGUIManager.GameBuffer.RecycleGameObject(it.Context.gameObject);
            }
            dragSwap.Done();
            OrderLine();
            for (int i = 0; i < temp.Count; i++)
                temp[i].gameObject.SetActive(false);
        }
        void OrderItem(DataGridColumn column,float os,int col)
        {
            int s = (int)(m_pointY / itemY)-1;
            if (s < 0)
                s = 0;
            var data = column.datas;
            float end = Enity.m_sizeDelta.y;
            float oy = s * itemY-m_pointY;
            for (int i = s; i < data.Count; i++)
            {
                UpdateItem(new Vector3(os, oy, 0), data[i],i,col);
                oy += itemY;
                if (oy > end)
                    break;
            }
        }
        void OrderLine()
        {
            int s = (int)(m_pointY / itemY) - 1;
            if (s < 0)
                s = 0;
            if(columns.Count>0)
            {
                var data = columns[0].datas;
                float end = Enity.m_sizeDelta.y;
                float oy = s * itemY- m_pointY;
                oy += itemY * 0.5f;
                for (int i = s; i < data.Count; i++)
                {
                    oy += itemY;
                    UpdateHorizontalLine(oy);
                    if (oy > end + itemY)
                        break;
                }
            }
        }
        /// <summary>
        /// 添加列
        /// </summary>
        /// <param name="column">列数据实体</param>
        public void AddColumn(DataGridColumn column)
        {
            int c = 0;
            if (columns.Count > 0)
            {
                c = columns[0].datas.Count;
            }
            column.width = headSize.x;
            if (column.width < 80)
                column.width = 80;
            int o = column.datas.Count;
            if(o<c)
            {
                for (int i = o; i < c; i++)
                    column.datas.Add(null);
            }
            else if(o>c)
            {
                for(int i=0;i<columns.Count;i++)
                {
                    var col = columns[i];
                    for (int j = c; j < o; j++)
                        col.datas.Add(null);
                }
            }
            if (column.itemCreator == null)
                column.itemCreator = itemCreator;
            if (column.mod == null)
                column.mod = ItemMod;
            columns.Add(column);
        }
        /// <summary>
        /// 移除列
        /// </summary>
        /// <param name="index">列索引</param>
        public void RemoveColumn(int index)
        {
            if (index < 0)
                return;
            if (index < columns.Count)
                columns.RemoveAt(index);
        }
        /// <summary>
        /// 清除所有数据
        /// </summary>
        public void ClearData()
        {
            for (int i = 0; i < columns.Count; i++)
            {
                columns[i].datas.Clear();
            }
        }
        /// <summary>
        /// 清除所有列
        /// </summary>
        public void ClearColumn()
        {
            columns.Clear();
            HeadSwap.Clear();
            ItemSwap.Clear();
            dragSwap.Clear();
            headQueue.Clear();
            itemQueue.Clear();
            HGUIManager.GameBuffer.RecycleChild(Items.gameObject);
            HGUIManager.GameBuffer.RecycleChild(Heads.gameObject);
            HGUIManager.GameBuffer.RecycleChild(Drags.gameObject);
        }
        /// <summary>
        /// 添加一行
        /// </summary>
        /// <param name="content">数据</param>
        public void AddRow(params DataGridItemContext[] content )
        {
            if(content==null)
            {
                for (int i = 0; i < columns.Count; i++)
                    columns[i].datas.Add(null);
            }
            else
            {
                int c = content.Length;
                for (int i = 0; i < c; i++)
                    columns[i].datas.Add(content[i]);
                for (int i = c; i < columns.Count; i++)
                    columns[i].datas.Add(null);
            }
        }
        /// <summary>
        /// 移除一行数据
        /// </summary>
        /// <param name="index">行索引</param>
        public void RemoveRow(int index)
        {
            if (index < 0)
                return;
            if(columns.Count>0)
            {
                var c = columns[0].datas.Count;
                if(index<c)
                {
                    for (int i = 0; i < columns.Count; i++)
                        columns[i].datas.RemoveAt(index);
                }
            }
        }
        ModelConstructor headCreator;
        ModelConstructor itemCreator;
        /// <summary>
        /// 设置标头更新函数回调
        /// </summary>
        /// <typeparam name="T">UI模型</typeparam>
        /// <typeparam name="U">数据模型</typeparam>
        /// <param name="action">更新回调</param>
        public void SetHeadUpdate<T, U>(Action<T, U> action)
     where T : DataGridHead, new() where U : DataGridColumn, new()
        {
            var m = new ModelMiddleware<T, U>();
            m.Invoke = action;
            headCreator = m;
        }
        /// <summary>
        /// 设置列数据项更新函数
        /// </summary>
        /// <typeparam name="T">UI模型</typeparam>
        /// <typeparam name="U">数据模型</typeparam>
        /// <param name="action">更新回调</param>
        public void SetItemUpdate<T, U>(Action<T, U> action)
            where T : DataGridItem, new() where U : DataGridItemContext, new()
        {
            var m = new ModelMiddleware<T, U>();
            m.Invoke = action;
            itemCreator = m;
        }
        void DefHeadUpdate(DataGridHead head,DataGridColumn col)
        {
            if (head.Text != null)
                head.Text.Text = col.Head;
        }
        void DefItemUpdate(DataGridItem item,DataGridItemContext data)
        {
            if (item.Text != null)
            {
                if (data != null)
                    item.Text.Text = data.Text;
                else item.Text.Text = null;
            }
        }
        /// <summary>
        /// X轴向的滚动限制
        /// </summary>
        /// <param name="callBack">用户事件</param>
        /// <param name="x">移动距离</param>
        protected void LimitX(UserEvent callBack, float x)
        {
            var size = Enity.m_sizeDelta;
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
                callBack.VelocityX = 0;
                return;
            }
            else if (vx + size.x > contentSize.x)
            {
                m_pointX = contentSize.x - size.x;
                callBack.VelocityX = 0;
                return;
            }
            m_pointX += x;
        }
        /// <summary>
        /// Y轴向的滚动限制
        /// </summary>
        /// <param name="callBack">用户事件</param>
        /// <param name="y">移动距离</param>
        protected void LimitY(UserEvent callBack, float y)
        {
            var size = Enity.m_sizeDelta;
            size.y -= headSize.y;
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
                callBack.VelocityY = 0;
                return;
            }
            else if (vy + size.y > contentSize.y)
            {
                m_pointY = contentSize.y - size.y;
                callBack.VelocityY = 0;
                return;
            }
            m_pointY += y;
        }
    }
}
