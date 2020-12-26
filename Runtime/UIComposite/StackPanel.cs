using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using huqiang.Core.HGUI;
using huqiang.Core.UIData;
using huqiang.Data;
using huqiang.UIEvent;
using UnityEngine;

namespace huqiang.UIComposite
{
    public enum Direction
    {
        /// <summary>
        /// 水平方向
        /// </summary>
        Horizontal,
        /// <summary>
        /// 垂直方向
        /// </summary>
        Vertical
    }
    /// <summary>
    /// 栈面板,用于排序
    /// </summary>
    public class StackPanel:Composite
    {
        int c = 0;
        /// <summary>
        /// 排序方向
        /// </summary>
        public Direction direction = Direction.Horizontal;
        /// <summary>
        /// 项目之间的间隔
        /// </summary>
        public float spacing = 0;
        /// <summary>
        /// 开启固定尺寸,会改变项目的尺寸
        /// </summary>
        public bool FixedSize;
        /// <summary>
        /// 固定尺寸比例
        /// </summary>
        public float FixedSizeRatio = 1;
        /// <summary>
        /// 项目偏移位置
        /// </summary>
        public float ItemOffset = 0;
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="mod">模型数据</param>
        /// <param name="script">元素主体</param>
        public override void Initial(FakeStruct mod, UIElement script,UIInitializer initializer)
        {
            base.Initial(mod, script, initializer);
            script.SizeChanged = (o) => Order();
            var ex = UIElementLoader.GetCompositeData(mod);
            if (ex != null)
            {
                direction = (Direction)ex[0];
                spacing = ex.GetFloat(1);
                FixedSize = ex[2] == 1;
                FixedSizeRatio = ex.GetFloat(3);
                ItemOffset = ex.GetFloat(4);
            }
        }
       /// <summary>
       /// 排序
       /// </summary>
        public void Order()
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
                float ps = Enity.SizeDelta.x;
                float sx = ps * -0.5f;
                if (FixedSizeRatio > 0)
                    ps *= FixedSizeRatio;
                var c = Enity.childCount;
                float ox = ItemOffset;
                for (int i = 0; i < c; i++)
                {
                    var son = Enity.GetChild(i);
                    float w = 0;
                    float p = 0.5f;
                    if (son != null)
                    {
                        if (FixedSize)
                        {
                            w = ps;
                            ox = ItemOffset * w;
                        }
                        else
                        {
                            w = son.SizeDelta.x;
                        }
                        p = son.Pivot.x;
                    }
                    float os = sx - w * -p + ox;
                    son.localPosition = new Vector3(os, 0, 0);
                    son.localScale = Vector3.one;
                    sx += w + spacing;
                }
            }
        }
        void OrderVertical()
        {
            if (Enity != null)
            {
                float ps =Enity.SizeDelta.y;
                float sy = ps * (1 - Enity.Pivot.y);
                if (FixedSizeRatio > 0)
                    ps *= FixedSizeRatio;
                var c = Enity.childCount;
                float oy = ItemOffset;
                for (int i = 0; i < c; i++)
                {
                    var son = Enity.GetChild(i);
                    float h = 0;
                    float p = 0.5f;
                    if (son != null)
                    {
                        if (FixedSize)
                        {
                            h = ps;
                            oy = h * ItemOffset;
                        }
                        else
                        {
                            h = son.SizeDelta.y;
                        }
                        p = son.Pivot.y;
                    }
                    float os = sy + h * (p - 1) - oy;
                    son.localPosition = new Vector3(0, os, 0);
                    son.localScale = Vector3.one;
                    sy -= h + spacing;
                }
            }
        }
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="time"></param>
        public override void Update(float time)
        {
            var a = Enity.childCount;
            if (a != c)
            {
                c = a;
                Order();
            }
        }
    }
 
}
