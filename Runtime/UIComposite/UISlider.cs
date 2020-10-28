using huqiang.Core.HGUI;
using huqiang.Data;
using huqiang.UIEvent;
using System;
using UnityEngine;

namespace huqiang.UIComposite
{
    /// <summary>
    /// 滑块条
    /// </summary>
    public unsafe struct SliderInfo
    {
        /// <summary>
        /// 前位置的偏移量
        /// </summary>
        public Vector2 StartOffset;
        /// <summary>
        /// 终点位置的偏移量
        /// </summary>
        public Vector2 EndOffset;
        /// <summary>
        /// 起点位置的握柄缩放
        /// </summary>
        public float MinScale;
        /// <summary>
        /// 终点位置的握柄缩放
        /// </summary>
        public float MaxScale;
        /// <summary>
        /// 方向
        /// </summary>
        public UISlider.Direction direction;
        public static int Size = sizeof(SliderInfo);
        public static int ElementSize = Size / 4;
    }
    public class UISlider : Composite
    {
        public enum Direction
        {
            Horizontal, Vertical
        }
        /// <summary>
        /// 填充的图像主体
        /// </summary>
        public HImage FillImage;
        /// <summary>
        /// 握柄,可空
        /// </summary>
        public HImage Nob;
        /// <summary>
        /// 附加信息
        /// </summary>
        public SliderInfo info;
        float ratio;
        UserEvent callBack;
        Vector2 pos;
        /// <summary>
        /// 设置填充值
        /// </summary>
        /// <param name="value"></param>
        public void SetFillSize(float value)
        {
            if (value < 0)
                value = 0;
            else if (value > 1)
                value = 1;
            if(Nob!=null)
            {
                if (info.direction == Direction.Horizontal)
                {
                    float w = Enity.SizeDelta.x;
                    var size = Nob.SizeDelta;
                    size.x = value * w;
                    Nob.SizeDelta = size;
                    ApplyValue();
                }
                else
                {
                    float w = Enity.SizeDelta.y;
                    var size = Nob.SizeDelta;
                    size.y = value * w;
                    Nob.SizeDelta = size;
                    ApplyValue();
                }
            }
        }
        /// <summary>
        /// 滑块值被改变事件
        /// </summary>
        public Action<UISlider> OnValueChanged;
        /// <summary>
        /// 滑块值改变完毕事件
        /// </summary>
        public Action<UISlider> ValueEndChange;
        /// <summary>
        /// 滑块位置百分比,0-1
        /// </summary>
        public float Percentage { get { return ratio; } set {
                if (value < 0)
                    value = 0;
                else if (value > 1)
                    value = 1;
                ratio = value;
                RatioToPos();
                ApplyValue();
            } }
        /// <summary>
        /// 构造函数
        /// </summary>
        public UISlider()
        {
            info.MinScale = 1;
            info.MaxScale = 1;
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="mod">模型数据</param>
        /// <param name="script">元素主体</param>
        public override void Initial(FakeStruct mod, UIElement script)
        {
            base.Initial(mod,script);
            var trans = script.transform;
            callBack = Enity.RegEvent<UserEvent>();
            callBack.Drag  = Draging;
            callBack.DragEnd = DragEnd;
            callBack.PointerDown = PointDown;
            callBack.AutoColor = false;
            var tmp = trans.Find("FillImage");
            if (tmp != null)
                FillImage = tmp.GetComponent<HImage>();
            tmp = trans.Find("Nob");
            Nob = tmp.GetComponent<HImage>();
            unsafe
            {
                var ex = mod.buffer.GetData(((TransfromData*)mod.ip)->ex) as FakeStruct;
                if (ex != null)
                    info = *(SliderInfo*)ex.ip;
            }
        }
        void Draging(UserEvent back, UserAction action, Vector2 v)
        {
            pos += v;
            ApplyValue();
            if (OnValueChanged != null)
                OnValueChanged(this);
        }
        void DragEnd(UserEvent back, UserAction action, Vector2 v)
        {
            pos += v;
            ApplyValue();
            if (ValueEndChange != null)
                ValueEndChange(this);
        }
        void PointDown(UserEvent back, UserAction action)
        {
            var v = new Vector2(back.GlobalPosition.x,back.GlobalPosition.y);
            pos = action.CanPosition - v;
            pos.x /= back.GlobalScale.x;
            pos.y /= back.GlobalScale.y;
            ApplyValue();
            if (OnValueChanged != null)
                OnValueChanged(this);
        }
        void RatioToPos()
        {
            var size = Enity.SizeDelta;
            if (info.direction == Direction.Horizontal)
            {
                float rx = size.x * 0.5f;
                float lx = -rx;
                float nx = Nob.SizeDelta.x * 0.5f;
                Vector2 start = new Vector2(lx + info.StartOffset.x + nx, info.StartOffset.y);
                Vector2 end = new Vector2(rx - info.EndOffset.x - nx, info.EndOffset.y);
                float w = end.x - start.x;
                pos.x = ratio * w + start.x;
            }
            else
            {
                float ty = size.y * 0.5f;
                float dy = -ty;
                float ny = Nob.SizeDelta.y * 0.5f;
                Vector2 start = new Vector2(info.StartOffset.x, dy + info.StartOffset.y + ny);
                Vector2 end = new Vector2(info.EndOffset.x, ty - info.EndOffset.y - ny);
                float w = end.y - start.y;
                pos.y = ratio * w + start.y;
            }
        }
        void ApplyValue()
        {
            if (Nob == null)
                return;
            var size = Enity.SizeDelta;
            if (info.direction==Direction.Horizontal)
            {
                float rx = size.x * 0.5f;
                float lx = -rx;
                float nx = Nob.SizeDelta.x * 0.5f;
                Vector2 start = new Vector2(lx + info.StartOffset.x+nx, info.StartOffset.y);
                Vector2 end = new Vector2(rx - info.EndOffset.x-nx, info.EndOffset.y);
                if (pos.x < start.x)
                    pos.x = start.x;
                else if (pos.x > end.x)
                    pos.x = end.x;
                float w = end.x - start.x;
                ratio = (pos.x - start.x) / w;
                pos = (end - start) * ratio + start;
                if(Nob!=null)
                {
                    var trans = Nob.transform;
                    trans.localPosition = pos;
                    float s = (info.MaxScale - info.MinScale) * ratio + info.MinScale;
                    trans.localScale = new Vector3(s, s, s);
                }
            }
            else
            {
                float ty = size.y * 0.5f;
                float dy = -ty;
                float ny = Nob.SizeDelta.y * 0.5f;
                Vector2 start = new Vector2( info.StartOffset.x,dy+ info.StartOffset.y+ny);
                Vector2 end = new Vector2(info.EndOffset.x,ty- info.EndOffset.y-ny);
                if (pos.y < start.y)
                    pos.y = start.y;
                else if (pos.y > end.y)
                    pos.y = end.y;
                float w = end.y - start.y;
                ratio = (pos.y - start.y) / w;
                pos = (end - start) * ratio + start;
                if (Nob != null)
                {
                    var trans = Nob.transform;
                    trans.localPosition = pos;
                    float s = (info.MaxScale - info.MinScale) * ratio + info.MinScale;
                    trans.localScale = new Vector3(s, s, s);
                }
            }
            if(FillImage!=null)
            {
                FillImage.SprType = SpriteType.Filled;
                FillImage.FillAmount = ratio;
            }
        }
    }
}
