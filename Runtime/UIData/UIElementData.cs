using huqiang.Core.HGUI;
using huqiang.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace huqiang.Core.UIData
{
    public enum HEventType
    {
        None,
        UserEvent,
        TextInput,
        GestureEvent
    }
    public enum CompositeType
    {
        None,
        Slider,
        ScrollX,
        ScrollY,
        GridScroll,
        Paint,
        Rocker,
        UIContainer,
        TreeView,
        UIDate,
        UIPalette,
        ScrollYExtand,
        DropDown,
        StackPanel,
        TabControl,
        DockPanel,
        DesignedDockPanel,
        DragContent,
        DataGrid,
        InputBox,
        PopMenu
    }
    public unsafe struct UIElementData
    {
        /// <summary>
        /// 对象类型, 所有组件的位或值
        /// </summary>
        public Int32 type;
        /// <summary>
        /// 实例ID
        /// </summary>
        public Int32 insID;
        public bool activeSelf;
        public StringPoint name;
        public Vector3 localScale;
        public Vector3 localPosition;
        public Quaternion localRotation;
        public Vector2 m_sizeDelta;
        public Vector2 Pivot;
        public ScaleType scaleType;
        public AnchorType anchorType;
        public AnchorPointType anchorPointType;
        public Vector2 anchorOffset;
        public MarginType marginType;
        public ParentType parentType;
        public Margin margin;
        public HEventType eventType;
        public CompositeType compositeType;
        public Int16ArrayPoint child;
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
        public bool Mask;

        public static int Size = sizeof(UIElementData);
        public static int ElementSize = Size / 4;
    }
}
