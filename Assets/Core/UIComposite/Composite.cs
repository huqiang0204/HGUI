using huqiang.Core.HGUI;
using huqiang.Data;
using System;

namespace huqiang.UIComposite
{
    public abstract class Composite
    {
        public FakeStruct BufferData;
        public UIElement Enity;
        public virtual void Initial(FakeStruct mod, UIElement script)
        {
            BufferData = mod;
            Enity = script;
            Enity.composite = this;
        }
        //public virtual void Update()
        //{
        //}
    }
}
