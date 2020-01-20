using huqiang.Core.HGUI;
using huqiang.Data;
using huqiang.UIEvent;
using System;
using UnityEngine;

namespace huqiang.UIComposite
{
    public class DragContent: Composite
    {
        public enum FreezeDirection
        {
            None,X,Y
        }
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
        public Vector2 Size;
        public Vector2 Position;
        public Vector2 ContentSize;
        public FreezeDirection freeze = FreezeDirection.None;
        public ScrollType scrollType = ScrollType.BounceBack;
        public UIElement Content;
        public UserEvent eventCall;
        public Action<DragContent, Vector2> Scroll;
        public Action<DragContent> ScrollEnd;
        public override void Initial(FakeStruct fake,UIElement script)
        {
            base.Initial(fake,script);
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
