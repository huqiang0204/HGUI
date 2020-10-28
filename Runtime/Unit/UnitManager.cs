using huqiang.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace huqiang.Unit
{
    /// <summary>
    /// 单元管理器
    /// </summary>
    public class HotUnitManager : UnitCarrier, ICarrier
    {
        public HotUnit AddUnit()
        {
            if(buffer.Count>0)
            {
                var t = buffer[0];
                buffer.RemoveAt(0);
                t.active = true;
                Units.Add(t);
                return t as HotUnit;
            }
            var bul = new HotUnit();
            Units.Add(bul);
            return bul;
        }
        public void Update(float time)
        {
            int c = Units.Count;
            for (int i = c - 1; i >= 0; i--)
            {
                var bu = Units[i];
                bu.OnUpdate(time);
                if (!bu.active)
                { 
                    Units.RemoveAt(i);
                    buffer.Add(bu);
                }
            }
        }
        public void UpdateMesh()
        {
            if (game == null)
            {
                Initial(textrue);
                if (textrue != null)
                    renderer.material.mainTexture = ElementAsset.FindTexture("picture.unity3d", textrue);
            }
            vertex.Clear();
            uv.Clear();
            tris.Clear();
            colors.Clear();
            for (int i = 0; i < Units.Count; i++)
                if (Units[i] != null)
                    if (Units[i].show)
                        Units[i].CreateMesh(this);
        }
        public void ApplyToMesh()
        {
            var mesh = meshFilter.mesh;
            if (mesh == null)
            {
                mesh = new Mesh();
                meshFilter.mesh = mesh;
            }
            mesh.Clear();
            mesh.vertices = vertex.ToArray();
            mesh.uv = uv.ToArray();
            mesh.triangles = tris.ToArray();
            if (colors.Count > 0)
                mesh.colors32 = colors.ToArray();
        }
        public void UpdateCollider()
        {
            for (int i = 0; i < Units.Count; i++)
            {
                var uni = Units[i];
                var col = uni.collider;
                if (col != null)
                {
                    col.Target = uni;
                    col.Update();
                }
            }
        }
    }
    public class UnitManager<T>:UnitCarrier,ICarrier where T:Unit,new()
    {
        public T AddUnit()
        {
            var bul = new T();
            Units.Add(bul);
            return bul;
        }
        public void Update(float time)
        {
            int c = Units.Count;
            for (int i = c - 1; i >= 0; i--)
            {
                var bu = Units[i];
                bu.OnUpdate(time);
                if (!bu.active)
                {
                    Units.RemoveAt(i);
                    buffer.Add(bu);
                }
            }
        }
        public void UpdateMesh()
        {
            if(game==null)
            {
                Initial(textrue);
                if(textrue!=null)
                renderer.material.mainTexture = ElementAsset.FindTexture("picture.unity3d", textrue);
            }
            vertex.Clear();
            uv.Clear();
            tris.Clear();
            colors.Clear();
            for (int i = 0; i < Units.Count; i++)
                if (Units[i] != null)
                    if(Units[i].show)
                        Units[i].CreateMesh(this);
        }
        public void ApplyToMesh()
        {
            var mesh = meshFilter.mesh;
            if(mesh==null)
            {
                mesh = new Mesh();
                meshFilter.mesh = mesh;
            }
            mesh.Clear();
            mesh.SetVertices(vertex);//vertices = vertex.ToArray();
            mesh.SetUVs(0,uv);//.uv = uv.ToArray();
            mesh.SetTriangles(tris,0);//triangles = tris.ToArray();
            if (colors.Count > 0)
                mesh.SetColors(colors);//.colors32 = colors.ToArray();
        }
        public void UpdateCollider()
        {
            for (int i = 0; i < Units.Count; i++)
            {
                var uni = Units[i];
                var col = uni.collider;
                if (col != null)
                {
                    col.Target = uni;
                    col.Update();
                }
            }
        }
    }
}
