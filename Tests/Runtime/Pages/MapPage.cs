using huqiang.UIComposite;
using huqiang.UIEvent;
using huqiang.UIModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class MapPage:UIPage
    {
        class View
        {
            public LBSMap lbsMap;
            public UserEvent Locate;
            public UserEvent last;
            public UserEvent next;
        }
        View view;
        public override void Initial(Transform parent, object dat = null)
        {
            base.Initial(parent, dat);
            view = LoadUI<View>("baseUI", "map");//"baseUI"创建的bytes文件名,"page"为创建的页面名
            view.Locate.Click = (o, e) => { view.lbsMap.Location(); };
            view.last.Click = (o, e) => { LoadPage<ScrollPage>(); };
            view.next.Click = (o, e) => { LoadPage<DockPage>(); };
        }
    }
}
