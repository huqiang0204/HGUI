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
        /// 内容的起始位置从坐上角计算,与UI载体的偏移位置
        /// </summary>
        Vector2 ContentOffset;
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
        public override void Initial(FakeStruct fake,UIElement script,UIInitializer initializer)
        {
            base.Initial(fake,script,initializer);
            Size = Enity.m_sizeDelta;
            eventCall = Enity.RegEvent<UserEvent>();
            eventCall.PointerDown = (o, e) => { UpdateVelocity = false; };
            eventCall.Drag = (o, e, s) =>
            {
                Size = Enity.m_sizeDelta;
                Scrolling(o, s);
            };
            eventCall.DragEnd = OnDragEnd;
            eventCall.ForceEvent = true;
            eventCall.AutoColor = false;

            eventCall.CutRect = true;
            Content = Enity.Find("Content");
            if (Content != null)
            {
                Move(Vector2.zero);
            }
        }

        void Scrolling(UserEvent back, Vector2 v)
        {
            var ls = Enity.localScale;
            v.x /= ls.x;
            v.y /= ls.y;
            v.x = -v.x;
            Move(v);
        }
        void OnDragEnd(UserEvent back, UserAction action, Vector2 v)
        {
            Scrolling(back, v);
            startVelocity.x = mVelocity.x = -back.VelocityX;
            startVelocity.y = mVelocity.y = back.VelocityY;
            UpdateVelocity = true;
        }
        void GetOffset()
        {
            float ax = Enity.m_sizeDelta.x;
            float ay = Enity.m_sizeDelta.y;
            float apx = Enity.Pivot.x;
            float apy = Enity.Pivot.y;
            float alx = ax * -apx;
            float ady = ay * -apy;

            var ls = Content.localScale;
            float x = Content.m_sizeDelta.x;
            x *= ls.x;
            float y = Content.m_sizeDelta.y;
            y *= ls.y;
            ContentSize.x = x;
            ContentSize.y = y;
            float px = Content.Pivot.x;
            float py = Content.Pivot.y;
            float lx = x * -px;
            float dy = y * -py;
            x = alx - lx;
            y = (ay + ady) - (y + dy);
            ContentOffset.x = x;
            ContentOffset.y = y;
        }
        /// <summary>
        /// 移动内容
        /// </summary>
        /// <param name="v">移动量</param>
        public void Move(Vector2 v)
        {
            if (Content == null)
                return;
            GetOffset();
            if (scrollType == ScrollType.BounceBack)
            {
                v = BounceBack(v);
            }
            else
            {
                v = ScrollNone(v);
            }
            switch (freeze)
            {
                case FreezeDirection.None:
                    Position.x += v.x;
                    Position.y += v.y;
                    break;
                case FreezeDirection.X:
                    Position.y += v.y;
                    break;
                case FreezeDirection.Y:
                    Position.x += v.x;
                    break;
            }
            var p = ContentOffset;
            p.x -= Position.x;
            p.y += Position.y;
            Content.localPosition = p;
            if (Scroll != null)
                Scroll(this, v);
        }
        /// <summary>
        /// y轴位置,范围0-1
        /// </summary>
        public float Pos
        {
            get
            {
                float y = Content.SizeDelta.y - Enity.SizeDelta.y;
                float p = Content.localPosition.y;
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
                Content.localPosition = new Vector3(0, y, 0);
            }
        }
        /// <summary>
        /// 初始速率
        /// </summary>
        protected Vector2 startVelocity;
        Vector2 mVelocity;
        public float DecayRate = 0.997f;
        protected bool UpdateVelocity = true;
        public override void Update(float time)
        {
            if (!UpdateVelocity)
                return;
            float x = 0;
            float y = 0;
            int count = UserAction.TimeSlice;
            if (mVelocity.x != 0)
            {
                float dr = DecayRate;
                if (scrollType == ScrollType.BounceBack)
                {
                    if (mVelocity.x < 0)
                    {
                        if (Position.x < 0)
                        {
                            dr *= 0.9f;
                        }
                    }
                    else
                    {
                        if (Position.x + Size.x > ContentSize.x)
                        {
                            dr *= 0.9f;
                        }
                    }
                }
                for (int i = 0; i < count; i++)
                {
                    mVelocity.x *= dr;
                    x += mVelocity.x;
                }
                if (mVelocity.x < 0.01f & mVelocity.x > -0.01f)
                {
                    mVelocity.x = 0;
                }
            }
            if (mVelocity.y != 0)
            {
                float dr = DecayRate;
                if (scrollType == ScrollType.BounceBack)
                {
                    if (mVelocity.y < 0)
                    {
                        if (Position.y < 0)
                        {
                            dr *= 0.9f;
                        }
                    }
                    else
                    {
                        if (Position.y + Size.y > ContentSize.y)
                        {
                            dr *= 0.9f;
                        }
                    }
                }
                for (int i = 0; i < count; i++)
                {
                    mVelocity.y *= dr;
                    y += mVelocity.y;
                }
                if (mVelocity.y < 0.01f & mVelocity.y > -0.01f)
                {
                    mVelocity.y = 0;
                }
            }
            if (x != 0 | y != 0)
                Move(new Vector2(x, y));
            if(scrollType==ScrollType.BounceBack)
            {
                if (mVelocity.x == 0)
                {
                    if (scrollType == ScrollType.BounceBack)
                    {
                        if (Position.x < -ScrollContent.Tolerance)
                        {
                            mVelocity.x = MathH.DistanceToVelocity(DecayRate, -Position.x);
                        }
                        else if (Position.x + Size.x > ContentSize.x + ScrollContent.Tolerance)
                        {
                            mVelocity.x = MathH.DistanceToVelocity(DecayRate, ContentSize.x - Position.x - Size.x);
                        }
                    }
                }
                if (mVelocity.y == 0)
                {
                    if (scrollType == ScrollType.BounceBack)
                    {
                        if (Position.y < -ScrollContent.Tolerance)
                        {
                            mVelocity.y = MathH.DistanceToVelocity(DecayRate, -Position.y);
                        }
                        else if (Position.y + Size.y > ContentSize.y + ScrollContent.Tolerance)
                        {
                            mVelocity.y = MathH.DistanceToVelocity(DecayRate, ContentSize.y - Position.y - Size.y);
                        }
                    }
                }
            }
            if (mVelocity.x == 0 & mVelocity.y == 0)
                UpdateVelocity = false;
        }
        protected Vector2 ScrollNone(Vector2 v)
        {
            if (ContentSize.x <= Enity.m_sizeDelta.x)
            {
                Position.x = 0;
                v.x = 0;
                mVelocity.x = 0;
                if (ContentSize.y <= Enity.m_sizeDelta.y)
                {
                    Position.y = 0;
                    v.y = 0;
                    mVelocity.y = 0;
                    return v;
                }
            }
            else
             if (ContentSize.y <= Enity.m_sizeDelta.y)
            {
                Position.y = 0;
                v.y = 0;
                mVelocity.y = 0;
            }

            if (v.x <= 0)
            {
                if (Position.x + v.x < 0)
                {
                    v.x = 0 - Position.x;
                    mVelocity.x = 0;
                }
            }
            else
            {
                if (Position.x + v.x + Enity.m_sizeDelta.x > ContentSize.x)
                {
                    v.x = ContentSize.x - Position.x - Enity.m_sizeDelta.x;
                    mVelocity.y = 0;
                }
            }
            if (v.y <= 0)
            {
                if (Position.y + v.y < 0)
                {
                    v.y = 0 - Position.y;
                    mVelocity.y = 0;
                }
            }
            else
            {
                if (Position.y + v.y + Enity.m_sizeDelta.y > ContentSize.y)
                {
                    v.y = ContentSize.y - Position.y - Enity.m_sizeDelta.y;
                    mVelocity.y = 0;
                }
            }
            return v;
        }
        protected Vector2 BounceBack(Vector2 v)
        {
            if (v.x < 0)
            {
                if (Position.x + v.x < 0)
                {
                    if (Position.x < 0)
                    {
                        float hx = Enity.m_sizeDelta.x * 0.5f;
                        float r = Position.x / hx;
                        r = -r;
                        r = 1 - r;
                        if (r < 0)
                            r = 0;
                        v.x *= r;
                    }
                }
            }
            else
            {
                if (Position.x + v.x + Enity.m_sizeDelta.x > ContentSize.x)
                {
                    float rx = Position.x + Enity.m_sizeDelta.x - ContentSize.x;
                    if (rx > 0)
                    {
                        float hx = Enity.m_sizeDelta.x * 0.5f;
                        float r = rx / hx;
                        r = 1 - r;
                        if (r < 0)
                            r = 0;
                        v.x *= r;
                    }
                }
            }
            if (v.y < 0)
            {
                if (Position.y + v.y < 0)
                {
                    if (Position.y < 0)
                    {
                        float hy = Enity.m_sizeDelta.y * 0.5f;
                        float r = Position.y / hy;
                        r = -r;
                        r = 1 - r;
                        if (r < 0)
                            r = 0;
                        v.y *= r;
                    }
                }
            }
            else
            {
                if (Position.y + v.y + Enity.m_sizeDelta.y > ContentSize.y)
                {
                    float ty = Position.y + Enity.m_sizeDelta.y - ContentSize.y;
                    if (ty > 0)
                    {
                        float hy = Enity.m_sizeDelta.x * 0.5f;
                        float r = ty / hy;
                        r = 1 - r;
                        if (r < 0)
                            r = 0;
                        v.y *= r;
                    }
                }
            }
            return v;
        }
    }
}
