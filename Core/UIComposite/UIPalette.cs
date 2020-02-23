using huqiang.Core.HGUI;
using huqiang.Data;
using huqiang.Other;
using huqiang.UIEvent;
using System;
using UnityEngine;

namespace huqiang.UIComposite
{
    public class UIPalette : Composite
    {
        UserEvent callBackR;
        UserEvent callBackC;
        Transform hc;
        Transform NobA;
        Transform NobB;
        HImage template;
        HImage htemp;
        HImage slider;
        Palette palette;
        public Color SelectColor = Color.white;
        float Alpha;
        public Action<UIPalette> ColorChanged;
        public Action<UIPalette> TemplateChanged;
        UISlider uISlider;
        public override void Initial(FakeStruct fake,UIElement element)
        {
            base.Initial(fake,element);
            palette = new Palette();
            callBackR = element.RegEvent<UserEvent>();
            callBackR.IsCircular = true;
            callBackR.Drag = callBackR.DragEnd = DragingR;
            callBackR.PointerDown = PointDownR;
            var mod = element.transform;
            NobA = mod.Find("NobA");
            NobB = mod.Find("NobB");
            hc = mod.Find("HTemplate");
            template = hc.GetComponent<HImage>();
            callBackC = template.RegEvent<UserEvent>();
            callBackC.Drag = callBackC.DragEnd = DragingC;
            callBackC.PointerDown = PointDownC;
            htemp = mod.GetComponent<HImage>();
            htemp.MainTexture = Palette.LoadCTemplateAsync();
            template.MainTexture = palette.texture;
            palette.AwaitLoadHSVT(1);
            SelectColor.a = 1;
            var son = mod.Find("Slider");
            slider = son.GetComponent<HImage>();
            slider.MainTexture = Palette.AlphaTemplate();
            uISlider =slider.composite as UISlider;
            uISlider.OnValueChanged = AlphaChanged;
            uISlider.Percentage = 1;
        }
        void DragingR(UserEvent back, UserAction action, Vector2 v)
        {
            PointDownR(back, action);
        }
        void PointDownR(UserEvent back, UserAction action)
        {
            float x = action.CanPosition.x - back.GlobalPosition.x;
            float y = action.CanPosition.y - back.GlobalPosition.y;
            x /= back.GlobalScale.x;
            y /= back.GlobalScale.y;
            float sx = x * x + y * y;
            float r = Mathf.Sqrt(220 * 220 / sx);
            x *= r;
            y *= r;
            if (NobA != null)
            {
                NobA.localPosition = new Vector3(x, y, 0);
            }
            float al = MathH.atan(-x, -y);
            palette.AwaitLoadHSVT(al / 360);
            Color col = palette.buffer[Index];
            SelectColor.r = col.r;
            SelectColor.g = col.g;
            SelectColor.b = col.b;
            if (TemplateChanged != null)
                TemplateChanged(this);
        }
        void DragingC(UserEvent back, UserAction action, Vector2 v)
        {
            PointDownC(back, action);
        }
        int Index;
        void PointDownC(UserEvent back, UserAction action)
        {
            float x = action.CanPosition.x - back.GlobalPosition.x;
            float y = action.CanPosition.y - back.GlobalPosition.y;
            x /= back.GlobalScale.x;
            y /= back.GlobalScale.y;
            if (x < -128)
                x = -128;
            else if (x > 128)
                x = 128;
            if (y < -128)
                y = -128;
            else if (y > 128)
                y = 128;
            if (NobB != null)
            {
                NobB.localPosition = new Vector3(x, y, 0);
            }
            int dx = (int)x + 128;
            if (dx < 0)
                dx = 0;
            else if (dx > 255)
                dx = 255;
            int dy = (int)y + 128;
            if (dy < 0)
                dy = 0;
            else if (dy > 255)
                dy = 255;
            Index = dy * 256 + dx;
            if (Index >= 256 * 256)
                Index = 256 * 256 - 1;
            Color col = palette.buffer[Index];
            SelectColor.r = col.r;
            SelectColor.g = col.g;
            SelectColor.b = col.b;
            if (ColorChanged != null)
                ColorChanged(this);
        }
        void AlphaChanged(UISlider slider)
        {
            SelectColor.a = slider.Percentage;
            if (ColorChanged != null)
                ColorChanged(this);
        }
    }
}
