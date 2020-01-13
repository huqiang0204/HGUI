using huqiang.Core.HGUI;
using huqiang.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace huqiang.UIComposite
{
    public abstract class Composite
    {
        public FakeStruct BufferData;
        public AsyncScript Enity;
        public virtual void Initial(FakeStruct mod, AsyncScript script)
        {
            BufferData = mod;
            Enity = script;
            Enity.composite = this;
        }
    }
}
