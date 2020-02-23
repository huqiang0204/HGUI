using System;
using UnityEngine;

namespace huqiang.Core.HGUI
{
    public struct HGUIElement
    {
        public int childCount;
        public int childOffset;
        public int parentIndex;
        public Vector3 localPosition;
        public Quaternion localRotation;
        public Vector3 localScale;
        public bool active;
        public Transform trans;
        public UIElement script;
    }
}
