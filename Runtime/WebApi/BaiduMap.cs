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
        public float ox;
        public float oy;
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
            if (info != null)
                info.webRequest.Dispose();
        }
        public const int MinLevel = 3;
        public const int MaxLevel = 18;
        public static string AK = "MgRcqKAaBmaP3jIozTcTORIpuZ08oISA";
        static int Index = 0;
        /// <summary>
        /// GPS坐标转墨卡坐标
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="action"></param>
        public static void GPSToMercato(double x, double y, Action<LanLat> action)
        {
            string url = string.Format("http://api.map.baidu.com/geoconv/v1/?coords={0},{1}&from=1&to=6&ak={2}", x, y,AK);
            UnityWebRequest.Get(url).SendWebRequest().completed+= (o) => {
                UnityWebRequestAsyncOperation ua = o as UnityWebRequestAsyncOperation;
                if(ua.webRequest.responseCode==200)
                {
                    string str = Encoding.UTF8.GetString(ua.webRequest.downloadHandler.data);
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
                }
            };
        }
        /// <summary>
        /// 墨卡坐标转瓦片坐标
        /// </summary>
        /// <param name="ll"></param>
        /// <param name="zoom">3-18</param>
        public static TilePos MercatoToTile(LanLat ll, int zoom)
        {
            int s = 1 <<(18 - zoom);
            double tx = ll.x / s;
            double mx = tx / 256;
            double rx = tx % 256;
            double ty = ll.y / s;
            double my =ty  / 256;
            double ry = ty % 256;
            var ti = new TilePos();
            ti.x = (int)mx;
            ti.y = (int)my;
            ti.ox = (float)rx;
            ti.oy = (float)ry;
            return ti;
        }
        /// <summary>
        /// 瓦片坐标转墨卡坐标
        /// </summary>
        /// <param name="tile"></param>
        /// <param name="zoom"></param>
        /// <returns></returns>
        public static LanLat TileToMercato(ref TilePos tile, int zoom)
        {
            int s = 1 << (18 - zoom);
            double x = tile.x * 256 + tile.ox;
            x *= s;
            double y = tile.y * 256 + tile.oy;
            y *= s;
            var ll = new LanLat();
            ll.x = x;
            ll.y = y;
            return ll;
        }
        /// <summary>
        /// GPS坐标转瓦片坐标
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="zoom"></param>
        /// <param name="action"></param>
        public static void GPSToTile(double x, double y,int zoom, Action<LanLat> action)
        {
            string url = string.Format("http://api.map.baidu.com/geoconv/v1/?coords={0},{1}&from=1&to=6&ak={2}", x, y,AK);

            UnityWebRequest.Get(url).SendWebRequest().completed += (o) => {
                UnityWebRequestAsyncOperation ua = o as UnityWebRequestAsyncOperation;
                if (ua.webRequest.responseCode == 200)
                {
                    string str = Encoding.UTF8.GetString(ua.webRequest.downloadHandler.data);
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
                }
            };
        }
        /// <summary>
        /// 获取瓦片图片
        /// </summary>
        /// <param name="tileX"></param>
        /// <param name="tileY"></param>
        /// <param name="zoom"></param>
        /// <param name="name"></param>
        /// <param name="action"></param>
        /// <param name="context"></param>
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
