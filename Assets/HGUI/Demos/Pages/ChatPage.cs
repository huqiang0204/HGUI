using huqiang.Core.HGUI;
using System;
using UnityEngine;
using huqiang.UIComposite;
using huqiang.UIEvent;
using huqiang;
using huqiang.Data;

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
            public UIElement treeView;
            public HImage send;
            public UserEvent last;
            public UserEvent next;
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
        TextInput input;
        public override void Initial(Transform parent, object dat = null)
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
            input = view.input.userEvent as TextInput;
           
            input.OnSubmit = OnSubmit;
            container = view.chatbox.composite as UIContainer;
            other = container.RegLinker<ChatItem, ChatData>("other");
            other.CalculItemHigh = GetContentSize;
            other.ItemUpdate = ItemUpdate;
            self = container.RegLinker<ChatItem, ChatData>("self");
            self.CalculItemHigh = GetContentSize;
            self.ItemUpdate = ItemUpdate;
            container.RegLinker<TipItem, TipData>("tip");
            view.send.userEvent.Click = (o, e) => {OnSubmit(input); };
        }
        void SelectChanged(OptionGroup option,UserAction action)
        {
            var ue = option.LastSelect;
            if(ue!=null)
            {
                var trans = ue.Context.transform;
                trans.GetComponentInChildren<HImage>().MainColor = 0x5E5E5EFF.ToColor();
                trans.GetComponentInChildren<HText>().MainColor = Color.white;
            }
            ue = option.Selecet;
            if(ue!=null)
            {
                opt = ue.Context.name;
                var trans = ue.Context.transform;
                trans.GetComponentInChildren<HImage>().MainColor = Color.blue;
                trans.GetComponentInChildren<HText>().MainColor = Color.red;
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
                    chat.name = "江海胡";
                    chat.content = str;
                    other.AddAndMove(chat);
                    break;
                case "center":
                    break;
                case "right":
                    chat = new ChatData();
                    chat.name = "胡强";
                    chat.content = str;
                    self.AddAndMove(chat);
                    //container.Move(0);
                    break;
            }
            input.InputString = "";
        }
        float GetContentSize(ChatItem chat, ChatData data)
        {
            if (data.conSize == Vector2.zero)
            {
                Vector2 size = new Vector2(360, 60);
                chat.content.GetPreferredSize(ref size, data.content);
                size.x += 8;
                size.y += 8;
                data.conSize = size;
            }
            chat.content.SizeDelta = data.conSize;
            var s = data.conSize;
            s.x += 10;
            s.y += 10;
            chat.box.SizeDelta = s;
            var ui = chat.box.transform.parent.GetComponent<UIElement>();
            s.y += 40;
            s.x = 600;
            ui.SizeDelta = s;
            UIElement.ResizeChild(ui);
            return s.y;
        }
        void ItemUpdate(ChatItem chat,ChatData data,int index)
        {
            chat.content.Text = data.content;
            chat.name.Text = data.name;
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
