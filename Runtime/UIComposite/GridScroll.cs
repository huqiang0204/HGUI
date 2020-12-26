using huqiang.Core.HGUI;
using huqiang.Data;
using huqiang.UIEvent;
using System;
using UnityEngine;

namespace huqiang.UIComposite
{
    /// <summary>
    /// 网格滚动框
    /// </summary>
    public class GridScroll:ScrollContent 
    {
        /// <summary>
        /// 列数
        /// </summary>
        public int Column = 1;
        /// <summary>
        /// 行数
        /// </summary>
        public int Row = 0;
        /// <summary>
        /// 事件
        /// </summary>
        public UserEvent eventCall;
        /// <summary>
        /// 滚动事件
        /// </summary>
        public Action<GridScroll, Vector2> Scroll;
        /// <summary>
        /// 滚动结束事件
        /// </summary>
        public Action<GridScroll> ScrollEnd;
        void Calcul()
        {
            if (BindingData == null)
            {
                Row = 0;
                return;
            }
            int c = DataLength;
            Row = c / Column;
            if (c % Column > 0)
                Row++;
            _contentSize = new Vector2(Column * ItemSize.x, Row * ItemSize.y);
            ItemActualSize = ItemSize;
            GetItemOffset();
        }
        public override void Initial(FakeStruct fake,UIElement script,UIInitializer initializer)
        {
            base.Initial(fake,script,initializer);
            eventCall = script.RegEvent<UserEvent>();
            eventCall.PointerDown = (o, e) => { UpdateVelocity = false; };
            eventCall.Drag = (o, e, s) => { Scrolling(o, s); };
            eventCall.DragEnd = OnDragEnd;
            eventCall.ForceEvent = true;
            Size =Enity.SizeDelta;
            eventCall.CutRect = true;
            Enity.SizeChanged = (o) =>
            {
                Refresh(Position);
            };
        }
        void Scrolling(UserEvent back, Vector2 v)
        {
            if (Main == null)
                return;
            if (BindingData == null)
                return;
            v.x /= eventCall.Context.localScale.x;
            v.x = -v.x;
            v.y /= eventCall.Context.localScale.y;
            switch (scrollType)
            {
                case ScrollType.None:
                    v = ScrollNone(v);
                    _pos += v;
                    break;
                case ScrollType.Loop:
                    _pos.x += v.x;
                    if (_pos.x < 0)
                        _pos.x += _contentSize.x;
                    else _pos.x %= _contentSize.x;
                    _pos.y += v.y;
                    if (_pos.y < 0)
                        _pos.y += _contentSize.y;
                    else _pos.y %= _contentSize.y;
                    break;
                case ScrollType.BounceBack:
                    v = BounceBack(v);
                    _pos += v;
                    break;
            }
            Order();
            if (v != Vector2.zero)
            {
                if (Scroll != null)
                    Scroll(this, v);
            }
            else
            {
                if (ScrollEnd != null)
                    ScrollEnd(this);
            }
        }
        void OnDragEnd(UserEvent back, UserAction action, Vector2 v)
        {
            Scrolling(back, v);
            startVelocity.x = mVelocity.x = -back.VelocityX;
            startVelocity.y = mVelocity.y = back.VelocityY;
            UpdateVelocity = true;
        }
        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="size">视口尺寸</param>
        /// <param name="pos">视口位置</param>
        public void Order(bool force = false)
        {
            float w = Size.x;
            float h = Size.y;

            int colStart = (int)(_pos.x / ItemActualSize.x);
            if (colStart < 0)
                colStart = 0;
            int rowStart = (int)(_pos.y / ItemActualSize.y);
            if (rowStart < 0)
                rowStart = 0;
            int rc = (int)(h / ItemActualSize.y) + 1;
            int cc = (int)(w / ItemActualSize.x) + 1;
            if (scrollType != ScrollType.Loop)
            {
                if (cc + colStart > Column)
                    cc = Column - colStart;
                if (rc + rowStart > Row)
                    rc = Row - rowStart;
            }

            Recycler.AddRange(Items);
            Items.Clear();
            for (int i = 0; i < rc; i++)
            {
                int index = (rowStart+ i )* Column + colStart;
                int cou = DataLength;
                for (int j = 0; j < cc; j++)
                {
                    if (index >= cou)
                        break;
                    for (int k = 0; k < Recycler.Count; k++)
                    {
                        var t = Recycler[k];
                        if (t.index == index)
                        {
                            Items.Add(t);
                            Recycler.RemoveAt(k);
                            t.target.activeSelf = true;
                            break;
                        }
                    }
                    index++;
                }
            }
            float oy = 0;
            for (int i=0; i < rc; i++)
            {
                UpdateRow(rowStart, colStart, cc, force, oy);
                rowStart++;
                if (rowStart >= Row)
                { 
                    rowStart = 0;
                    oy = _contentSize.y;
                }
            }
            for (int i = 0; i < Recycler.Count; i++)
                Recycler[i].target.activeSelf = false;
        }
        void UpdateRow(int row, int colStart, int colLen, bool force,float oy)
        {
            float ox = 0;
            int index = row * Column;
            for (int i = 0; i < colLen; i++)
            {
                UpdateItem(index + colStart, force,ox, oy);
                colStart++;
                if (colStart >= Column)
                { 
                    colStart = 0;
                    ox = _contentSize.x;
                }
            }
        }
        void UpdateItem(int index, bool force,float ox, float oy)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                var item = Items[i];
                if (item.index == index)
                {
                    SetItemPostion(item,ox,oy);
                    if (force)
                       ItemUpdate(item.obj,item.datacontext, index);
                    return;
                }
            }
            var it = CreateItem();
            Items.Add(it);
            it.index = index;
            it.datacontext = GetData(index);//dataList[index];
            SetItemPostion(it,ox,oy);
            ItemUpdate(it.obj,it.datacontext, index);
        }
        void SetItemPostion(ScrollItem item,float ox,float oy)
        {
            int r = item.index / Column;
            int c = item.index % Column;
            float sx = c * ItemActualSize.x;
            float sy = r * ItemActualSize.y;
            sx -= _pos.x - ox;
            sy -= _pos.y - oy;
            var p = ItemOffset;
            p.x += sx;
            p.y -= sy;
            item.target.localPosition = p;
        }
        /// <summary>
        /// 刷新到指定位置
        /// </summary>
        /// <param name="pos"></param>
        public void Refresh(Vector2 pos)
        {
            Position = pos;
            Calcul();
            Order();
        }
        /// <summary>
        /// 刷新到默认位置
        /// </summary>
        public void Refresh()
        {
            Calcul();
            Order(true);
        }
        public override void DurScroll(Vector2 v)
        {
            _pos += v;
            if(scrollType==ScrollType.Loop)
            {
                if (_pos.x < 0)
                    _pos.x += _contentSize.x;
                else _pos.x %= _contentSize.x;
                if (_pos.y < 0)
                    _pos.y += _contentSize.y;
                else _pos.y %= _contentSize.y;
            }
            Order();
        }
    }
}
