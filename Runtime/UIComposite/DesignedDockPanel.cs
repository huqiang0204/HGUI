using huqiang.UIEvent;
using huqiang.Data;
using System;
using System.Collections.Generic;
using huqiang.Core.HGUI;
using UnityEngine;
using huqiang.UIModel;

namespace huqiang.UIComposite
{
    /// <summary>
    /// 可设计停靠面板
    /// </summary>
    public class DesignedDockPanel:DockPanel
    {
        /// <summary>
        /// 辅助类数据模型
        /// </summary>
        public FakeStruct Auxiliary;
        /// <summary>
        /// 可拖放的UI元素实体
        /// </summary>
        public UIElement Drag;
        /// <summary>
        /// 辅助停靠内容列表
        /// </summary>
        public List<DesignedDockAuxiliary> contents=new List<DesignedDockAuxiliary>();
        /// <summary>
        /// 初始化函数
        /// </summary>
        /// <param name="fake">数据模型</param>
        /// <param name="element">UI主体元素</param>
        public override void Initial(FakeStruct fake,UIElement element,UIInitializer initializer)
        {
            contents.Clear();
            base.Initial(fake,element,initializer);
            Auxiliary = HGUIManager.FindChild(fake, "Auxiliary");
            Drag = element.Find("Drag");
            Drag.activeSelf = false;
            MainContent = new DesignedDockAuxiliary(this);
            MainContent.Initial(MainArea, element.Find("Auxiliary"));
            contents.Add(MainContent);
            MainContent.SetParent(MainArea);
        }
        /// <summary>
        /// 显示所有可停靠区域
        /// </summary>
        public void ShowAllDocker()
        {
            for (int i = 0; i < contents.Count; i++)
                contents[i].ShowDocker();
        }
        /// <summary>
        /// 隐藏所有可停靠区域
        /// </summary>
        public void HideAllDocker()
        {
            for (int i = 0; i < contents.Count; i++)
                contents[i].HideDocker();
            Drag.activeSelf = false;
        }
        /// <summary>
        /// 拖放中
        /// </summary>
        /// <param name="action"></param>
        public void Draging(UserAction action)
        {
            Drag.localPosition = UIElement.ScreenToLocal(Drag.parent, action.CanPosition);
            Drag.activeSelf = true;
        }
        /// <summary>
        /// 拖放完毕
        /// </summary>
        /// <param name="action"></param>
        public void DragEnd(UserAction action)
        {
            Drag.activeSelf = false;
        }
        /// <summary>
        /// 当前拖放的内容
        /// </summary>
        public DesignedDockAuxiliary.ItemContent DragContent;
        /// <summary>
        /// 当前拖放的停靠辅助实体
        /// </summary>
        public DesignedDockAuxiliary DragAuxiliary;
        /// <summary>
        /// 当前拖放的停靠辅助实体的主体内容
        /// </summary>
        public DesignedDockAuxiliary MainContent;
    }
    /// <summary>
    /// 停靠辅助实体
    /// </summary>
    public class DesignedDockAuxiliary
    {
        /// <summary>
        /// 项目内容
        /// </summary>
        public class ItemContent : TabControl.TableContent
        {
            /// <summary>
            /// 窗口关闭按钮
            /// </summary>
            public UIElement Close;
            /// <summary>
            /// 窗口实体
            /// </summary>
            public PopWindow window;
            /// <summary>
            /// 载入一个窗口
            /// </summary>
            /// <typeparam name="T"></typeparam>
            public void LoadPopWindow<T>() where T : PopWindow, new()
            {
                if (window != null)
                    window.Dispose();
                var t = new T();
                t.Initial(Content, null);
                t.ReSize();
                window = t;
            }
        }
        /// <summary>
        /// 停靠的区域
        /// </summary>
        public DockpanelArea dockArea;
        /// <summary>
        /// 辅助区域模型
        /// </summary>
        public UIElement model;
        /// <summary>
        /// 停靠的坐标变换
        /// </summary>
        public UIElement docker;
        /// <summary>
        /// 表主体元素
        /// </summary>
        public UIElement tab;
        /// <summary>
        /// 控制表
        /// </summary>
        public TabControl control;
        /// <summary>
        /// 停靠区域显示元素
        /// </summary>
        public UIElement Cover;
        DesignedDockPanel layout;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="panel">可设计停靠面板载体</param>
        public DesignedDockAuxiliary(DesignedDockPanel panel)
        {
            layout = panel;
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="area">停靠面板区域实例</param>
        /// <param name="mod">停靠辅助区域模板</param>
        public void Initial(DockpanelArea area, UIElement mod)
        {
            dockArea = area;
            model = mod;
            docker = mod.Find("Docker");
            tab = mod.Find("TabControl");
            Cover = mod.Find("Cover");
            control = tab.composite as TabControl;
            Cover.activeSelf = false;
            docker.activeSelf = false;
            InitialDocker();
        }
        /// <summary>
        /// 附属于某个停靠区域
        /// </summary>
        /// <param name="area">停靠区域</param>
        public void SetParent(DockpanelArea area)
        {
            dockArea = area;
            model.SetParent(area.model);
        }
        void InitialDocker()
        {
            var mod = docker.Find("Center");
            var eve = mod.RegEvent<UserEvent>();
            eve.PointerUp = CenterPointUp;
            eve.PointerEntry = CenterPointEntry;
            eve.PointerLeave = PointLeave;

            mod = docker.Find("Left");
            eve = mod.RegEvent<UserEvent>();
            eve.PointerUp = PointUp;
            eve.PointerEntry = LeftPointEntry;
            eve.PointerLeave = PointLeave;
            eve.DataContext = DockpanelArea.Dock.Left;

            mod = docker.Find("Top");
            eve = mod.RegEvent<UserEvent>();
            eve.PointerUp = PointUp;
            eve.PointerEntry = TopPointEntry;
            eve.PointerLeave = PointLeave;
            eve.DataContext = DockpanelArea.Dock.Top;

            mod = docker.Find("Right");
            eve = mod.RegEvent<UserEvent>();
            eve.PointerUp = PointUp;
            eve.PointerEntry = RightPointEntry;
            eve.PointerLeave = PointLeave;
            eve.DataContext = DockpanelArea.Dock.Right;

            mod = docker.Find("Down");
            eve = mod.RegEvent<UserEvent>();
            eve.PointerUp = PointUp;
            eve.PointerEntry = DownPointEntry;
            eve.PointerLeave = PointLeave;
            eve.DataContext = DockpanelArea.Dock.Down;
        }
        void CenterPointEntry(UserEvent callBack, UserAction action)
        {
            Cover.localPosition = Vector3.zero;
            Cover.SizeDelta = model.SizeDelta;
            Cover.activeSelf = true;
        }
        void CenterPointUp(UserEvent callBack, UserAction action)
        {
            Cover.activeSelf = false;
            if (control.ExistContent(layout.DragContent))
                return;
            layout.DragAuxiliary.RemoveContent(layout.DragContent);
            AddContent(layout.DragContent);
            layout.HideAllDocker();
            layout.Refresh();
            UIElement.Resize(layout.DragContent.Content);
        }
        void PointLeave(UserEvent callBack, UserAction action)
        {
            Cover.activeSelf = false;
        }
        void PointUp(UserEvent callBack, UserAction action)
        {
            Cover.activeSelf = false;
            if (layout.DragAuxiliary == this)
            {
                if (control.contents.Count < 2)
                {
                    return;
                }
            }
            layout.DragAuxiliary.RemoveContent(layout.DragContent);
            var area = AddArea((DockpanelArea.Dock)callBack.DataContext);
            area.AddContent(layout.DragContent);
            area.dockArea.SizeChanged();
            layout.HideAllDocker();
            layout.Refresh();
            UIElement.Resize(layout.DragContent.Content);
        }
        void LeftPointEntry(UserEvent callBack, UserAction action)
        {
            var size = model.SizeDelta;
            Cover.localPosition = new Vector3(size.x * -0.25f, 0, 0);
            Cover.SizeDelta = new Vector2(size.x * 0.5f, size.y);
            Cover.activeSelf = true;
        }
        void TopPointEntry(UserEvent callBack, UserAction action)
        {
            var size = model.SizeDelta;
            Cover.localPosition = new Vector3(0, size.y * 0.25f, 0);
            Cover.SizeDelta = new Vector2(size.x, size.y * 0.5f);
            Cover.activeSelf = true;
        }
        void RightPointEntry(UserEvent callBack, UserAction action)
        {
            var size = model.SizeDelta;
            Cover.localPosition = new Vector3(size.x * 0.25f, 0, 0);
            Cover.SizeDelta = new Vector2(size.x * 0.5f, size.y);
            Cover.activeSelf = true;
        }
        void DownPointEntry(UserEvent callBack, UserAction action)
        {
            var size = model.SizeDelta;
            Cover.localPosition = new Vector3(0, size.y * -0.25f, 0);
            Cover.SizeDelta = new Vector2(size.x, size.y * 0.5f);
            Cover.activeSelf = true;
        }
        int ac;
        void HeadPointDown(UserEvent eventCall, UserAction action)
        {
            ac = 0;
        }
        void HeadDrag(UserEvent eventCall, UserAction action, Vector2 v)
        {
            if (!layout.LockLayout)
            {
                if (ac == 0)
                {
                    float y = action.CanPosition.y - eventCall.RawPosition.y;
                    if (y < -30 | y > 30)
                    {
                        layout.ShowAllDocker();
                        ac = 2;
                        layout.DragAuxiliary = this;
                        layout.DragContent = eventCall.DataContext as ItemContent;
                    }
                }
                else if (ac == 2)
                {
                    layout.Draging(action);
                }
            }
        }
        void HeadDragEnd(UserEvent eventCall, UserAction action, Vector2 v)
        {
            if (!layout.LockLayout)
            {
                layout.HideAllDocker();
                layout.DragEnd(action);
            }
        }
        void CloseClick(UserEvent eventCall, UserAction action)
        {
            ItemContent con = eventCall.DataContext as ItemContent;
            if(con!=null)
            {
                if (con.window != null)
                    con.window.Dispose();
                control.ReleseContent(con);
                if (control.contents.Count == 0)
                {
                    if (layout.contents.Count > 1)
                        Dispose();
                    else layout.MainContent = this;
                }
            }
        }
        /// <summary>
        /// 添加一个内容标签,将使用默认模板
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns></returns>
        public ItemContent AddContent(string name)
        {
            var item = HGUIManager.Clone(control.Item);
            ItemContent con = new ItemContent();
            con.Parent = control;
            con.Item = item;
            item.RegEvent<UserEvent>();
            item.userEvent.PointerDown = HeadPointDown;
            item.userEvent.Drag = HeadDrag;
            item.userEvent.DragEnd = HeadDragEnd;
            item.userEvent.DataContext = con;

            var content = UICreator.CreateElement(Vector3.zero,Vector2.zero,"Content",control.Content);
            content.marginType = MarginType.Margin;

            con.Content = content;
            con.Back = item.Find("Image")as HImage;

            con.Label = item.Find("Text") as HText;
            var txt = con.Label;
            txt.Text = name;
            Vector2 v = txt.SizeDelta;
            txt.GetPreferredWidth(ref v, name);
            con.Back.SizeDelta = item.SizeDelta = new Vector2(v.x + 40, v.y);
            
            var clo = item.Find("Close");
            if (clo != null)
            {
                con.Close = clo;
                con.Close.RegEvent<UserEvent>();
                con.Close.userEvent.Click = CloseClick;
                con.Close.userEvent.DataContext = con;
                clo.localPosition = new Vector3(v.x * 0.5f + 8, 0, 0);
            }
            control.AddContent(con);
            return con;
        }
        /// <summary>
        /// 添加一个内容
        /// </summary>
        /// <param name="con"></param>
        public void AddContent(ItemContent con)
        {
            var eve = con.Item.userEvent;
            eve.PointerDown = HeadPointDown;
            eve.Drag = HeadDrag;
            eve.DragEnd = HeadDragEnd;
            control.AddContent(con);
        }
        /// <summary>
        /// 移除内容
        /// </summary>
        /// <param name="con">内容实例</param>
        public void RemoveContent(TabControl.TableContent con)
        {
            control.RemoveContent(con);
            con.Item.SetParent(null);
            con.Content.SetParent(null);
            if (control.contents.Count == 0)
            {
                dockArea.Dispose();
                layout.contents.Remove(this);
                layout.Refresh();
            }
        }
        /// <summary>
        /// 展示内容
        /// </summary>
        /// <param name="con"></param>
        public void ShowContent(TabControl.TableContent con)
        {
            control.ShowContent(con);
        }
        /// <summary>
        /// 显示可停靠区域
        /// </summary>
        public void ShowDocker()
        {
            docker.activeSelf = true;
        }
        /// <summary>
        /// 隐藏可停靠区域
        /// </summary>
        public void HideDocker()
        {
            docker.activeSelf = false;
        }
        /// <summary>
        /// 添加可停靠区域
        /// </summary>
        /// <param name="dock">区域停靠方位</param>
        /// <param name="r">占用大小</param>
        /// <returns></returns>
        public DesignedDockAuxiliary AddArea(DockpanelArea.Dock dock, float r = 0.5f)
        {
            var area = dockArea.AddAreaR(dock,r);
            var go = HGUIManager.Clone(layout.Auxiliary);
            go.SetParent(area.model);
            go.localScale = Vector3.one;
            go.localRotation = Quaternion.identity;
            var con = new DesignedDockAuxiliary(layout);
            con.Initial(area, go);
            layout.contents.Add(con);
            return con;
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            dockArea.Dispose();
            layout.contents.Remove(this);
            layout.Refresh();
        }
    }
}
