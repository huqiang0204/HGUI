using huqiang.Core.HGUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huqiang
{
    /// <summary>
    /// 颜色过渡动画
    /// </summary>
    public class ColorAnimat : AnimatBase, AnimatInterface
    {
        /// <summary>
        /// 目标图形
        /// </summary>
        public HGraphics Target { get; private set; }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="img">图形实例对象</param>
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
        /// <summary>
        /// 开始颜色值
        /// </summary>
        public Color StartColor;
        /// <summary>
        /// 目标颜色值
        /// </summary>
        public Color EndColor;
        /// <summary>
        /// 状态更新
        /// </summary>
        /// <param name="time">帧时间</param>
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
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            AnimationManage.Manage.ReleaseAnimat(this);
        }
    }
}
