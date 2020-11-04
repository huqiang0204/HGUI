using huqiang.Core.HGUI;
using huqiang.Data;
using huqiang.UIEvent;
using System;
using UnityEngine;

namespace huqiang.UIComposite
{
    /// <summary>
    /// 内容拖拽框
    /// </summary>
    public class DragContent: Composite
    {
        /// <summary>
        /// 冻结方向
        /// </summary>
        public enum FreezeDirection
        {
            None,X,Y
        }
        /// <summary>
        /// 固定滚动,撞到边界立马停止
        /// </summary>
        /// <param name="eventCall">用户事件</param>
        /// <param name="v">参考移动量</param>
        /// <param name="x">限定移动量x</param>
        /// <param name="y">限定移动量y</param>
        /// <returns></returns>
        protected Vector2 ScrollNone(UserEvent eventCall, ref Vector2 v, ref float x, ref float y)
        {
            Vector2 v2 = Vector2.zero;
            float vx = x - v.x;
            if (vx < 0)
            {
                x = 0;
                eventCall.VelocityX = 0;
                v.x = 0;
            }
            else if (vx + Size.x > ContentSize.x)
            {
                x = ContentSize.x - Size.x;
                eventCall.VelocityX = 0;
                v.x = 0;
            }
            else
            {
                x -= v.x;
                v2.x = v.x;
            }
            float vy = y + v.y;
            if (vy < 0)
            {
                y = 0;
                eventCall.VelocityY = 0;
                v.y = 0;
            }
            else if (vy + Size.y > ContentSize.y)
            {
                y = ContentSize.y - Size.y;
                eventCall.VelocityY = 0;
                v.y = 0;
            }
            else
            {
                y += v.y;
                v2.y = v.y;
            }
            return v2;
        }
        /// <summary>
        /// 回弹滚动,撞到边界会有减速回弹效果
        /// </summary>
        /// <param name="eventCall">用户事件</param>
        /// <param name="v">参考移动量</param>
        /// <param name="x">衰减移动量x</param>
        /// <param name="y">衰减移动量y</param>
        /// <returns></returns>
        protected Vector2 BounceBack(UserEvent eventCall, ref Vector2 v, ref float x, ref float y)
        {
            x -= v.x;
            y += v.y;
            if (!eventCall.Pressed)
            {
                if (x < 0)
                {
                    if (v.x > 0)
                        if (eventCall.DecayRateX >= 0.99f)
                        {
                            eventCall.DecayRateX = 0.9f;
                            eventCall.VelocityX = eventCall.VelocityX;
                        }
                }
                else if (x + Size.x > ContentSize.x)
                {
                    if (v.x < 0)
                        if (eventCall.DecayRateX >= 0.99f)
                        {
                            eventCall.DecayRateX = 0.9f;
                            eventCall.VelocityX = eventCall.VelocityX;
                        }
                }
                if (y < 0)
                {
                    if (v.y < 0)
                        if (eventCall.DecayRateY >= 0.99f)
                        {
                            eventCall.DecayRateY = 0.9f;
                            eventCall.VelocityY = eventCall.VelocityY;
                        }
                }
                else if (y + Size.y > ContentSize.y)
                {
                    if (v.y > 0)
                        if (eventCall.DecayRateY >= 0.99f)
                        {
                            eventCall.DecayRateY = 0.9f;
                            eventCall.VelocityY = eventCall.VelocityY;
                        }
                }
            }
            return v;
        }
        /// <summary>
        /// 滚动框尺寸
        /// </summary>
        public Vector2 Size;
        /// <summary>
        /// 当前内容坐标
        /// </summary>
        public Vector2 Position;
        /// <summary>
        /// 内容尺寸
        /// </summary>
        public Vector2 ContentSize;
        /// <summary>
        /// 冻结方向
        /// </summary>
        public FreezeDirection freeze = FreezeDirection.None;
        /// <summary>
        /// 滚动类型
        /// </summary>
        public ScrollType scrollType = ScrollType.BounceBack;
        /// <summary>
        /// 内容主体元素
        /// </summary>
        public UIElement Content;
        /// <summary>
        /// 事件
        /// </summary>
        public UserEvent eventCall;
        /// <summary>
        /// 滚动事件
        /// </summary>
        public Action<DragContent, Vector2> Scroll;
        /// <summary>
        /// 滚动完毕事件
        /// </summary>
        public Action<DragContent> ScrollEnd;
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="fake">数据模型</param>
        /// <param name="script">元素主体</param>
        public override void Initial(FakeStruct fake,UIElement script,Initializer initializer)
        {
            base.Initial(fake,script,initializer);
            Size = Enity.SizeDelta;
            eventCall = Enity.RegEvent<UserEvent>();
            eventCall.Drag = (o, e, s) =>
            {
                Scrolling(o, s);
            };
            eventCall.DragEnd = (o, e, s) =>
            {
                Scrolling(o, s);
                o.DecayRateX = 0.998f;
                o.DecayRateY = 0.998f;
            };
            eventCall.ScrollEndX = OnScrollEndX;
            eventCall.ScrollEndY = OnScrollEndY;
            eventCall.Scrolling = Scrolling;
            eventCall.ForceEvent = true;

            eventCall.CutRect = true;
            var chi = Enity.transform.Find("Content");
            if(chi!=null)
            {
                Content = chi.GetComponent<UIElement>();
                if (Content != null)
                    ContentSize = Content.SizeDelta;
            }
        }

        void Scrolling(UserEvent back, Vector2 v)
        {
            var ls = Enity.transform.localScale;
            v.x /= ls.x;
            v.y /= ls.y;
            Move(v);
        }
        /// <summary>
        /// 移动内容
        /// </summary>
        /// <param name="v">移动量</param>
        public void Move(Vector2 v)
        {
            if (Content == null)
                return;
            ContentSize = Content.SizeDelta;
            var ls = Enity.transform.localScale;
            v.x /= ls.x;
            v.y /= ls.y;
            switch (scrollType)
            {
                case ScrollType.None:
                    v = ScrollNone(eventCall, ref v, ref Position.x, ref Position.y);
                    break;
                case ScrollType.BounceBack:
                    v = BounceBack(eventCall, ref v, ref Position.x, ref Position.y);
                    break;
            }
            var offset = ContentSize - Size;
            offset *= 0.5f;
            var p = Position;
            switch (freeze)
            {
                case FreezeDirection.None:
                    p.x = -p.x;
                    p.x += offset.x;
                    p.y -= offset.y;
                    break;
                case FreezeDirection.X:
                    p.y -= offset.y;
                    break;
                case FreezeDirection.Y:
                    p.x = -p.x;
                    p.x += offset.x;
                    break;
            }
            Content.transform.localPosition = p;
            if (Scroll != null)
                Scroll(this, v);
        }
        void OnScrollEndX(UserEvent back)
        {
            if (scrollType == ScrollType.BounceBack)
            {
                if (Position.x < -ScrollContent.Tolerance)
                {
                    back.DecayRateX = 0.988f;
                    float d = -Position.x;
                    back.ScrollDistanceX = -d * eventCall.Context.transform.localScale.x;
                }
                else
                {
                    float max = ContentSize.x + ScrollContent.Tolerance;
                    if (max < Size.x)
                        max = Size.x + ScrollContent.Tolerance;
                    if (Position.x + Size.x > max)
                    {
                        back.DecayRateX = 0.988f;
                        float d = ContentSize.x - Position.x - Size.x;
                        back.ScrollDistanceX = -d * eventCall.Context.transform.localScale.x;
                    }
                    else
                    {
                        if (ScrollEnd != null)
                            ScrollEnd(this);
                    }
                }
            }
            else if (ScrollEnd != null)
                ScrollEnd(this);
        }
        void OnScrollEndY(UserEvent back)
        {
            if (scrollType == ScrollType.BounceBack)
            {
                if (Position.y < -ScrollContent.Tolerance)
                {
                    back.DecayRateY = 0.988f;
                    float d = -Position.y;
                    back.ScrollDistanceY = d * eventCall.Context.transform.localScale.y;
                }
                else
                {
                    float max = ContentSize.y + ScrollContent.Tolerance;
                    if (max < Size.y)
                        max = Size.y + ScrollContent.Tolerance;
                    if (Position.y + Size.y > max)
                    {
                        back.DecayRateY = 0.988f;
                        float d = ContentSize.y - Position.y - Size.y;
                        back.ScrollDistanceY = d * eventCall.Context.transform.localScale.y;
                    }
                    else
                    {
                        if (ScrollEnd != null)
                            ScrollEnd(this);
                    }
                }
            }
            else if (ScrollEnd != null)
                ScrollEnd(this);
        }
        /// <summary>
        /// y轴位置,范围0-1
        /// </summary>
        public float Pos
        {
            get
            {
                float y = Content.SizeDelta.y - Enity.SizeDelta.y;
                float p = Content.transform.localPosition.y;
                p += 0.5f * y;
                p /= y;
                if (p < 0)
                    p = 0;
                else if (p > 1)
                    p = 1;
                return p;
            }
            set
            {
                if (value < 0)
                    value = 0;
                else if (value > 1)
                    value = 1;
                float y = Content.SizeDelta.y - Enity.SizeDelta.y;
                if (y < 0)
                    y = 0;
                y *= (value - 0.5f);
                Content.transform.localPosition = new Vector3(0, y, 0);
            }
        }
    }
}
