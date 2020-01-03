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
        public override void Initial(Transform parent, object dat = null)
        {
            //model = HGUIManager.FindModel("baseUI", "anitest");
            base.Initial(parent, dat);
        }
    }
}
