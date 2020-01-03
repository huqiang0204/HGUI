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

        }
        View view;
        public override void Initial(Transform parent, object dat = null)
        {
            view = LoadUI<View>("baseUI", "anitest");
            base.Initial(parent, dat);
        }
    }
}
