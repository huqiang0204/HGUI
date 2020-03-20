using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huqiang
{
    /// <summary>
    /// 用于精确的计算,需要更多的计算
    /// </summary>
    public partial class Physics2D
    {
        public static bool CircleToCircle(Vector2 A, Vector2 B, float radiusA, float radiusB,ref  Vector2 p0,ref Vector2 p1)
        {
            float r = radiusA + radiusB;
            Vector2 v = A - B;
            float sx = v.x * v.x + v.y * v.y;
            if (r * r > sx)
            {
                v = v.normalized;
                float d = Mathf.Sqrt(radiusA * radiusA / sx);
                p0 = A;
                p0.x += v.x * d;
                p0.y += v.y * d;
                d = Mathf.Sqrt(radiusB * radiusB / sx);
                p1 = B;
                p1.x -= v.x * d;
                p1.y -= v.y * d;
                return true;
            }
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a">线段1起点</param>
        /// <param name="b">线段1终点</param>
        /// <param name="c">线段2起点</param>
        /// <param name="d">线段2终点</param>
        /// <param name="p0">线段的交点</param>
        /// <returns></returns>
        public static bool LineToLine(ref Vector3 a, ref Vector3 b, ref Vector3 c, ref Vector3 d, ref Vector3 p0)//相交线相交点
        {
            float ax = b.x - a.x;
            float ay = b.y - a.y;
            float cx = d.x - c.x;
            float cy = d.y - c.y;
            float y = ay * cx - ax * cy;
            if (y == 0)
                return false;
            float x = (c.y - a.y) * cx + (a.x - c.x) * cy;
            float r = x / y;
            if (r >= 0 & r <= 1)
            {
                if (cx == 0)
                {
                    y = (a.y - c.y + r * ay) / cy;
                }
                else
                {
                    y = (a.x - c.x + r * ax) / cx;
                }
                if (y >= 0 & y <= 1)
                {
                    p0.x = a.x + r * ax;
                    p0.y = a.y + r * ay;
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 圆与线相交
        /// </summary>
        /// <param name="C">圆心位置</param>
        /// <param name="r">半径</param>
        /// <param name="A">线段起点</param>
        /// <param name="B">线段终点</param>
        /// <param name="p0">切点</param>
        /// <param name="p1">相交点1,如果超出起点则为起点</param>
        /// <param name="p2">相交点2,如果超出终点则为终点</param>
        /// <returns></returns>
        public static bool CircleToLine(Vector2 C, float r, Vector2 A, Vector2 B, ref Vector2 p0, ref Vector2 p1, ref Vector2 p2)
        {
            float o = r * r;
            Vector2 v1 = A - C;
            float a = v1.x * v1.x + v1.y * v1.y;
            if (a <= o)
            {
                p1 = A;//A点在圆里面
            }
            Vector2 v2 = B - C;
            float b = v2.x * v2.x + v2.y * v2.y;
            if (b <= o)
            {
                p2 = B;//B点在圆里面
            }
            Vector2 v3 = C - A;
            Vector2 v4 = B - A;
            float sl = v4.x * v4.x + v4.y * v4.y;
            float l = Mathf.Sqrt(sl);
            Vector2 p = Vector3.Project(v3, v4);
            float pl = Mathf.Sqrt(p.x * p.x + p.y * p.y) / l;//百分比位置
            if (Vector2.Dot(v3, v4) < 0)//反方向
                pl = -pl;
            p.x += A.x;
            p.y += A.y;//实际投影位置
            v2.x = p.x - C.x;
            v2.y = p.y - C.y;
            float sp = v2.x * v2.x + v2.y * v2.y;//圆心投影的长度的平方
            if (sp < o)//如过小于半径的平方
            {
                float ol = o - sp;
                var v = v4.normalized;
                float sx = v.x * v.x + v.y * v.y;
                float os = Mathf.Sqrt(ol / sx);
                float pos = os / l;
                v *= os;
                p0 = p;
                if (pl - pos > 0 & pl - pos < 1)
                    p1 = p - v;
                if (pl + pos > 0 & pl + pos < 1)
                    p2 = p + v;
                return true;
            }
            return false;
        }
    }
}
