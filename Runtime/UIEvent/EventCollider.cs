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
    public interface EventCollider
    {
        void Initial(FakeStruct fake);
        bool InThere(UIElement script, UserEvent user, Vector2 dot);
    }
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

    public class UICircleCollider : EventCollider
    {
        public float Radius;
        public float Ratio = 1;
        public bool InThere(UIElement script, UserEvent user, Vector2 dot)
        {
            var o = user.GlobalPosition;
            float w = Radius;
            if(w==0)
            {
                w = script.m_sizeDelta.x;
                if (w > script.m_sizeDelta.y)
                    w = script.m_sizeDelta.y;
                w *= 0.5f;
                w *= Ratio;
            }
            Vector3 os = Vector3.zero;
            float px = script.Pivot.x;
            os.x = (0.5f - px) * script.m_sizeDelta.x;
            float py = script.Pivot.y;
            os.y = (0.5f - py) * script.m_sizeDelta.y;
            os = user.GlobalRotation * os;
            float x = dot.x - o.x - os.x;
            float y = dot.y - o.y - os.y;
            if (x * x + y * y < w * w)
                return true;
            return false;
        }
        public void Initial(FakeStruct fake)
        {
            Radius = fake.GetFloat(1);
            Ratio = fake.GetFloat(2);
        }
    }

    public class UIPolygonCollider : EventCollider
    {
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
