using huqiang.Core.HGUI;
using System;
using UnityEngine;
using huqiang.UIComposite;
using huqiang.UIEvent;
using huqiang;
using huqiang.Data;
public class ButtonPage:UIPage
{
    //反射UI界面上的物体
    class View
    {
        public HImage button;
        public HText buttonText;
    }
    View view;
    int buttonClickNum;
    public override void Initial(Transform parent, object dat = null)
    {
        base.Initial(parent, dat);
        view = LoadUI<View>("baseUI", "ButtonPage");//"baseUI"创建的bytes文件名,"page"为创建的页面名
        InitialEvent();
    }
    //初始化UI事件
    public void InitialEvent()
    {
        view.button.userEvent.Click = buttonClick;
    }
    public void buttonClick(UserEvent e, UserAction a)
    {
        buttonClickNum++;
        view.buttonText.Text = "点击按钮次数 " + buttonClickNum;
    }
    //语言切换功能用
    public override void ChangeLanguage()
    {
        base.ChangeLanguage();
    }
    //接收消息
    public override void Cmd(DataBuffer dat)
    {
        base.Cmd(dat);
    }
    //页面弹窗
    public override T PopUpWindow<T>(object obj = null)
    {
        return base.PopUpWindow<T>(obj);
    }
}
