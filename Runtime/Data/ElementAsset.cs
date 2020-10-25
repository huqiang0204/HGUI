using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huqiang.Data
{
    /// <summary>
    /// 精灵矩形信息
    /// </summary>
    public struct SpriteRectInfo
    {
        /// <summary>
        /// 纹理尺寸
        /// </summary>
        public Vector2 txtSize;
        /// <summary>
        /// 矩形
        /// </summary>
        public Rect rect;
        /// <summary>
        /// 轴心
        /// </summary>
        public Vector2 pivot;
    }
    public class ElementAsset
    {
        /// <summary>
        /// 异步载入一个资源包,返回进度管理器
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="callback">载入完毕的回调</param>
        /// <returns></returns>
        public static Progress LoadAssetsAsync(string path,Action<Progress,AssetBundleCreateRequest> callback=null)
        {
            Progress pro = new Progress();
            pro.Play(LoadAssets(path));
            pro.PlayOver = callback;
            return pro;
        }
        /// <summary>
        /// 同步载入一个资源包
        /// </summary>
        /// <param name="path">文件路劲</param>
        /// <returns></returns>
        public static AssetBundleCreateRequest LoadAssets(string path)
        {
            return AssetBundle.LoadFromFileAsync(path);
        }
        /// <summary>
        /// 添加一个资源包,默认路径为Application.streamingAssetsPath
        /// </summary>
        /// <param name="name">文件名</param>
        public static void AddBundle(string name)
        {
            var dic = Application.streamingAssetsPath;
            dic += "/" + name;
            var asset = AssetBundle.LoadFromFile(dic);
            bundles.Add(asset);
        }
        /// <summary>
        /// 所有的资源包
        /// </summary>
        public static List<AssetBundle> bundles = new List<AssetBundle>();
        /// <summary>
        /// 查找某个资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="bundle">资源包名</param>
        /// <param name="tname">资源名</param>
        /// <returns></returns>
        public static T FindResource<T>(string bundle, string tname) where T : UnityEngine.Object
        {
            if (bundles == null)
                return null;
            for (int i = 0; i < bundles.Count; i++)
            {
                var tmp = bundles[i];
                if (bundle == tmp.name)
                {
                    return tmp.LoadAsset<T>(tname);
                }
            }
            return null;
        }
        /// <summary>
        /// 查找纹理
        /// </summary>
        /// <param name="bundle">资源包名</param>
        /// <param name="tname">纹理名</param>
        /// <returns></returns>
        public static Texture FindTexture(string bundle, string tname)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                return FindTexture(tname);
#endif
            if (bundle == null)
            {
                return UnityEngine.Resources.Load<Texture>(tname);
            }
            if (bundles == null)
                return null;
            for (int i = 0; i < bundles.Count; i++)
            {
                var tmp = bundles[i];
                if (bundle == tmp.name)
                {
                    return tmp.LoadAsset<Texture>(tname);
                }
            }
            return null;
        }
        /// <summary>
        /// 查找精灵
        /// </summary>
        /// <param name="bundle">资源包名</param>
        /// <param name="tname">纹理名</param>
        /// <param name="name">精灵名</param>
        /// <returns></returns>
        public static Sprite FindSprite(string bundle, string tname, string name)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                return FindSprite(tname, name);
#endif
            if (bundle==null)
            {
                var ss = UnityEngine.Resources.LoadAll<Sprite>(tname);
                if(ss!=null)
                {
                    for (int i = 0; i < ss.Length; i++)
                        if (ss[i].name == name)
                            return ss[i];
                }
                return null;
            }
            if (bundles == null)
                return null;
            for(int i=0;i<bundles.Count;i++)
            {
                var tmp = bundles[i];
                if(bundle==tmp.name)
                {
                    var sp = tmp.LoadAssetWithSubAssets<Sprite>(tname);
                    for(int j = 0; j < sp.Length; j++)
                    {
                        if (sp[j].name == name)
                            return sp[j];
                    }
                    break;
                }
            }
            return null;
        }
        /// <summary>
        /// 使用纹理名称查询资源包名
        /// </summary>
        /// <param name="name">纹理名</param>
        /// <returns></returns>
        public static string TxtureFormAsset(string name)
        {
            if (bundles == null)
                return null;
            for(int i=0;i<bundles.Count;i++)
            {
                if (bundles[i].LoadAsset<Texture>(name) != null)
                    return bundles[i].name;
            }
            return null;
        }
        /// <summary>
        /// 查询资源包
        /// </summary>
        /// <param name="name">资源包名</param>
        /// <returns></returns>
        public static AssetBundle FindBundle(string name)
        {
            if (bundles == null)
                return null;
            for (int i = 0; i < bundles.Count; i++)
                if (bundles[i].name == name)
                    return bundles[i];
            return null;
        }
        /// <summary>
        /// 查询精灵组
        /// </summary>
        /// <param name="bundle">资源包名</param>
        /// <param name="tname">纹理名</param>
        /// <param name="names">精灵名数组,如果为空,则返回该纹理上的所有精灵</param>
        /// <returns></returns>
        public static Sprite[] FindSprites(string bundle, string tname, string[] names = null)
        {
            var bun = FindBundle(bundle);
            if (bun == null)
                return null;
            var sp = bun.LoadAssetWithSubAssets<Sprite>(tname);
            if (sp == null)
                return null;
            if (names == null)
                return sp;
            int len = names.Length;
            Sprite[] sprites = new Sprite[len];
            int c = 0;
            for (int i = 0; i < sp.Length; i++)
            {
                var s = sp[i];
                for (int j = 0; j < len; j++)
                {
                    if (s.name == names[j])
                    {
                        sprites[j] = s;
                        c++;
                        if (c >= len)
                            return sprites;
                        break;
                    }
                }
            }
            return sprites;
        }
        /// <summary>
        /// 查询二维精灵组
        /// </summary>
        /// <param name="bundle">资源包名</param>
        /// <param name="tname">纹理名</param>
        /// <param name="names">精灵名二维数组</param>
        /// <returns></returns>
        public static Sprite[][] FindSprites(string bundle, string tname, string[][] names)
        {
            var bun = FindBundle(bundle);
            if (bun == null)
                return null;
            var sp = bun.LoadAssetWithSubAssets<Sprite>(tname);
            if (sp == null)
                return null;
            if (names == null)
                return null;
            int len = names.Length;
            Sprite[][] sprites = new Sprite[len][];
            for(int k=0;k<len;k++)
            {
                var t = names[k];
                if(t!=null)
                {
                    Sprite[] ss = new Sprite[t.Length];
                    sprites[k] = ss;
                    for (int i = 0; i < ss.Length; i++)
                    {
                        var s = t[i];
                        for (int j = 0; j < len; j++)
                        {
                            if (s== sp[j].name)
                            {
                                ss[i] = sp[j];
                                break;
                            }
                        }
                    }
                }
            }
            return sprites;
        }
        class SpriteData
        {
            public string name;
            public DataBuffer buffer;
        }
        static List<SpriteData> SpriteDatas=new List<SpriteData>();
        /// <summary>
        /// 添加精灵信息数据
        /// </summary>
        /// <param name="name">数据包名</param>
        /// <param name="dat">数据</param>
        public static void AddSpriteData(string name, byte[] dat)
        {
            if (dat == null)
                return;
            RemoveSpriteData(name);
            DataBuffer db = new DataBuffer(dat);
            SpriteData data = new SpriteData();
            data.name = name;
            data.buffer = db;
            SpriteDatas.Add(data);
        }
        /// <summary>
        /// 移除某个精灵数据
        /// </summary>
        /// <param name="name">数据包名</param>
        public static void RemoveSpriteData(string name)
        {
            for(int i=0;i<SpriteDatas.Count;i++)
            {
                if (SpriteDatas[i].name == name)
                {
                    SpriteDatas.RemoveAt(i);
                    return;
                }
            }
        }
        /// <summary>
        /// 清除所有精灵包数据
        /// </summary>
        public static void ClearSpriteData()
        {
            SpriteDatas.Clear();
        }
        /// <summary>
        /// 查询精灵的uv
        /// </summary>
        /// <param name="tName">纹理名</param>
        /// <param name="sName">精灵名</param>
        /// <param name="rect">精灵矩形</param>
        /// <param name="txtSize">纹理尺寸</param>
        /// <param name="pivot">精灵轴心</param>
        public static void FindSpriteUV(string tName, string sName,ref Rect rect, ref Vector2 txtSize,ref Vector2 pivot)
        {
            for(int k=0;k<SpriteDatas.Count;k++)
            {
                var fs = SpriteDatas[k].buffer.fakeStruct;
                if(fs!=null)
                {
                    var fsa = fs.GetData<FakeStructArray>(1);
                    if (fsa != null)
                    {
                        for (int i = 0; i < fsa.Length; i++)
                        {
                            if (fsa.GetData(i, 0) as string == tName)
                            {
                                fsa = fsa.GetData(i, 1) as FakeStructArray;
                                if (fsa != null)
                                {
                                    for (int j = 0; j < fsa.Length; j++)
                                    {
                                        if (fsa.GetData(j, 0) as string == sName)
                                        {
                                            unsafe
                                            {
                                                SpriteDataS* sp =(SpriteDataS*)fsa[j];
                                                txtSize = sp->txtSize;
                                                rect= sp->rect;
                                                pivot = sp->pivot;
                                            }
                                        }
                                    }
                                }
                                break;
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 查询精灵的uv
        /// </summary>
        /// <param name="tName">纹理名</param>
        /// <param name="sns">精灵名数组</param>
        /// <returns></returns>
        public static SpriteDataS[] FindSpriteUVs(string tName, string[] sns)
        {
            SpriteDataS[] infos = new SpriteDataS[sns.Length];
            for (int k = 0; k < SpriteDatas.Count; k++)
            {
                var fs = SpriteDatas[k].buffer.fakeStruct;
                if (fs != null)
                {
                    var fsa = fs.GetData<FakeStructArray>(1);
                    if (fsa != null)
                    {
                        for (int i = 0; i < fsa.Length; i++)
                        {
                            var ts = fsa.GetData(i, 0) as string;
                            if (ts == tName)
                            {
                                fsa = fsa.GetData(i, 1) as FakeStructArray;
                                if (fsa != null)
                                {
                                    for (int t = 0; t < sns.Length; t++)
                                    {
                                        var sName = sns[t];
                                        for (int j = 0; j < fsa.Length; j++)
                                        {
                                            if (fsa.GetData(j, 0) as string == sName)
                                            {
                                                unsafe
                                                {
                                                    infos[t]= *(SpriteDataS*)fsa[j];
                                                }
                                                break;
                                            }
                                        }
                                    }
                                }
                                break;
                            }
                        }
                    }
                }
            }
            return infos;
        }
        /// <summary>
        /// 载入资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="bundle">资源包名</param>
        /// <param name="name">资源名</param>
        /// <returns></returns>
        public static T LoadAssets<T>(string bundle, string name) where T : UnityEngine.Object
        {
            if (bundle == null)
            {
                return UnityEngine.Resources.Load<T>(name);
            }
            if (bundles == null)
                return null;
            for (int i = 0; i < bundles.Count; i++)
            {
                var tmp = bundles[i];
                if (bundle == tmp.name)
                {
                    return tmp.LoadAsset<T>(name);
                }
            }
            return null;
        }

#if UNITY_EDITOR
        static List<UnityEngine.Object[]> objects = new List<UnityEngine.Object[]>();
        /// <summary>
        /// 加载所有的纹理资源
        /// </summary>
        /// <param name="folder">文件夹名</param>
        public static void LoadAllTexture(string folder)
        {
            var path = Application.dataPath;
            if (folder != null)
                path += "/" + folder;
        }
        static UnityEngine.Object[] LoadSprite(string name)
        {
            string path = null;
            var fs = UnityEditor.AssetDatabase.FindAssets(name);
            if (fs != null)
            {
                HashSet<string> hash = new HashSet<string>();
                for (int i = 0; i < fs.Length; i++)
                    hash.Add(fs[i]);
                var list = hash.ToArray();
                for (int i = 0; i < list.Length; i++)
                {
                    path = UnityEditor.AssetDatabase.GUIDToAssetPath(list[i]);
                    var ss = path.Split('/');
                    var str = ss[ss.Length - 1];
                    ss = str.Split('.');
                    if (ss[0] == name)
                    {
                        var sp = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(path);
                        if (sp != null)
                            if (sp.Length > 0)
                            {
                                objects.Add(sp);
                                return sp;
                            }
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// 查询纹理
        /// </summary>
        /// <param name="tname">纹理名</param>
        /// <returns></returns>
        public static Texture FindTexture(string tname)
        {
            for (int i = 0; i < objects.Count; i++)
            {
                var objs = objects[i];
                if (objs != null)
                {
                    if (objs.Length > 0)
                        if (objs[0].name == tname)
                            return objs[0] as Texture;
                }
            }
            var os = LoadSprite(tname);
            if (os != null)
            {
                if (os.Length > 0)
                    return os[0] as Texture;
            }
            return null;
        }
        /// <summary>
        /// 查询精灵
        /// </summary>
        /// <param name="tname">纹理名</param>
        /// <param name="name">精灵名</param>
        /// <returns></returns>
        public static Sprite FindSprite(string tname, string name)
        {
            for (int i = 0; i < objects.Count; i++)
            {
                var objs = objects[i];
                if (objs != null)
                {
                    if (objs.Length > 0)
                        if (objs[0] != null)
                            if (objs[0].name == tname)
                            {
                                for (int j = 1; j < objs.Length; j++)
                                {
                                    if (objs[j] != null)
                                        if (objs[j].name == name)
                                            return objs[j] as Sprite;
                                }
                            }
                }
            }
            var os = LoadSprite(tname);
            if (os != null)
            {
                if (os.Length > 0)
                {
                    for (int j = 1; j < os.Length; j++)
                    {
                        if (os[j] != null)
                            if (os[j].name == name)
                                return os[j] as Sprite;
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// 清除内存对象
        /// </summary>
        public static void Clear()
        {
            objects.Clear();
        }
#endif
    }
}