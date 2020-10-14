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
            public UISlider MapScale;
        }
        View view;
        public override void Initial(Transform parent, object dat = null)
        {
            base.Initial(parent, dat);
            view = LoadUI<View>("baseUI", "map");//"baseUI"创建的bytes文件名,"page"为创建的页面名
            InitialEvent();
        }
        int CurLevel = 18;
        void InitialEvent()
        {
            view.Locate.Click = (o, e) => {
#if UNITY_EDITOR
                view.lbsMap.Location(113.904228210449f, 22.5839462280273f);
#else
                view.lbsMap.Location();
#endif
            };
            view.last.Click = (o, e) => { LoadPage<ScrollPage>(); };
            view.next.Click = (o, e) => { LoadPage<DockPage>(); };
            view.MapScale.ValueEndChange = (o) => {
                float r = (int)Mathf.Round(o.Percentage * 12);
                int level = 6 + (int)r;
                view.MapScale.Percentage = r / 12;
                if(level!=CurLevel)
                {
                    CurLevel = level;
                    view.lbsMap.SetLevel(level);
                }
            };
        }
    }
}
