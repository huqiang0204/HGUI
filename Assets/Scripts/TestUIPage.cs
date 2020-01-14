using huqiang.Core.HGUI;
using huqiang.UIComposite;
using huqiang.UIEvent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class TestUPage : UIPage
    {
        class View
        {
            public HText  textinput;
        }
        View view;

        public override void Initial(Transform parent, object dat = null)
        {
            base.Initial(parent, dat);
            view = LoadUI<View>("baseUI", "test");
        }
    }
}
