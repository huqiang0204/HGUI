using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace huqiang.Core.HGUI
{
    public class TextCollector
    {
        struct TextInfo
        {
            public HText[] texts;
            public bool[] state;
            public int max;
            public bool end;
        }
        TextInfo[] buffer = new TextInfo[64];
        int[] indexs = new int[64];
        int point;
        int max;
        int top = 0;
        public void AddText(HText text,bool active)
        {
            var buf = buffer[point].texts;
            if (buf == null)
            {
                buf = new HText[1024];
                buffer[point].state = new bool[1024];
                buffer[point].texts = buf;
            }
            int top = buffer[point].max;
            buf[top] = text;
            buffer[point].state[top] = active;
            buffer[point].max++;
        }
        public void Clear()
        {
            for (int i = 0; i <= max; i++)
            {
                int c = buffer[i].max;
                var buf = buffer[i];
                for (int j = 0; j < c; j++)
                    buf.texts[j] = null;
                buffer[i].max = 0;
                buffer[i].end = false;
            }
            max = 1;
            point = 0;
            top = 0;
        }
        public int Next()
        {
            point++;
            top++;
            for (int i=point;i<64;i++)
            {
                if(!buffer[i].end)
                {
                    point = i;
                    indexs[top] = point;
                    break;
                }
            }
            if (max <= point)
                max = point + 1;
            return point;
        }
        public void Back()
        {
            top--;
            buffer[point].end = true;
            point = indexs[top];
        }
        public void GenerateTexture(bool force = false)
        {
            for (int i = 0; i < max; i++)
            {
                int c = buffer[i].max;
                var buf = buffer[i].texts;
                var state = buffer[i].state;
                bool dirty = force;
                if (!force)
                {
                    for (int j = 0; j < c; j++)
                    {
                        if (buf[j].m_dirty | buf[j].m_colorChanged)
                        {
                            dirty = true;
                            break;
                        }
                    }
                }
                if (dirty)
                {
                    for (int j = 0; j < c; j++)
                    {
                        if(force)
                            buf[j].Populate();
                        else
                        if (state[j])
                            buf[j].Populate();
                    }
                    if(buf!=null)
                    {
                        if (buf.Length > 0)
                        {
                            var txt = buf[0];
                            if(txt!=null)
                            {
                                var font = txt.Font;
                                if (font != null)
                                {
                                    var tex = font.material.mainTexture;
                                    var id = tex.GetInstanceID();
                                    for (int j = 0; j < c; j++)
                                    {
                                        buf[j].texIds[0] = id;
                                        buf[j].textures[0] = tex;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
