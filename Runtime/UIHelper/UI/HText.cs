using huqiang.Core.UIData;
using huqiang.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huqiang.Helper.HGUI
{
    public struct TextVertex
    {
        public Vector3 position;
        public Color32 color;
        public Vector2 uv;
        public int Index;
    }
    public class HText: UIContext
    {
        public huqiang.Core.HGUI.HText Content;
        [SerializeField]
        [HideInInspector]
        public int ContextID;
        public void Awake()
        {
            if (Content == null)
                Content = new Core.HGUI.HText();
            ContextID = Content.GetInstanceID();
        }
        public override Core.HGUI.UIElement GetUIData()
        {
            if (Content == null)
                Content = new Core.HGUI.HText();
            return Content;
        }
    }
}
