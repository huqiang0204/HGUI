using huqiang.UIEvent;
using System;
using System.Collections.Generic;
using UnityEngine;
using huqiang.Core.HGUI;
using huqiang.Data;

namespace huqiang.UIComposite
{
    /// <summary>
    /// 表控制器
    /// </summary>
    public class TabControl:Composite
    {
        /// <summary>
        /// 表内容
        /// </summary>
        public class TableContent
        {
            /// <summary>
            /// 父控制器
            /// </summary>
            public TabControl Parent;
            /// <summary>
            /// 项目主体
            /// </summary>
            public UIElement Item;
            /// <summary>
            /// 主体事件
            /// </summary>
            public UserEvent eventCall;
            /// <summary>
            /// 标签
            /// </summary>
            public HText Label;
            /// <summary>
            /// 背景
            /// </summary>
            public HImage Back;
            /// <summary>
            /// 绑定内容
            /// </summary>
            public UIElement Content;
            /// <summary>
            /// 联系上下文
            /// </summary>
            public object Context;
        }
        /// <summary>
        /// 选项头停靠方向
        /// </summary>
        public enum HeadDock
        {
            Top, Down
        }
        /// <summary>
        /// 标头
        /// </summary>
        public UIElement Head;
        /// <summary>
        /// 所有项目父坐标变换
        /// </summary>
        public UIElement Items;
        /// <summary>
        /// 所有项目的父元素
        /// </summary>
        public UIElement Content;
        /// <summary>
        /// 项目模型
        /// </summary>
        public FakeStruct Item;
        /// <summary>
        /// 当前激活状态的表内容
        /// </summary>
        public TableContent CurContent;
        /// <summary>
        /// 激活的内容被改变事件
        /// </summary>
        public Action<TabControl> SelectChanged;
        StackPanel stackPanel;
        HeadDock dock;
        /// <summary>
        /// 头部停靠位置
        /// </summary>
        public HeadDock headDock { 
            get=>dock; 
            set {
                if (value == dock)
                    return;
                dock = value;
                switch(dock)
                {
                    case HeadDock.Top:
                        Head.anchorPointType = AnchorPointType.Top;
                        Content.margin.top = Head.SizeDelta.y;
                        Content.margin.down = 0;
                        break;
                    case HeadDock.Down:
                        Head.anchorPointType = AnchorPointType.Down;
                        Content.margin.down = Head.SizeDelta.y;
                        Content.margin.top = 0;
                        break;
                }
                UIElement.ResizeChild(Enity);
            } }
        /// <summary>
        /// 标头高度
        /// </summary>
        public float headHigh = 0;
        /// <summary>
        /// 所有内容列表
        /// </summary>
        public List<TableContent> contents;
        /// <summary>
        /// 当前被选中项的背景色
        /// </summary>
        public Color SelectColor = 0x2656FFff.ToColor();
        /// <summary>
        /// 鼠标停靠时的背景色
        /// </summary>
        public Color HoverColor = 0x5379FFff.ToColor();
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="mod">数据模型</param>
        /// <param name="element">元素主体</param>
        public override void Initial(FakeStruct mod,UIElement element,UIInitializer initializer)
        {
            base.Initial(mod,element,initializer);
            contents = new List<TableContent>();
            Head = Enity.Find("Head");
            Items = Head.Find("Items");
            stackPanel = Items.composite as StackPanel;
            Item= HGUIManager.FindChild(mod, "Item");
            Content = Enity.Find("Content");
            HGUIManager.RecycleUI(Enity.Find("Item"));
        }
        /// <summary>
        /// 使用默认标签页
        /// </summary>
        /// <param name="model"></param>
        /// <param name="label"></param>
        public TableContent AddContent(UIElement model, string label)
        {
            if (Item == null)
                return null;
            TableContent content = new TableContent();
            content.Parent = this;
            var mod = HGUIManager.Clone(Item);

            content.Item = mod;
            content.Label = mod.Find("Text")as HText;
            content.Label.Text = label;
            content.Back = mod.Find("Image")as HImage;
            content.Content = model;

            var eve = content.Item.RegEvent<UserEvent>();
            eve.Click = ItemClick;
            eve.PointerEntry = ItemPointEntry;
            eve.PointerLeave = ItemPointLeave;
            content.eventCall = eve;
            content.eventCall.DataContext = content;
            if (CurContent != null)
            {
                CurContent.Content.activeSelf = false;
                if (CurContent.Back != null)
                    CurContent.Back.activeSelf = false;
            }
            model.SetParent(Content);
            model.localScale = Vector3.one;
            UIElement.Resize(model);
            CurContent = content;
            CurContent.Back.MainColor = SelectColor;
            contents.Add(CurContent);
            mod.SetParent(Items);
            return content;
        }
        /// <summary>
        /// 使用自定义标签页,标签模型自行管理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="content"></param>
        /// <param name="dat"></param>
        /// <param name="callback"></param>
        public void AddContent(TableContent table)
        {
            if (CurContent != null)
            {
                CurContent.Content.activeSelf = false;
                if (CurContent.Back != null)
                    CurContent.Back.activeSelf = false;
            }
            table.Item.SetParent(Items);
            table.Item.userEvent.Click = ItemClick;
            table.Item.userEvent.PointerEntry = ItemPointEntry;
            table.Item.userEvent.PointerLeave = ItemPointLeave;
            table.Content.SetParent(Content);
            CurContent = table;
            CurContent.Content.activeSelf = true;
            if (CurContent.Back != null)
                CurContent.Back.activeSelf = true;
            contents.Add(table);
        }
        /// <summary>
        /// 移除某个标签和其内容
        /// </summary>
        /// <param name="table"></param>
        public void RemoveContent(TableContent table)
        {
            contents.Remove(table);
            if(contents.Count>0)
            {
                CurContent = contents[0];
                if (CurContent.Content != null)
                    CurContent.Content.activeSelf = true;
                if (CurContent.Back != null)
                    CurContent.Back.activeSelf = true;
            }
        }
        /// <summary>
        /// 释放某个标签和其内容,其对象会被回收
        /// </summary>
        /// <param name="table"></param>
        public void ReleseContent(TableContent table)
        {
            contents.Remove(table);
            if (table == CurContent)
                CurContent = null;
            HGUIManager.RecycleUI(table.Content);
            HGUIManager.RecycleUI(table.Item);
        }
        /// <summary>
        /// 添加一张表
        /// </summary>
        /// <param name="table">实例</param>
        public void AddTable(TableContent table)
        {
            table.eventCall.Click = ItemClick;
            table.eventCall.PointerEntry = ItemPointEntry;
            table.eventCall.PointerLeave = ItemPointLeave;
            table.Label.SetParent(Head);
            table.Content.SetParent(Content);
        }
        /// <summary>
        /// 显示某张表
        /// </summary>
        /// <param name="content">实例</param>
        public void ShowContent(TableContent content)
        {
            if (CurContent != null)
            {
                if (CurContent.Content != null)
                    CurContent.Content.activeSelf = false;
                if (CurContent.Back != null)
                    CurContent.Back.activeSelf = false;
            }
            CurContent = content;
            CurContent.Content.activeSelf = true;
            if (CurContent.Back != null)
            {
                CurContent.Back.MainColor = SelectColor;
                CurContent.Back.activeSelf = true;
            }
        }
        /// <summary>
        /// 是否存在此表
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public bool ExistContent(TableContent content)
        {
            return contents.Contains(content);
        }
        /// <summary>
        /// 标头被单击的默认处理函数
        /// </summary>
        /// <param name="callBack"></param>
        /// <param name="action"></param>
        public void ItemClick(UserEvent callBack,UserAction action)
        {
            var con = callBack.DataContext as TableContent;
            var cur = CurContent;
            ShowContent(con);
            if (con != cur)
                if (SelectChanged != null)
                    SelectChanged(this);
        }
        /// <summary>
        /// 标头项鼠标进入默认处理函数
        /// </summary>
        /// <param name="callBack"></param>
        /// <param name="action"></param>
        public void ItemPointEntry(UserEvent callBack,UserAction action)
        {
            var c = callBack.DataContext as TableContent;
            if (c == CurContent)
                return;
            if (c != null)
            {
                if (c.Back != null)
                {
                    c.Back.MainColor = HoverColor;
                    c.Back.activeSelf = true;
                }
            }
        }
        /// <summary>
        /// 标头项鼠标离开默认处理函数
        /// </summary>
        /// <param name="callBack"></param>
        /// <param name="action"></param>
        public void ItemPointLeave(UserEvent callBack,UserAction action)
        {
            var c = callBack.DataContext as TableContent;
            if (c == CurContent)
                return;
            if (c != null)
            {
                if (c != CurContent)
                    if (c.Back != null)
                    {
                        c.Back.activeSelf = false;
                    }
            }
        }
    }
}