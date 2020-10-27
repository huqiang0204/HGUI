using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huqiang
{
    /// <summary>
    /// 用于控制着色器浮点数的动画
    /// </summary>
    public class ShaderFloat
    {
        /// <summary>
        /// 开始值
        /// </summary>
        public float StartValue;
        /// <summary>
        /// 目标值
        /// </summary>
        public float EndValue;
        float delay;
        /// <summary>
        /// 延迟开始时间,单位毫秒
        /// </summary>
        public float Delay;
        /// <summary>
        /// 总计时间
        /// </summary>
        public float Time;
        /// <summary>
        /// 着色器属性参数名
        /// </summary>
        public string ParameterName;
        float SurplusTime;
        /// <summary>
        /// 目标材质球
        /// </summary>
        internal Material Target;
        /// <summary>
        /// 状态重置
        /// </summary>
        internal void Reset()
        {
            SurplusTime = Time;
            delay = Delay;
        }
        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="t">着色器目标</param>
        /// <param name="timeslice">时间片</param>
        internal static void Update(ShaderFloat t, float timeslice)
        {
            if (t.delay > 0)
            {
                t.delay -= timeslice;
                if (t.delay <= 0)
                {
                    t.SurplusTime += t.delay;
                    t.Target.SetFloat(t.ParameterName, t.StartValue);
                }
            }
            else
            {
                t.SurplusTime -= timeslice;
                if (t.SurplusTime <= 0)
                {
                    t.Target.SetFloat(t.ParameterName, t.EndValue);
                    return;
                }
                float r = 1 - t.SurplusTime / t.Time;
                float d = t.EndValue - t.StartValue;
                d *= r;
                d += t.StartValue;
                t.Target.SetFloat(t.ParameterName, d);
            }
        }
    }
    /// <summary>
    /// 用于控制着色器四维向量的动画
    /// </summary>
    public class ShaderVector4
    {
        /// <summary>
        /// 开始值
        /// </summary>
        public Vector4 StartValue;
        /// <summary>
        /// 目标值
        /// </summary>
        public Vector4 EndValue;
        float delay;
        /// <summary>
        /// 延迟开始时间,单位毫秒
        /// </summary>
        public float Delay;
        /// <summary>
        /// 总计时间
        /// </summary>
        public float Time;
        float SurplusTime;
        /// <summary>
        /// 着色器属性参数名
        /// </summary>
        public string ParameterName;
        /// <summary>
        /// 目标材质球
        /// </summary>
        internal Material Target;
        /// <summary>
        /// 状态重置
        /// </summary>
        internal void Reset()
        {
            SurplusTime = Time;
            delay = Delay;
        }
        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="t">着色器目标</param>
        /// <param name="timeslice">时间片</param>
        internal static void Update(ShaderVector4 t, float timeslice)
        {
            if (t.delay > 0)
            {
                t.delay -= timeslice;
                if (t.delay <= 0)
                {
                    t.SurplusTime += t.delay;
                    t.Target.SetVector(t.ParameterName, t.StartValue);
                }
            }
            else
            {
                t.SurplusTime -= timeslice;
                if (t.SurplusTime <= 0)
                {
                    t.Target.SetVector(t.ParameterName, t.EndValue);
                    return;
                }
                float r = 1 - t.SurplusTime / t.Time;
                Vector4 d = t.EndValue - t.StartValue;
                d *= r;
                d += t.StartValue;
                t.Target.SetVector(t.ParameterName, d);
            }
        }
    }
    /// <summary>
    /// 着色器动画基本类
    /// </summary>
    public class ShaderAnimat : AnimatBase, AnimatInterface
    {
        List<ShaderFloat> lsf;
        List<ShaderVector4> lsv;
        /// <summary>
        /// 启动时的委托函数
        /// </summary>
        public Action<ShaderAnimat> PlayStart;
        /// <summary>
        /// 播放完毕时的委托函数
        /// </summary>
        public Action<ShaderAnimat> PlayOver;
        /// <summary>
        /// 查询某个着色器浮点属性
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns></returns>
        public ShaderFloat FindFloatShader(string name)
        {
            for (int i = 0; i < lsf.Count; i++)
                if (lsf[i].ParameterName == name)
                    return lsf[i];
            return null;
        }
        /// <summary>
        /// 查询某个着色器向量属性
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns></returns>
        public ShaderVector4 FindVectorShader(string name)
        {
            for (int i = 0; i < lsv.Count; i++)
                if (lsv[i].ParameterName == name)
                    return lsv[i];
            return null;
        }
        /// <summary>
        /// 目标材质球
        /// </summary>
        public Material Target { get; private set; }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="m">材质球</param>
        public ShaderAnimat(Material m)
        {
            Target = m;
            lsf = new List<ShaderFloat>();
            lsv = new List<ShaderVector4>();
            AnimationManage.Manage.AddAnimat(this);
        }
        public void AddDelegate(ShaderFloat sf)
        {
            lsf.Add(sf);
            sf.Target = Target;
        }
        public void AddDelegate(ShaderVector4 sv)
        {
            lsv.Add(sv);
            sv.Target = Target;
        }
        public void RemoveDelegate(ShaderFloat sf)
        {
            lsf.Remove(sf);
        }
        public void RemoveDelegate(ShaderVector4 sv)
        {
            lsv.Remove(sv);
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
                        if (lsf != null)
                            for (int i = 0; i < lsf.Count; i++)
                                ShaderFloat.Update(lsf[i], timeslice);
                        if (lsv != null)
                            for (int i = 0; i < lsv.Count; i++)
                                ShaderVector4.Update(lsv[i], timeslice);
                    }
                }
                else
                {
                    c_time -= timeslice;
                    if (lsf != null)
                        for (int i = 0; i < lsf.Count; i++)
                            ShaderFloat.Update(lsf[i], timeslice);
                    if (lsv != null)
                        for (int i = 0; i < lsv.Count; i++)
                            ShaderVector4.Update(lsv[i], timeslice);
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
            if (lsf != null)
                for (int i = 0; i < lsf.Count; i++)
                    lsf[i].Reset();
            if (lsv != null)
                for (int i = 0; i < lsv.Count; i++)
                    lsv[i].Reset();
            playing = true;
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
