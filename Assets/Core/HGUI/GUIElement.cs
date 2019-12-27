using System;
using UnityEngine;

namespace Assets.Core.HGUI
{
    public struct GUIElement
    {
        public int childCount;
        public int childOffset;
        public int parentIndex;
        public Vector3 localPosition;
        public Quaternion localRotation;
        public Vector3 localScale;
        public bool active;
        public Transform trans;
        public AsyncScript script;
    }
    public struct Batch
    {
        public Material Fmaterial;
        public Material Smaterial;
        public Material Tmaterial;
        public int MatCount;
        public int CombineCount;
        HGraphics current;
        public void AddGraphics(HCanvas canvas, HGraphics graphics)
        {
            if (current == null)
            {
                current = graphics;
            }
            if (graphics.Mask)
            {

            }
            if (current.CompareMaterial(graphics))
            {

            }
            else
            {

            }
        }
    }
}
