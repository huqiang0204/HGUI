using huqiang.Core.HGUI;
using System;
using UnityEngine;

namespace huqiang.UIComposite
{
    public class UICreator
    {
        private const string icons = "icons";
        private const string Aim = "Aim";
        private const string background = "Background2";
        private const string file = "Pinned-Notices";
        private const string close = "Close";
        private const string list = "list";
        private const string line = "Line";
        private const string leaves = "Leaves";
        private const string ufo = "Ufo";
        private const string circleol = "Circle-Outline";
        private const string circlesm = "Circle-Small";
        private const string magicstick = "Magic-Stick";
        public static UIElement CreateElement(Vector3 pos, Vector2 size, string name, Transform parent)
        {
            var go = new GameObject(name);
            var img = go.AddComponent<UIElement>();
            img.SizeDelta = size;
            var trans = go.transform;
            trans.SetParent(parent);
            trans.localPosition = pos;
            trans.localScale = Vector3.one;
            trans.localRotation = Quaternion.identity;
            return img;
        }
        public static HImage CreateHImage(Vector3 pos,Vector2 size, string name,Transform parent)
        {
            var go = new GameObject(name);
            var img = go.AddComponent<HImage>();
            img.SizeDelta = size;
            var trans = go.transform;
            trans.SetParent(parent);
            trans.localPosition = pos;
            trans.localScale = Vector3.one;
            trans.localRotation = Quaternion.identity;
            return img;
        }
        public static HText CreateHText(Vector3 pos, Vector2 size, string name, Transform parent)
        {
            var go = new GameObject(name);
            var img = go.AddComponent<HText>();
            img.SizeDelta = size;
            var trans = go.transform;
            trans.SetParent(parent);
            trans.localPosition = pos;
            trans.localScale = Vector3.one;
            trans.localRotation = Quaternion.identity;
            return img;
        }
    }
}
