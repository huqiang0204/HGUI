using huqiang.Core.HGUI;
using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class ChatPage:UIPage
    {
        class View
        {
            public HImage chatbox;
            public HText input;
            public HImage Scroll;
        }
        View view;
        public override void Initial(Transform parent, object dat = null)
        {
            base.Initial(parent, dat);
            view = LoadUI<View>("baseUI", "chat");
            
        }
    }
}
