using huqiang.Core.HGUI;
using System;
using UnityEngine;
using huqiang.UIComposite;
using huqiang.UIEvent;
using huqiang;
using huqiang.Data;
using Assets.Scripts;
using huqiang.UIModel;

public class DrawingPage:UIPage
{
    //反射UI界面上的物体
    class View
    {
        public Paint paint;
        public UIPalette palette;
        public UISlider paintSize;
        public HText size;
        public HImage color;
        public UserEvent last;
        public UserEvent next;
        public UserEvent mode;
        public HText modeTip;
    }
    View view;
    int drawMode;
    string[] tips = new string[] { "画笔","缩放","旋转"};
    public override void Initial(UIElement parent, object dat = null)
    {
        base.Initial(parent, dat);
        view = LoadUI<View>("baseUI", "drawing");//"baseUI"创建的bytes文件名,"page"为创建的页面名
        InitialUI();
        view.last.Click = (o, e) => { LoadPage<ChatPage>(); };
        view.next.Click = (o, e) => { LoadPage<ScrollPage>(); };
    }
    void InitialUI()
    {
        view.paint.BrushColor = Color.black;
        view.paint.BrushSize = 36;
        view.palette.TemplateChanged=
        view.palette.ColorChanged = (o) => {
            view.color.MainColor = o.SelectColor;
            view.paint.BrushColor = o.SelectColor;
        };
        view.paintSize.OnValueChanged = (o) => {
            var v = o.Percentage * 36;
            if (v < 1)
                v = 1;
            view.size.Text = v.ToString();
            view.paint.BrushSize = v;
        };
        view.mode.Click = (o, e) => {
            drawMode++;
            if (drawMode > 2)
                drawMode = 0;
            view.paint.drawModel = (Paint.DrawModel)drawMode;
            view.modeTip.Text = tips[drawMode];
        };
        view.modeTip.Text = tips[0];
    }
}
