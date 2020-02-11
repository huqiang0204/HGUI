using huqiang.Core.HGUI;
using huqiang.Data;
using huqiang.UIEvent;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace huqiang.UIComposite
{
    public class DataBaseItem
    {
        public int index;
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
        public string Head;
        public List<DataGridItemContext> datas = new List<DataGridItemContext>();
    }
    public class ModelConstructor
    {
        public UIInitializer initializer;
        public virtual object Create() { return null; }
        public virtual void Update(object obj, object dat) { }
    }
 
    public class ModelMiddleware<T, U> : ModelConstructor where T :class, new() where U : class, new()
    {
        public ModelMiddleware()
        {
            initializer = new UIInitializer(TempReflection.ObjectFields(typeof(T)));
        }
        public override object Create()
        {
            var t = new T();
            initializer.Reset(t);
            return t;
        }
        public Action<T, U> Invoke;
        public override void Update(object obj, object dat)
        {
            if (Invoke != null)
                Invoke(obj as T, dat as U);
        }
    }
    public class DataGrid:Composite
    {
        /// <summary>
        /// 当前滚动的位置
        /// </summary>
        public Vector2 Position;
        FakeStruct ItemMod;
        FakeStruct HeadMod;
        FakeStruct DragMod;
        FakeStruct Line;
        Transform Heads;
        Transform Items;
        Transform Drags;
        Vector2 contentSize;
        Vector2 headSize;
        float itemY;
        float m_pointY;
        float m_pointX;
        List<DataGridColumn> columns = new List<DataGridColumn>();
        public List<DataGridColumn> BindingData { get => columns; }
        public SwapBuffer<DataGridHead, DataGridColumn> HeadSwap;
        QueueBuffer<DataGridHead> headQueue;
        public SwapBuffer<DataGridItem, DataGridItemContext> ItemSwap;
        QueueBuffer<DataGridItem> itemQueue;
        SwapBuffer<UserEvent, DataGridHead> dragSwap;
        List<HImage> lines;
        public UserEvent eventCall;
        public DataGrid()
        {
            HeadSwap = new SwapBuffer<DataGridHead, DataGridColumn>(128);
            headQueue = new QueueBuffer<DataGridHead>(32);
            ItemSwap = new SwapBuffer<DataGridItem, DataGridItemContext>(1024);
            itemQueue = new QueueBuffer<DataGridItem>(256);
            SetHeadUpdate<DataGridHead, DataGridColumn>(DefHeadUpdate);
            SetItemUpdate<DataGridItem, DataGridItemContext>(DefItemUpdate);
            dragSwap = new SwapBuffer<UserEvent, DataGridHead>(128);
            lines = new List<HImage>();
        }
        public override void Initial(FakeStruct mod, UIElement element)
        {
            base.Initial(mod, element);
            HeadMod = HGUIManager.FindChild(mod, "Head");
            ItemMod = HGUIManager.FindChild(mod, "Item");
            DragMod = HGUIManager.FindChild(mod, "Drag");
            Line = HGUIManager.FindChild(mod, "Line");
            var trans = element.transform;
            Heads = trans.Find("Heads");
            Items = trans.Find("Items");
            Drags = trans.Find("Drags");
            unsafe
            {
                headSize = ((TransfromData*)HeadMod.ip)->size;
                itemY = ((TransfromData*)ItemMod.ip)->size.y;
            }
            HGUIManager.GameBuffer.RecycleGameObject(trans.Find("Head").gameObject);
            HGUIManager.GameBuffer.RecycleGameObject(trans.Find("Item").gameObject);
            HGUIManager.GameBuffer.RecycleGameObject(trans.Find("Drag").gameObject);
            HGUIManager.GameBuffer.RecycleGameObject(trans.Find("Line").gameObject);
            eventCall = element.RegEvent<UserEvent>();
            eventCall.Drag = (o, e, s) => { Scrolling(o, s); };
            eventCall.DragEnd = (o, e, s) => { Scrolling(o, s); };
            eventCall.Scrolling = Scrolling;
            eventCall.ForceEvent = true;
            eventCall.AutoColor = false;
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
        public void Refresh()
        {
            float x = 0;
            for (int i = 0; i < columns.Count; i++)
                x += columns[i].width;
            contentSize.x = x;
            OrderHead();
            if (columns.Count > 0)
                contentSize.y = itemY * columns[0].datas.Count;
        }
        T CreateEnity<T>(QueueBuffer<T> buf,FakeStruct mod, ModelConstructor creator,Transform parent)
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
        void UpdateHead(Vector3 pos, DataGridColumn col)
        {
            var item = HeadSwap.Exchange((o, e) => { return o.Context == e; }, col);
            if (item == null)
            {
                item = CreateEnity<DataGridHead>(headQueue, HeadMod, headCreator,Heads);
                item.Context = col;
                HeadSwap.Push(item);
            }
            headCreator.Update(item, col);
            item.target.transform.localPosition = pos;
            pos.x += col.width;
            UpdateDrag(pos,item);
        }
        void UpdateItem(Vector3 pos,DataGridItemContext data)
        {
            var item = ItemSwap.Exchange((o, e) => { return o.Context == e; }, data);
    
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
        void CreateLine(Vector3 pos, DataGridColumn column)
        {

        }
        void UpdateVerticalLine(Vector3 pos,Vector2 size)
        {

        }
        void UpdateHorizLine(Vector3 pos,Vector2 size)
        {

        }
        void OrderHead()
        {
            float x = m_pointX;
            float os  = -x;
            for (int i = 0; i < columns.Count; i++)
            {
                var col = columns[i];
                if (os + col.width > 0)
                { 
                    UpdateHead(new Vector3(os, 0, 0), col);
                }
                os += col.width;
                if (os > Enity.SizeDelta.x)
                    break;
            }
            RecycleEnity(HeadSwap,headQueue);
            int len = dragSwap.Length;
            for (int i = 0; i < len; i++)
            {
                var it = dragSwap.Pop();
                HGUIManager.GameBuffer.RecycleGameObject(it.Context.gameObject);
            }
            dragSwap.Done();
        }
        void OrderItem()
        {

        }
        void OrderLine()
        {

        }

        public void AddColumn(DataGridColumn column)
        {
            //int c = 0;
            //if (columns.Count > 0)
            //{
            //    c = columns[0].datas.Count;
            //}
            column.width = headSize.x;
            if (column.width < 80)
                column.width = 80;
            columns.Add(column);

        }
        public void AddRow()
        {

        }
        public void RemoveRow()
        {

        }
        ModelConstructor headCreator;
        ModelConstructor itemCreator;
        public void SetHeadUpdate<T, U>(Action<T, U> action)
     where T : DataGridHead, new() where U : DataGridColumn, new()
        {
            var m = new ModelMiddleware<T, U>();
            m.Invoke = action;
            headCreator = m;
        }
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
                item.Text.Text = data.Text;
        }
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
        protected void LimitY(UserEvent callBack, float y)
        {
            var size = Enity.m_sizeDelta;
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
