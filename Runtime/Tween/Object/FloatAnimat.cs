using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace huqiang
{
    /// <summary>
    /// 浮点值更新动画
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FloatAnimat<T> : AnimatBase, AnimatInterface where T: class
    {
        /// <summary>
        /// 目标对象
        /// </summary>
        public T Target { get; private set; }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="tar">目标实例</param>
        public FloatAnimat(T tar)
        {
            Target = tar;
            AnimationManage.Manage.AddAnimat(this);
        }
        public override void Play()
        {
            playing = true;
        }
        /// <summary>
        /// 更新目标浮点值的委托
        /// </summary>
        public Action<T, float> ValueChanged;
        /// <summary>
        /// 更新完毕后的委托
        /// </summary>
        public Func<T, float, bool> PlayOver;
        /// <summary>
        /// 开始值
        /// </summary>
        public float Start;
        /// <summary>
        /// 目标值
        /// </summary>
        public float End;
        /// <summary>
        /// 帧更新
        /// </summary>
        /// <param name="time">每帧时间</param>
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
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            AnimationManage.Manage.ReleaseAnimat(this);
        }
    }
    /// <summary>
    /// 二维向量更新动画
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Vector2Animat<T> : AnimatBase, AnimatInterface where T : class
    {
        /// <summary>
        /// 目标对象
        /// </summary>
        public T Target { get; private set; }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="tar">目标实例</param>
        public Vector2Animat(T tar)
        {
            Target = tar;
            AnimationManage.Manage.AddAnimat(this);
        }
        public override void Play()
        {
            playing = true;
        }
        /// <summary>
        /// 更新目标二维向量的委托
        /// </summary>
        public Action<T, Vector2> ValueChanged;
        /// <summary>
        /// 更新完毕后的委托
        /// </summary>
        public Action<T, Vector2> PlayOver;
        /// <summary>
        /// 开始值
        /// </summary>
        public Vector2 Start;
        /// <summary>
        /// 目标值
        /// </summary>
        public Vector2 End;
        /// <summary>
        /// 帧更新
        /// </summary>
        /// <param name="time">每帧时间</param>
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
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            AnimationManage.Manage.ReleaseAnimat(this);
        }
    }
    /// <summary>
    /// 三维向量更新动画
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Vector3Animat<T> : AnimatBase, AnimatInterface where T : class
    {
        /// <summary>
        /// 目标对象
        /// </summary>
        public T Target { get; private set; }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="tar">目标实例</param>
        public Vector3Animat(T tar)
        {
            Target = tar;
            AnimationManage.Manage.AddAnimat(this);
        }
        public override void Play()
        {
            playing = true;
        }
        /// <summary>
        /// 更新目标三维向量的委托
        /// </summary>
        public Action<T, Vector3> ValueChanged;
        /// <summary>
        /// 更新完毕后的委托
        /// </summary>
        public Action<T, Vector3> PlayOver;
        /// <summary>
        /// 开始值
        /// </summary>
        public Vector3 Start;
        /// <summary>
        /// 目标值
        /// </summary>
        public Vector3 End;
        /// <summary>
        /// 帧更新
        /// </summary>
        /// <param name="time">每帧时间</param>
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
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            AnimationManage.Manage.ReleaseAnimat(this);
        }
    }
}
