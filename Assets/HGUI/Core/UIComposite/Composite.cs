using huqiang.Core.HGUI;
using huqiang.Data;
using System;

namespace huqiang.UIComposite
{
    public abstract class Composite
    {
        public FakeStruct BufferData;
        public UIElement Enity;
        public virtual void Initial(FakeStruct mod, UIElement element)
        {
            BufferData = mod;
            Enity = element;
            Enity.composite = this;
        }
        public virtual void Update(float time)
        {
        }
    }
}
