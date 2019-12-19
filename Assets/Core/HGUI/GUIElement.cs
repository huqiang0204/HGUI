using System;
using UnityEngine;

namespace Assets.Core.HGUI
{
    public struct GUIElement
    {
        public int childCount;
        public int parentIndex;
        public Vector3 localPosition;
        public Quaternion localRotation;
        public Vector3 localScale;
        public Transform trans;
        public AsyncScript script;
    }
}
