using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huqiang
{
    /// <summary>
    /// 位移动画
    /// </summary>
    public class MoveAnimat : AnimatBase, AnimatInterface
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="t">目标对象实例</param>
        public MoveAnimat(Transform t)
        {
            Target = t;
            AnimationManage.Manage.AddAnimat(this);
        }
        /// <summary>
        /// 目标对象实例
        /// </summary>
        public Transform Target;
        /// <summary>
        /// 开始位置
        /// </summary>
        public Vector3 StartPosition;
        /// <summary>
        /// 结束位置
        /// </summary>
        public Vector3 EndPosition;
        /// <summary>
        /// 动画启动时的委托
        /// </summary>
        public Action<MoveAnimat> PlayStart;
        /// <summary>
        /// 动画结束时的委托
        /// </summary>
        public Action<MoveAnimat> PlayOver;
        /// <summary>
        /// 帧更新
        /// </summary>
        /// <param name="timeslice"></param>
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
                        Target.localPosition = EndPosition;
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
                        Vector3 v = EndPosition - StartPosition;
                        Target.localPosition = StartPosition + v * r;
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
