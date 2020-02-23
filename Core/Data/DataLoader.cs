using huqiang.Data;
using System;
using UnityEngine;

namespace huqiang.Data
{
    public abstract class Initializer
    {
        public virtual void Initialiezd(FakeStruct fake, Component com)
        {
        }
        public virtual void Reset(object obj) { }
        public virtual void Done() { }
    }
    public abstract class DataLoader
    {
        public GameobjectBuffer gameobjectBuffer;
        public virtual void LoadToComponent(FakeStruct fake, Component com, FakeStruct main) { }
        public virtual void LoadToObject(FakeStruct fake, Component com, Initializer initializer) { }
        public virtual FakeStruct LoadFromObject(Component com, DataBuffer buffer) { return null; }
    }
}
