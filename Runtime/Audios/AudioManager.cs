using huqiang.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace huqiang
{
    public class AudioChannel
    {
        /// <summary>
        /// UI交互的声音
        /// </summary>
        public const int UI = 0;
        /// <summary>
        /// 背景声音
        /// </summary>
        public const int BGM = 1;
        /// <summary>
        /// 道具或怪物声音
        /// </summary>
        public const int GameSound = 2;
        /// <summary>
        /// 语音聊天
        /// </summary>
        public const int Speech = 3;
    }

    public class AudioManager
    {
        class AudioChannelInfo
        {
            public int Channel;
            public List<AudioSource> audios=new List<AudioSource>();
        }
        static List<AudioChannelInfo> lac = new List<AudioChannelInfo>();
        /// <summary>
        /// 添加一个音源通道
        /// </summary>
        /// <param name="channel">通道id</param>
        /// <param name="audio">音源</param>
        public static void AddAudioSource(int channel,AudioSource audio)
        {
            for(int i=0;i<lac.Count;i++)
            {
                if(channel==lac[i].Channel)
                {
                    lac[i].audios.Add(audio);
                    return;
                }
            }
            AudioChannelInfo info = new AudioChannelInfo();
            info.Channel = channel;
            info.audios.Add(audio);
            lac.Add(info);
        }
        /// <summary>
        /// 使用某个通道播放声音
        /// </summary>
        /// <param name="channel">通道id</param>
        /// <param name="clip">声音片段</param>
        /// <param name="loop">是否循环</param>
        public static void Play(int channel, AudioClip clip, bool loop)
        {
            for(int i=0;i<lac.Count;i++)
            {
                if(channel==lac[i].Channel)
                {
                    Play(lac[i].audios,clip,loop);
                    return;
                }
            }    
        }
        static void Play(List<AudioSource> las, AudioClip clip,bool loop)
        {
            for (int i = 0; i < las.Count; i++)
            {
                if(!las[i].isPlaying)
                {
                    las[i].clip = clip;
                    las[i].loop = loop;
                    las[i].Play();
                    return;
                }
            }
        }
        /// <summary>
        /// 停止某个通道播放声音
        /// </summary>
        /// <param name="channel">通道id</param>
        public static void Stop(int channel)
        {
            for (int i = 0; i < lac.Count; i++)
            {
                if (channel == lac[i].Channel)
                {
                    var las = lac[i].audios;
                    for (int j = 0; j < las.Count; j++)
                        las[j].Stop();
                    return;
                }
            }
        }
        /// <summary>
        /// 查询某个通道的所有音源
        /// </summary>
        /// <param name="channel">通道id</param>
        /// <returns>返回音源列表</returns>
        public static List<AudioSource> FindAudioSourc(int channel)
        {
            for(int i = 0; i < lac.Count; i++)
            {
                if (lac[i].Channel == channel)
                    return lac[i].audios;
            }
            return null;
        }
        public static void ChangeVolume(int channel,float value)
        {
            for (int i = 0; i < lac.Count; i++)
            {
                if (lac[i].Channel == channel)
                {
                    var list = lac[i].audios;
                    for (int j = 0; j < list.Count; j++)
                        list[j].volume = value;
                }
            }
        }
    }
    /// <summary>
    /// ui的音频管理器
    /// </summary>
    public class UIAudioManager
    {
        /// <summary>
        /// 光标进入某个UI区域触发的声音
        /// </summary>
        public static AudioClip pointerEntry;
        /// <summary>
        /// 光标离开某个区域触发的声音
        /// </summary>
        public static AudioClip pointerLeave;
        /// <summary>
        /// 光标按下触发的声音
        /// </summary>
        public static AudioClip pointerDown;
        /// <summary>
        /// 光标抬起触发的声音
        /// </summary>
        public static AudioClip pointerUp;
        /// <summary>
        /// 光标单击触发的声音
        /// </summary>
        public static AudioClip onClick;
        /// <summary>
        /// 开启或关闭UI声音播放
        /// </summary>
        public static bool Open = true;
        /// <summary>
        /// 初始化UI音频组件
        /// </summary>
        /// <param name="root"></param>
        public static void Initial(Transform root)
        {
            GameObject a0 = new GameObject("UIAudio");
            a0.transform.SetParent(root);
            var src0 = a0.AddComponent<AudioSource>();
            var src1 = a0.AddComponent<AudioSource>();
            AudioManager.AddAudioSource(AudioChannel.UI, src0);
            AudioManager.AddAudioSource(AudioChannel.UI, src1);
        }
        /// <summary>
        /// 播放声音
        /// </summary>
        /// <param name="clip"></param>
        public static void Play(AudioClip clip)
        {
            if (Open)
                AudioManager.Play(AudioChannel.UI, clip, false);
        }
        /// <summary>
        /// 播放声音
        /// </summary>
        /// <param name="clip"></param>
        public static void Play(string bundleName, string assetName)
        {
            if (Open)
            {
                var ac = ElementAsset.FindResource<AudioClip>(bundleName, assetName);
                if (ac == null)
                    return;
                AudioManager.Stop(AudioChannel.UI);
                AudioManager.Play(AudioChannel.UI, ac, false);
            }
        }
        /// <summary>
        /// 播放光标进入某个区域的声音
        /// </summary>
        public static void PointerEntry()
        {
            if (Open)
                if (pointerEntry != null)
                    AudioManager.Play(AudioChannel.UI, pointerEntry,false);
        }
        /// <summary>
        /// 播放光标离开某个区域的声音
        /// </summary>
        public static void PointerLeave()
        {
            if (Open)
                if (pointerLeave != null)
                    AudioManager.Play(AudioChannel.UI, pointerLeave,false);
        }
        /// <summary>
        /// 播放光标按下进入某个区域的声音
        /// </summary>
        public static void PointerDown()
        {
            if (Open)
                if (pointerDown != null)
                    AudioManager.Play(AudioChannel.UI, pointerDown,false);
        }
        /// <summary>
        /// 播放光标弹起某个区域的声音
        /// </summary>
        public static void PointerUp()
        {
            if (Open)
                if (pointerUp != null)
                    AudioManager.Play(AudioChannel.UI, pointerUp,false);
        }
        /// <summary>
        /// 播放光标单击某个区域的声音
        /// </summary>
        public static void OnClick()
        {
            if (Open)
                if (onClick != null)
                    AudioManager.Play(AudioChannel.UI, onClick,false);
        }
        /// <summary>
        /// 停止播放UI声音
        /// </summary>
        public static void Stop()
        {
            AudioManager.Stop(AudioChannel.UI);
        }
        public static void ChangeVolume(float value)
        {
            AudioManager.ChangeVolume(AudioChannel.UI,value);
        }
    }
    /// <summary>
    /// 背景音乐管理器
    /// </summary>
    public class BGMAudioManager
    {
        /// <summary>
        /// 开启或关闭背景音乐
        /// </summary>
        public static bool Open = true;
        /// <summary>
        /// 初始化背景音乐组件
        /// </summary>
        /// <param name="root"></param>
        public static void Initial(Transform root)
        {
            GameObject a0 = new GameObject("BGMAudio");
            a0.transform.SetParent(root);
            var src = a0.AddComponent<AudioSource>();
            src.loop = true;
            AudioManager.AddAudioSource(AudioChannel.BGM, src);
        }
        /// <summary>
        /// 播放背景音乐
        /// </summary>
        /// <param name="clip"></param>
        public static void Play(AudioClip clip)
        {
            if (Open)
            {
                AudioManager.Stop(AudioChannel.BGM);
                AudioManager.Play(AudioChannel.BGM, clip, true);
            }
        }
        /// <summary>
        /// 播放背景音乐
        /// </summary>
        /// <param name="clip"></param>
        public static void Play(string bundleName, string assetName)
        {
            if (Open)
            {
                var ac = ElementAsset.FindResource<AudioClip>(bundleName, assetName);
                if (ac == null)
                    return;
                AudioManager.Stop(AudioChannel.BGM);
                AudioManager.Play(AudioChannel.BGM, ac, true);
            }
        }
        /// <summary>
        /// 停止播放背景声音
        /// </summary>
        public static void Stop()
        {
             AudioManager.Stop(AudioChannel.BGM);
        }
        public static void ChangeVolume(float value)
        {
            AudioManager.ChangeVolume(AudioChannel.BGM, value);
        }
    }
}
