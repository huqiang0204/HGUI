using System.Collections.Generic;
using UnityEngine;

namespace huqiang.Unit
{
    /// <summary>
    /// 单元工厂
    /// </summary>
    public class UnitFactory
    {
        public static List<UnitFactory> factories;
        public static bool Open = false;
        public static void Update(float time)
        {
            if(Open)
            {
                if (factories == null)
                    return;
                for (int i = 0; i < factories.Count; i++)
                    factories[i].Refresh(time);
                for (int i = 0; i < factories.Count; i++)
                    factories[i].RefreshMesh();
                CheckCollision(time);
            }
        }

        static void CheckCollision(float time)
        {
            ThreadMission.AddMission((o)=> {
                for (int i = 0; i < factories.Count; i++)
                    factories[i].RefreshCollider();
                UnitCollision.Collision();
            },null);
        }
        public UnitFactory()
        {
            if (factories == null)
                factories = new List<UnitFactory>();
            factories.Add(this);
        }
        public List<ManagerInfo> managers = new List<ManagerInfo>();
        public HotUnitManager CreateHotUnitManager(string tex, string shader, CollisionType collisionType)
        {
            ManagerInfo m = null;
            for (int i = 0; i < managers.Count; i++)
            {
                m = managers[i];
                if (m.textureName == tex)
                    if (m.shaderName == shader)
                        if (m.collisionType == collisionType)
                        {
                            return m.Instance as HotUnitManager;
                        }
            }
            m = new ManagerInfo();
            var ins = new HotUnitManager();
            ins.collisionType = collisionType;
            m.Instance = ins;
            m.carrier = ins;
            m.textureName = tex;
            m.shaderName = shader;
            m.collisionType = collisionType;
            managers.Add(m);
            ins.textrue = tex;
            return ins;
        }
        public UnitManager<T> CreateUnitManager<T>(string tex,string shader, CollisionType collisionType) where T : Unit, new()
        {
            ManagerInfo m = null;
            for (int i = 0; i < managers.Count; i++)
            {
                m = managers[i];
                if (m.textureName == tex)
                    if (m.shaderName == shader)
                        if (m.collisionType == collisionType)
                        {
                            return m.Instance as UnitManager<T>;
                        }
            }
            m = new ManagerInfo();
            var ins = new UnitManager<T>();
            ins.collisionType = collisionType;
            m.Instance = ins;
            m.carrier = ins;
            m.textureName = tex;
            m.shaderName = shader;
            m.collisionType = collisionType;
            managers.Add(m);
            ins.textrue = tex;
            return ins;
        }
        void Refresh(float time)
        {
            for (int i = 0; i < managers.Count; i++)
                managers[i].Instance.Update(time);
        }
        void RefreshMesh()
        {
            for (int i = 0; i < managers.Count; i++)
            {
                managers[i].Instance.UpdateMesh();
                managers[i].Instance.ApplyToMesh();
            }
        }
        void RefreshCollider()
        {
            for (int i = 0; i < managers.Count; i++)
            {
                managers[i].Instance.UpdateCollider();
            }
        }
        public void ClearAll()
        {
            for (int i = 0; i < managers.Count; i++)
                GameObject.Destroy(managers[i].carrier.game);
            managers.Clear();
        }
        public void Dispose()
        {
            factories.Remove(this);
        }
    }
    public class ManagerInfo
    {
        public ICarrier Instance;
        public UnitCarrier carrier;
        public string textureName;
        public string shaderName;
        public CollisionType collisionType;
    }
}
