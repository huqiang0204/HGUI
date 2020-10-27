using huqiang.Core.HGUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huqiang
{
    /// <summary>
    /// 动画函数式编写扩展
    /// </summary>
    public static class AnimationExtand
    {
        /// <summary>
        /// 查询一个位移动画
        /// </summary>
        /// <param name="trans">实例目标</param>
        /// <returns></returns>
        public static MoveAnimat FindMoveAni(this Transform trans)
        {
            if (trans == null)
                return null;
           return AnimationManage.Manage.FindAni<MoveAnimat>((o) => { return o.Target == trans ? true : false; });
        }
        /// <summary>
        /// 移动到某个位置
        /// </summary>
        /// <param name="trans">实例</param>
        /// <param name="pos">目标位置</param>
        /// <param name="time">总计事件</param>
        /// <param name="hide">完成后自动隐藏</param>
        /// <param name="delay">延迟启动</param>
        /// <param name="over">完毕后的回调函数</param>
        /// <param name="cover">是否覆盖已有的动画</param>
        public static void MoveTo(this Transform trans, Vector3 pos, float time,bool hide=false, float delay = 0, Action<MoveAnimat> over = null, bool cover = true)
        {
            if (trans == null)
                return;
            trans.gameObject.SetActive(true);
            var ani = AnimationManage.Manage.FindAni<MoveAnimat>((o) => { return o.Target == trans ? true : false; });
            if (ani == null)
                ani = new MoveAnimat(trans);
            else if (!cover)
                return;
            ani.StartPosition = trans.localPosition;
            ani.EndPosition = pos;
            ani.Time = time;
            ani.Delay = delay;
            ani.AutoHide = hide;
            if (over == null)
                ani.PlayOver = (o) => { o.Dispose(); };
            else ani.PlayOver = over;
            ani.Play();
        }
        /// <summary>
        /// 查询一个旋转动画
        /// </summary>
        /// <param name="trans">实例目标</param>
        /// <returns></returns>
        public static RotateAnimat FindRotateAni(this Transform trans)
        {
            if (trans == null)
                return null;
            return AnimationManage.Manage.FindAni<RotateAnimat>((o) => { return o.Target == trans ? true : false; });
        }
        /// <summary>
        /// 旋转到某个位置
        /// </summary>
        /// <param name="trans">实例对象</param>
        /// <param name="angle">目标欧拉角</param>
        /// <param name="time">总计时间</param>
        /// <param name="hide">完成后自动隐藏</param>
        /// <param name="delay">延迟启动</param>
        /// <param name="over">完毕后的回调函数</param>
        /// <param name="cover">是否覆盖已有的动画</param>
        public static void RotateTo(this Transform trans, Vector3 angle, float time, bool hide = false, float delay = 0, Action<RotateAnimat> over = null, bool cover = true)
        {
            if (trans == null)
                return;
            trans.gameObject.SetActive(true);
            var ani = AnimationManage.Manage.FindAni<RotateAnimat>((o) => { return o.Target == trans ? true : false; });
            if (ani == null)
                ani = new RotateAnimat(trans);
            else if (!cover)
                return;
            ani.StartAngle = trans.localEulerAngles;
            ani.EndAngle = angle;
            ani.Time = time;
            ani.Delay = delay;
            ani.AutoHide = hide;
            if (over == null)
                ani.PlayOver = (o) => { o.Dispose(); };
            else ani.PlayOver = over;
            ani.Play();
        }
        /// <summary>
        /// 查询一个缩放动画
        /// </summary>
        /// <param name="trans">实例目标</param>
        /// <returns></returns>
        public static ScaleAnimat FindScaleAni(this Transform trans)
        {
            if (trans == null)
                return null;
            return AnimationManage.Manage.FindAni<ScaleAnimat>((o) => { return o.Target == trans ? true : false; });
        }
        /// <summary>
        /// 旋转到某个比例
        /// </summary>
        /// <param name="trans">实例对象</param>
        /// <param name="scale">目标比例</param>
        /// <param name="time">总计时间</param>
        /// <param name="hide">完成后自动隐藏</param>
        /// <param name="delay">延迟启动</param>
        /// <param name="over">完毕后的回调函数</param>
        /// <param name="cover">是否覆盖已有的动画</param>
        public static void ScaleTo(this Transform trans, Vector3 scale, float time, bool hide = false, float delay=0, Action<ScaleAnimat> over = null, bool cover = true)
        {
            if (trans == null)
                return;
            trans.gameObject.SetActive(true);
            var ani = AnimationManage.Manage.FindAni<ScaleAnimat>((o) => { return o.Target == trans ? true : false; });
            if (ani == null)
                ani = new ScaleAnimat(trans);
            else if (!cover)
                return;
            ani.StartScale = trans.localScale;
            ani.EndScale = scale;
            ani.Time = time;
            ani.Delay = delay;
            ani.AutoHide = hide;
            if (over == null)
                ani.PlayOver = (o) => { o.Dispose(); };
            else ani.PlayOver = over;
            ani.Play();
        }
        /// <summary>
        /// 查询一个颜色过度动画
        /// </summary>
        /// <param name="grap">实例目标</param>
        /// <returns></returns>
        public static ColorAnimat FindColorAni(this HGraphics grap)
        {
            if (grap == null)
                return null;
            grap.gameObject.SetActive(true);
            return AnimationManage.Manage.FindAni<ColorAnimat>((o) => { return o.Target == grap ? true : false; });
        }
        /// <summary>
        /// 颜色过渡动画
        /// </summary>
        /// <param name="grap">实例对象</param>
        /// <param name="col">目标颜色值</param>
        /// <param name="time">总计时间</param>
        /// <param name="delay">延迟启动</param>
        /// <param name="over">完毕后的回调函数</param>
        /// <param name="cover">是否覆盖已有的动画</param>
        public static void ColorTo(this HGraphics grap, Color col, float time, float delay=0, Action<ColorAnimat> over = null, bool cover = true)
        {
            if (grap == null)
                return;
            grap.gameObject.SetActive(true);
            var ani = AnimationManage.Manage.FindAni<ColorAnimat>((o) => { return o.Target == grap ? true : false; });
            if (ani == null)
                ani = new ColorAnimat(grap);
            else if (!cover)
                return;
            ani.StartColor = grap.MainColor;
            ani.EndColor = col;
            ani.Time = time;
            ani.Delay = delay;
            if (over == null)
                ani.PlayOver = (o) => { o.Dispose(); };
            else ani.PlayOver = over;
            ani.Play();
        }
        /// <summary>
        /// 查询一个精灵动画
        /// </summary>
        /// <param name="img">实例目标</param>
        /// <returns></returns>
        public static ImageAnimat FindSpritesAni(this HImage img)
        {
            if (img == null)
                return null;
            img.gameObject.SetActive(true);
            return AnimationManage.Manage.FindAni<ImageAnimat>((o) => { return o.image == img ? true : false; });
        }
        /// <summary>
        /// 查询或创建一个精灵动画
        /// </summary>
        /// <param name="img">实例目标</param>
        /// <returns></returns>
        public static ImageAnimat FindOrCreateSpritesAni(this HImage img)
        {
            if (img == null)
                return null;
            img.gameObject.SetActive(true);
            var ani= AnimationManage.Manage.FindAni<ImageAnimat>((o) => { return o.image == img ? true : false; });
            if(ani==null)
                ani = new ImageAnimat(img);
            return ani;
        }
        /// <summary>
        /// 播放一个精灵动画
        /// </summary>
        /// <param name="img">实例目标</param>
        /// <param name="sprites">精灵图集</param>
        /// <param name="inter">每帧间隔单位毫秒</param>
        /// <param name="loop">是否循环</param>
        /// <param name="over">完毕后的回调函数</param>
        /// <param name="hide">播放完自动隐藏</param>
        /// <param name="cover">是否覆盖已有的动画</param>
        public static void Play(this HImage img, Sprite[] sprites, float inter = 16, bool loop = false, Action<ImageAnimat> over = null, bool hide = true, bool cover = true)
        {
            if (img == null)
                return;
            img.gameObject.SetActive(true);
            var ani = AnimationManage.Manage.FindAni<ImageAnimat>((o) => { return o.image == img ? true : false; });
            if (ani == null)
                ani = new ImageAnimat(img);
            else if (!cover)
                return;
            ani.autoHide = hide;
            if (over == null)
                ani.PlayOver = (o) => { o.Dispose(); };
            else ani.PlayOver = over;
            ani.Loop = loop;
            ani.Interval = inter;
            ani.Play(sprites);
        }
        /// <summary>
        /// 更新材质球浮点参数动画
        /// </summary>
        /// <param name="mat">目标实例</param>
        /// <param name="name">属性名称</param>
        /// <param name="sv">开始值</param>
        /// <param name="ev">结束值</param>
        /// <param name="time">总计时间</param>
        /// <param name="delay">延迟开始时间</param>
        /// <param name="over">完毕后的回调函数</param>
        /// <param name="cover">是否覆盖已有的动画</param>
        public static void Play(this Material mat,string name, float sv,float ev,float time,float delay=0, Action<ShaderAnimat> over = null, bool cover =true)
        {
            if (mat == null)
                return;
            var ani = AnimationManage.Manage.FindAni<ShaderAnimat>((o) => { return o.Target == mat ? true : false; });
            if (ani == null)
            {
                ani = new ShaderAnimat(mat);
                ShaderFloat sf = new ShaderFloat();
                sf.ParameterName = name;
                sf.StartValue = sv;
                sf.EndValue = ev;
                sf.Time = time;
                sf.Delay = delay;
                ani.Time = time;
                if (over == null)
                    ani.PlayOver = (o) => { o.Dispose(); };
                else ani.PlayOver = over;
            }
            else
            {
                var sf = ani.FindFloatShader(name);
                if (sf != null)
                {
                    if (!cover)
                        return;
                }
                else sf = new ShaderFloat();
                sf.StartValue = sv;
                sf.EndValue = ev;
                sf.Time = time;
                sf.Delay = delay;
                ani.Time = time;
                if (over == null)
                    ani.PlayOver = (o) => { o.Dispose(); };
                else ani.PlayOver = over;
            }
            ani.Play();
        }
        /// <summary>
        /// 更新材质球Vector4参数动画
        /// </summary>
        /// <param name="mat">目标实例</param>
        /// <param name="name">属性名称</param>
        /// <param name="sv">开始值</param>
        /// <param name="ev">结束值</param>
        /// <param name="time">总计时间</param>
        /// <param name="delay">延迟开始时间</param>
        /// <param name="over">完毕后的回调函数</param>
        /// <param name="cover">是否覆盖已有的动画</param>
        public static void Play(this Material mat, string name, Vector4 sv, Vector4 ev, float time, float delay = 0, Action<ShaderAnimat> over = null, bool cover = true)
        {
            if (mat == null)
                return;
            var ani = AnimationManage.Manage.FindAni<ShaderAnimat>((o) => { return o.Target == mat ? true : false; });
            if (ani == null)
            {
                ani = new ShaderAnimat(mat);
                ShaderVector4 sf = new ShaderVector4();
                sf.ParameterName = name;
                sf.StartValue = sv;
                sf.EndValue = ev;
                sf.Time = time;
                sf.Delay = delay;
                ani.Time = time;
                if (over == null)
                    ani.PlayOver = (o) => { o.Dispose(); };
                else ani.PlayOver = over;
            }
            else
            {
                var sf = ani.FindVectorShader(name);
                if (sf != null)
                {
                    if (!cover)
                        return;
                }
                else sf = new ShaderVector4();
                sf.StartValue = sv;
                sf.EndValue = ev;
                sf.Time = time;
                sf.Delay = delay;
                ani.Time = time;
                if (over == null)
                    ani.PlayOver = (o) => { o.Dispose(); };
                else ani.PlayOver = over;
            }
            ani.Play();
        }
        /// <summary>
        /// image的填充动画
        /// </summary>
        /// <param name="img">目标对象</param>
        /// <param name="end">目标值</param>
        /// <param name="time">总计时间</param>
        /// <param name="delay">延迟开始时间</param>
        /// <param name="over">完毕后的回调函数</param>
        public static void DoFillAmount(this HImage img,float end, float time, float delay = 0,Action over = null)
        {
            if (img == null)
                return;
            var ani = AnimationManage.Manage.FindAni<FloatAnimat<HImage>>((o) => { return o.Target == img ? true : false; });
            if (ani == null)
                ani = new FloatAnimat<HImage>(img);
            ani.Start = img.FillAmount;
            ani.End = end;
            ani.Time = time;
            ani.Delay = delay;
            ani.ValueChanged = (o, e) => { o.FillAmount = e; };
            ani.PlayOver =(o,e)=>{ 
                o.FillAmount = e;
                if (over != null)
                    over();
                return true; };
            ani.Play();
        }
    }
}