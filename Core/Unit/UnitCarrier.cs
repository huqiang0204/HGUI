using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace huqiang.Unit
{
    public interface ICarrier
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
        protected List<Unit> buffer = new List<Unit>();
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
            renderer.material = new Material(Shader.Find("Sprites/Default"));
            var trans = game.transform;
            trans.SetParent(Root);
            trans.localScale = Vector3.one;
            trans.localPosition = new Vector3(0, 0, level);
        }
        public void AddUnit(Unit dat)
        {
            Units.Add(dat);
        }
    }
}
