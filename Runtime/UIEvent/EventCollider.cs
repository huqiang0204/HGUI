using huqiang.Core.HGUI;
using huqiang.Data;
using System;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huqiang.UIEvent
{
    public enum EventColliderType
    {
        Box,
        Circle,
        Polygon
    }
    /// <summary>
    /// 事件碰撞器
    /// </summary>
    public interface EventCollider
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="fake">数据缓存</param>
        void Initial(FakeStruct fake);
        /// <summary>
        /// 检测点是否在碰撞器里面
        /// </summary>
        /// <param name="script">ui对象实例</param>
        /// <param name="user">用户时间</param>
        /// <param name="dot">检测点</param>
        /// <returns>返回真,则点在碰撞器里面</returns>
        bool InThere(UIElement script, UserEvent user, Vector2 dot);
    }
    /// <summary>
    /// 矩形碰撞器
    /// </summary>
    public class UIBoxCollider : EventCollider
    {
        Vector2[] Box = new Vector2[4];
        public bool InThere(UIElement script, UserEvent user, Vector2 dot)
        {
            float w = script.m_sizeDelta.x * user.GlobalScale.x;
            float h = script.m_sizeDelta.y * user.GlobalScale.y;
            float px = script.Pivot.x;
            float py = script.Pivot.y;
            float lx = -px * w;
            float rx = lx + w;
            float dy = -py * h;
            float ty = dy + h;

            Box[0] = user.GlobalRotation * new Vector3(lx, dy) + user.GlobalPosition;
            Box[1] = user.GlobalRotation * new Vector3(lx, ty) + user.GlobalPosition;
            Box[2] = user.GlobalRotation * new Vector3(rx, ty) + user.GlobalPosition;
            Box[3] = user.GlobalRotation * new Vector3(rx, dy) + user.GlobalPosition;
            return  huqiang.Physics2D.DotToPolygon(Box, dot);
        }
        public void Initial(FakeStruct fake) { }

    }
    /// <summary>
    /// 圆形碰撞器
    /// </summary>
    public class UICircleCollider : EventCollider
    {
        public float Radius;
        public float Ratio = 1;
        bool InArc(ref Vector2 a,ref Vector2 b,ref Vector2 c,ref Vector2 dot)
        {
            var p = MathH.GetCentre(a,b,c);
            float ax = a.x - p.x;
            float ay = a.y - p.y;
            float dx = dot.x - p.x;
            float dy = dot.y - p.y;

            if (ax * ax + ay * ay >= dx * dx + dy * dy)
            {
                return huqiang.Physics2D.LineToLine(ref a, ref c, ref p, ref dot);
            }
            return false;
        }
        public bool InThere(UIElement script, UserEvent user, Vector2 dot)
        {
            Vector3 os = Vector3.zero;
            float px = script.Pivot.x;
            os.x = (0.5f - px) * script.m_sizeDelta.x;
            float py = script.Pivot.y;
            os.y = (0.5f - py) * script.m_sizeDelta.y;
            var q = user.GlobalRotation;
            
            var o = user.GlobalPosition;
            Vector3 scale = user.GlobalScale;
            float w = Radius;
            if (w == 0)
            {
                w = script.m_sizeDelta.x;
                if (w > script.m_sizeDelta.y)
                    w = script.m_sizeDelta.y;
                w *= 0.5f;
                w *= Ratio;
            }
            if (scale.x != scale.y)
            {
                float h = w * scale.y;
                w = w * scale.x;
                float rx = w ;
                float lx = -rx;
                lx += os.x;
                rx += os.x;
                float ty = h;
                float dy = -ty;
                ty += os.y;
                dy += os.y;
                dot.x -= o.x;
                dot.y -= o.y;
                if (scale.x > scale.y)
                {
                    Vector2 a = new Vector2(lx, os.y);
                    a = q * a;
                    Vector2 b = new Vector2(os.x, ty);
                    b = q * b;
                    Vector2 c = new Vector2(rx, os.y);
                    c = q * c;
                    if (InArc(ref a, ref b, ref c, ref dot))
                        return true;
                    b.x = os.x;
                    b.y = dy;
                    b = q * b;
                    return InArc(ref a, ref b, ref c, ref dot);
                }
                else
                {
                    Vector2 a = new Vector2(os.x, ty);
                    a = q * a;
                    Vector2 b = new Vector2(rx, os.y);
                    b = q * b;
                    Vector2 c = new Vector2(os.x, dy);
                    c = q * c;
                    if (InArc(ref a, ref b, ref c, ref dot))
                        return true;
                    b.x = lx;
                    b.y = os.y;
                    b = q * b;
                    return InArc(ref a, ref b, ref c, ref dot);
                }
            }
            else
            {
                os = q * os;
                w *= scale.x;
                float x = dot.x - o.x - os.x;
                float y = dot.y - o.y - os.y;
                if (x * x + y * y < w * w)
                    return true;
            }
            return false;
        }
        public void Initial(FakeStruct fake)
        {
            Radius = fake.GetFloat(1);
            Ratio = fake.GetFloat(2);
        }
    }
    /// <summary>
    /// 多边形碰撞器
    /// </summary>
    public class UIPolygonCollider : EventCollider
    {
        /// <summary>
        /// 多边形所有顶点
        /// </summary>
        public Vector2[] Points;
        Vector2[] tmp;
        public bool InThere(UIElement script, UserEvent user, Vector2 dot)
        {
            if (Points == null)
                return false;
            if (Points.Length < 3)
                return false;
            if (tmp == null)
                tmp = new Vector2[Points.Length];
            else
            if (tmp.Length != Points.Length)
                tmp = new Vector2[Points.Length];
            var os = user.GetOffset();
            var p = user.GlobalPosition;
            var q = user.GlobalRotation;
            for(int i=0;i<Points.Length;i++)
            {
                tmp[i] = p + q * Points[i] + os;
            }
            return huqiang.Physics2D.DotToPolygon(tmp, dot);
        }
        public void Initial(FakeStruct fake) {
            Points = fake.buffer.GetArray<Vector2>(fake[1]);
        }
    }
}
