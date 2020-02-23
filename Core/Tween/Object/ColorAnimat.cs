using huqiang.Core.HGUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huqiang
{
    public class ColorAnimat : AnimatBase, AnimatInterface
    {
        public HGraphics Target { get; private set; }
        public ColorAnimat(HGraphics img)
        {
            Target = img;
            AnimationManage.Manage.AddAnimat(this);
        }
        public override void Play()
        {
            playing = true;
        }
        public Action<ColorAnimat> PlayOver;
        public float Interval = 100;
        public bool autoHide;
        public Color StartColor;
        public Color EndColor;
        public void Update(float time)
        {
            if (playing)
            {
                if (Delay > 0)
                {
                    Delay -= time;
                    if (Delay <= 0)
                    {
                        c_time = -Delay;
                    }
                }
                else
                {
                    c_time += time;
                    if (!Loop & c_time >= m_time)
                    {
                        playing = false;
                        Target.MainColor = EndColor;
                        if (PlayOver != null)
                            PlayOver(this);
                    }
                    else
                    {
                        if (c_time >= m_time)
                            c_time -= m_time;
                        float r = c_time / m_time;
                        if (Linear != null)
                            r = Linear(this, r);
                        Color v = EndColor - StartColor;
                        Target.MainColor = StartColor + v * r;
                    }
                }
            }
        }
        public void Dispose()
        {
            AnimationManage.Manage.ReleaseAnimat(this);
        }
    }
}
