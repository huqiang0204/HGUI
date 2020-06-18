using huqiang.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace huqiang.Unit
{
    public class Unit
    {
        public Vector2 size;
        public Color32 color = Color.white;
        public Vector2[] uv;
        public Vector2 pivot=new Vector2(0.5f,0.5f);
        public Vector3 localPosition;
        public Vector3 localScale = Vector3.one;
        public Quaternion localRoate = Quaternion.identity;
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
            v = localRoate * v + localPosition;
            vertex.Add(v);
            v.x = lx;
            v.y = ty;
            v = localRoate * v + localPosition;
            vertex.Add(v);
            v.x = rx;
            v.y = ty;
            v = localRoate * v + localPosition;
            vertex.Add(v);
            v.x = rx;
            v.y = dy;
            v = localRoate * v + localPosition;
            vertex.Add(v);
            carrier.uv.AddRange(uv);
            for (int i = 0; i < 4; i++)
                carrier.colors.Add(color);
            var tris = carrier.tris;
            tris.Add(s);
            tris.Add(s+1);
            tris.Add(s+2);
            tris.Add(s);
            tris.Add(s+2);
            tris.Add(s+3);
        }
        public virtual void OnUpdate(float time)
        {
        }
        public virtual void Oncollision(Unit other)
        {
        }
        public UCollider2D collider;
    }
    public interface Manager
    {
        void Update(float time);
        void UpdateMesh();
        void ApplyToMesh();
        void UpdateCollider();
    }
    public class UnitCarrier
    {
        public static Transform Root;
        public List<Vector3> vertex = new List<Vector3>();
        public List<Vector2> uv = new List<Vector2>();
        public List<int> tris = new List<int>();
        public List<Color32> colors = new List<Color32>();
        public List<Unit> Units = new List<Unit>();
        public CollisionType collisionType;
        public GameObject game;
        public MeshFilter meshFilter;
        public MeshRenderer renderer;
        public string textrue;
        public float level;
        public void Initial(string name = "car")
        {
            game = new GameObject(name);
            meshFilter = game.AddComponent<MeshFilter>();
            renderer = game.AddComponent<MeshRenderer>();
            renderer.material = new Material(Shader.Find("Universal Render Pipeline/2D/Sprite-Lit-Default"));
            var trans = game.transform;
            trans.SetParent(Root);
            trans.localScale = Vector3.one;
            trans.localPosition = new Vector3(0,0,level);
        }
        public void AddUnit(Unit dat)
        {
            Units.Add(dat);
        }
    }
    public class UnitManager<T>:UnitCarrier,Manager where T:Unit,new()
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
                    Units.RemoveAt(i);
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
    public enum CollisionType
    {
        None,
        Cricle,
        Pollygon,
        Line,
        Dot,
        Capsules
    }
}
