using huqiang.Core.HGUI;
using huqiang.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace huqiang.UIComposite
{
    public class DataGridItem
    {
        public UIElement Item;
        public HText Text;
    }
    public class DataGridItemContext
    {
        public string Text;
    }
    public class DataGridColumn
    {
        public string Head;
        public List<DataGridItemContext> datas = new List<DataGridItemContext>();
    }
    public class DataGrid:Composite
    {
        /// <summary>
        /// 当前滚动的位置
        /// </summary>
        public Vector2 Position;
        FakeStruct ItemMod;
        FakeStruct HeadMod;
        FakeStruct Drag;
        FakeStruct Line;
        public List<DataGridColumn> BindingData = new List<DataGridColumn>();
        public override void Initial(FakeStruct mod, UIElement element)
        {
            base.Initial(mod, element);
            HeadMod = HGUIManager.FindChild(mod, "Head");
            ItemMod = HGUIManager.FindChild(mod, "Item");
            Drag = HGUIManager.FindChild(mod, "Drag");
            Line = HGUIManager.FindChild(mod, "Line");
        }

        void Order()
        {

        }
    }
}
