using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace huqiang.Core.HGUI
{
    public struct HGUIElement
    {
        /// <summary>
        /// 子元素数量
        /// </summary>
        public int childCount;
        /// <summary>
        /// 子元素在流水线中的其实位置
        /// </summary>
        public int childOffset;
        /// <summary>
        /// 夫元素在流水线中的位置
        /// </summary>
        public int parentIndex;
        public Vector3 localPosition;
        public Quaternion localRotation;
        public Vector3 localScale;
        /// <summary>
        /// 相对于画布的坐标
        /// </summary>
        public Vector3 Position;
        /// <summary>
        /// 相对于画布的旋转角
        /// </summary>
        public Quaternion Rotation;
        /// <summary>
        /// 相对于画布的缩放
        /// </summary>
        public Vector3 Scale;
        /// <summary>
        /// 是否是激活状态
        /// </summary>
        public bool active;
        public UIElement script;
    }
}
