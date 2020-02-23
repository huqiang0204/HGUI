using huqiang;
using huqiang.UIComposite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public class EditorModelManager
{
    static List<UnityEngine.Object[]> objects = new List<UnityEngine.Object[]>();
    public static void LoadAllTexture(string folder)
    {
        var path = Application.dataPath;
        if (folder != null)
            path += "/" + folder;
    }
    static UnityEngine.Object[] LoadSprite(string name)
    {
        string path = null;
        var fs = AssetDatabase.FindAssets(name);
        if (fs != null)
        {
            HashSet<string> hash = new HashSet<string>();
            for (int i = 0; i < fs.Length; i++)
                hash.Add(fs[i]);
            var list = hash.ToArray();
            for (int i = 0; i < list.Length; i++)
            {
                path = AssetDatabase.GUIDToAssetPath(list[i]);
                var ss = path.Split('/');
                var str = ss[ss.Length - 1];
                ss = str.Split('.');
                var sp = AssetDatabase.LoadAllAssetsAtPath(path);
                if (sp != null)
                    if (sp.Length > 0)
                    {
                        objects.Add(sp);
                        return sp;
                    }
            }
        }
        return null;
    }
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
    public static void Clear()
    {
        objects.Clear();
    }
}

