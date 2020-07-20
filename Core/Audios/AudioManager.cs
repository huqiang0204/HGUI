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
    }
    public class UIAudioManager
    {
        public static AudioClip pointerEntry;
        public static AudioClip pointerLeave;
        public static AudioClip pointerDown;
        public static AudioClip pointerUp;
        public static AudioClip onClick;
        public static bool Open = true;
        public static void Initial(Transform root)
        {
            GameObject a0 = new GameObject("UIAudio0");
            a0.transform.SetParent(root);
            var src = a0.AddComponent<AudioSource>();
            AudioManager.AddAudioSource(AudioChannel.UI, src);
            GameObject a1 = new GameObject("UIAudio1");
            a1.transform.SetParent(root);
            src = a1.AddComponent<AudioSource>();
            AudioManager.AddAudioSource(AudioChannel.UI, src);
        }
        public static void Play(AudioClip clip)
        {
            if (Open)
                AudioManager.Play(AudioChannel.UI, clip, false);
        }
        public static void PointerEntry()
        {
            if (Open)
                if (pointerEntry != null)
                    AudioManager.Play(AudioChannel.UI, pointerEntry,false);
        }
        public static void PointerLeave()
        {
            if (Open)
                if (pointerLeave != null)
                    AudioManager.Play(AudioChannel.UI, pointerLeave,false);
        }
        public static void PointerDown()
        {
            if (Open)
                if (pointerDown != null)
                    AudioManager.Play(AudioChannel.UI, pointerDown,false);
        }
        public static void PointerUp()
        {
            if (Open)
                if (pointerUp != null)
                    AudioManager.Play(AudioChannel.UI, pointerUp,false);
        }
        public static void OnClick()
        {
            if (Open)
                if (onClick != null)
                    AudioManager.Play(AudioChannel.UI, onClick,false);
        }
    }
    public class BGMAudioManager
    {
        public static bool Open = true;
        public static void Initial(Transform root)
        {
            GameObject a0 = new GameObject("BGMAudio");
            a0.transform.SetParent(root);
            var src = a0.AddComponent<AudioSource>();
            AudioManager.AddAudioSource(AudioChannel.BGM, src);
        }
        public static void Play(AudioClip clip)
        {
            if (Open)
                AudioManager.Play(AudioChannel.BGM, clip, true);
        }
    }
}
