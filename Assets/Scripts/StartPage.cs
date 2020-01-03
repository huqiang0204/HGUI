using Assets.Core.HGUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class StartPage:UIPage
    {
        class View
        {
            public UserEvent img;
            public HText t1;
        }
        View view;
        public override void Initial(Transform parent, object dat = null)
        {
            base.Initial(parent, dat);
            view = LoadUI<View>("baseUI", "start");
        }
    }
}
