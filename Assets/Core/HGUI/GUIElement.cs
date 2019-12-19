using System;
using UnityEngine;

namespace Assets.Core.HGUI
{
    public struct GUIElement
    {
        public int childCount;
        public int parentIndex;
        public Vector3 localPos;
        public Quaternion localRot;
        public Vector3 localScale;
        public Transform trans;
        public AsyncScript script;
    }
}
