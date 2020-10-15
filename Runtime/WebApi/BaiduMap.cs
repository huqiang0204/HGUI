using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace huqiang.WebApi
{
    [Serializable]
    public class BaiDuTrans
    {
        public int status;
        public LanLat[] result;
    }
    [Serializable]
    public class LanLat
    {
        public double x;
        public double y;
    }
    public struct TilePos
    {
        public int x;
        public int y;
        public int ox;
        public int oy;
    }
    public class BaiduMap
    {
        class BindingInfo
        {
            public string url;
            public string filePath;
            public string name;
            public object context;
            public UnityWebRequest webRequest;
            public Action<string, string, object,byte[]> CallBack;
        }
        static List<BindingInfo> lbi = new List<BindingInfo>();
        static void DownloadAsync(string url, string filePath, string name, Action<string, string, object, byte[]> callBack, object context = null)
        {
            for(int i=0;i<lbi.Count;i++)
            {
                if (lbi[i].filePath == filePath)
                    return;
            }
            var uwr = UnityWebRequest.Get(url);
            var ao = uwr.SendWebRequest();
            ao.completed += DownloadComplete;
            BindingInfo info = new BindingInfo();
            info.url = url;
            info.filePath = filePath;
            info.webRequest = uwr;
            info.name = name;
            info.context = context;
            info.CallBack = callBack;
            lbi.Add(info);
        }
        static void DownloadComplete(AsyncOperation o)
        {
            var uwr = o as UnityWebRequestAsyncOperation;
            var web = uwr.webRequest;
            BindingInfo info = null;
            for(int i=0;i<lbi.Count;i++)
            {
                if(web==lbi[i].webRequest)
                {
                    info = lbi[i];
                    lbi.RemoveAt(i);
                    break;
                }
            }
            if (o.isDone)
            {
                var dat = uwr.webRequest.downloadHandler.data;
                if(info!=null)
                {
                    if (File.Exists(info.filePath))
                        File.Delete(info.filePath);
                    File.WriteAllBytes(info.filePath, dat);
                    if (info.CallBack != null)
                        info.CallBack(info.filePath,info.name,info.context, dat);
                }
            }
        }
        public const int MinLevel = 3;
        public const int MaxLevel = 18;
        public static string AK = "MgRcqKAaBmaP3jIozTcTORIpuZ08oISA";
        static int Index = 0;
        public static void GPSToMercato(double x, double y, Action<LanLat> action)
        {
            string url = string.Format("http://api.map.baidu.com/geoconv/v1/?coords={0},{1}&from=1&to=6&ak={2}", x, y,AK);
            Http.HttpControl.Get(url, (o) => {
                string str = Encoding.UTF8.GetString(o.Data);
                var point = JsonUtility.FromJson<BaiDuTrans>(str);
                if (point.status == 0)
                {
                    if (point.result != null)
                    {
                        var r = point.result[0];
                        if (action != null)
                            action(r);
                    }
                }
            });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ll"></param>
        /// <param name="zoom">3-18</param>
        public static TilePos MercatoToTile(LanLat ll, int zoom)
        {
            int s = 1 <<(18 - zoom);
            double mx = ll.x / s / 256;
            int ix = (int)mx;
            int ox = (int)((mx - ix) * 256);
            double my = ll.y / s / 256;
            int iy = (int)my;
            int oy = (int)((my - iy) * 256);
            var ti = new TilePos();
            ti.x = ix;
            ti.y = iy;
            ti.ox = ox;
            ti.oy = oy;
            return ti;
        }
        public static void GPSToTile(double x, double y,int zoom, Action<TilePos> action)
        {
            string url = string.Format("http://api.map.baidu.com/geoconv/v1/?coords={0},{1}&from=1&to=6&ak={2}", x, y,AK);
            Http.HttpControl.Get(url, (o) => {
                string str = Encoding.UTF8.GetString(o.Data);
                var point = JsonUtility.FromJson<BaiDuTrans>(str);
                if (point.status == 0)
                {
                    if (point.result != null)
                    {
                        var r = point.result[0];
                        var t = MercatoToTile(r, zoom);
                        if (action != null)
                            action(t);
                    }
                }
            });
        }
        public static void GetTileMap(int tileX, int tileY, int zoom, string name, Action<string, string, object, byte[]> action, object context)
        {
            int z = zoom;
            string file = name;
            string folder = Application.persistentDataPath + "/map";
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
            folder = folder + "/baidu";
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
            string path = folder + "/" + file;
            if (File.Exists(path))
            {
                action(path, file, context,null);
                return;
            }
            Index++;
            if (Index >= 10)
                Index = 0;
            string http = string.Format("http://online{0}.map.bdimg.com/onlinelabel/?qt=tile&x={1}&y={2}&z={3}&ak={4}",Index, tileX, tileY, z,AK);
            DownloadAsync(http,path, file, action, context);
        }
    }
}
