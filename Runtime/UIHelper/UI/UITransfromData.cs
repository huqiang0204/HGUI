using huqiang.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace huqiang.Helper.HGUI
{
    public unsafe struct UITransfromData
    {
        /// <summary>
        /// 对象类型, 所有组件的位或值
        /// </summary>
        public Int64 type;
        /// <summary>
        /// 实例ID
        /// </summary>
        public Int32 insID;
        public Vector3 localEulerAngles;
        public Vector3 localPosition;
        public Vector3 localScale;
        /// <summary>
        /// UI元素尺寸
        /// </summary>
        public Vector2 size;
        /// <summary>
        /// 轴心
        /// </summary>
        public Vector2 pivot;
        /// <summary>
        /// 名称
        /// </summary>
        public StringPoint name;
        /// <summary>
        /// 标记
        /// </summary>
        public StringPoint tag;
        /// <summary>
        /// int32数组,高16位为索引,低16位为类型
        /// </summary>
        public IntArrayPoint coms;
        /// <summary>
        /// int16数组
        /// </summary>
        public Int16ArrayPoint child;
        public int layer;
        /// <summary>
        /// 用户事件的附加信息
        /// </summary>
        public FakeStrcutPoint eve;
        /// <summary>
        /// 复合型UI附加信息
        /// </summary>
        public FakeStrcutPoint composite;
        /// <summary>
        /// 附加信息,用于存储helper中写入的数据
        /// </summary>
        public FakeStrcutPoint ex;
        public static int Size = sizeof(UITransfromData);
        public static int ElementSize = Size / 4;
    }
}
