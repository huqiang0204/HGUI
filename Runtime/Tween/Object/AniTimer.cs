using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace huqiang
{
    /// <summary>
    /// 线性变化值，参数范围为0-1
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="ratio"></param>
    /// <returns></returns>
    public delegate float LinearTransformation(AnimatBase sender, float ratio);
 

    /// <summary>
    /// 定时器
    /// </summary>
    public class Timer : AnimatBase, AnimatInterface
    {
        public Action<Timer> PlayStart;
        public Action<Timer> PlayOver;
        public void Update(float timeslice)
        {
            if (playing)
            {
                if (Delay > 0)
                {
                    Delay -= timeslice;
                    if (Delay <= 0)
                    {
                        if (PlayStart != null)
                            PlayStart(this);
                        c_time += Delay;
                    }
                }
                else
                {
                    c_time -= timeslice;
                    if (c_time <= 0)
                    {
                        if (!Loop)
                            playing = false;
                        else c_time += m_time;
                        if (PlayOver != null)
                        {
                            PlayOver(this);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public Timer()
        {
            AnimationManage.Manage.AddAnimat(this);
        }
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            AnimationManage.Manage.ReleaseAnimat(this);
        }
    }
 
}