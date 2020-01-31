using huqiang.UIEvent;
using huqiang.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using huqiang.Core.HGUI;
using UnityEngine;

namespace huqiang.UIComposite
{
    public class DesignedDockPanel:DockPanel
    {
        public FakeStruct Auxiliary;
        public UIElement Drag;
        public List<DesignedDockAuxiliary> contents=new List<DesignedDockAuxiliary>();
        public override void Initial(FakeStruct fake,UIElement element)
        {
            contents.Clear();
            base.Initial(fake,element);
            Auxiliary = HGUIManager.FindChild(fake, "Auxiliary");
            var trans = element.transform;
            Drag = trans.Find("Drag").GetComponent<UIElement>();
            Drag.gameObject.SetActive(false);
            MainContent = new DesignedDockAuxiliary(this);
            MainContent.Initial(MainArea, trans.Find("Auxiliary").GetComponent<UIElement>());
            contents.Add(MainContent);
            MainContent.SetParent(MainArea);
        }
        public void ShowAllDocker()
        {
            for (int i = 0; i < contents.Count; i++)
                contents[i].ShowDocker();
        }
        public void HideAllDocker()
        {
            for (int i = 0; i < contents.Count; i++)
                contents[i].HideDocker();
            Drag.gameObject.SetActive(false);
        }
        public void Draging(UserAction action)
        {
            Drag.transform.localPosition = UIElement.ScreenToLocal(Drag.transform.parent, action.CanPosition);
            Drag.gameObject.SetActive(true);
        }
        public void DragEnd(UserAction action)
        {
            Drag.gameObject.SetActive(false);
        }
        public DesignedDockAuxiliary.ItemContent DragContent;
        public DesignedDockAuxiliary DragAuxiliary;
        public DesignedDockAuxiliary MainContent;
    }
    public class DesignedDockAuxiliary
    {
        public class ItemContent : TabControl.TableContent
        {
            public UIElement Close;
            public PopWindow window;
            public void LoadPopWindow<T>() where T : PopWindow, new()
            {
                if (window != null)
                    window.Dispose();
                var t = new T();
                t.Initial(Content.transform, null);
                t.ReSize();
                window = t;
            }
        }
        public DockpanelArea dockArea;
        public UIElement model;
        public Transform docker;
        public UIElement tab;
        public TabControl control;
        public UIElement Cover;
        DesignedDockPanel layout;
        public DesignedDockAuxiliary(DesignedDockPanel panel)
        {
            layout = panel;
        }
        public void Initial(DockpanelArea area, UIElement mod)
        {
            dockArea = area;
            model = mod;
            var trans = mod.transform;
            docker = trans.Find("Docker");
            tab = trans.Find("TabControl").GetComponent<UIElement>();
            Cover = trans.Find("Cover").GetComponent<UIElement>();
            control = tab.composite as TabControl;
            Cover.gameObject.SetActive(false);
            docker.gameObject.SetActive(false);
            InitialDocker();
        }
        public void SetParent(DockpanelArea area)
        {
            dockArea = area;
            model.transform.SetParent(area.model.transform);
        }
        void InitialDocker()
        {
            var trans = docker.transform;
            var mod = trans.Find("Center");
            var eve = mod.GetComponent<UIElement>().RegEvent<UserEvent>();
            eve.PointerUp = CenterPointUp;
            eve.PointerEntry = CenterPointEntry;
            eve.PointerLeave = PointLeave;

            mod = trans.Find("Left");
            eve = mod.GetComponent<UIElement>().RegEvent<UserEvent>();
            eve.PointerUp = PointUp;
            eve.PointerEntry = LeftPointEntry;
            eve.PointerLeave = PointLeave;
            eve.DataContext = DockpanelArea.Dock.Left;

            mod = trans.Find("Top");
            eve = mod.GetComponent<UIElement>().RegEvent<UserEvent>();
            eve.PointerUp = PointUp;
            eve.PointerEntry = TopPointEntry;
            eve.PointerLeave = PointLeave;
            eve.DataContext = DockpanelArea.Dock.Top;

            mod = trans.Find("Right");
            eve = mod.GetComponent<UIElement>().RegEvent<UserEvent>();
            eve.PointerUp = PointUp;
            eve.PointerEntry = RightPointEntry;
            eve.PointerLeave = PointLeave;
            eve.DataContext = DockpanelArea.Dock.Right;

            mod = trans.Find("Down");
            eve = mod.GetComponent<UIElement>().RegEvent<UserEvent>();
            eve.PointerUp = PointUp;
            eve.PointerEntry = DownPointEntry;
            eve.PointerLeave = PointLeave;
            eve.DataContext = DockpanelArea.Dock.Down;
        }
        void CenterPointEntry(UserEvent callBack, UserAction action)
        {
            Cover.transform.localPosition = Vector3.zero;
            Cover.SizeDelta = model.SizeDelta;
            Cover.gameObject.SetActive(true);
        }
        void CenterPointUp(UserEvent callBack, UserAction action)
        {
            Cover.gameObject.SetActive(false);
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
            Cover.gameObject.SetActive(false);
        }
        void PointUp(UserEvent callBack, UserAction action)
        {
            Cover.gameObject.SetActive(false);
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
            Cover.transform.localPosition = new Vector3(size.x * -0.25f, 0, 0);
            Cover.SizeDelta = new Vector2(size.x * 0.5f, size.y);
            Cover.gameObject.SetActive(true);
        }
        void TopPointEntry(UserEvent callBack, UserAction action)
        {
            var size = model.SizeDelta;
            Cover.transform.localPosition = new Vector3(0, size.y * 0.25f, 0);
            Cover.SizeDelta = new Vector2(size.x, size.y * 0.5f);
            Cover.gameObject.SetActive(true);
        }
        void RightPointEntry(UserEvent callBack, UserAction action)
        {
            var size = model.SizeDelta;
            Cover.transform.localPosition = new Vector3(size.x * 0.25f, 0, 0);
            Cover.SizeDelta = new Vector2(size.x * 0.5f, size.y);
            Cover.gameObject.SetActive(true);
        }
        void DownPointEntry(UserEvent callBack, UserAction action)
        {
            var size = model.SizeDelta;
            Cover.transform.localPosition = new Vector3(0, size.y * -0.25f, 0);
            Cover.SizeDelta = new Vector2(size.x, size.y * 0.5f);
            Cover.gameObject.SetActive(true);
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
        public ItemContent AddContent(string name)
        {
            var item = HGUIManager.GameBuffer.Clone(control.Item).GetComponent<UIElement>();
            ItemContent con = new ItemContent();
            con.Parent = control;
            con.Item = item;
            item.RegEvent<UserEvent>();
            item.userEvent.PointerDown = HeadPointDown;
            item.userEvent.Drag = HeadDrag;
            item.userEvent.DragEnd = HeadDragEnd;
            item.userEvent.DataContext = con;

            //var t = UICreator.CreateElement(Vector3.zero,Vector2.zero, name);
            //t.marginType = MarginType.Margin;
            con.Content = control.Content;
            //con.Back = item.Find("Back");

            //con.Label = item.Find("Label");
            //var txt = con.Label.GetComponent<HText>();
            //txt.text = name;
            //txt.AsyncGetTextSizeX((o,e)=> {
            //    o.model.data.sizeDelta = e;
            //    OrderHeadLabel(con);
            //});
            //con.Close = item.Find("Close");
            if (con.Close != null)
            {
                con.Close.RegEvent<UserEvent>();
                con.Close.userEvent.Click = CloseClick;
                con.Close.userEvent.DataContext = con;
            }
            control.AddContent(con);
            return con;
        }
        /// <summary>
        /// 标签页排列
        /// </summary>
        /// <param name="obj"></param>
        public void OrderHeadLabel(ItemContent ic)
        {
            //var w = ic.Label.data.sizeDelta.x;
            //ic.Item.data.sizeDelta.x = w + 48;
            //ic.Back.data.sizeDelta.x = w + 48;
            //ic.Close.data.localPosition.x = w * 0.5f;
            //ic.Item.IsChanged = true;
            //ic.Back.IsChanged = true;
            //ic.Close.IsChanged = true;
            //if (control.panel != null)
            //    control.panel.IsChanged = true;
        }
        public void AddContent(ItemContent con)
        {
            var eve = con.Item.userEvent;
            eve.PointerDown = HeadPointDown;
            eve.Drag = HeadDrag;
            eve.DragEnd = HeadDragEnd;
            control.AddContent(con);
        }
        public void RemoveContent(TabControl.TableContent con)
        {
            control.RemoveContent(con);
            //con.Item.SetParent(null);
            //con.Content.SetParent(null);
            //if(control.contents.Count==0)
            //{
            //    dockArea.Dispose();
            //    layout.contents.Remove(this);
            //    layout.Refresh();
            //}
        }
        public void ShowContent(TabControl.TableContent con)
        {
            control.ShowContent(con);
        }
        public void ShowDocker()
        {
            docker.gameObject.SetActive(true);
        }
        public void HideDocker()
        {
            docker.gameObject.SetActive(false);
        }
        public DesignedDockAuxiliary AddArea(DockpanelArea.Dock dock, float r = 0.5f)
        {
            var area = dockArea.AddArea(dock,r);
            var go = HGUIManager.GameBuffer.Clone(layout.Auxiliary);
            var trans = go.transform;
            trans.SetParent(area.model.transform);
            trans.localScale = Vector3.one;
            trans.localRotation = Quaternion.identity;
            var au =go.GetComponent<UIElement>();
            var con = new DesignedDockAuxiliary(layout);
            con.Initial(area, au);
            layout.contents.Add(con);
            return con;
        }
        public void Dispose()
        {
            dockArea.Dispose();
            layout.contents.Remove(this);
            layout.Refresh();
        }
    }
}
