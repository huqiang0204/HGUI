using huqiang.UIEvent;
using System;
using System.Collections.Generic;
using UnityEngine;
using huqiang.Core.HGUI;
using huqiang.Data;

namespace huqiang.UIComposite
{
    /// <summary>
    /// 停靠面板的线
    /// </summary>
    public class DockPanelLine
    {
        /// <summary>
        /// 拖动纵向线时,光标显示的Icon
        /// </summary>
        public static Texture2D CursorX;
        /// <summary>
        /// 拖动横向线时,光标显示的Icon
        /// </summary>
        public static Texture2D CursorY;
        /// <summary>
        /// 目标面板
        /// </summary>
        public DockPanel layout;
        UserEvent callBack;
        public UIElement Enity;
        /// <summary>
        /// 线的方向
        /// </summary>
        public Direction direction { get; private set; }
        /// <summary>
        /// 需要绘制线
        /// </summary>
        public bool realLine { get; private set; }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="lay">目标面板</param>
        /// <param name="mod">线模型</param>
        /// <param name="dir">方向</param>
        /// <param name="real">是否需要绘制显示</param>
        public DockPanelLine(DockPanel lay,UIElement mod,Direction dir,bool real=true)
        {
            realLine= real;
            layout = lay;
            layout.lines.Add(this);
            Enity = mod;
            if (real)
            {
                callBack = Enity.RegEvent<UserEvent>();
                callBack.Drag = Drag;
                callBack.DragEnd = (o, e, v) => {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
                    Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
#endif
                    if (layout.LayOutChanged != null)
                        layout.LayOutChanged(layout);
                };
                direction = dir;
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
                callBack.PointerEntry = (o, e) =>
                {
                    if(direction==Direction.Horizontal)
                    {
                        Cursor.SetCursor(CursorY, new Vector2(64, 64), CursorMode.Auto);
                    }
                    else
                    {
                        Cursor.SetCursor(CursorX, new Vector2(64, 64), CursorMode.Auto);
                    } 
                };
                callBack.PointerLeave = (o, e) => {
                    if (!o.Pressed)
                        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                };
#endif
            }
            else mod.gameObject.SetActive(false);
            mod.transform.SetParent(layout.LineLevel);
            mod.transform.localPosition = Vector3.zero;
            mod.transform.localScale = Vector3.one;
            mod.transform.localRotation = Quaternion.identity;
        }
        void Drag(UserEvent callBack,UserAction action,Vector2 v)
        {
            if(direction==Direction.Vertical)//竖线只能横向移动
            {
                if(v.x<0)
                    MoveLeft(v.x);
                else
                    MoveRight(v.x);
                for (int i = 0; i < Left.Count; i++)
                    Left[i].SizeChanged();
                for (int i = 0; i <Right.Count; i++)
                    Right[i].SizeChanged();
            }
            else//横线只能纵向移动
            {
                if(v.y<0)
                    MoveDown(v.y);
                else
                    MoveTop(v.y);
                for (int i = 0; i < Top.Count; i++)
                    Top[i].SizeChanged();
                for (int i = 0; i < Down.Count; i++)
                    Down[i].SizeChanged();
            }
            for (int i = 0; i < AdjacentLines.Count; i++)
                AdjacentLines[i].SizeChanged();
        }
       
        void MoveLeft(float dis)
        {
            var trans = Enity.transform;
            Vector3 pos = trans.localPosition;
            pos.x += dis;
            for (int i = 0; i < Left.Count; i++)
            {
                var lx = Left[i].Left.Enity.transform.localPosition.x;
                if (pos.x <= lx + layout.AreaWidth)
                    pos.x = lx + layout.AreaWidth;
            }
            trans.localPosition = pos;
        }
        void MoveTop(float dis)
        {
            var trans = Enity.transform;
            Vector3 pos = trans.localPosition;
            pos.y += dis;
            for (int i = 0; i < Top.Count; i++)
            {
                var ty = Top[i].Top.Enity.transform.localPosition.y;
                if (pos.y >= ty - layout.AreaWidth)
                    pos.y = ty - layout.AreaWidth;
            }
            trans.localPosition = pos;
        }
        void MoveRight(float dis)
        {
            var trans = Enity.transform;
            Vector3 pos = trans.localPosition;
            pos.x += dis;
            for (int i = 0; i < Right.Count; i++)
            {
                var rx = Right[i].Right.Enity.transform.localPosition.x;
                if (pos.x >= rx- layout.AreaWidth)
                    pos.x = rx - layout.AreaWidth;
            }
            trans.localPosition = pos;
        }
        void MoveDown(float dis)
        {
            var trans = Enity.transform;
            Vector3 pos = trans.localPosition;
            pos.y += dis;
            for (int i = 0; i < Down.Count; i++)
            {
                var ty = Down[i].Down.Enity.transform.localPosition.y;
                if (pos.y <= ty + layout.AreaWidth)
                    pos.y = ty + layout.AreaWidth;
            }
            trans.localPosition = pos;
        }
        public void SetSize(Vector3 pos,Vector2 size)
        {
            Enity.transform.localPosition = pos;
            Enity.SizeDelta = size;
        }
    
        /// <summary>
        /// 左边相邻的所有区域
        /// </summary>
        public List<DockpanelArea> Left = new List<DockpanelArea>();
        /// <summary>
        /// 右边相邻的所有区域
        /// </summary>
        public List<DockpanelArea> Right = new List<DockpanelArea>();
        /// <summary>
        /// 顶部相邻的区域
        /// </summary>
        public List<DockpanelArea> Top = new List<DockpanelArea>();
        /// <summary>
        /// 底部相邻的区域
        /// </summary>
        public List<DockpanelArea> Down = new List<DockpanelArea>();
        DockPanelLine LineStart;//起点相邻的线
        DockPanelLine LineEnd;//终点相邻的线
        /// <summary>
        /// 所有与之相邻的线
        /// </summary>
        List<DockPanelLine> AdjacentLines = new List<DockPanelLine>();
        /// <summary>
        /// 当临边的线改动时会牵动此线的尺寸改变
        /// </summary>
        public void SizeChanged()
        {
            if(LineStart!=null&LineEnd!=null)
            {
                if(direction==Direction.Horizontal)
                {
                    float sx = LineStart.Enity.transform.localPosition.x;
                    float ex = LineEnd.Enity.transform.localPosition.x;
                    float w= ex - sx;
                    var trans = Enity.transform;
                    var pos = trans.localPosition;
                    pos.x = sx + 0.5f * w;
                    trans.localPosition = pos;
                    if (w < 0)
                        w = -w;
                    var size = Enity.SizeDelta;
                    size.x = w;
                    Enity.SizeDelta = size;
                }
                else
                {
                    float sx = LineStart.Enity.transform.localPosition.y;
                    float ex = LineEnd.Enity.transform.localPosition.y;
                    float w = ex - sx;
                    var trans = Enity.transform;
                    var pos = trans.localPosition;
                    pos.y = sx + 0.5f * w;
                    trans.localPosition= pos;
                    if (w < 0)
                        w = -w;
                    var size = Enity.SizeDelta;
                    size.y = w;
                    Enity.SizeDelta = size;
                }
                for (int i = 0; i < Left.Count; i++)
                    Left[i].SizeChanged();
                for (int i = 0; i < Right.Count; i++)
                    Right[i].SizeChanged();
                for (int i = 0; i < Top.Count; i++)
                    Top[i].SizeChanged();
                for (int i = 0; i < Down.Count; i++)
                    Down[i].SizeChanged();
            }
        }
        /// <summary>
        /// 设置起点相关的线
        /// </summary>
        /// <param name="line"></param>
        public void SetLineStart(DockPanelLine line)
        {
            if (LineStart != null)
                LineStart.AdjacentLines.Remove(this);
            LineStart = line;
            LineStart.AdjacentLines.Add(this);
        }
        /// <summary>
        /// 设置终点相关的线
        /// </summary>
        /// <param name="line"></param>
        public void SetLineEnd(DockPanelLine line)
        {
            if (LineEnd != null)
                LineEnd.AdjacentLines.Remove(this);
            LineEnd = line;
            LineEnd.AdjacentLines.Add(this);
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (LineStart != null)
                LineStart.AdjacentLines.Remove(this);
            if (LineEnd != null)
                LineEnd.AdjacentLines.Remove(this);
            layout.lines.Remove(this);
            HGUIManager.GameBuffer.RecycleGameObject(Enity.gameObject);
        }
        void Release()
        {
            if (LineStart != null)
                LineStart.AdjacentLines.Remove(this);
            if (LineEnd != null)
                LineEnd.AdjacentLines.Remove(this);
            LineStart = null;
            LineEnd = null;
        }
        /// <summary>
        /// 向左合并
        /// </summary>
        /// <param name="line">目标线</param>
        public void MergeLeft(DockPanelLine line)
        {
            line.Release();
            Left.AddRange(line.Left);
            var areas = line.Left;
            for (int i = 0; i < areas.Count; i++)
                areas[i].Right = this;
            var trans = Enity.transform;
            var pos = trans.localPosition;
            pos.y = line.Enity.transform.localPosition.y;
            trans.localPosition = pos;
            var al = line.AdjacentLines;
            int c = al.Count - 1;
            for (; c >= 0; c--)
            {
                var l = al[c];
                l.SetLineEnd(this);
            }
        }
        /// <summary>
        /// 向右合并
        /// </summary>
        /// <param name="line">目标线</param>
        public void MergeRight(DockPanelLine line)
        {
            line.Release();
            Right.AddRange(line.Right);
            var areas = line.Right;
            for (int i = 0; i < areas.Count; i++)
                areas[i].Left = this;
            var trans = Enity.transform;
            var pos = trans.localPosition;
            pos.y = line.Enity.transform.localPosition.y;
            trans.localPosition = pos;
            var al = line.AdjacentLines;
            int c = al.Count - 1;
            for (; c >= 0; c--)
            {
                var l = al[c];
                l.SetLineStart(this);
            }
        }
        /// <summary>
        /// 向上合并
        /// </summary>
        /// <param name="line">目标线</param>
        public void MergeTop(DockPanelLine line)
        {
            line.Release();
            Top.AddRange(line.Top);
            var areas = line.Top;
            for (int i = 0; i < areas.Count; i++)
                areas[i].Down = this;
            var trans = Enity.transform;
            var pos = trans.localPosition;
            pos.x = line.Enity.transform.localPosition.x;
            trans.localPosition = pos;
            var al = line.AdjacentLines;
            int c = al.Count - 1;
            for (; c >= 0; c--)
            {
                var l = al[c];
                l.SetLineStart(this);
            }
        }
        /// <summary>
        /// 向下合并
        /// </summary>
        /// <param name="line">目标线</param>
        public void MergeDown(DockPanelLine line)
        {
            line.Release();
            Down.AddRange(line.Down);
            var areas = line.Down;
            for (int i = 0; i < areas.Count; i++)
                areas[i].Top = this;
            var trans = Enity.transform;
            var pos = trans.localPosition;
            pos.x = line.Enity.transform.localPosition.x;
            trans.localPosition = pos;
            var al = line.AdjacentLines;
            int c = al.Count-1;
            for (; c>=0;c--)
            {
                var l = al[c];
                l.SetLineEnd(this);
            }
        }
        /// <summary>
        /// 存储布局数据
        /// </summary>
        /// <param name="fake"></param>
        /// <param name="index"></param>
        public void SaveToDataBuffer(FakeStructArray fake,int index)
        {
            fake[index,0] = (int)direction;
            fake.SetFloat(index, 1,Enity.m_sizeDelta.x);
            fake.SetFloat(index,2, Enity.m_sizeDelta.y);
            fake.SetFloat(index,3,Enity.transform.localPosition.x);
            fake.SetFloat(index,4, Enity.transform.localPosition.y);
            fake[index,5] = layout.GetLineID(LineStart);
            fake[index,6] = layout.GetLineID(LineEnd);
            fake.SetData(index,7,SaveAdjacentArea(Left));
            fake.SetData(index,8, SaveAdjacentArea(Right));
            fake.SetData(index,9, SaveAdjacentArea(Top));
            fake.SetData(index,10, SaveAdjacentArea(Down));
            if(AdjacentLines.Count>0)
            {
                int[] tmp = new int[AdjacentLines.Count];
                for (int i = 0; i < tmp.Length; i++)
                    tmp[i] = layout.GetLineID(AdjacentLines[i]);
                fake.SetData(index,11,tmp);
            }
        }
        int[] SaveAdjacentArea(List<DockpanelArea> areas)
        {
            if(areas.Count>0)
            {
                int[] tmp = new int[areas.Count];
                for (int i = 0; i < tmp.Length; i++)
                    tmp[i] = layout.GetAreaID(areas[i]);
                return tmp;
            }
            return null;
        }
        /// <summary>
        /// 从布局数据中载入
        /// </summary>
        /// <param name="fake">数据缓存</param>
        /// <param name="index">索引</param>
        public void LoadFromBuffer(FakeStructArray fake, int index)
        {
            direction =(Direction)fake[index,0];
            Enity.m_sizeDelta.x = fake.GetFloat(index,1);
            Enity.m_sizeDelta.y = fake.GetFloat(index,2);
            Enity.transform.localPosition = new Vector3(fake.GetFloat(index,3),fake.GetFloat(index,4),0);
            LineStart = layout.GetLine(fake[index,5]);
            LineEnd = layout.GetLine(fake[index,6]);
            LoadAdjacentArea(Left, fake.GetData<int[]>(index,7));
            LoadAdjacentArea(Right, fake.GetData<int[]>(index,8));
            LoadAdjacentArea(Top, fake.GetData<int[]>(index,9));
            LoadAdjacentArea(Down, fake.GetData<int[]>(index,10));
            int[] ids = fake.GetData<int[]>(index,11);
            if (ids != null)
            {
                for (int i = 0; i < ids.Length; i++)
                    AdjacentLines.Add(layout.GetLine(ids[i]));
            }
        }
        void LoadAdjacentArea(List<DockpanelArea> areas, int[] ids)
        {
            if(ids!=null)
            {
                for (int i = 0; i < ids.Length; i++)
                    areas.Add(layout.GetArea(ids[i]));
            }
        }
    }
    /// <summary>
    /// 停靠面板区域
    /// </summary>
    public class DockpanelArea
    {
        public enum Dock
        {
            Left, Top, Right, Down
        }
        /// <summary>
        /// 区域模型
        /// </summary>
        public UIElement model;
        /// <summary>
        /// 停靠面板主体
        /// </summary>
        public DockPanel layout;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="lay">停靠面板主体</param>
        public DockpanelArea(DockPanel lay)
        {
            layout = lay;
            model= HGUIManager.GameBuffer.Clone(layout.AreaMod).GetComponent<UIElement>();
            layout = lay;
            model.transform.SetParent(layout.AreaLevel);
            model.transform.localPosition = Vector3.zero;
            model.transform.localScale = Vector3.one;
            model.transform.localRotation = Quaternion.identity;
            layout.areas.Add(this);
        }
        /// <summary>
        /// 左边相邻的线
        /// </summary>
        internal DockPanelLine Left;
        /// <summary>
        /// 右边相邻的线
        /// </summary>
        internal DockPanelLine Right;
        /// <summary>
        /// 顶部相邻的线
        /// </summary>
        internal DockPanelLine Top;
        /// <summary>
        /// 底部相邻的线
        /// </summary>
        internal DockPanelLine Down;
        /// <summary>
        /// 设置左边的线
        /// </summary>
        /// <param name="line"></param>
        public void SetLeftLine(DockPanelLine line)
        {
            if (Left != null)
                Left.Right.Remove(this);
            Left = line;
            line.Right.Add(this);
        }
        /// <summary>
        /// 设置右边的线
        /// </summary>
        /// <param name="line"></param>
        public void SetRightLine(DockPanelLine line)
        {
            if (Right != null)
                Right.Left.Remove(this);
            Right = line;
            line.Left.Add(this);
        }
        /// <summary>
        /// 设置顶部的线
        /// </summary>
        /// <param name="line"></param>
        public void SetTopLine(DockPanelLine line)
        {
            if (Top != null)
                Top.Down.Remove(this);
            Top = line;
            line.Down.Add(this);
        }
        /// <summary>
        /// 设置底部的线
        /// </summary>
        /// <param name="line"></param>
        public void SetDownLine(DockPanelLine line)
        {
            if (Down != null)
                Down.Top.Remove(this);
            Down = line;
            line.Top.Add(this);
        }
        /// <summary>
        /// 区域尺寸被改变了,调用此函数
        /// </summary>
        public void SizeChanged()
        {
            float hl = DockPanel.LineWidth * 0.5f;
            float rx = Right.Enity.transform.localPosition.x;
            if (Right.realLine)
                rx -= hl;
            float lx = Left.Enity.transform.localPosition.x;
            if (Left.realLine)
                lx += hl;
            float ty = Top.Enity.transform.localPosition.y;
            if (Top.realLine)
                ty -= hl;
            float dy = Down.Enity.transform.localPosition.y;
            if (Down.realLine)
                dy += hl;
            float w = rx - lx;
            float h = ty - dy;
            bool c = false;
            if (model.SizeDelta.x != w)
                c = true;
            if (model.SizeDelta.y != h)
                c = true;
            model.SizeDelta = new Vector2(w,h);
            float x = lx + w * 0.5f;
            float y = dy + h * 0.5f;
            model.transform.localPosition = new Vector3(x,y,0);
            if (c)
            {
                UIElement.ResizeChild(model);//触发SizeChange事件
            }
        }
        /// <summary>
        /// 添加一个区域
        /// </summary>
        /// <param name="dock">停靠方位</param>
        /// <param name="r">区域大小比例</param>
        /// <returns></returns>
        public DockpanelArea AddAreaR(Dock dock, float r = 0.5f)
        {
            switch (dock)
            {
                case Dock.Left:
                    float dx = Left.Enity.transform.localPosition.x;
                    float x = Right.Enity.transform.localPosition.x - dx;
                    x *= r;
                    x += dx;
                    return AddLeftArea(x);
                case Dock.Top:
                    float dy = Down.Enity.transform.localPosition.y;
                    float y = Top.Enity.transform.localPosition.y - dy;
                    y *=(1- r);
                    y += dy;
                    return AddTopArea(y);
                case Dock.Right:
                    dx = Left.Enity.transform.localPosition.x;
                    x = Right.Enity.transform.localPosition.x - dx;
                    x *= (1 - r);
                    x += dx;
                    return AddRightArea(x);
                case Dock.Down:
                    dy = Down.Enity.transform.localPosition.y;
                    y = Top.Enity.transform.localPosition.y - dy;
                    y *= r;
                    y += dy;
                    return AddDownArea(y);
            }
            return this;
        }
        /// <summary>
        /// 添加一个区域
        /// </summary>
        /// <param name="dock">停靠方位</param>
        /// <param name="w">所占宽度</param>
        /// <returns></returns>
        public DockpanelArea AddArea(Dock dock, float w = 100f)
        {
            switch (dock)
            {
                case Dock.Left:
                    return AddLeftArea(Left.Enity.transform.localPosition.x + w);
                case Dock.Top:
                    return AddTopArea(Top.Enity.transform.localPosition.y - w);
                case Dock.Right:
                    return AddRightArea(Right.Enity.transform.localPosition.x - w);
                case Dock.Down:
                    return AddDownArea(Down.Enity.transform.localPosition.y + w);
            }
            return this;
        }
        DockPanelLine AddHLine(float y)
        {
            var m = HGUIManager.GameBuffer.Clone(layout.LineMod).GetComponent<UIElement>();
            float ex = Right.Enity.transform.localPosition.x;
            float sx = Left.Enity.transform.localPosition.x;
            float w = ex - sx;
            if (w < 0)
                w = -w;
            DockPanelLine line = new DockPanelLine(layout, m, Direction.Horizontal);
            var pos = model.transform.localPosition;
            pos.y = y;
            line.SetSize(pos, new Vector2(w, DockPanel.LineWidth));
            line.SetLineStart(Left);
            line.SetLineEnd(Right);
            return line;
        }
        DockPanelLine AddVLine(float x)
        {
            var m = HGUIManager.GameBuffer.Clone(layout.LineMod).GetComponent<UIElement>();
            float ex = Top.Enity.transform.localPosition.y;
            float sx = Down.Enity.transform.localPosition.y;
            float w = ex - sx;
            if (w < 0)
                w = -w;
            DockPanelLine line = new DockPanelLine(layout, m, Direction.Vertical);
            var pos = model.transform.localPosition;
            pos.x = x;
            line.SetSize(pos, new Vector2(DockPanel.LineWidth, w));
            line.SetLineStart(Down);
            line.SetLineEnd(Top);
            return line;
        }
        DockpanelArea AddLeftArea(float x)
        {
            DockpanelArea area = new DockpanelArea(layout);
            var line = AddVLine(x);
            area.SetLeftLine(Left);
            area.SetRightLine(line);
            area.SetTopLine(Top);
            area.SetDownLine(Down);
            SetLeftLine(line);
            return area;
        }
        DockpanelArea AddRightArea(float x)
        {
            DockpanelArea area = new DockpanelArea(layout);
            var line = AddVLine(x);
            area.SetLeftLine(line);
            area.SetRightLine(Right);
            area.SetTopLine(Top);
            area.SetDownLine(Down);
            SetRightLine(line);
            return area;
        }
        DockpanelArea AddTopArea(float y)
        {
            DockpanelArea area = new DockpanelArea(layout);
            var line = AddHLine(y);
            area.SetLeftLine(Left);
            area.SetRightLine(Right);
            area.SetTopLine(Top);
            area.SetDownLine(line);
            SetTopLine(line);
            return area;
        }
        DockpanelArea AddDownArea(float y)
        {
            DockpanelArea area = new DockpanelArea(layout);
            var line = AddHLine(y);
            area.SetLeftLine(Left);
            area.SetRightLine(Right);
            area.SetTopLine(line);
            area.SetDownLine(Down);
            SetDownLine(line);
            return area;
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (Left.realLine | Right.realLine | Top.realLine | Down.realLine)
            {
                Left.Right.Remove(this);
                Right.Left.Remove(this);
                Top.Down.Remove(this);
                Down.Top.Remove(this);
                MergeArea();
                HGUIManager.GameBuffer.RecycleGameObject(model.gameObject);
                layout.areas.Remove(this);
            }
        }
        void MergeArea()
        {
            if (Left.realLine)
            {
                if (Left.Right.Count < 1)
                {
                    Right.MergeLeft(Left);
                    Left.Dispose();
                    return;
                }
            }
            if (Right.realLine)
            {
                if (Right.Left.Count < 1)
                {
                    Left.MergeRight(Right);
                    Right.Dispose();
                    return;
                }
            }
            if (Top.realLine)
            {
                if (Top.Down.Count < 1)
                {
                    Down.MergeTop(Top);
                    Top.Dispose();
                    return;
                }
            }
            if (Down.realLine)
            {
                if (Down.Top.Count < 1)
                {
                    Top.MergeDown(Down);
                    Down.Dispose();
                    return;
                }
            }
        }
        /// <summary>
        /// 存储布局信息
        /// </summary>
        /// <param name="fake">数据缓存</param>
        /// <param name="index">索引位置</param>
        public void SaveToDataBuffer(FakeStructArray fake, int index)
        {
            fake[index, 0] = layout.GetLineID(Left);
            fake[index, 1] = layout.GetLineID(Right);
            fake[index, 2] = layout.GetLineID(Top);
            fake[index, 3] = layout.GetLineID(Down);
        }
        /// <summary>
        /// 从缓存中读取布局信息
        /// </summary>
        /// <param name="fake">数据缓存</param>
        /// <param name="index">索引位置</param>
        public void LoadFromBuffer(FakeStructArray fake, int index)
        {
            Left = layout.GetLine(fake[index, 0]);
            Right = layout.GetLine(fake[index, 1]);
            Top = layout.GetLine(fake[index, 2]);
            Down = layout.GetLine(fake[index, 3]);
        }
    }
    /// <summary>
    /// 停靠面板
    /// </summary>
    public class DockPanel : Composite
    {
        /// <summary>
        /// 区域最小宽度
        /// </summary>
        public float AreaWidth = 40f;
        /// <summary>
        /// 线宽
        /// </summary>
        public static float LineWidth = 8f;
        /// <summary>
        /// 线列表
        /// </summary>
        public List<DockPanelLine> lines = new List<DockPanelLine>();
        /// <summary>
        /// 区域列表
        /// </summary>
        public List<DockpanelArea> areas = new List<DockpanelArea>();
        /// <summary>
        /// 左边相邻的线
        /// </summary>
        public DockPanelLine Left;
        /// <summary>
        /// 右边相邻的线
        /// </summary>
        public DockPanelLine Right;
        /// <summary>
        /// 顶部相邻的线
        /// </summary>
        public DockPanelLine Top;
        /// <summary>
        /// 底部相邻的线
        /// </summary>
        public DockPanelLine Down;
        /// <summary>
        /// 线模型
        /// </summary>
        public FakeStruct LineMod;
        /// <summary>
        /// 区域模型
        /// </summary>
        public FakeStruct AreaMod;
        /// <summary>
        /// 管理线的父物体
        /// </summary>
        public Transform LineLevel;
        /// <summary>
        /// 管理区域的父物体
        /// </summary>
        public Transform AreaLevel;
        /// <summary>
        /// 当布局被改变时回调
        /// </summary>
        public Action<DockPanel> LayOutChanged;
        /// <summary>
        /// 主区域
        /// </summary>
        public DockpanelArea MainArea { get; private set; }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="fake">数据模型</param>
        /// <param name="element">元素主体</param>
        public override void Initial(FakeStruct fake,UIElement element,Initializer initializer)
        {
            base.Initial(fake,element,initializer);
            var mod = Enity.transform;
            LineLevel = mod.Find("LineLevel");
            AreaLevel= mod.Find("AreaLevel");
            LineMod = HGUIManager.FindChild(fake, "Line");
            AreaMod = HGUIManager.FindChild(fake,"Area");
            HGUIManager.GameBuffer.RecycleGameObject(mod.Find("Line").gameObject);
            HGUIManager.GameBuffer.RecycleGameObject(mod.Find("Area").gameObject);
            InitialFixLine();
            InitialArea();
            Enity.SizeChanged = SizeChanged;
            var ex = UITransfromLoader.GetCompositeData(fake);
            if (ex != null)
            {
                LoadFromBuffer(ex);
            }
        }
        void InitialFixLine()
        {
            var m = HGUIManager.GameBuffer.Clone(LineMod).GetComponent<UIElement>();
            Left = new DockPanelLine(this,m,Direction.Vertical,false);

            m = HGUIManager.GameBuffer.Clone(LineMod).GetComponent<UIElement>();
            Right = new DockPanelLine(this, m, Direction.Vertical, false);

            m = HGUIManager.GameBuffer.Clone(LineMod).GetComponent<UIElement>();
            Top = new DockPanelLine(this, m, Direction.Vertical, false);

            m = HGUIManager.GameBuffer.Clone(LineMod).GetComponent<UIElement>();
            Down = new DockPanelLine(this, m, Direction.Vertical, false);

            Vector2 size = Enity.SizeDelta;
            float rx = size.x * 0.5f;
            float ty = size.y * 0.5f;

            Left.SetSize(new Vector2(-rx, 0), new Vector2(LineWidth, size.y));
            Right.SetSize(new Vector2(rx, 0), new Vector2(LineWidth, size.y));
            Top.SetSize(new Vector2(0, ty), new Vector2(size.x, LineWidth));
            Down.SetSize(new Vector2(0, -ty), new Vector2(size.x, LineWidth));
        }
        void InitialArea()
        {
            DockpanelArea area = new DockpanelArea(this);
            area.Left = Left;
            area.Right = Right;
            area.Top = Top;
            area.Down = Down;
            MainArea = area;
            area.SizeChanged();
        }
        /// <summary>
        /// 当元素尺寸被改变了
        /// </summary>
        /// <param name="mod">ui实体</param>
        public void SizeChanged(UIElement mod)
        {
            Vector2 size = Enity.SizeDelta;
            float rx = size.x * 0.5f;
            float ty = size.y*0.5f;

            Left.SetSize(new Vector2(-rx, 0), new Vector2(LineWidth, size.y));
            Right.SetSize(new Vector2(rx, 0), new Vector2(LineWidth, size.y));
            Top.SetSize(new Vector2(0, ty), new Vector2(size.x, LineWidth));
            Down.SetSize(new Vector2(0, -ty), new Vector2(size.x, LineWidth));
            for (int i = 0; i < lines.Count; i++)
                lines[i].SizeChanged();
            if (LayOutChanged != null)
                LayOutChanged(this);
        }
        /// <summary>
        /// 刷新显示
        /// </summary>
        public void Refresh()
        {
            for (int i = 0; i < lines.Count; i++)
                lines[i].SizeChanged();
            for (int i = 0; i < areas.Count; i++)
                areas[i].SizeChanged();
        }
        /// <summary>
        /// 锁定布局
        /// </summary>
        public bool LockLayout;
        public void LoadFromBuffer(FakeStruct fake)
        {
            var fsa = fake.GetData<FakeStructArray>(0);
            var fsa2 = fake.GetData<FakeStructArray>(1);
            int max = fsa.Length;
            if (max > lines.Count)
                max = lines.Count;
            for (int i = 0; i < max; i++)
                lines[i].LoadFromBuffer(fsa, i);
            max = fsa2.Length;
            if (max > areas.Count)
                max = areas.Count;
            for (int i = 0; i < max; i++)
                areas[i].LoadFromBuffer(fsa2,i);
        }
        /// <summary>
        /// 储存数据布局信息
        /// </summary>
        /// <param name="db">缓存实例</param>
        /// <returns></returns>
        public FakeStruct SaveToDataBuffer(DataBuffer db)
        {
            FakeStruct fake = new FakeStruct(db, 2);
            FakeStructArray fsa = new FakeStructArray(db, 12, lines.Count);
            for (int i = 0; i < lines.Count; i++)
                lines[i].SaveToDataBuffer(fsa, i);
            fake.SetData(0,fsa);
            fsa = new FakeStructArray(db,4, areas.Count);
            for (int i = 0; i < areas.Count; i++)
                areas[i].SaveToDataBuffer(fsa,i);
            fake.SetData(1,fsa);
            return fake;
        }
        /// <summary>
        /// 获取id的存储id
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public int GetLineID(DockPanelLine line)
        {
            if(line==Left)
            return 0;
            if (line == Right)
                return 1;
            if (line == Top)
                return 2;
            if (line == Down)
                return 3;
            for (int i = 0; i < lines.Count; i++)
                if (lines[i] == line)
                    return i + 3;
            return -1;
        }
        /// <summary>
        /// 通过ID查找线
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>
        public DockPanelLine GetLine(int id)
        {
            if (id < 0)
                return null;
            if (id == 0)
                return Left;
            if (id == 1)
                return Right;
            if (id == 2)
                return Top;
            if (id == 3)
                return Down;
            id -= 3;
            if (id < lines.Count)
                return lines[id];
            return null;
        }
        /// <summary>
        /// 获取区域id
        /// </summary>
        /// <param name="area">区域实例</param>
        /// <returns></returns>
        public int GetAreaID(DockpanelArea area)
        {
            for (int i = 0; i < areas.Count; i++)
                if (areas[i] == area)
                    return i;
            return -1;
        }
        /// <summary>
        /// 通过id查找区域
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>
        public DockpanelArea GetArea(int id)
        {
            if (id < 0)
                return null;
            if (id < areas.Count)
                return areas[id];
            return null;
        }
    }
}
