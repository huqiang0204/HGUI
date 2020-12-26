using huqiang.Core.HGUI;
using System;
using UnityEngine;
using huqiang.UIComposite;
using huqiang.UIEvent;
using huqiang;
using huqiang.Data;
using System.Collections.Generic;
using Assets.Scripts;
using huqiang.UIModel;

public class TreeViewPage:UIPage
{
    class TreeViewItemEx : TreeViewItem
    {
        public UserEvent visble;
    }
    class TreeViewNodeEx : TreeViewNode
    {
        public bool active;
        public GameObject game;
    }
    //反射UI界面上的物体
    class View
    {
        public UserEvent back;
        public TreeView tree;
        public UserEvent last;
        public UserEvent next;
    }
    View view;
    GameObject TreeModel;
    public override void Initial(UIElement parent, object dat = null)
    {
        base.Initial(parent, dat);
        view = LoadUI<View>("baseUI", "treeview");//"baseUI"创建的bytes文件名,"page"为创建的页面名
        InitalTreeView();
        view.back.Click = RayCast;
        view.last.Click = (o, e) => { LoadPage<LayoutPage>(); };
        view.next.Click = (o, e) => { LoadPage<DataGridPage>(); };
    }
    void InitalTreeView()
    {
        TreeModel = GameObject.Find("TreeModel");
        var trans = TreeModel.transform;
        var root = trans.GetChild(0);
        root.gameObject.SetActive(true);
        view.tree.Root = CreateNodeChild(root);
        view.tree.SetItemUpdate<TreeViewItemEx, TreeViewNodeEx>(TreeItemUpdate);
        view.tree.SelectChanged = SelectChanged;
        view.tree.Refresh();
    }
    void TreeItemUpdate(TreeViewItemEx item, TreeViewNodeEx node)
    {
        item.Item.DataContext = item;
        item.Item.AutoColor = false;
        item.Item.Click = view.tree.DefultItemClick;
        item.Text.Text = node.content;
        item.visble.DataContext = item;
        item.visble.Click = VisbleClick;
        item.visble.AutoColor = false;
        if (node.active)
        {
            item.visble.Context.MainColor = 0xFFF41FFF.ToColor();
        }
        else
        {
            item.visble.Context.MainColor = Color.gray;
        }
        if (node == view.tree.SelectNode)
        {
            item.Item.Context.MainColor = new Color32(128,164,255,255);
        }
        else
        {
            item.Item.Context.MainColor = new Color32(0, 0, 0, 0);
        }
    }
    void SelectChanged(TreeView tv, TreeViewItem item)
    {
        var items = tv.swap;
        int len = items.Length;
        for (int i = 0; i < len; i++)
        {
            var it = items[i];
            it.Item.Context.MainColor = new Color32(0, 0, 0, 0);
        }
        item.Item.Context.MainColor = 0x4B75FFff.ToColor();
        var node = tv.SelectNode;
        if (node != null)
        {
           
        }
    }
    void VisbleClick(UserEvent callBack, UserAction action)
    {
        var item = callBack.DataContext as TreeViewItemEx;
        var node = item.node as TreeViewNodeEx;
        node.active = !node.active;
        if (node.active)
        {
            item.visble.Context.MainColor = 0xFFF41FFF.ToColor();
            node.game.SetActive(true);
        }
        else
        {
            item.visble.Context.MainColor = Color.gray;
            node.game.SetActive(false);
        }
    }
    public override void Dispose()
    {
        base.Dispose();
        var root = TreeModel.transform.GetChild(0);
        root.gameObject.SetActive(false);
    }
    TreeViewNodeEx CreateNodeChild(Transform part)
    {
        TreeViewNodeEx node = new TreeViewNodeEx();
        node.context = part;
        node.content = part.name;
        node.game = part.gameObject;
        node.active = part.gameObject.activeSelf;
        int c = part.childCount;
        for (int i = 0; i < c; i++)
            node.Add(CreateNodeChild(part.GetChild(i)));
        return node;
    }
    void RayCast(UserEvent user,UserAction action)
    {
        var ray = Camera.main.ScreenPointToRay(action.Position);
        RaycastHit hit;
        if (UnityEngine.Physics.Raycast(ray, out hit, 10000))
        {
            var level = FindLevel(hit.transform,TreeModel.transform.GetChild(0));
            int l = level.Length - 1;
            int[] buf = new int[l];
            for (int i = 0; i < l; i++)
                buf[i] = level[i + 1];
            var nod = view.tree.Root.Find(buf);
            if (nod != null)
            {
                nod.Expand();
                view.tree.SelectNode = nod;
                view.tree.Refresh();
            }
        }
    }
    static int[] FindLevel(Transform transform, Transform root)
    {
        List<int> list = new List<int>();
        for (int i = 0; i < 256; i++)
        {
            list.Add(transform.GetSiblingIndex());
            if (transform == root)
            {
                break;
            }
            transform = transform.parent;
        }
        var arry = list.ToArray();
        int len = arry.Length;
        int c = len / 2;
        len--;
        for (int i = 0; i < c; i++)
        {
            int a = arry[i];
            arry[i] = arry[len];
            arry[len] = a;
            len--;
        }
        return arry;
    }
}
