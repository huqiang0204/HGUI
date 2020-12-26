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
    }
    View view;
    public override void Initial(UIElement parent, object dat = null)
    {
        base.Initial(parent, dat);
        view = LoadUI<View>("baseUI", "datagrid");//"baseUI"创建的bytes文件名,"page"为创建的页面名
        InitialDataGrid();
        view.last.Click = (o, e) => { LoadPage<TreeViewPage>(); };
        view.next.Click = (o, e) => { LoadPage<StartPage>(); };
    }
    void InitialDataGrid()
    {
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
        for(int i=0;i<20;i++)
        view.grid.AddRow(
            new DataGridItemContext() { Text="胡强"},
            new DataGridItemContext() { Text="28"},
            new DataGridItemContext() { Text="男"},
            new DataGridItemContext() { Text="168"},
            new DataGridItemContext() { Text="单身贵族"}
            );
        view.grid.Refresh();
    }
}
