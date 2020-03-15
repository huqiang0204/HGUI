using huqiang.UIEvent;
using System;
using System.Collections.Generic;
using UnityEngine;
using huqiang.Core.HGUI;
using huqiang.Data;

namespace huqiang.UIComposite
{
    public class DockPanelLine
    {
        public DockPanel layout;
        UserEvent callBack;
        public Direction direction { get; private set; }
        /// <summary>
        /// 需要绘制线
        /// </summary>
        public bool realLine { get; private set; }
        public DockPanelLine(DockPanel lay,UIElement mod,Direction dir,bool real=true)
        {
            realLine= real;
            layout = lay;
            layout.lines.Add(this);
            model = mod;
            if (real)
            {
                callBack = model.RegEvent<UserEvent>();
                callBack.Drag = Drag;

                direction = dir;
                switch(dir)
                {
                    case Direction.Horizontal:
                        callBack.BoxAdjuvant = new Vector2(0,8);
                        break;
                    case Direction.Vertical:
                        callBack.BoxAdjuvant = new Vector2(8, 0);
                        break;
                           
                }
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
                //callBack.PointerEntry = (o, e) => {
                //    ThreadMission.InvokeToMain((y) => {
                //        Cursor.SetCursor(UnityEngine.Resources.Load<Texture2D>("emoji"),Vector2.zero,CursorMode.Auto);
                //    },null);
                //};
                //callBack.DragEnd = (o, e, v) => {
                //    ThreadMission.InvokeToMain((y) =>
                //    {
                //        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                //    }, null);
                //};
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
            var trans = model.transform;
            Vector3 pos = trans.localPosition;
            pos.x += dis;
            for (int i = 0; i < Left.Count; i++)
            {
                var lx = Left[i].Left.model.transform.localPosition.x;
                if (pos.x <= lx)
                    pos.x = lx + 1;
            }
            trans.localPosition = pos;
        }
        void MoveTop(float dis)
        {
            var trans = model.transform;
            Vector3 pos = trans.localPosition;
            pos.y += dis;
            for (int i = 0; i < Top.Count; i++)
            {
                var ty = Top[i].Top.model.transform.localPosition.y;
                if (pos.y >= ty)
                    pos.y = ty - 1;
            }
            trans.localPosition = pos;
        }
        void MoveRight(float dis)
        {
            var trans = model.transform;
            Vector3 pos = trans.localPosition;
            pos.x += dis;
            for (int i = 0; i < Right.Count; i++)
            {
                var rx = Right[i].Right.model.transform.localPosition.x;
                if (pos.x >= rx)
                    pos.x = rx - 1;
            }
            trans.localPosition = pos;
        }
        void MoveDown(float dis)
        {
            var trans = model.transform;
            Vector3 pos = trans.localPosition;
            pos.y += dis;
            for (int i = 0; i < Down.Count; i++)
            {
                var ty = Down[i].Down.model.transform.localPosition.y;
                if (pos.y <= ty)
                    pos.y = ty - 1;
            }
            trans.localPosition = pos;
        }
        public void SetSize(Vector3 pos,Vector2 size)
        {
            model.transform.localPosition = pos;
            model.SizeDelta = size;
        }
        public UIElement model;
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
        List<DockPanelLine> AdjacentLines = new List<DockPanelLine>();//所有与之相邻的线
        public void SizeChanged()
        {
            if(LineStart!=null&LineEnd!=null)
            {
                if(direction==Direction.Horizontal)
                {
                    float sx = LineStart.model.transform.localPosition.x;
                    float ex = LineEnd.model.transform.localPosition.x;
                    float w= ex - sx;
                    var trans = model.transform;
                    var pos = trans.localPosition;
                    pos.x = sx + 0.5f * w;
                    trans.localPosition = pos;
                    if (w < 0)
                        w = -w;
                    var size = model.SizeDelta;
                    size.x = w;
                    model.SizeDelta = size;
                }
                else
                {
                    float sx = LineStart.model.transform.localPosition.y;
                    float ex = LineEnd.model.transform.localPosition.y;
                    float w = ex - sx;
                    var trans = model.transform;
                    var pos = trans.localPosition;
                    pos.y = sx + 0.5f * w;
                    trans.localPosition= pos;
                    if (w < 0)
                        w = -w;
                    var size = model.SizeDelta;
                    size.y = w;
                    model.SizeDelta = size;
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
        public void SetLineStart(DockPanelLine line)
        {
            if (LineStart != null)
                LineStart.AdjacentLines.Remove(this);
            LineStart = line;
            LineStart.AdjacentLines.Add(this);
        }
        public void SetLineEnd(DockPanelLine line)
        {
            if (LineEnd != null)
                LineEnd.AdjacentLines.Remove(this);
            LineEnd = line;
            LineEnd.AdjacentLines.Add(this);
        }
        public void Dispose()
        {
            if (LineStart != null)
                LineStart.AdjacentLines.Remove(this);
            if (LineEnd != null)
                LineEnd.AdjacentLines.Remove(this);
            layout.lines.Remove(this);
            HGUIManager.GameBuffer.RecycleGameObject(model.gameObject);
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
        public void MergeLeft(DockPanelLine line)
        {
            line.Release();
            Left.AddRange(line.Left);
            var areas = line.Left;
            for (int i = 0; i < areas.Count; i++)
                areas[i].Right = this;
            var trans = model.transform;
            var pos = trans.localPosition;
            pos.y = line.model.transform.localPosition.y;
            trans.localPosition = pos;
            var al = line.AdjacentLines;
            int c = al.Count - 1;
            for (; c >= 0; c--)
            {
                var l = al[c];
                l.SetLineEnd(this);
            }
        }
        public void MergeRight(DockPanelLine line)
        {
            line.Release();
            Right.AddRange(line.Right);
            var areas = line.Right;
            for (int i = 0; i < areas.Count; i++)
                areas[i].Left = this;
            var trans = model.transform;
            var pos = trans.localPosition;
            pos.y = line.model.transform.localPosition.y;
            trans.localPosition = pos;
            var al = line.AdjacentLines;
            int c = al.Count - 1;
            for (; c >= 0; c--)
            {
                var l = al[c];
                l.SetLineStart(this);
            }
        }
        public void MergeTop(DockPanelLine line)
        {
            line.Release();
            Top.AddRange(line.Top);
            var areas = line.Top;
            for (int i = 0; i < areas.Count; i++)
                areas[i].Down = this;
            var trans = model.transform;
            var pos = trans.localPosition;
            pos.x = line.model.transform.localPosition.x;
            trans.localPosition = pos;
            var al = line.AdjacentLines;
            int c = al.Count - 1;
            for (; c >= 0; c--)
            {
                var l = al[c];
                l.SetLineStart(this);
            }
        }
        public void MergeDown(DockPanelLine line)
        {
            line.Release();
            Down.AddRange(line.Down);
            var areas = line.Down;
            for (int i = 0; i < areas.Count; i++)
                areas[i].Top = this;
            var trans = model.transform;
            var pos = trans.localPosition;
            pos.x = line.model.transform.localPosition.x;
            trans.localPosition = pos;
            var al = line.AdjacentLines;
            int c = al.Count-1;
            for (; c>=0;c--)
            {
                var l = al[c];
                l.SetLineEnd(this);
            }
        }
    }
    public class DockpanelArea
    {
        public enum Dock
        {
            Left, Top, Right, Down
        }
        public UIElement model;
        public DockPanel layout;
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
        public void SetLeftLine(DockPanelLine line)
        {
            if (Left != null)
                Left.Right.Remove(this);
            Left = line;
            line.Right.Add(this);
        }
        public void SetRightLine(DockPanelLine line)
        {
            if (Right != null)
                Right.Left.Remove(this);
            Right = line;
            line.Left.Add(this);
        }
        public void SetTopLine(DockPanelLine line)
        {
            if (Top != null)
                Top.Down.Remove(this);
            Top = line;
            line.Down.Add(this);
        }
        public void SetDownLine(DockPanelLine line)
        {
            if (Down != null)
                Down.Top.Remove(this);
            Down = line;
            line.Top.Add(this);
        }
        public void SizeChanged()
        {
            float hl = DockPanel.LineWidth * 0.5f;
            float rx = Right.model.transform.localPosition.x;
            if (Right.realLine)
                rx -= hl;
            float lx = Left.model.transform.localPosition.x;
            if (Left.realLine)
                lx += hl;
            float ty = Top.model.transform.localPosition.y;
            if (Top.realLine)
                ty -= hl;
            float dy = Down.model.transform.localPosition.y;
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
        public DockpanelArea AddArea(Dock dock, float r = 0.5f)
        {
            switch (dock)
            {
                case Dock.Left:
                    return AddLeftArea(r);
                case Dock.Top:
                    return AddTopArea(r);
                case Dock.Right:
                    return AddRightArea(r);
                case Dock.Down:
                    return AddDownArea(r);
            }
            return this;
        }
        DockPanelLine AddHorizontalLine(float r)
        {
            var m = HGUIManager.GameBuffer.Clone(layout.LineMod).GetComponent<UIElement>();
            float ex = Right.model.transform.localPosition.x;
            float sx = Left.model.transform.localPosition.x;
            float w = ex - sx;
            if (w < 0)
                w = -w;
            DockPanelLine line = new DockPanelLine(layout, m, Direction.Horizontal);
            var pos = model.transform.localPosition;
            float dy = Down.model.transform.localPosition.y;
            pos.y = Top.model.transform.localPosition.y - dy;
            pos.y *= r;
            pos.y += dy;
            line.SetSize(pos, new Vector2(w, DockPanel.LineWidth));
            line.SetLineStart(Left);
            line.SetLineEnd(Right);
            return line;
        }
        DockPanelLine AddVerticalLine(float r)
        {
            var m = HGUIManager.GameBuffer.Clone(layout.LineMod).GetComponent<UIElement>();
            float ex = Top.model.transform.localPosition.y;
            float sx = Down.model.transform.localPosition.y;
            float w = ex - sx;
            if (w < 0)
                w = -w;
            DockPanelLine line = new DockPanelLine(layout, m, Direction.Vertical);
            var pos = model.transform.localPosition;
            float dx = Left.model.transform.localPosition.x;
            pos.x = Right.model.transform.localPosition.x - dx;
            pos.x *= r;
            pos.x += dx;
            line.SetSize(pos, new Vector2(DockPanel.LineWidth, w));
            line.SetLineStart(Down);
            line.SetLineEnd(Top);
            return line;
        }
        DockpanelArea AddLeftArea(float r)
        {
            DockpanelArea area = new DockpanelArea(layout);
            layout.areas.Add(area);
            var line = AddVerticalLine(r);
            area.SetLeftLine(Left);
            area.SetRightLine(line);
            area.SetTopLine(Top);
            area.SetDownLine(Down);
            SetLeftLine(line);
            //UIElement.ResizeChild(model);
            return area;
        }
        DockpanelArea AddRightArea(float r)
        {
            DockpanelArea area = new DockpanelArea(layout);
            layout.areas.Add(area);
            var line = AddVerticalLine(1 - r);
            area.SetLeftLine(line);
            area.SetRightLine(Right);
            area.SetTopLine(Top);
            area.SetDownLine(Down);
            SetRightLine(line);
            //UIElement.ResizeChild(model);
            return area;
        }
        DockpanelArea AddTopArea(float r)
        {
            DockpanelArea area = new DockpanelArea(layout);
            layout.areas.Add(area);
            var line = AddHorizontalLine(1 - r);
            area.SetLeftLine(Left);
            area.SetRightLine(Right);
            area.SetTopLine(Top);
            area.SetDownLine(line);
            SetTopLine(line);
            //UIElement.ResizeChild(model);
            return area;
        }
        DockpanelArea AddDownArea(float r)
        {
            DockpanelArea area = new DockpanelArea(layout);
            layout.areas.Add(area);
            var line = AddHorizontalLine(r);
            area.SetLeftLine(Left);
            area.SetRightLine(Right);
            area.SetTopLine(line);
            area.SetDownLine(Down);
            SetDownLine(line);
            //UIElement.ResizeChild(model);
            return area;
        }
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
    }
    public class DockPanel : Composite
    {
        public static float LineWidth = 8f;
        public List<DockPanelLine> lines = new List<DockPanelLine>();
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
        public FakeStruct LineMod;
        public FakeStruct AreaMod;
        public Transform LineLevel;
        public Transform AreaLevel;
  
        public DockpanelArea MainArea { get; private set; }
        public override void Initial(FakeStruct fake,UIElement element)
        {
            base.Initial(fake,element);
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
            areas.Add(area);
            MainArea = area;
            area.SizeChanged();
        }
        void SizeChanged(UIElement mod)
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
        }
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
    }
}
