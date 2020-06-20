using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace huqiang
{
    public class FloatAnimat<T> : AnimatBase, AnimatInterface where T: class
    {
        public T Target { get; private set; }
        public FloatAnimat(T tar)
        {
            Target = tar;
            AnimationManage.Manage.AddAnimat(this);
        }
        public override void Play()
        {
            playing = true;
        }
        public Action<T, float> ValueChanged;
        public Func<T, float, bool> PlayOver;
        public float Start;
        public float End;
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
                        if (PlayOver != null)
                        {
                            if (PlayOver(Target, End))
                                Dispose();
                        }
                        else Dispose();  
                    }
                    else
                    {
                        if (c_time >= m_time)
                            c_time -= m_time;
                        float r = c_time / m_time;
                        if (Linear != null)
                            r = Linear(this, r);
                        var v = (End - Start) * r + Start;
                        if (ValueChanged != null)
                            ValueChanged(Target, v);
                    }
                }
            }
        }
        public void Dispose()
        {
            AnimationManage.Manage.ReleaseAnimat(this);
        }
    }
    public class Vector2Animat<T> : AnimatBase, AnimatInterface where T : class
    {
        public T Target { get; private set; }
        public Vector2Animat(T tar)
        {
            Target = tar;
            AnimationManage.Manage.AddAnimat(this);
        }
        public override void Play()
        {
            playing = true;
        }
        public Action<T, Vector2> ValueChanged;
        public Action<T, Vector2> PlayOver;
        public Vector2 Start;
        public Vector2 End;
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
                        if (PlayOver != null)
                            PlayOver(Target, End);
                    }
                    else
                    {
                        if (c_time >= m_time)
                            c_time -= m_time;
                        float r = c_time / m_time;
                        if (Linear != null)
                            r = Linear(this, r);
                        var v = (End - Start) * r + Start;
                        if (ValueChanged != null)
                            ValueChanged(Target, v);
                    }
                }
            }
        }
        public void Dispose()
        {
            AnimationManage.Manage.ReleaseAnimat(this);
        }
    }
    public class Vector3Animat<T> : AnimatBase, AnimatInterface where T : class
    {
        public T Target { get; private set; }
        public Vector3Animat(T tar)
        {
            Target = tar;
            AnimationManage.Manage.AddAnimat(this);
        }
        public override void Play()
        {
            playing = true;
        }
        public Action<T, Vector3> ValueChanged;
        public Action<T, Vector3> PlayOver;
        public Vector3 Start;
        public Vector3 End;
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
                        if (PlayOver != null)
                            PlayOver(Target, End);
                    }
                    else
                    {
                        if (c_time >= m_time)
                            c_time -= m_time;
                        float r = c_time / m_time;
                        if (Linear != null)
                            r = Linear(this, r);
                        var v = (End - Start) * r + Start;
                        if (ValueChanged != null)
                            ValueChanged(Target, v);
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
