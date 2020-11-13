using huqiang.Core.HGUI;
using System;
using UnityEngine;
using huqiang.UIComposite;
using huqiang.UIEvent;
using huqiang;
using huqiang.Data;
using Assets.Scripts;
using huqiang.UIModel;

public class DataGridPage:UIPage
{
    //反射UI界面上的物体
    class View
    {
        public DataGrid grid;
        public UserEvent last;
        public UserEvent next;
        public InputBox inputBox;
    }
    View view;
    public override void Initial(Transform parent, object dat = null)
    {
        base.Initial(parent, dat);
        view = LoadUI<View>("baseUI", "datagrid");//"baseUI"创建的bytes文件名,"page"为创建的页面名
        InitialDataGrid();
        view.inputBox.OnDone = InputDone;
        view.inputBox.OnSubmit = InputSubmit;
        view.last.Click = (o, e) => { LoadPage<TreeViewPage>(); };
        view.next.Click = (o, e) => { LoadPage<StartPage>(); };
        view.inputBox.Enity.gameObject.SetActive(false);
    }
    void InputDone(InputBox input)
    {
        var dgc = input.DataContext as DataGridItem;
        dgc.Text.Text = dgc.Context.Text = input.InputString;
        view.inputBox.Enity.gameObject.SetActive(false);
    }
    void InputSubmit(InputBox input)
    {
        var dgc = input.DataContext as DataGridItem;
        dgc.Text.Text = dgc.Context.Text = input.InputString;
    }
    void InitialDataGrid()
    {
        view.grid.SetItemUpdate<DataGridItem, DataGridItemContext>(ItemUpdate);
        DataGridColumn column = new DataGridColumn();
        column.Head = "姓名";
        view.grid.AddColumn(column);
        column = new DataGridColumn();
        column.Head = "年龄";
        view.grid.AddColumn(column);
        column = new DataGridColumn();
        column.Head = "性别";
        view.grid.AddColumn(column);
        column = new DataGridColumn();
        column.Head = "身高";
        view.grid.AddColumn(column);
        column = new DataGridColumn();
        column.Head = "婚否";
        view.grid.AddColumn(column);
        for(int i=0;i<1;i++)
        view.grid.AddRow(
            new DataGridItemContext() { Text="胡强"},
            new DataGridItemContext() { Text="28"},
            new DataGridItemContext() { Text="男"},
            new DataGridItemContext() { Text="168"},
            new DataGridItemContext() { Text="单身贵族"}
            );
        view.grid.Refresh();
    }
    int SelectRow;
    void ItemUpdate(DataGridItem item, DataGridItemContext context)
    {
        item.Text.Text = context.Text;
        var ue = item.Item.RegEvent<UserEvent>();
        ue.DataContext = item;
        ue.Click = (o, e) => {
            var i = o.DataContext as DataGridItem;
            SelectRow = i.Row;
            var trans = view.inputBox.Enity.transform;
            trans.SetParent(i.Text.transform);
            trans.localScale = Vector3.one;
            UIElement.Resize(view.inputBox.Enity);
            view.inputBox.InputString = i.Text.Text;
            e.RemoveFocus(o);
            e.AddFocus(view.inputBox.InputEvent);
            view.inputBox.DataContext = o.DataContext;
            view.inputBox.Enity.gameObject.SetActive(true);
            view.inputBox.PointerMoveEnd();
            TextOperation.EndPress = TextOperation.StartPress;
            //TextOperation.SelectAll();
        };
    }
}
