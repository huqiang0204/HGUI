using huqiang.Core.HGUI;
using huqiang.Data;
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
    public class DGConstructor
    {
        public UIInitializer initializer;
        public virtual object Create() { return null; }
        public virtual void Update(object obj, object dat) { }
    }
 
    public class DGMiddleware<T, U> : DGConstructor where T :class, new() where U : class, new()
    {
        public DGMiddleware()
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
        FakeStruct Drag;
        FakeStruct Line;
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

        public DataGrid()
        {
            HeadSwap = new SwapBuffer<DataGridHead, DataGridColumn>(128);
            headQueue = new QueueBuffer<DataGridHead>(32);
            ItemSwap = new SwapBuffer<DataGridItem, DataGridItemContext>(1024);
            itemQueue = new QueueBuffer<DataGridItem>(256);
            SetHeadUpdate<DataGridHead, DataGridColumn>(DefHeadUpdate);
        }
        public override void Initial(FakeStruct mod, UIElement element)
        {
            base.Initial(mod, element);
            HeadMod = HGUIManager.FindChild(mod, "Head");
            ItemMod = HGUIManager.FindChild(mod, "Item");
            Drag = HGUIManager.FindChild(mod, "Drag");
            Line = HGUIManager.FindChild(mod, "Line");
            unsafe
            {
                headSize = ((TransfromData*)HeadMod.ip)->size;
                itemY = ((TransfromData*)ItemMod.ip)->size.y;
            }
            HGUIManager.GameBuffer.RecycleChild(Enity.gameObject);
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
        T CreateEnity<T>(QueueBuffer<T> buf,FakeStruct mod, DGConstructor creator)
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
            trans.SetParent(Enity.transform);
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
                item = CreateEnity<DataGridHead>(headQueue, HeadMod, headCreator);
                HeadSwap.Push(item);
            }
            headCreator.Update(item, col);
            item.target.transform.localPosition = pos;
        }
        void UpdateItem(Vector3 pos,DataGridItemContext data)
        {
            var item = ItemSwap.Exchange((o, e) => { return o.Context == e; }, data);
    
        }
        void CreateLine(Vector3 pos, DataGridColumn column)
        {

        }

        void OrderHead()
        {
            float x = m_pointX;
            float os  = Enity.SizeDelta.x * -0.5f;
            for (int i = 0; i < columns.Count; i++)
            {
                var col = columns[i];
                UpdateHead(new Vector3(os,0,0),col);
                os += col.width;
            }
            RecycleEnity(HeadSwap,headQueue);
        }
        void OrderItem()
        {

        }
        void OrderLine()
        {

        }

        public void AddColumn(DataGridColumn column)
        {
            columns.Add(column);
        }
        public void AddRow()
        {

        }
        public void RemoveRow()
        {

        }
        DGConstructor headCreator;
        DGConstructor itemCreator;
        public void SetHeadUpdate<T, U>(Action<T, U> action)
     where T : DataGridHead, new() where U : DataGridColumn, new()
        {
            var m = new DGMiddleware<T, U>();
            m.Invoke = action;
            headCreator = m;
        }
        public void SetItemUpdate<T, U>(Action<T, U> action)
            where T : DataGridItem, new() where U : DataGridItemContext, new()
        {
            var m = new DGMiddleware<T, U>();
            m.Invoke = action;
            itemCreator = m;
        }
        void DefHeadUpdate(DataGridHead head,DataGridColumn col)
        {
            head.Text.Text = col.Head;
        }
    }
}
