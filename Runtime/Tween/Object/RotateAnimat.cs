using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huqiang
{
    /// <summary>
    /// 旋转动画
    /// </summary>
    public class RotateAnimat : AnimatBase, AnimatInterface
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="t">目标对象</param>
        public RotateAnimat(Transform t)
        {
            Target = t;
            AnimationManage.Manage.AddAnimat(this);
        }
        /// <summary>
        /// 目标对象
        /// </summary>
        public Transform Target;
        /// <summary>
        /// 开始角度
        /// </summary>
        public Vector3 StartAngle;
        /// <summary>
        /// 结束角度
        /// </summary>
        public Vector3 EndAngle;
        /// <summary>
        /// 启动时的委托函数
        /// </summary>
        public Action<RotateAnimat> PlayStart;
        /// <summary>
        /// 播放完毕时的委托函数
        /// </summary>
        public Action<RotateAnimat> PlayOver;
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
                    if (!Loop & c_time >= m_time)
                    {
                        playing = false;
                        Target.localEulerAngles = EndAngle;
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
                        Vector3 v = EndAngle - StartAngle;
                        Target.localEulerAngles= StartAngle + v * r;
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
                Target.gameObject.SetActive(false);
            AnimationManage.Manage.ReleaseAnimat(this);
        }
    }
}
