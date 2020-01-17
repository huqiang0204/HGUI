using huqiang.Core.HGUI;
using System;
using UnityEngine;
using huqiang.UIComposite;
using huqiang.UIEvent;
using huqiang;

namespace Assets.Scripts
{
    public class ChatPage:UIPage
    {
        class View
        {
            public HImage chatbox;
            public HText input;
            public HImage Scroll;
            public UIElement opts;
            public UIElement left;
            public UIElement center;
            public UIElement right;
        }
  
        class ChatItem
        {
            public HImage head;
            public HText name;
            public HImage box;
            public HText content;
        }
        class TipItem
        {
            public HText content;
        }
        class ChatData
        {
            public string picName;
            public string name;
            public string content;
        }
        class TipData
        {
            public string content;
        }
        View view;
        OptionGroup option;
        string opt;
        UIContainer container;
        UILinker<ChatItem, ChatData> other;
        UILinker<ChatItem, ChatData> self;
        UILinker<TipItem, TipData> tip;
        public override void Initial(Transform parent, object dat = null)
        {
            base.Initial(parent, dat);
            view = LoadUI<View>("baseUI", "chat");
            option = new OptionGroup();
            option.AddEvent(view.left.userEvent);
            option.AddEvent(view.right.userEvent);
            option.AddEvent(view.center.userEvent);
            option.SelectChanged = SelectChanged;
            option.Selecet = view.right.userEvent;
            TextInput input = view.input.userEvent as TextInput;
            input.OnSubmit = OnSubmit;
            container = view.chatbox.composite as UIContainer;
            other = container.RegLinker<ChatItem, ChatData>("other");
            other.CalculItemHigh = GetContentSize;
            self = container.RegLinker<ChatItem, ChatData>("self");
            self.CalculItemHigh = GetContentSize;
            container.RegLinker<TipItem, TipData>("tip");
        }
        void SelectChanged(OptionGroup option,UserAction action)
        {
            var ue = option.LastSelect;
            if(ue!=null)
            {
                var trans = ue.Context.transform;
                trans.GetComponentInChildren<HImage>().Chromatically = 0x5E5E5EFF.ToColor();
                trans.GetComponentInChildren<HText>().Chromatically = Color.white;
            }
            ue = option.Selecet;
            if(ue!=null)
            {
                opt = ue.Context.name;
                var trans = ue.Context.transform;
                trans.GetComponentInChildren<HImage>().Chromatically = Color.blue;
                trans.GetComponentInChildren<HText>().Chromatically = Color.red;
            }
        }
        void OnSubmit(TextInput input)
        {
            string str = input.InputString;
            if (str == "")
                return;
            switch (opt)
            {
                case "left":
                    ChatData chat = new ChatData();
                    chat.name = "胡江海";
                    chat.content = str;
                    other.AddData(chat);
                    break;
                case "center":
                    break;
                case "right":
                    chat = new ChatData();
                    chat.name = "江海胡";
                    chat.content = str;
                    other.AddData(chat);
                    container.Move(0);
                    break;
            }
            input.InputString = "";
        }
        float GetContentSize(ChatItem chat, ChatData data)
        {
            Vector2 size = new Vector2(360, 60);
            chat.content.GetPreferredHeight(ref size, data.content);
            chat.content.SizeDelta = size;
            return size.y;
        }
        float GetTipSize(TipItem tip,TipData data)
        {
            return 40;
        }
    }
}
