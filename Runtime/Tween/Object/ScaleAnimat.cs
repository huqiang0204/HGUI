using huqiang.Core.HGUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huqiang
{
    /// <summary>
    /// 尺寸缩放动画
    /// </summary>
    public class ScaleAnimat : AnimatBase, AnimatInterface
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="t">目标对象</param>
        public ScaleAnimat(UIElement t)
        {
            Target = t;
            AnimationManage.Manage.AddAnimat(this);
        }
        /// <summary>
        /// 目标对象
        /// </summary>
        public UIElement Target;
        /// <summary>
        /// 开始比例
        /// </summary>
        public Vector3 StartScale;
        /// <summary>
        /// 目标比例
        /// </summary>
        public Vector3 EndScale;
        /// <summary>
        /// 启动时的委托函数
        /// </summary>
        public Action<ScaleAnimat> PlayStart;
        /// <summary>
        /// 播放完毕时的委托函数
        /// </summary>
        public Action<ScaleAnimat> PlayOver;
        /// <summary>
        /// 帧更新
        /// </summary>
        /// <param name="timeslice">时间片</param>
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
                        c_time = -Delay;
                    }
                }
                else
                {
                    c_time += timeslice;
                    if (!Loop & c_time >= m_time)//不会循环且超过总时长
                    {
                        playing = false;
                        Target.localScale = EndScale;
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
                        Vector3 v = EndScale - StartScale;
                        Target.localScale = StartScale + v * r;
                    }
                }
            }
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (AutoHide)
                Target.activeSelf = false;
            AnimationManage.Manage.ReleaseAnimat(this);
        }
    }
}
