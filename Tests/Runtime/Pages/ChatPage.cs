using huqiang.Core.HGUI;
using System;
using UnityEngine;
using huqiang.UIComposite;
using huqiang.UIEvent;
using huqiang;
using huqiang.Data;
using huqiang.UIModel;

namespace Assets.Scripts
{
    public class ChatPage:UIPage
    {
        class View
        {
            public HImage chatbox;
            public InputBox input;
            public HImage Scroll;
            public UIElement opts;
            public UIElement left;
            public UIElement center;
            public UIElement right;
            public UIElement treeView;
            public HImage send;
            public UserEvent last;
            public UserEvent next;
            public HText ItemText;
            public UIElement other;
            public UIElement self;
            public UIElement tip;
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
            public Vector2 conSize;
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
        TextGenerationSettings settings;
        public override void Initial(UIElement parent, object dat = null)
        {
            base.Initial(parent, dat);
            view = LoadUI<View>("baseUI", "chat");
            InitialChat();
            view.last.Click = (o, e) => { LoadPage<TestUPage>(); };
            view.next.Click = (o, e) => { LoadPage<DrawingPage>(); };
        }
        void InitialChat()
        {
            option = new OptionGroup();
            option.AddEvent(view.left.userEvent);
            option.AddEvent(view.right.userEvent);
            option.AddEvent(view.center.userEvent);
            option.SelectChanged = SelectChanged;
            option.Selecet = view.right.userEvent;

            //input = view.input.userEvent as TextInput;
           
            view.input.OnSubmit = OnSubmit;
            container = view.chatbox.composite as UIContainer;
            //other = new UILinker<ChatItem, ChatData>(container,view.other);
            other = new UILinker<ChatItem, ChatData>(container, view.other);
            other.LayoutCallback = GetContentSize;
            other.ItemUpdate = ItemUpdate;
            self = new UILinker<ChatItem, ChatData>(container, view.self);
            self.LayoutCallback = GetContentSize;
            self.ItemUpdate = ItemUpdate;
            tip = new UILinker<TipItem, TipData>(container ,view.tip);
            //tip.ItemUpdate = TipItemUpdate;
            view.send.userEvent.Click = (o, e) => {OnSubmit(view.input); };
            view.ItemText.activeSelf = false;
            var size = view.ItemText.SizeDelta;
            view.ItemText.GetGenerationSettings(ref size, ref settings);
        }
        void SelectChanged(OptionGroup option,UserAction action)
        {
            var ue = option.LastSelect;
            if(ue!=null)
            {
                var trans = ue.Context;
                trans.GetComponentInChildren<HImage>().MainColor = 0x5E5E5EFF.ToColor();
                trans.GetComponentInChildren<HText>().MainColor = Color.white;
            }
            ue = option.Selecet;
            if(ue!=null)
            {
                opt = ue.Context.name;
                var trans = ue.Context;
                trans.GetComponentInChildren<HImage>().MainColor = Color.blue;
                trans.GetComponentInChildren<HText>().MainColor = Color.red;
            }
        }
        void OnSubmit(InputBox input)
        {
            string str = input.InputString;
            if (str == "")
                return;
            switch (opt)
            {
                case "left":
                    ChatData chat = new ChatData();
                    chat.name = "江海胡";
                    chat.content = str;
                    settings.textAnchor = TextAnchor.UpperLeft;
                    chat.conSize = HTextGenerator.GetPreferredSize(new StringEx(str,settings.richText),ref settings);
                    other.AddAndMove(chat,chat.conSize.y+10);
                    break;
                case "center":
                    TipData t = new TipData();
                    str= DateTime.Now.ToString();
                    t.content = str;
                    tip.AddAndMove(t,30);
                    break;
                case "right":
                    chat = new ChatData();
                    chat.name = "胡强";
                    chat.content = str;
                    settings.textAnchor = TextAnchor.UpperRight;
                    chat.conSize = HTextGenerator.GetPreferredSize(new StringEx(str, settings.richText), ref settings);
                    self.AddAndMove(chat, chat.conSize.y + 10);
                    break;
            }
            input.InputString = "";
        }
        float GetContentSize(UIElement mod, ChatData data)
        {
            data.conSize = HTextGenerator.GetPreferredSize(new StringEx(data.content, settings.richText), ref settings);
            float y = data.conSize.y;
            y += 60;
            if (y < 120)
                y = 120;
            mod.SizeDelta = new Vector2(600, y);
            var box = mod.Find("box");
            box.SizeDelta = new Vector2(data.conSize.x + 10, data.conSize.y + 10);
            box.GetChild(0).SizeDelta = data.conSize;
            UIElement.ResizeChild(mod);
            return y;
        }
        void ItemUpdate(ChatItem chat,ChatData data,int index)
        {
            chat.content.Text = data.content;
            chat.name.Text = data.name;
        }
        void TipItemUpdate(TipItem tip, TipData data, int index)
        {
            tip.content.Text = data.content;
        }
        float GetTipSize(TipItem tip,TipData data)
        {
            return 40;
        }
        public override void Update(float time)
        {
            
        }
    }
}
