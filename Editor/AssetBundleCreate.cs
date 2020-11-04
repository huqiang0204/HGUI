using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using huqiang;
using huqiang.Data;
using System.IO;
using System.Runtime.InteropServices;
using huqiang.Core.HGUI;
using SevenZip.Compression.LZMA;
using System.Threading.Tasks;

public class AssetBundleCreate : Editor {

    [MenuItem("Assets/Create Scene/StreamedScenes")]
    static void CreateSceneA()
    {
        //清空一下缓存  
        Caching.ClearCache();
        string o_path = EditorUtility.SaveFilePanel("Save Resource", "Assets/StreamingAssets", "Assets", "unity3d");
        int index = o_path.LastIndexOf('/');
        string o_folder = o_path.Substring(0, index);
        index++;
        string o_file = o_path.Substring(index, o_path.Length - index);
        if (o_path.Length != 0)
        {
            List<string> names = new List<string>();
            var o = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets);
            foreach (UnityEngine.Object obj in o)
            {
                string filePath =  AssetDatabase.GetAssetPath(obj);
                if (filePath.IndexOf(".unity") > 0)
                {
                    names.Add(filePath);
                }
            }
            BuildPipeline.BuildPlayer(names.ToArray(), o_path, BuildTarget.StandaloneWindows, BuildOptions.BuildAdditionalStreamedScenes);
            AssetDatabase.Refresh();
            Debug.Log("打包完成");
        }
    }
    [MenuItem("Assets/Create Scene/Uncompressed")]
    static void CreateSceneB()
    {
        //清空一下缓存  
        Caching.ClearCache();
        string o_path = EditorUtility.SaveFilePanel("Save Resource", "Assets/StreamingAssets", "Assets", "unity3d");
        int index = o_path.LastIndexOf('/');
        string o_folder = o_path.Substring(0, index);
        index++;
        string o_file = o_path.Substring(index, o_path.Length - index);
        if (o_path.Length != 0)
        {
            List<string> names = new List<string>();
            var o = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets);
            foreach (UnityEngine.Object obj in o)
            {
                string filePath = AssetDatabase.GetAssetPath(obj);
                if (filePath.IndexOf(".unity") > 0)
                {
                    names.Add(filePath);
                }
            }
            BuildPipeline.BuildPlayer(names.ToArray(), o_path, BuildTarget.StandaloneWindows, 
                BuildOptions.BuildAdditionalStreamedScenes | BuildOptions.UncompressedAssetBundle);
            AssetDatabase.Refresh();
            Debug.Log("打包完成");
        }
    }
    [MenuItem("Assets/Create Scene/Lzma")]
    static async void CreateSceneC()
    {
        //清空一下缓存  
        Caching.ClearCache();
        string o_path = EditorUtility.SaveFilePanel("Save Resource", "Assets/StreamingAssets", "Assets", "unity3d");
        int index = o_path.LastIndexOf('/');
        string o_folder = o_path.Substring(0, index);
        index++;
        string o_file = o_path.Substring(index, o_path.Length - index);
        if (o_path.Length != 0)
        {
            List<string> names = new List<string>();
            var o = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets);
            foreach (UnityEngine.Object obj in o)
            {
                string filePath = AssetDatabase.GetAssetPath(obj);
                if (filePath.IndexOf(".unity") > 0)
                {
                    names.Add(filePath);
                }
            }
            BuildPipeline.BuildPlayer(names.ToArray(), o_path, BuildTarget.StandaloneWindows, 
                BuildOptions.BuildAdditionalStreamedScenes | BuildOptions.UncompressedAssetBundle);
            var decoder = new Decoder();
            await Task.Run(() => { decoder.DecompressFile(o_path, o_path+".lzma"); });
            AssetDatabase.Refresh();
            Debug.Log("打包完成");
        }
    }

    [MenuItem("Assets/ExportBundles/Win/Default")]
    static void BuildAssetBundlesForWin()
    {
        BuildAllAssetBundles(BuildTarget.StandaloneWindows,BuildAssetBundleOptions.None);
    }
    [MenuItem("Assets/ExportBundles/Win/Uncompressed")]
    static void BuildBundlesForWin()
    {
        BuildAllAssetBundles(BuildTarget.StandaloneWindows, BuildAssetBundleOptions.UncompressedAssetBundle);
    }
    [MenuItem("Assets/ExportBundles/Android/Default")]
    static void BuildAssetBundlesForAndroid()
    {
        BuildAllAssetBundles(BuildTarget.Android,BuildAssetBundleOptions.None);
    }
    [MenuItem("Assets/ExportBundles/Android/Uncompressed")]
    static void BuildBundlesForAndroid()
    {
        BuildAllAssetBundles(BuildTarget.Android, BuildAssetBundleOptions.UncompressedAssetBundle);
    }
    [MenuItem("Assets/ExportBundles/IOS/Default")]
    static void BuildAssetBundlesForIos()
    {
        BuildAllAssetBundles(BuildTarget.iOS, BuildAssetBundleOptions.None);
    }
    [MenuItem("Assets/ExportBundles/IOS/Uncompressed")]
    static void BuildtBundlesForIos()
    {
        BuildAllAssetBundles(BuildTarget.iOS, BuildAssetBundleOptions.UncompressedAssetBundle);
    }
    static void BuildAllAssetBundlesFolder(BuildTarget target)
    {
        // 打开保存面板，获得用户选择的路径  
        string i_path = EditorUtility.OpenFolderPanel("Assets", "Assets", "Resources");
        if (i_path.Contains(Application.dataPath))
        {
            if (i_path.Length != 0)
            {
                char[] buff = i_path.ToCharArray();
                string i_folder = new string(CopyCharArry(buff, Application.dataPath.Length + 1,
                i_path.Length - Application.dataPath.Length - 1));
                string o_path = EditorUtility.SaveFilePanel("Save Resource", "Assets/StreamingAssets", "Assets", "unity3d");
                buff = o_path.ToCharArray();
                int s = FallFindChar(buff, '/');
                string o_folder = new string(CopyCharArry(buff, 0, s));
                string o_file = new string(CopyCharArry(buff, s + 1, buff.Length - s - 1));
                if (o_path.Length != 0)
                {
                    var di = new System.IO.DirectoryInfo(i_path);
                    var fi = di.GetFiles("*.*");//这里可以自己选择过滤
                    List<string> names = new List<string>();
                    for (int i = 0; i < fi.Length; i++)
                    {
                        if (fi[i].Name.Split('.').Length < 3)
                        {
                            names.Add("Assets/" + i_folder + "/" + fi[i].Name);
                        }
                    }
                    AssetBundleBuild[] abb = new AssetBundleBuild[1];
                    abb[0].assetBundleName = o_file;
                    abb[0].assetNames = names.ToArray();
                    BuildPipeline.BuildAssetBundles(o_folder, abb, BuildAssetBundleOptions.None, target);
                }
            }
        }
        else Debug.Log("请选择 " + Application.dataPath + "里面的文件夹");
    }
    static void BuildAllAssetBundles(BuildTarget target, BuildAssetBundleOptions option)
    {
        string o_path = EditorUtility.SaveFilePanel("Save Resource", "Assets/StreamingAssets", "Assets", "unity3d");
        int index = o_path.LastIndexOf('/');
        string o_folder = o_path.Substring(0,index);
        index++;
        string o_file = o_path.Substring(index, o_path.Length - index);
        if (o_path.Length != 0)
        {
            List<string> names = new List<string>();
            var o = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);
            foreach (UnityEngine.Object obj in o)
            {
                string filePath = AssetDatabase.GetAssetPath(obj);
                if (filePath.IndexOf('.') > 0)
                    names.Add(filePath);
            }
            if (names.Count == 0)
                return;
            AssetBundleBuild[] abb = new AssetBundleBuild[1];
            abb[0].assetBundleName = o_file;
            abb[0].assetNames = names.ToArray();
            BuildPipeline.BuildAssetBundles(o_folder, abb, option, target);
            Debug.Log("打包完成");
        }
    }
    static int FallFindChar(char[] c_buff, char t)
    {
        int c = c_buff.Length - 1;
        for (int i = c; i > -1; i--)
        {
            if (c_buff[i] == t)
                return i;
        }
        return -1;
    }
    static char[] CopyCharArry(char[] c_buff, int s, int l)
    {
        char[] temp = new char[l];
        for (int i = 0; i < l; i++)
        {
            temp[i] = c_buff[s];
            s++;
        }
        return temp;
    }
    [MenuItem("Assets/CreateDataBuffer/EmojiInfo")]
    static void CreateEmojiInfo()
    {
        var o = Selection.activeObject;
        if(o is Texture2D)
        {
            string path = AssetDatabase.GetAssetPath(o);
            var ss = path.Split('/');
            var str = ss[ss.Length - 1];
            ss = str.Split('.');
            var sp = AssetDatabase.LoadAllAssetsAtPath(path);
            string o_path = EditorUtility.SaveFilePanel("Save Resource", "Assets/AssetsBundle", "Emoji", "bytes");
            CreateMapInfo(sp,o_path);
        }
    }
    class CharInfoA
    {
        public int len;
        public List<char> dat = new List<char>();
        public List<CharUV> uvs = new List<CharUV>();
    }
    static void CalculUV(Rect sr, float w, float h, ref CharUV uv)
    {
        float x = sr.x;
        float rx = sr.width + x;
        float y = sr.y;
        float ty = sr.height + y;
        x /= w;
        rx /= w;
        y /= h;
        ty /= h;
        uv.uv0.x = x;
        uv.uv0.y = ty;
        uv.uv1.x = rx;
        uv.uv1.y = ty;
        uv.uv2.x = rx;
        uv.uv2.y = y;
        uv.uv3.x = x;
        uv.uv3.y = y;
    }
    static int UnicodeToUtf16(string code)
    {
        int uni = int.Parse(code, System.Globalization.NumberStyles.HexNumber);
        if (uni > 0x10000)
        {
            uni = uni - 0x10000;
            int vh = (uni & 0xFFC00) >> 10;
            int vl = uni & 0x3ff;
            int h = 0xD800 | vh;
            int l = 0xDC00 | vl;
            int value = h << 16 | l;
            return value;
        }
        return uni;
    }
    static byte[] buff = new byte[16];
    private unsafe static int AddSpriteInfo(Sprite spr)
    {
        for (int i = 0; i < 16; i++)
        {
            buff[i] = 0;
        }
        string str = spr.name;
        int len = 0;
        var t = spr.uv;
        fixed (byte* bp = &buff[0])
        {
            UInt16* ip = (UInt16*)bp;
            string[] ss = str.Split('-');
            for (int j = 0; j < ss.Length; j++)
            {
                UInt32 uni = UInt32.Parse(ss[j], System.Globalization.NumberStyles.HexNumber);
                if (uni > 0x10000)
                {
                    uni = uni - 0x10000;
                    UInt32 vh = (uni & 0xFFC00) >> 10;
                    UInt32 vl = uni & 0x3ff;
                    UInt32 h = 0xD800 | vh;
                    UInt32 l = 0xDC00 | vl;
                    //int value = h << 16 | l;
                    *ip = (UInt16)h;
                    ip++;
                    *ip = (UInt16)l;
                    ip++;
                    len += 2;
                }
                else
                {
                    *ip = (UInt16)uni;
                    ip++;
                    len++;
                }
            }
        }
        return len;
    }

    public static void CreateMapInfo(UnityEngine.Object[] sprites, string savepath)
    {
        CharInfoA[] tmp = new CharInfoA[16];
        for (int i = 0; i < 16; i++)
        {
            tmp[i] = new CharInfoA();
            tmp[i].len = i + 1;
        }
        CharUV uv = new CharUV();
        unsafe
        {
            fixed (byte* bp = &buff[0])
            {
                for (int i = 0; i < sprites.Length; i++)
                {
                    var sp = sprites[i] as Sprite;
                    if(sp!=null)
                    {
                        int len = AddSpriteInfo(sp);
                        var dat = tmp[len-1].dat;
                        char* cp = (char*)bp;
                        for (int j = 0; j < len; j++)
                        {
                            dat.Add(*cp);
                            cp++;
                        }
                        CalculUV(sp.rect, sp.texture.width, sp.texture.height, ref uv);
                        tmp[len-1].uvs.Add(uv);
                    }
                }
            }
        }
        DataBuffer db = new DataBuffer();
        FakeStruct fake = new FakeStruct(db, 16);
        for (int i = 0; i < 16; i++)
        {
            var t = tmp[i];
            FakeStruct fs = new FakeStruct(db, 3);
            fs[0] = t.len;
            if (tmp[i].dat.Count > 0)
            {
                fs[1] = db.AddArray<char>(t.dat.ToArray());
                fs[2] = db.AddArray<CharUV>(t.uvs.ToArray());
            }
            fake.SetData(i, fs);
        }
        db.fakeStruct = fake;
        byte[] data = db.ToBytes();
        File.WriteAllBytes(savepath, data);
        Debug.Log("emoji info create done");
    }
    static void WriteTable(Stream stream, Array array, Int32 structLen, Int32 charLen)
    {
        int len = array.Length * structLen;
        stream.Write(charLen.ToBytes(), 0, 4);
        stream.Write(len.ToBytes(), 0, 4);
        var tmp = new byte[len];
        Marshal.Copy(Marshal.UnsafeAddrOfPinnedArrayElement(array, 0), tmp, 0, len);
        stream.Write(tmp, 0, len);
    }
    [MenuItem("Assets/CreateDataBuffer/SpriteInfo")]
    public static void GetAllSprite()
    {
        lsc.Clear();
        var o = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);
        var g = Selection.assetGUIDs;
        for (int i = 0; i < o.Length; i++)
        {
            var p = AssetDatabase.GetAssetPath(o[i]);
            var sp = AssetDatabase.LoadAllAssetsAtPath(p);
            if (sp != null)
            {
                if (sp.Length > 0)
                {
                    for (int j = 0; j < sp.Length; j++)
                    {
                        AddSprite(sp[j] as Sprite);
                    }
                }
            }
        }

        string path = Application.dataPath + "/AssetsBundle/SpriteInfo.bytes";
        Save("spriteInfo", path);
        lsc.Clear();
        Debug.Log("create done : " + path);
    }
    class SpriteCategory
    {
        public string txtName;
        public int width;
        public int height;
        public List<Sprite> sprites = new List<Sprite>();
    }
    static List<SpriteCategory> lsc = new List<SpriteCategory>();
    /// <summary>
    /// 添加精灵
    /// </summary>
    /// <param name="sprite">精灵</param>
    static void AddSprite(Sprite sprite)
    {
        if (sprite == null)
            return;
        string tname = sprite.texture.name;
        for (int i = 0; i < lsc.Count; i++)
        {
            if (tname == lsc[i].txtName)
            {
                lsc[i].sprites.Add(sprite);
                return;
            }
        }
        SpriteCategory category = new SpriteCategory();
        category.txtName = tname;
        category.width = sprite.texture.width;
        category.height = sprite.texture.height;
        category.sprites.Add(sprite);
        lsc.Add(category);
    }
    static FakeStructArray SaveCategory(DataBuffer buffer)
    {
        FakeStructArray array = new FakeStructArray(buffer, 4, lsc.Count);
        for (int i = 0; i < lsc.Count; i++)
        {
            array.SetData(i, 0, lsc[i].txtName);
            array.SetData(i, 1, SaveSprites(buffer, lsc[i].sprites));
            array.SetInt32(i, 2, lsc[i].width);
            array.SetInt32(i, 3, lsc[i].height);
        }
        return array;
    }
    static unsafe FakeStructArray SaveSprites(DataBuffer buffer, List<Sprite> sprites)
    {
        FakeStructArray array = new FakeStructArray(buffer, SpriteDataS.ElementSize, sprites.Count);
        float tx = sprites[0].texture.width;
        float ty = sprites[0].texture.height;
        for (int i = 0; i < sprites.Count; i++)
        {
            var sprite = sprites[i];
            string name = sprite.name;
            SpriteDataS* sp = (SpriteDataS*)array[i];
            sp->name = buffer.AddData(name);
            var sr = sp->rect = sprite.rect;
            sp->pivot = sprite.pivot;
            float w = sprite.texture.width;
            float h = sprite.texture.width;
            float x = sr.x;
            float rx = sr.width + x;
            float y = sr.y;
            ty = sr.height + y;
            x /= w;
            rx /= w;
            y /= h;
            ty /= h;
            sp->uv0.x = x;
            sp->uv0.y = y;
            sp->uv1.x = x;
            sp->uv1.y = ty;
            sp->uv2.x = rx;
            sp->uv2.y = ty;
            sp->uv3.x = rx;
            sp->uv3.y = y;
        }
        return array;
    }
    /// <summary>
    /// 保存精灵信息为二进制数据
    /// </summary>
    /// <param name="name">数据包名</param>
    /// <param name="path">文件路径</param>
    static void Save(string name, string path)
    {
        DataBuffer buffer = new DataBuffer(4096);
        var fs = buffer.fakeStruct = new FakeStruct(buffer, 2);
        fs.SetData(0, name);
        fs.SetData(1, SaveCategory(buffer));
        byte[] dat = buffer.ToBytes();
        File.WriteAllBytes(path, dat);
    }
}

