using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Networking;

namespace huqiang.Data
{
    /// <summary>
    /// 下载管理器
    /// </summary>
    public class DownloadManager
    {
        /// <summary>
        /// 下载任务列表
        /// </summary>
        public static List<DownLoadMission> Mission;
        /// <summary>
        /// 更新任务列表状态
        /// </summary>
        public static void UpdateMission()
        {
            if (Mission == null)
                return;
            int c = Mission.Count - 1;
            for (; c >= 0; c--)
            {
                var mis = Mission[c];
                if (mis.webRequest != null)
                {
                    mis.err = (int)mis.webRequest.responseCode;
                    if (mis.webRequest.isDone)
                    {
                        mis.result = mis.webRequest.downloadHandler.data;
                        Mission.RemoveAt(c);
                        if (mis.OnDone != null)
                            mis.OnDone(mis);
                        mis.webRequest.Dispose();
                    }
                    else if (mis.webRequest.isHttpError)
                    {
                        Mission.RemoveAt(c);
                        if (mis.OnDone != null)
                            mis.OnDone(mis);
                        mis.webRequest.Dispose();
                    }
                }
            }
        }
        /// <summary>
        /// 查找某条下载任务
        /// </summary>
        /// <param name="cmd">任务指令</param>
        /// <returns></returns>
        public static DownLoadMission FindMission(string cmd)
        {
            for (int i = 0; i < Mission.Count; i++)
            {
                if (cmd == Mission[i].cmd)
                    return Mission[i];
            }
            return null;
        }
        /// <summary>
        /// 下载资源
        /// </summary>
        /// <param name="cmd">任务指令</param>
        /// <param name="name">文件名</param>
        /// <param name="url">远程地址</param>
        /// <param name="context">联系上下文</param>
        /// <param name="done">下载完成后的回调</param>
        /// <param name="version">版本号</param>
        /// <returns></returns>
        public static DownLoadMission DownloadAsset(string cmd, string name, string url,  object context,Action<DownLoadMission> done, int version=0)
        {
            if (Mission == null)
                Mission = new List<DownLoadMission>();
            else
            {
                for (int i = 0; i < Mission.Count; i++)
                {
                    var mis = Mission[i];
                    if (mis.type == name)
                        return mis;//任务不能重复添加
                }
            }
            DownLoadMission r = new DownLoadMission();
            r.filename = name;
            r.cmd = cmd;
            r.DataContext = context;
            r.url = url;
            r.OnDone = done;
            r.version = version;
            var web = UnityWebRequest.Get(url);// (uint)version,0
            web.SendWebRequest();
            r.webRequest = web;
            Mission.Add(r);
            return r;
        }
    }
    /// <summary>
    /// 下载任务
    /// </summary>
    public class DownLoadMission
    {
        /// <summary>
        /// 下载指令,用于做回调时的比对
        /// </summary>
        public string cmd;
        /// <summary>
        /// 联系上下文
        /// </summary>
        public object DataContext;
        /// <summary>
        /// 下载的数据
        /// </summary>
        public byte[] result;
        /// <summary>
        /// 下载的类型
        /// </summary>
        public string type;
        /// <summary>
        /// 文件名
        /// </summary>
        public string filename;
        /// <summary>
        /// 远程地址
        /// </summary>
        public string url;
        /// <summary>
        /// 错误编码
        /// </summary>
        public int err;
        /// <summary>
        /// 版本号
        /// </summary>
        public int version;

        public UnityWebRequest webRequest;
        /// <summary>
        /// 下载完成的回调
        /// </summary>
        public Action<DownLoadMission> OnDone;
    }
}