using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace huqiang.Unit
{
    /// <summary>
    /// 网格单元
    /// </summary>
    public class Unit
    {
        public Vector2 size;
        public Color32 color = Color.white;
        public Vector2[] uv;
        public Vector2 pivot = new Vector2(0.5f, 0.5f);
        public Vector3 localPosition;
        public Vector3 localScale = Vector3.one;
        public Quaternion localRotate = Quaternion.identity;
        public float time;
        public bool active = true;
        public bool show = true;
        public virtual void CreateMesh(UnitCarrier carrier)
        {
            if (uv == null)
                return;
            if (uv.Length != 4)
                return;
            var vertex = carrier.vertex;
            int s = vertex.Count;
            float lx = size.x * -pivot.x;
            float rx = lx + size.x;
            lx *= localScale.x;
            rx *= localScale.x;
            float dy = size.y * -pivot.y;
            float ty = dy + size.y;
            dy *= localScale.y;
            ty *= localScale.y;

            Vector3 v = Vector3.zero;
            v.x = lx;
            v.y = dy;
            v = localRotate * v + localPosition;
            vertex.Add(v);
            v.x = lx;
            v.y = ty;
            v = localRotate * v + localPosition;
            vertex.Add(v);
            v.x = rx;
            v.y = ty;
            v = localRotate * v + localPosition;
            vertex.Add(v);
            v.x = rx;
            v.y = dy;
            v = localRotate * v + localPosition;
            vertex.Add(v);
            carrier.uv.AddRange(uv);
            for (int i = 0; i < 4; i++)
                carrier.colors.Add(color);
            var tris = carrier.tris;
            tris.Add(s);
            tris.Add(s + 1);
            tris.Add(s + 2);
            tris.Add(s);
            tris.Add(s + 2);
            tris.Add(s + 3);
        }
        public virtual void OnUpdate(float time)
        {
        }
        public virtual void OnCollision(Unit other)
        {
        }
        public UCollider2D collider;
    }
    /// <summary>
    /// 热更新网格单元
    /// </summary>
    public class HotUnit : Unit
    {
        public object Context;
        public Action<UnitCarrier> createMesh;
        public Action<Unit> onCollision;
        public Action<float> onUpdate;
        public override void CreateMesh(UnitCarrier carrier)
        {
            if (createMesh != null)
                createMesh(carrier);
            else base.CreateMesh(carrier);
        }
        public override void OnCollision(Unit other)
        {
            if (onCollision != null)
                onCollision(other);
        }
        public override void OnUpdate(float time)
        {
            if (onUpdate != null)
                onUpdate(time);
        }
    }
}
