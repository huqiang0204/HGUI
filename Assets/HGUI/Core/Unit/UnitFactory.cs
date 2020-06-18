using huqiang.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace huqiang.Unit
{
    public enum MeshType
    {
        Box,
        Custom
    }
    public class UnitFactory
    {
        public List<ManagerInfo> managers = new List<ManagerInfo>();
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
        public void Update(float time)
        {
            for (int i = 0; i < managers.Count; i++)
                managers[i].Instance.Update(time);
        }
        public void UpdateMesh()
        {
            for (int i = 0; i < managers.Count; i++)
            {
                managers[i].Instance.UpdateMesh();
                managers[i].Instance.ApplyToMesh();
            }
        }
        public void UpdateCollider()
        {
            for (int i = 0; i < managers.Count; i++)
            {
                managers[i].Instance.UpdateCollider();
            }
        }
        public void DisposeAll()
        {
            for (int i = 0; i < managers.Count; i++)
                GameObject.Destroy(managers[i].carrier.game);
            managers.Clear();
        }
    }
    public class ManagerInfo
    {
        public Manager Instance;
        public UnitCarrier carrier;
        public string textureName;
        public string shaderName;
        public CollisionType collisionType;
    }
}
