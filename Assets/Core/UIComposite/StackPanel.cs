using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using huqiang.Core.HGUI;
using huqiang.Data;

namespace huqiang.UIComposite
{
    public enum Direction
    {
        Horizontal,
        Vertical
    }
    public class StackPanel:Composite
    {
        public override void Initial(FakeStruct mod, AsyncScript script)
        {
            base.Initial(mod, script);
        }
    }
}
