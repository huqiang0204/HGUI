using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using huqiang.Core.HGUI;
using huqiang.Data;
using UnityEngine;

namespace huqiang.UIComposite
{
    public enum Direction
    {
        Horizontal,
        Vertical
    }
    public class StackPanel:Composite
    {
        public override void Initial(FakeStruct mod, UIElement script)
        {
            base.Initial(mod, script);
            script.SizeChanged = (o) => Order();
        }
        public Direction direction = Direction.Horizontal;
        void Order()
        {
            switch (direction)
            {
                case Direction.Horizontal:
                    OrderHorizontal();
                    break;
                case Direction.Vertical:
                    OrderVertical();
                    break;
            }
        }
        void OrderHorizontal()
        {
            if (Enity != null)
            {
                var sx =Enity.SizeDelta.x * -0.5f;
                var y = Enity.SizeDelta.y;
                var trans = Enity.transform;
                var c = trans.childCount;
                for (int i = 0; i < c; i++)
                {
                    var son = trans.GetChild(i);
                    var ss = son.GetComponent<UIElement>();
                    float w = 0;
                    if (ss != null)
                    {
                        w = ss.SizeDelta.x;
                    }
                    float os = sx + w * 0.5f;
                    son.localPosition = new Vector3(os, 0, 0);
                    son.localScale = Vector3.one;
                    sx += w;
                }
            }
        }
        void OrderVertical()
        {
            if (Enity != null)
            {
                var sy = Enity.SizeDelta.y * 0.5f;
                var x = Enity.SizeDelta.x;
                var trans = Enity.transform;
                var c = trans.childCount;
                for (int i = 0; i < c; i++)
                {
                    var son = trans.GetChild(i);
                    var ss = son.GetComponent<UIElement>();
                    float h = 0;
                    if (ss != null)
                    {
                        h = ss.SizeDelta.y;
                    }
                    float os = sy - h * 0.5f;
                    son.localPosition = new Vector3(0, os, 0);
                    son.localScale = Vector3.one;
                    sy -= h;
                }
            }
        }
    }
}
