using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace huqiang.Core.HGUI
{
    public class TextCollector
    {
        struct TextState
        {
            public HText text;
            public bool active;
        }
        struct TextInfo
        {
            public TextState[] texts;
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
                buf = new TextState[1024];
                buffer[point].texts = buf;
            }
            int top = buffer[point].max;
            buf[top].text = text;
            buf[top].active = active;
            buffer[point].max++;
        }
        public void Clear()
        {
            for (int i = 0; i <= max; i++)
            {
                int c = buffer[i].max;
                var buf = buffer[i];
                for (int j = 0; j < c; j++)
                    buf.texts[j].text = null;
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
                bool dirty = force;
                if (!force)
                {
                    for (int j = 0; j < c; j++)
                    {
                        if (buf[j].text.m_dirty | buf[j].text.m_colorChanged)
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
                            buf[j].text.Populate();
                        else
                        if (buf[j].active)
                            buf[j].text.Populate();
                    }
                    if(buf!=null)
                    {
                        if (buf.Length > 0)
                        {
                            var txt = buf[0].text;
                            if(txt!=null)
                            {
                                var font = txt.Font;
                                if (font != null)
                                {
                                    var tex = font.material.mainTexture;
                                    var id = tex.GetInstanceID();
                                    for (int j = 0; j < c; j++)
                                    {
                                        buf[j].text.texIds[0] = id;
                                        buf[j].text.textures[0] = tex;
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
