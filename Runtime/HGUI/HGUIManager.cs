﻿using huqiang.Core.UIData;
using huqiang.Data;
using huqiang.UIEvent;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        /// <summary>
        /// 结构体声明
        /// </summary>
        public FakeStruct[] tables;
        public string Version;
    }
    public class HGUIManager
    {
        /// <summary>
        /// 游戏对象缓存
        /// </summary>
        public static UIelementBuffer GameBuffer;
        public static String Version = "1.0.30";
        /// <summary>
        /// 初始化UI组件,包含有:Transform,HImage,TextBox,HText,HLine,UIElement
        /// </summary>
        /// <param name="buff">回收站的父物体</param>
        public static void Initial()
        {
            GameBuffer = new UIelementBuffer();
            var imgHelper = GameBuffer.RegFakeStructHelper<HImageData>();
            var txtHelper = GameBuffer.RegFakeStructHelper<HTextData>();
            var graphHelper = GameBuffer.RegFakeStructHelper<HGraphicsData>();
            var eleHelper = GameBuffer.RegFakeStructHelper<UIElementData>();
            GameBuffer.RegFakeStructHelper<TextInputData>();

            var imgLoader = new HImageLoader() { ImageHelper = imgHelper, GraphicsHelper = graphHelper, ElementHelper = eleHelper };
            var txtLoader = new HTextLoader() { TextHelper = txtHelper, GraphicsHelper = graphHelper, ElementHelper = eleHelper };
            var grapLoader = new HGraphicsLoader() { GraphicsHelper = graphHelper, ElementHelper = eleHelper };
            var eleLoader = new UIElementLoader() { ElementHelper = eleHelper };

            GameBuffer.RegDataLoader<UIElement>(eleLoader);
            GameBuffer.RegDataLoader<HImage>(imgLoader);
            GameBuffer.RegDataLoader<HText>(txtLoader);
            GameBuffer.RegDataLoader<TextBox>(txtLoader);
            GameBuffer.RegDataLoader<HLine>(grapLoader);
            GameBuffer.RegDataLoader<HGraphics>(grapLoader);
            GameBuffer.RegDataLoader<HCanvas>(eleLoader);
        }
        static List<FakeStruct> AddDataTable(DataBuffer db)
        {
            List<FakeStruct> list = new List<FakeStruct>();
            list.Add(FakeStructHelper.CreateTable<UIElementData>(db));
            list.Add(FakeStructHelper.CreateTable<HGraphicsData>(db));
            list.Add(FakeStructHelper.CreateTable<HImageData>(db));
            list.Add(FakeStructHelper.CreateTable<HTextData>(db));
            list.Add(FakeStructHelper.CreateTable<TextInputData>(db));
            return list;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uiRoot"></param>
        /// <returns></returns>
        public static DataBuffer GetPrefab(Transform uiRoot)
        {
            var ui = uiRoot.GetComponent<Helper.HGUI.UIContext>();
            if (ui != null)
            {
                DataBuffer db = new DataBuffer(1024);
                string tn = ui.GetType().Name;
                var loader = GameBuffer.FindDataLoader(tn);
                var root = loader.SaveUI(ui, db);
                FakeStruct fake = new FakeStruct(db, 3);
                fake[0] = db.AddData(root);
                var list = AddDataTable(db);
                int c = list.Count;
                Int16[] arr = new Int16[c];
                for (int i = 0; i < arr.Length; i++)
                {
                    arr[i] = (Int16)db.AddData(list[i]);
                }
                fake[1] = db.AddData(arr);
                fake[2] = db.AddData(Version);
                db.fakeStruct = fake;
                return db;
            }
            return null;
        }
        /// <summary>
        /// 将场景内的对象保存到文件
        /// </summary>
        /// <param name="uiRoot"></param>
        /// <param name="path"></param>
        public static void SavePrefab(Transform uiRoot, string path)
        {
            File.WriteAllBytes(path, GetPrefab(uiRoot).ToBytes());
        }
        /// <summary>
        /// 所有预制体资源列表
        /// </summary>
        public static List<PrefabAsset> prefabAssets = new List<PrefabAsset>();
        /// <summary>
        /// 载入一个预制体资源
        /// </summary>
        /// <param name="db">资源数据</param>
        /// <param name="name">资源名</param>
        /// <returns></returns>
        public unsafe static PrefabAsset LoadModels(DataBuffer db, string name)
        {
            var fake = db.fakeStruct;
            var asset = new PrefabAsset();
            asset.models = fake.GetData<FakeStruct>(0);
            Int16[] arr = fake.GetData<Int16[]>(1);
            if (arr != null)
            {
                FakeStruct[] fsa = new FakeStruct[arr.Length];
                for (int i = 0; i < arr.Length; i++)
                    fsa[i] = db.GetData(arr[i]) as FakeStruct;
                asset.tables = fsa;
            }
            asset.Version = fake.GetData<string>(2);
            asset.name = name;
            for (int i = 0; i < prefabAssets.Count; i++)
                if (prefabAssets[i].name == name)
                {
                    prefabAssets.RemoveAt(i);
                    break;
                }
            prefabAssets.Add(asset);
            GameBuffer.SetTables(asset.tables,name);
            return asset;
        }
        /// <summary>
        /// 载入一个预制体资源
        /// </summary>
        /// <param name="dat">资源数据</param>
        /// <param name="name">资源名</param>
        /// <returns></returns>
        public unsafe static PrefabAsset LoadModels(byte[] dat, string name)
        {
            DataBuffer db = new DataBuffer(dat);
            return LoadModels(db, name);
        }
        /// <summary>
        /// 查询transform的子物体
        /// </summary>
        /// <param name="fake"></param>
        /// <param name="childName"></param>
        /// <returns></returns>
        public static unsafe FakeStruct FindChild(FakeStruct fake, string childName)
        {
            var data = (UIElementData*)fake.ip;
            var buff = fake.buffer;
            Int16[] chi = fake.buffer.GetData(data->child) as Int16[];
            if (chi != null)
                for (int i = 0; i < chi.Length; i++)
                {
                    var fs = buff.GetData(chi[i]) as FakeStruct;
                    if (fs != null)
                    {
                        var cd = (UIElementData*)fs.ip;
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
        public static unsafe FakeStruct FindModel(string assetName, string childName)
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
        /// 查询一个模型
        /// </summary>
        /// <param name="assetName">资源包名</param>
        /// <param name="childName">模型名称</param>
        /// <returns></returns>
        public static unsafe FakeStruct FindModelAndSetAssets(string assetName, string childName)
        {
            for (int i = 0; i < prefabAssets.Count; i++)
            {
                if (prefabAssets[i].name == assetName)
                {
                    if (prefabAssets[i].tables != null)
                        GameBuffer.SetTables(prefabAssets[i].tables, assetName);
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
            var data = (UIElementData*)fake.ip;
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
        public static void RecycleChild(UIElement game)
        {
            if (game == null)
                return;
            GameBuffer.RecycleChild(game);
        }
        public static void RecycleUI(UIElement game)
        {
            GameBuffer.RecycleUI(game);
        }
        public static void RecycleChild(UIElement game, string[] keep)
        {
            var child = game.child;
            var c = child.Count-1;
            for(; c>=0;c--)
            {
                var son = child[c];
                for (int i = 0; i < keep.Length; i++)
                {
                    if (keep[i] == son.name)
                        goto label;
                }
                RecycleUI(son);
            label:;
            }
        } 
        public static UIElement Clone(FakeStruct fake, UIInitializer initializer = null)
        {
            if (fake == null)
                return null;
            var ui = GameBuffer.Clone(fake, initializer);
            if (initializer != null)
                initializer.Done();
            return ui;
        }
    }
}
