using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using huqiang.Data;
using UnityEngine;

namespace Assets.Core.HGUI
{
    public unsafe struct HTextData
    {
        public HGraphicsData graphicsData;
        public Int32 text;
        public Int32 font;
        public Vector2 pivot;
        public HorizontalWrapMode m_hof;
        public VerticalWrapMode m_vof;
        public TextAnchor anchor;
        public bool m_richText;
        public float m_lineSpace;
        public int m_fontSize;
        public bool m_align;
        public static int Size = sizeof(HTextData);
        public static int ElementSize = Size / 4;
    }
    public class HTextLoader:HGraphicsLoader
    {
        public override void LoadToObject(FakeStruct fake, Component com)
        {
            base.LoadToObject(fake, com);
        }
        public override FakeStruct LoadFromObject(Component com, DataBuffer buffer)
        {
            return base.LoadFromObject(com, buffer);
        }
    }
}
