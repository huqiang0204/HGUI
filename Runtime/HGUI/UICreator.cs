using System;
using UnityEngine;

namespace huqiang.Core.HGUI
{
    public class UICreator
    {
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
            var img = new UIElement();
            img.name = name;
            img.SizeDelta = size;
            img.SetParent(parent);
            img.localPosition = pos;
            //trans.localScale = Vector3.one;
            //trans.localRotation = Quaternion.identity;
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
        public static HImage CreateHImage(Vector3 pos,Vector2 size, string name,UIElement parent)
        {
            var img = new HImage();
            img.name = name;
            img.SizeDelta = size;
            img.SetParent(parent);
            img.localPosition = pos;
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
        public static HText CreateHText(Vector3 pos, Vector2 size, string name, UIElement parent)
        {
            var img = new HText();
            img.name = name;
            img.SizeDelta = size;
            img.SetParent(parent);
            img.localPosition = pos;
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
        public static TextBox CreateTextBox(Vector3 pos, Vector2 size, string name, UIElement parent)
        {
            var img = new TextBox();
            img.name = name;
            img.SizeDelta = size;
            img.SetParent(parent);
            img.localPosition = pos;
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
        public static HLine CreateHLine(Vector3 pos, Vector2 size, string name,UIElement parent)
        {
            var img = new HLine();
            img.name = name;
            img.SizeDelta = size;
            img.SetParent(parent);
            img.localPosition = pos;
            return img;
        }
    }
}
