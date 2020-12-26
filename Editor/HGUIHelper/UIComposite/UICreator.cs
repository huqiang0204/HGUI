using huqiang.Helper.HGUI;
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
        /// <summary>
        /// 创建一个元素
        /// </summary>
        /// <param name="pos">位置</param>
        /// <param name="size">尺寸</param>
        /// <param name="name">名称</param>
        /// <param name="parent">父坐标变换</param>
        /// <returns></returns>
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
        /// <summary>
        /// 创建一个元素
        /// </summary>
        /// <param name="pos">位置</param>
        /// <param name="size">尺寸</param>
        /// <param name="name">名称</param>
        /// <param name="parent">父坐标变换</param>
        /// <returns></returns>
        public static UIElement CreateElement(Vector3 pos, Vector2 size, string name, UIElement parent)
        {
            var go = new GameObject(name);
            var img = go.AddComponent<UIElement>();
            img.SizeDelta = size;
            img.SetParent(parent);
            img.localPosition = pos;
            img.localScale = Vector3.one;
            img.localRotation = Quaternion.identity;
            return img;
        }
        /// <summary>
        /// 创建一个图像
        /// </summary>
        /// <param name="pos">位置</param>
        /// <param name="size">尺寸</param>
        /// <param name="name">名称</param>
        /// <param name="parent">父坐标变换</param>
        /// <returns></returns>
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
        /// <summary>
        /// 创建一个文本
        /// </summary>
        /// <param name="pos">位置</param>
        /// <param name="size">尺寸</param>
        /// <param name="name">名称</param>
        /// <param name="parent">父坐标变换</param>
        /// <returns></returns>
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
        /// <summary>
        /// 创建一个文本
        /// </summary>
        /// <param name="pos">位置</param>
        /// <param name="size">尺寸</param>
        /// <param name="name">名称</param>
        /// <param name="parent">父坐标变换</param>
        /// <returns></returns>
        public static HText CreateTextBox(Vector3 pos, Vector2 size, string name, Transform parent)
        {
            var go = new GameObject(name);
            var img = go.AddComponent<TextBox>();
            img.SizeDelta = size;
            var trans = go.transform;
            trans.SetParent(parent);
            trans.localPosition = pos;
            trans.localScale = Vector3.one;
            trans.localRotation = Quaternion.identity;
            return img;
        }
        /// <summary>
        /// 创建一个画线面板
        /// </summary>
        /// <param name="pos">位置</param>
        /// <param name="size">尺寸</param>
        /// <param name="name">名称</param>
        /// <param name="parent">父坐标变换</param>
        /// <returns></returns>
        public static HLine CreateHLine(Vector3 pos, Vector2 size, string name, Transform parent)
        {
            var go = new GameObject(name);
            var img = go.AddComponent<HLine>();
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
