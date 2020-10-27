using huqiang.Core.HGUI;
using System;
using UnityEngine;

namespace huqiang
{
    /// <summary>
    /// 精灵动画
    /// </summary>
    public class ImageAnimat : AnimatInterface
    {
        /// <summary>
        /// 目标对象
        /// </summary>
        public HImage image { get; private set; }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="img">目标实例</param>
        public ImageAnimat(HImage img)
        {
            image = img;
            AnimationManage.Manage.AddAnimat(this);
        }
        Sprite[] sprites;
        int curFrame;
        int feIndex;
        /// <summary>
        /// 播放精灵集
        /// </summary>
        /// <param name="gif"></param>
        public void Play(Sprite[] gif)
        {
            PlayTime = 0;
            if (gif != null)
            {
                sprites = gif;
                image.Sprite = sprites[0];
                image.SetNativeSize();
                _playing = true;
                curFrame = 0;
                feIndex = 0;
            }
        }
        Sprite[][] spritesBuff;
        int curIndex=0;
        /// <summary>
        /// 多少帧时触发事件的集合
        /// </summary>
        public int[] conds;
        /// <summary>
        /// 帧事件缓存
        /// </summary>
        public int[][] allConds;
        /// <summary>
        /// 设置精灵缓存
        /// </summary>
        /// <param name="sprites"></param>
        public void SetSprites(Sprite[][] sprites)
        {
            spritesBuff = sprites;
            curIndex = -1;
        }
        /// <summary>
        /// 设置帧事件触发条件
        /// </summary>
        /// <param name="conditions">触发帧数组</param>
        public void SetFrameEvent(int[] conditions)
        {
            conds = conditions;
        }
        /// <summary>
        /// 设置触发帧事件缓存
        /// </summary>
        /// <param name="conditions">触发帧事件缓存</param>
        public void SetFrameEvent(int[][] conditions)
        {
            allConds = conditions;
        }
        /// <summary>
        /// 播放帧动画
        /// </summary>
        /// <param name="index">触发帧事件索引</param>
        /// <param name="cover">是否覆盖原来的状态</param>
        public void Play(int index,bool cover=true)
        {
            if (spritesBuff == null)
                return;
            if (index == curIndex)
                if (!cover)
                    return;
            if(index>-1&index<spritesBuff.Length)
            {
                if(index<spritesBuff.Length)
                {
                    curIndex = index;
                    Play(spritesBuff[index]);
                }
                if (allConds != null)
                    if (index < allConds.Length)
                        conds = allConds[index];
            }
        }
        /// <summary>
        /// 暂停
        /// </summary>
        public void Pause()
        {
            _playing = false;
        }
        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            _playing = false;
            if (image != null)
            {
                if (sprites != null)
                {
                    image.Sprite = sprites[0];
                    image.SetNativeSize();
                }
            }
        }
        /// <summary>
        /// 播放完毕时的委托
        /// </summary>
        public Action<ImageAnimat> PlayOver;
        /// <summary>
        /// 播放中的委托
        /// </summary>
        public Action<ImageAnimat> Playing;
        /// <summary>
        /// 帧事件触发委托
        /// </summary>
        public Action<ImageAnimat,int> FrameEvent;
        /// <summary>
        /// 是否循环
        /// </summary>
        public bool Loop;
        bool _playing;
        /// <summary>
        /// 播放状态
        /// </summary>
        public bool IsPlaying { get { return _playing; } }
        /// <summary>
        /// 波放到的当前帧
        /// </summary>
        public int PlayIndex { get { return curIndex; } }
        /// <summary>
        /// 当前播放时间
        /// </summary>
        public float PlayTime = 0;
        /// <summary>
        /// 每帧画面间隔时间,单位毫秒
        /// </summary>
        public float Interval = 100;
        /// <summary>
        /// 播放完毕自动隐藏
        /// </summary>
        public bool autoHide;
        /// <summary>
        /// 帧更新
        /// </summary>
        /// <param name="time">每帧时间</param>
        public void Update(float time)
        {
            if (_playing)
            {
                PlayTime += time;
                if (sprites != null)
                {
                    int c = (int)(PlayTime / Interval);
                    if (c != curFrame)
                    {
                        curFrame = c;
                        if (c >= sprites.Length)
                        {
                            if (Loop)
                            {
                                PlayTime = 0;
                                image.Sprite = sprites[0];
                                image.SetNativeSize();
                            }
                            else
                            {
                                _playing = false;
                                if (PlayOver != null)
                                    PlayOver(this);
                            }
                        }
                        else
                        {
                            image.Sprite = sprites[c];
                            image.SetNativeSize();
                        }
                        if (FrameEvent != null)
                            if (conds != null)
                                if (feIndex < conds.Length)
                                    if (conds[feIndex] == curFrame)
                                    {
                                        FrameEvent(this, feIndex);
                                        feIndex++;
                                    }
                    }
                }
                if (Playing != null)
                    Playing(this);
            }
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (autoHide)
                image.gameObject.SetActive(false);
            AnimationManage.Manage.ReleaseAnimat(this);
        }
    }
}
