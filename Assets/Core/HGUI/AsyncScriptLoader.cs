using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Core.HGUI
{
    public unsafe struct AsyncScriptData
    {
        public Vector2 sizeDelta;
        public Vector2 pivot;
        public ScaleType scaleType;
        public AnchorType sizeType;
        public AnchorPointType anchorType;
        public Margin margin;
        public static int Size = sizeof(AsyncScriptData);
        public static int ElementSize = Size / 4;
    }
    public class AsyncScriptLoader
    {
    }
}
