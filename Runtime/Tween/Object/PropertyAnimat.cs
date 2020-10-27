using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace huqiang
{
    /// <summary>
    /// 属性动画，用于更新某个类的某个属性的动画,使用反射,不推荐使用
    /// </summary>
    public class PropertyFloat
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="cla">目标对象</param>
        /// <param name="PropertyName">属性名称</param>
        public PropertyFloat(object cla, string PropertyName)
        {
            Target = cla;
            info = cla.GetType().GetProperty(PropertyName);
        }
        /// <summary>
        /// 开始值
        /// </summary>
        public float StartValue;
        /// <summary>
        /// 结束值
        /// </summary>
        public float EndValue;
        float delay;
        /// <summary>
        /// 延迟开始
        /// </summary>
        public float Delay;
        /// <summary>
        /// 总计时间
        /// </summary>
        public float Time;
        float SurplusTime;
        /// <summary>
        /// 目标对象
        /// </summary>
        internal object Target;
        PropertyInfo info;
        /// <summary>
        /// 重置状态
        /// </summary>
        internal void Reset()
        {
            SurplusTime = Time;
            delay = Delay;
            info.SetValue(Target, 0, null);
        }
        /// <summary>
        /// 帧更新
        /// </summary>
        /// <param name="t">目标属性</param>
        /// <param name="timeslice">时间片</param>
        internal static void Update(PropertyFloat t, float timeslice)
        {
            if (t.delay > 0)
            {
                t.delay -= timeslice;
                if (t.delay <= 0)
                {
                    t.SurplusTime += t.delay;
                    t.info.SetValue(t.Target, t.StartValue, null);
                }
            }
            else
            {
                t.SurplusTime -= timeslice;
                if (t.SurplusTime <= 0)
                {
                    t.info.SetValue(t.Target, t.EndValue, null);
                    return;
                }
                float r = 1 - t.SurplusTime / t.Time;
                float d = t.EndValue - t.StartValue;
                d *= r;
                d += t.StartValue;
                t.info.SetValue(t.Target, d, null);
            }
        }
    }
    /// <summary>
    /// 属性动画基本类
    /// </summary>
    public class PropertyAnimat : AnimatBase, AnimatInterface
    {
        List<PropertyFloat> lpf;
        /// <summary>
        /// 启动时的委托函数
        /// </summary>
        public Action<PropertyAnimat> PlayStart;
        /// <summary>
        /// 播放完毕时的委托函数
        /// </summary>
        public Action<PropertyAnimat> PlayOver;
        /// <summary>
        /// 添加属性更新委托
        /// </summary>
        /// <param name="pf"></param>
        public void AddDelegate(PropertyFloat pf)
        {
            if (lpf == null)
                lpf = new List<PropertyFloat>();
            lpf.Add(pf);
        }
        /// <summary>
        /// 移除属性更新委托
        /// </summary>
        /// <param name="pf"></param>
        public void RemoveDelegate(PropertyFloat pf)
        {
            if (lpf != null)
                lpf.Remove(pf);
        }
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
                        c_time += Delay;
                        timeslice += Delay;
                        if (lpf != null)
                            for (int i = 0; i < lpf.Count; i++)
                                PropertyFloat.Update(lpf[i], timeslice);
                    }
                }
                else
                {
                    c_time -= timeslice;
                    if (lpf != null)
                        for (int i = 0; i < lpf.Count; i++)
                            PropertyFloat.Update(lpf[i], timeslice);
                    if (!Loop & c_time <= 0)
                    {
                        playing = false;
                        if (PlayOver != null)
                        {
                            PlayOver(this);
                        }
                    }
                    else
                    {
                        if (c_time <= 0)
                            Play();
                    }
                }
            }
        }
        /// <summary>
        /// 开始播放
        /// </summary>
        public override void Play()
        {
            c_time = m_time;
            if (lpf != null)
                for (int i = 0; i < lpf.Count; i++)
                    lpf[i].Reset();
            playing = true;
        }
        public PropertyAnimat()
        {
            AnimationManage.Manage.AddAnimat(this);
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
