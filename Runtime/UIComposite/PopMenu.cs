using huqiang.Core.HGUI;
using huqiang.Core.UIData;
using huqiang.Data;
using huqiang.UIEvent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace huqiang.UIComposite
{
    public class MenuContext
    {
        public MenuContext Parent { get; private set; }
        public void SetParent(MenuContext menu)
        {
            if (Parent != null)
                Parent.Child.Remove(this);
            Parent = menu;
            if (menu != null)
                menu.Child.Add(this);
        }
        public string Key;
        public string Content;
        public object Context;
        internal List<MenuContext> Child = new List<MenuContext>();
    }
    public class MenuItem
    {
        public UIElement Item;
        public MenuContext Context;
        public int Index;
        public UserEvent Image;
        public HText Text;
        public UIElement Expand;
        [NonSerialized]
        public int dir;
        [NonSerialized]
        public List<MenuItem> Child=new List<MenuItem>();
        [NonSerialized]
        internal MenuItem CurItem;
        [NonSerialized]
        public UIElement Next;
        public MenuItem Parent { get; private set; }
        public void SetParent(MenuItem menu)
        {
            if (Parent != null)
                Parent.Child.Remove(this);
            Parent = menu;
            if (menu != null)
                menu.Child.Add(this);
        }
    }
    public class PopMenu:Composite
    {
        FakeStruct ItemMod;
        FakeStruct ContentMod;
        Constructor creator;
        MenuItem CurItem = new MenuItem();
        public Action<MenuItem> ItemClick;
        public float ContentW = 400;
        public float ItemH = 60;
        public INISection Language;
        public override void Initial(FakeStruct mod, UIElement script, UIInitializer initializer)
        {
            base.Initial(mod, script, initializer);
            ItemMod = HGUIManager.FindChild(BufferData, "Item");
            ContentMod = HGUIManager.FindChild(BufferData, "Content");
            HGUIManager.RecycleChild(script);
            var m = new Middleware<MenuItem, MenuContext>();
            m.Invoke = ItemUpdate;
            creator = m;
            if (ItemMod != null)
                ItemH = UIElementLoader.GetSize(ItemMod).y;
            if (ContentMod != null)
                ContentW = UIElementLoader.GetSize(ContentMod).x;
            CurItem.Context = new MenuContext();
        }
        public MenuContext Root { get => CurItem.Context; }
        void ItemUpdate(MenuItem menu, MenuContext context,int index)
        {
            if (menu.Text != null)
            {
                if (Language != null)
                {
                    string str = Language.GetValue(context.Key);
                    if (str != null)
                        menu.Text.Text = str;
                    else
                        menu.Text.Text = context.Content;
                }
                else
                menu.Text.Text = context.Content; 
            }
            if(menu.Expand!=null)
            {
                if (context.Child.Count > 0)
                {
                    menu.Expand.activeSelf = true;
                }
                else menu.Expand.activeSelf = false;
            }
            if (menu.Image != null)
            {
                menu.Image.Context.MainColor = new Color32();
                menu.Image.DataContext = menu;
                menu.Image.Click = DefItemClick;
                menu.Image.PointerEntry = PointerEntry;
                menu.Image.AutoColor = false;
            }
        }
        public void DefItemClick(UserEvent user,UserAction action)
        {
            var item = user.DataContext as MenuItem;
            if (item != null)
            {
                MenuItem cur = item.Parent.CurItem;
                if (cur != item & cur != null)
                {
                    cur.Image.Context.MainColor = new Color32();
                    if (cur.Next != null)
                    {
                        HGUIManager.RecycleUI(cur.Next);
                        cur.Child.Clear();
                        cur.Next = null;
                    }
                }
                int c = item.Context.Child.Count;
                if (c > 0)
                {
                    if (item.Next == null)
                    {
                        var coor = UIElement.GetGlobaInfo(item.Item, false);
                        item.Next = CreateMenu(item, item.Item);
                        item.dir = Anchor(item.Parent.dir, coor.Postion, new Vector2(ContentW, (c - 1) * ItemH), item.Next);
                    }
                }
                else
                {
                    if (ItemClick != null)
                        ItemClick(item);
                }
                item.Parent.CurItem = item;
            }
        }
        public void PointerEntry(UserEvent user, UserAction action)
        {
            user.Context.MainColor = new Color32(64,64,64,255);
            var item = user.DataContext as MenuItem;
            if (item != null)
            {
                MenuItem cur = item.Parent.CurItem;
                if (cur != item & cur != null)
                {
                    cur.Image.Context.MainColor = new Color32();
                    if (cur.Next != null)
                    {
                        HGUIManager.RecycleUI(cur.Next);
                        cur.Child.Clear();
                        cur.Next = null;
                    }
                }
                int c = item.Context.Child.Count;
                if (c > 0)
                {
                    if (item.Next == null)
                    {
                        var coor = UIElement.GetGlobaInfo(item.Item, false);
                        item.Next = CreateMenu(item, item.Item);
                        item.dir = Anchor(item.Parent.dir,coor.Postion,new Vector2(ContentW,(c-1)*ItemH),item.Next);
                    }
                }
                item.Parent.CurItem = item;
            }
        }
        public void SetItemUpdate<T,U>(Action<T, U, int> action) where T:MenuItem,new() where U : MenuContext
        {
            var m = new Middleware<T, U>();
            m.Invoke = action;
            creator = m;
        }
        public void Refresh()
        {
            HGUIManager.RecycleChild(Enity);
            CurItem.Next = CreateMenu(CurItem, Enity);
        }
        UIElement CreateMenu(MenuItem menu,UIElement parent)
        {
            var menus = menu.Context.Child;
            if (menus.Count == 0)
                return null;
            UIElement main = HGUIManager.Clone(ContentMod);
            main.SetParent(parent);
            main.localScale = Vector3.one;
            main.localRotation = Quaternion.identity;
            for (int i = 0; i < menus.Count; i++)
            {
                MenuItem item = creator.Create() as MenuItem;
                item.Child = new List<MenuItem>();
                var go = HGUIManager.Clone(ItemMod, creator.initializer);
                item.Index = i;
                item.Context = menus[i];
                creator.Call(item,menus[i],i);
                go.SetParent(main);
                go.localScale = Vector3.one;
                go.localRotation = Quaternion.identity;
                item.SetParent(menu);
            }
            main.SizeDelta = new Vector2(ContentW, menus.Count * ItemH);
            var sp = main.composite as StackPanel;
            if (sp != null)
                sp.Order();
            return main;
        }
        int Anchor(int dir, Vector2 world, Vector2 size,UIElement trans,float offset = 1)
        {
            Vector2 screen = HCanvas.CurrentCanvas.SizeDelta;
            float x = size.x * offset;
            float w = (offset + 0.5f) * size.x;
            if (dir == 0)
            {
                if (world.x + w > screen.x * 0.5f)
                {
                    dir = -1;//向左扩张
                    x = -x;
                }
            }
            else
            {
                if (world.x - w < screen.x * -0.5f)
                {
                    dir = 0;//向右扩张
                }
                else 
                { 
                    x = -x; 
                }
            }
            if (world.y - size.y > screen.y * -0.5f)//可以向下扩张
            {
                trans.localPosition = new Vector3(x, -size.y * 0.5f , 0);
            }
            else if (world.y + size.y < screen.y * 0.5f)//可以向上扩张
            {
                trans.localPosition = new Vector3(x, size.y * 0.5f, 0);
            }
            else//只能居中显示了
            {
                trans.localPosition = new Vector3(x, -world.y, 0);
            }
            return dir;
        }
        public void SetPosition(Vector3 pos)
        {
            var p = Enity.parent;
            var coor = UIElement.GetGlobaInfo(p, false);
            Enity.localPosition = pos - coor.Postion;
            if (CurItem.Context != null & CurItem.Next != null)
            {
                int c = CurItem.Context.Child.Count;
                Anchor(0, pos, new Vector2(ContentW, c * ItemH), CurItem.Next,0.5f);
            }
            MenuItem cur = CurItem.CurItem;
            if (cur != null)
            {
                cur.Image.Context.MainColor = new Color32();
                if (cur.Next != null)
                {
                    HGUIManager.RecycleUI(cur.Next);
                    cur.Child.Clear();
                    cur.Next = null;
                }
            }
        }
        public void Clear()
        {
            CurItem.Child.Clear();
            if (CurItem.Context != null)
                CurItem.Context.Child.Clear();
            if(CurItem.Next!=null)
                HGUIManager.RecycleUI(CurItem.Next);
            CurItem.Next = null;
            CurItem.CurItem = null;
        }
    }
}
