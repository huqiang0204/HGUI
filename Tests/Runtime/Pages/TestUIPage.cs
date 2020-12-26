using huqiang.Core.HGUI;
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
    public class TestUPage : UIPage
    {
        class View
        {
            public HText  textinput;
            public UserEvent last;
            public UserEvent next;
        }
        View view;

        public override void Initial(UIElement parent, object dat = null)
        {
            base.Initial(parent, dat);
            view = LoadUI<View>("baseUI", "test");
            view.last.Click = (o, e) => { LoadPage<StartPage>(); };
            view.next.Click = (o, e) => { LoadPage<ChatPage>(); };
        }
    }
}
