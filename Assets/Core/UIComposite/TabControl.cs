using huqiang.UIEvent;
using System;
using System.Collections.Generic;
using UnityEngine;
using huqiang.Core.HGUI;
using huqiang.Data;

namespace huqiang.UIComposite
{
    public class TabControl:Composite
    {
        public class TableContent
        {
            public TabControl Parent;
            public UIElement Item;
            public UserEvent eventCall;
            public HText Label;
            public HImage Back;
            public UIElement Content;
        }
        /// <summary>
        /// 选项头停靠方向
        /// </summary>
        public enum HeadDock
        {
            Top, Down
        }
        public Transform Head;
        public Transform Items;
        public Transform Content;
        public FakeStruct Item;
        public TableContent curContent;
        StackPanel stackPanel;
        /// <summary>
        /// 头部停靠位置
        /// </summary>
        public HeadDock headDock = HeadDock.Top;
        float headHigh = 0;
        public List<TableContent> contents;
        /// <summary>
        /// 当前被选中项的背景色
        /// </summary>
        public Color SelectColor = 0x2656FFff.ToColor();
        /// <summary>
        /// 鼠标停靠时的背景色
        /// </summary>
        public Color HoverColor = 0x5379FFff.ToColor();
        public override void Initial(FakeStruct mod,UIElement element)
        {
            base.Initial(mod,element);
            contents = new List<TableContent>();
            var trans = Enity.transform;
            Head = trans.Find("Head");
            Items = Head.Find("Items");
            stackPanel = Items.GetComponent<UIElement>().composite as StackPanel;
            Item= HGUIManager.FindChild(mod, "Item");
            Content = trans.Find("Content");
            HGUIManager.GameBuffer.RecycleGameObject(trans.Find("Item").gameObject);
        }
        /// <summary>
        /// 使用默认标签页
        /// </summary>
        /// <param name="model"></param>
        /// <param name="label"></param>
        public void AddContent(UIElement model, string label)
        {
            if (Item == null)
                return;
            TableContent content = new TableContent();
            content.Parent = this;
            var mod = HGUIManager.GameBuffer.Clone(Item).transform;

            content.Item = mod.GetComponent<UIElement>();
            content.Label = mod.Find("Text").GetComponent<HText>();
            content.Label.Text = label;
            content.Back = mod.Find("Image").GetComponent<HImage>();
            content.Content = model;

            var eve = content.Item.RegEvent<UserEvent>();
            eve.Click = ItemClick;
            eve.PointerEntry = ItemPointEntry;
            eve.PointerLeave = ItemPointLeave;
            content.eventCall = eve;
            content.eventCall.DataContext = content;
            if (curContent != null)
            {
                curContent.Content.gameObject.SetActive(false);
                if (curContent.Back != null)
                    curContent.Back.gameObject.SetActive(false);
            }
            model.transform.SetParent(Content);
            model.transform.localScale = Vector3.one;
            UIElement.Resize(model);
            curContent = content;
            curContent.Back.MainColor = SelectColor;
            contents.Add(curContent);
            mod.SetParent(Items);
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
            if (curContent != null)
            {
                curContent.Content.gameObject.SetActive(false);
                if (curContent.Back != null)
                    curContent.Back.gameObject.SetActive(false);
            }
            table.Item.transform.SetParent(Items);
            table.Item.userEvent.Click = ItemClick;
            table.Item.userEvent.PointerEntry = ItemPointEntry;
            table.Item.userEvent.PointerLeave = ItemPointLeave;
            table.Content.transform.SetParent(Content);
            curContent = table;
            curContent.Content.gameObject.SetActive(true);
            if (curContent.Back != null)
                curContent.Back.gameObject.SetActive(true);
            contents.Add(table);
            //if (panel != null)
            //    panel.IsChanged = true;
        }
        /// <summary>
        /// 添加外部标签页
        /// </summary>
        /// <param name="content"></param>
        /// <param name="mod"></param>
        public void AddContent(Transform content, Transform mod)
        {
            if (Head == null)
                return;
            mod.SetParent(Head);
            //panel.IsChanged = true;
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
                curContent = contents[0];
                if (curContent.Content != null)
                    curContent.Content.gameObject.SetActive(true);
                if (curContent.Back != null)
                    curContent.Back.gameObject.SetActive(true);
            }
            //panel.IsChanged = true;
        }
        /// <summary>
        /// 释放某个标签和其内容,其对象会被回收
        /// </summary>
        /// <param name="table"></param>
        public void ReleseContent(TableContent table)
        {
            contents.Remove(table);
            //table.Content.transform.SetParent(null);
            //table.Item.transform.SetParent(null);
            HGUIManager.GameBuffer.RecycleGameObject(table.Content.gameObject);
            HGUIManager.GameBuffer.RecycleGameObject(table.Item.gameObject);
        }
        public void AddTable(TableContent table)
        {
            table.eventCall.Click = ItemClick;
            table.eventCall.PointerEntry = ItemPointEntry;
            table.eventCall.PointerLeave = ItemPointLeave;
            table.Label.transform.SetParent(Head);
            table.Content.transform.SetParent(Content);
        }
        public void ShowContent(TableContent content)
        {
            if (curContent != null)
            {
                curContent.Content.gameObject.SetActive(false);
                curContent.Back.gameObject.SetActive(false);
            }
            curContent = content;
            curContent.Content.gameObject.SetActive(true);
            if (curContent.Back != null)
            {
                curContent.Back.MainColor = SelectColor;
                curContent.Back.gameObject.SetActive(true);
            }
        }
        public bool ExistContent(TableContent content)
        {
            return contents.Contains(content);
        }
        public void ItemClick(UserEvent callBack,UserAction action)
        {
            ShowContent(callBack.DataContext as TableContent);
        }
        public void ItemPointEntry(UserEvent callBack,UserAction action)
        {
            var c = callBack.DataContext as TableContent;
            if (c == curContent)
                return;
            if (c != null)
            {
                if (c.Back != null)
                {
                    c.Back.MainColor = HoverColor;
                    c.Back.gameObject.SetActive(true);
                }
            }
        }
        public void ItemPointLeave(UserEvent callBack,UserAction action)
        {
            var c = callBack.DataContext as TableContent;
            if (c == curContent)
                return;
            if (c != null)
            {
                if (c != curContent)
                    if (c.Back != null)
                    {
                        c.Back.gameObject.SetActive(false);
                    }
            }
        }
    }
}