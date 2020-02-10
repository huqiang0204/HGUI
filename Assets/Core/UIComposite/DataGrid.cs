using huqiang.Core.HGUI;
using huqiang.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace huqiang.UIComposite
{
    public class DataGridItem
    {
        public GameObject target;
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
        public virtual DataGridItem Create() { return null; }
        public virtual void Update(DataGridItem obj, DataGridItemContext dat) { }
    }
    public class DGMiddleware<T, U> : DGConstructor where T : DataGridItem, new() where U : DataGridItemContext, new()
    {
        public DGMiddleware()
        {
            initializer = new UIInitializer(TempReflection.ObjectFields(typeof(T)));
        }
        public override DataGridItem Create()
        {
            var t = new T();
            initializer.Reset(t);
            return t;
        }
        public Action<T, U> Invoke;
        public override void Update(DataGridItem obj, DataGridItemContext dat)
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
        List<DataGridColumn> columns = new List<DataGridColumn>();
        public List<DataGridColumn> BindingData { get => columns; }
        public SwapBuffer<DataGridItem, DataGridItemContext> swap;
        QueueBuffer<DataGridItem> queue;
        public DataGrid()
        {
            swap = new SwapBuffer<DataGridItem, DataGridItemContext>(1024);
            queue = new QueueBuffer<DataGridItem>(256);
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
        void CreateHead()
        {

        }
        void UpdateItem(Vector3 pos,DataGridItemContext data)
        {
            var item = swap.Exchange((o, e) => { return o.Context == e; }, data);
            if(item==null)
            {

            }
        }
        DataGridItem CreateItem()
        {
            DataGridItem it = queue.Dequeue();
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
                DataGridItem a = new DataGridItem();

                return a;
            }
        }
        void CreateLine()
        {

        }
        void OrderHead()
        {

        }
        void OrderItem()
        {

        }
        void OrderLine()
        {

        }

        public void AddColumn(DataGridColumn column)
        {

        }
        public void AddRow()
        {

        }
        public void RemoveRow()
        {

        }
        DGConstructor creator;
        public void SetItemUpdate<T, U>(Action<T, U> action)
            where T : DataGridItem, new() where U : DataGridItemContext, new()
        {
            var m = new DGMiddleware<T, U>();
            m.Invoke = action;
            creator = m;
        }
    }
}
