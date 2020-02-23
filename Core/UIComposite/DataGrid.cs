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
        public int Row;
        public int Column;
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
        FakeStruct LineMod;
        Transform Heads;
        Transform Grid;
        Transform Items;
        Transform Drags;
        Vector2 contentSize;
        Vector2 headSize;
        float itemY;
        float m_pointY;
        float m_pointX;
        float lineWidth;
        float lineHigh;
        List<DataGridColumn> columns = new List<DataGridColumn>();
        public List<DataGridColumn> BindingData { get => columns; }
        public SwapBuffer<DataGridHead, DataGridColumn> HeadSwap;
        QueueBuffer<DataGridHead> headQueue;
        public SwapBuffer<DataGridItem, DataGridItemContext> ItemSwap;
        QueueBuffer<DataGridItem> itemQueue;
        SwapBuffer<UserEvent, DataGridHead> dragSwap;
        List<HImage> lines;
        List<HImage> temp;
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
            temp = new List<HImage>();
        }
        public override void Initial(FakeStruct mod, UIElement element)
        {
            base.Initial(mod, element);
            HeadMod = HGUIManager.FindChild(mod, "Head");
            ItemMod = HGUIManager.FindChild(mod, "Item");
            DragMod = HGUIManager.FindChild(mod, "Drag");
            LineMod = HGUIManager.FindChild(mod, "Line");
            var trans = element.transform;
            Heads = trans.Find("Heads");
            Grid = trans.Find("Grid");
            Items = Grid.Find("Items");
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
            item.target.transform.localPosition = pos;
            pos.x += col.width;
            UpdateDrag(pos,item);
            pos.y = -0.5f*lineHigh ;
            UpdateLine(pos,new Vector2(2,lineHigh));
            headCreator.Update(item, col);
        }
        void UpdateItem(Vector3 pos, DataGridItemContext data,int row,int col)
        {
            var item = ItemSwap.Exchange((o, e) => { return o.Context == e; }, data);
            if (item == null)
            {
                item = CreateEnity<DataGridItem>(itemQueue, ItemMod, itemCreator, Items);
                item.Context = data;
                ItemSwap.Push(item);
            }
            pos.y = -pos.y;
            item.Row = row;
            item.Column = col;
            item.target.transform.localPosition = pos;
            itemCreator.Update(item, data);
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
        void UpdateLine(Vector3 pos,Vector2 size)
        {
            HImage item = null;
            if (temp.Count > 0)
            {
                item = temp[0];
                item.gameObject.SetActive(true);
                temp.RemoveAt(0);
            }else
            {
                item = HGUIManager.GameBuffer.Clone(LineMod).GetComponent<HImage>();
                item.transform.SetParent(Grid);
                item.transform.localScale = Vector3.one;
                item.transform.localRotation = Quaternion.identity;
            }
            lines.Add(item);
            item.transform.localPosition = pos;
            item.SizeDelta = size;
        }
        void Order()
        {
            temp.AddRange(lines);
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
            RecycleEnity(ItemSwap,itemQueue);
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
                float oy = s * itemY-m_pointY;
                for (int i = s; i < data.Count; i++)
                {
                    oy += itemY;
                    UpdateLine(new Vector3(lineWidth*0.5f,-oy,0),new Vector2(lineWidth,2));
                    if (oy > end)
                        break;
                }
            }
        }

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
            columns.Add(column);
        }
        public void RemoveColumn(int index)
        {
            if (index < 0)
                return;
            if (index < columns.Count)
                columns.RemoveAt(index);
        }
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
            {
                if (data != null)
                    item.Text.Text = data.Text;
                else item.Text.Text = null;
            }
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
