using huqiang.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huqiang.Core.HGUI
{
    /// <summary>
    /// 预制体资源
    /// </summary>
    public class PrefabAsset
    {
        /// <summary>
        /// 资源名称
        /// </summary>
        public string name;
        /// <summary>
        /// 模型数据
        /// </summary>
        public FakeStruct models;
    }
    public class HGUIManager
    {
        /// <summary>
        /// 游戏对象缓存
        /// </summary>
        public static GameobjectBuffer GameBuffer;
        /// <summary>
        /// 初始化UI组件,包含有:Transform,HImage,TextBox,HText,HLine,UIElement
        /// </summary>
        /// <param name="buff">回收站的父物体</param>
        public static void Initial(Transform buff)
        {
            GameBuffer = new GameobjectBuffer(buff);
            GameBuffer.RegComponent(new ComponentInfo<Transform>() { loader = new TransfromLoader() });
            GameBuffer.RegComponent(new ComponentInfo<HImage>() { loader = new HImageLoader() });
            GameBuffer.RegComponent(new ComponentInfo<TextBox>() { loader = new HTextLoader() });
            GameBuffer.RegComponent(new ComponentInfo<HText>() { loader = new HTextLoader() });
            GameBuffer.RegComponent(new ComponentInfo<HLine>() { loader = new HGraphicsLoader() });
            GameBuffer.RegComponent(new ComponentInfo<UIElement>() { loader = new UIElementLoader() });
          
        }
        /// <summary>
        /// 将场景内的对象保存到文件
        /// </summary>
        /// <param name="uiRoot"></param>
        /// <param name="path"></param>
        public static void SavePrefab(Transform uiRoot, string path)
        {
            DataBuffer db = new DataBuffer(1024);
            db.fakeStruct = GameBuffer.GetDataLoader(0).LoadFromObject(uiRoot, db);
            File.WriteAllBytes(path, db.ToBytes());
        }
        /// <summary>
        /// 所有预制体资源列表
        /// </summary>
        public static List<PrefabAsset> prefabAssets = new List<PrefabAsset>();
        /// <summary>
        /// 载入一个预制体资源
        /// </summary>
        /// <param name="dat">资源数据</param>
        /// <param name="name">资源名</param>
        /// <returns></returns>
        public unsafe static PrefabAsset LoadModels(byte[] dat, string name)
        {
            DataBuffer db = new DataBuffer(dat);
            var asset = new PrefabAsset();
            asset.models = db.fakeStruct;
            asset.name = name;
            for (int i = 0; i < prefabAssets.Count; i++)
                if (prefabAssets[i].name == name)
                { prefabAssets.RemoveAt(i); break; }
            prefabAssets.Add(asset);
            return asset;
        }
        /// <summary>
        /// 查询transform的子物体
        /// </summary>
        /// <param name="fake"></param>
        /// <param name="childName"></param>
        /// <returns></returns>
        public static unsafe FakeStruct FindChild(FakeStruct fake, string childName)
        {
            var data = (TransfromData*)fake.ip;
            var buff = fake.buffer;
            Int16[] chi = fake.buffer.GetData(data->child) as Int16[];
            if (chi != null)
                for (int i = 0; i < chi.Length; i++)
                {
                    var fs = buff.GetData(chi[i]) as FakeStruct;
                    if (fs != null)
                    {
                        var cd = (TransfromData*)fs.ip;
                        string name = buff.GetData(cd->name) as string;
                        if (name == childName)
                            return fs;
                    }
                }
            return null;
        }
        /// <summary>
        /// 查询一个模型
        /// </summary>
        /// <param name="assetName">资源包名</param>
        /// <param name="childName">模型名称</param>
        /// <returns></returns>
        public static unsafe FakeStruct FindModel(string assetName,string childName)
        {
            for (int i = 0; i < prefabAssets.Count; i++)
            {
                if (prefabAssets[i].name == assetName)
                {
                    return FindChild(prefabAssets[i].models, childName);
                }
            }
            return null;
        }
        /// <summary>
        /// 获取transform的所有子物体
        /// </summary>
        /// <param name="fake"></param>
        /// <param name="childName"></param>
        /// <returns></returns>
        public static unsafe FakeStruct[] GetAllChild(FakeStruct fake)
        {
            var data = (TransfromData*)fake.ip;
            var buff = fake.buffer;
            Int16[] chi = fake.buffer.GetData(data->child) as Int16[];
            if (chi != null)
            {
                FakeStruct[] fakes = new FakeStruct[chi.Length];
                for (int i = 0; i < chi.Length; i++)
                {
                    fakes[i] = buff.GetData(chi[i]) as FakeStruct;
                }
                return fakes;
            }
            return null;
        }
        /// <summary>
        /// 获取资源包下的所有子模型
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        public static FakeStruct[] GetAllChild(string asset)
        {
            for (int i = 0; i < prefabAssets.Count; i++)
            {
                if (prefabAssets[i].name == asset)
                {
                    var fake = prefabAssets[i].models;
                    return GetAllChild(fake);
                }
            }
            return null;
        }
    }
}
