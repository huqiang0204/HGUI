using huqiang.Core.HGUI;
using huqiang.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace huqiang.UIComposite
{
    public abstract class BaseComposite
    {
        public FakeStruct BufferData;
        public AsyncScript Enity;
        public virtual void Initial(FakeStruct mod, AsyncScript trans)
        {
            BufferData = mod;
            Enity = trans;
        }
    }
}
