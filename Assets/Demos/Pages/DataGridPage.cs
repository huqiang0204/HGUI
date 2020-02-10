using huqiang.Core.HGUI;
using System;
using UnityEngine;
using huqiang.UIComposite;
using huqiang.UIEvent;
using huqiang;
using huqiang.Data;
public class DataGridPage:UIPage
{
    //反射UI界面上的物体
    class View
    {
        public DataGrid grid;
    }
    View view;
    public override void Initial(Transform parent, object dat = null)
    {
        base.Initial(parent, dat);
        view = LoadUI<View>("baseUI", "datagrid");//"baseUI"创建的bytes文件名,"page"为创建的页面名
        InitialDataGrid();
    }
    void InitialDataGrid()
    {
        DataGridColumn column = new DataGridColumn();
        column.Head = "姓名";
        view.grid.AddColumn(column);
        view.grid.Refresh();
    }
}
