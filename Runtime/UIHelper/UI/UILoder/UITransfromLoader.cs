﻿using huqiang.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace huqiang.Helper.HGUI
{
    public class UITransfromLoader : DataLoader
    {
        //public FakeStructHelper TransHelper;
        /// <summary>
        /// 从假结构体中找到该类型的数据
        /// </summary>
        /// <param name="fake">Tranfrom的假结构体</param>
        /// <param name="type">数据类型</param>
        /// <returns></returns>
        public static FakeStruct GetComponent(FakeStruct fake, string type)
        {
            unsafe
            {
                var transfrom = (UITransfromData*)fake.ip;
                var buff = fake.buffer;
                Int16[] coms = buff.GetData(transfrom->coms) as Int16[];
                if(coms!=null)
                {
                    for (int i = 1; i < coms.Length; i += 2)
                    {
                        int a = coms[i];
                        var ti = HGUIManager.GameBuffer.GetTypeInfo(a);
                        if (ti != null)
                        {
                            if (ti.IsSubType(type))
                                return buff.GetData(coms[i - 1]) as FakeStruct;
                        }
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// 将假结构体中的数据载入到组件实体中
        /// </summary>
        /// <param name="fake">假结构体数据</param>
        /// <param name="com">unity组件</param>
        /// <param name="initializer">初始化器,用于反射</param>
        public unsafe override void LoadToObject(FakeStruct fake, Component com,Initializer initializer)
        {
            var src = (UITransfromData*)fake.ip;
            var buff = fake.buffer;
            var trans = com as Transform;
            com.name = buff.GetData(src->name) as string;
            com.tag = buff.GetData(src->tag) as string;
            trans.localEulerAngles = src->localEulerAngles;
            trans.localPosition = src->localPosition;
            trans.localScale = src->localScale;
            trans.gameObject.layer = src->layer;

            Int16[] chi = fake.buffer.GetData(src->child) as Int16[];
            if (chi != null)
                for (int i = 0; i < chi.Length; i++)
                {
                    var fs = buff.GetData(chi[i]) as FakeStruct;
                    if (fs != null)
                    {
                        var go = gameobjectBuffer.CreateNew(fs.GetInt64(0));
                        if (go != null)
                        {
                            go.transform.SetParent(trans);
                            this.LoadToObject(fs, go.transform, initializer);
                        }
                    }
                }

            Int16[] coms = buff.GetData(src->coms) as Int16[];
            if (coms != null)
            {
                for (int i = 0; i < coms.Length; i++)
                {
                    int index = coms[i];
                    i++;
                    int type = coms[i];
                    var fs = buff.GetData(index) as FakeStruct;
                    if (fs != null)
                    {
                        var loader = gameobjectBuffer.GetDataLoader(type);
                        if (loader != null)
                        {
                            loader.initializer = initializer;
                            loader.LoadToComponent(fs, com, fake);
                        }
                    }
                }
            }

            if (initializer != null)
            { 
                //initializer.Initialiezd(fake, trans);
                initializer.AddContext(trans,src->insID);
            }
            LoadHelper(com, buff, src->eve, initializer);
            LoadHelper(com, buff, src->composite, initializer);
            LoadHelper(com, buff, src->ex, initializer);
        }
        static void LoadHelper(Component com,DataBuffer buff,int v, Initializer initializer)
        {
            int type = v >> 16;
            string str = buff.GetData(type) as string;
            if (str != null)
            {
                var ex = buff.GetData(v & 0xffff) as FakeStruct;
                if (ex != null)
                {
                    var tps = typeof(UIHelper).Assembly.GetTypes();
                    if(tps!=null)
                    {
                        for (int i = 0; i < tps.Length; i++)
                        {
                            if (tps[i].Name == str)
                            {
                                var help = com.gameObject.AddComponent(tps[i]) as UIHelper;
                                help.LoadFromBuffer(ex,initializer);
                                return;
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        ///  将到组件实体中的数据载入假结构体中
        /// </summary>
        /// <param name="com">unity组件</param>
        /// <param name="buffer">数据缓存器</param>
        /// <returns></returns>
        public unsafe override FakeStruct LoadFromObject(Component com,DataBuffer buffer)
        {
            var trans =  com as Transform;
            if (trans == null)
                return null;
            FakeStruct fake = new FakeStruct(buffer, UITransfromData.ElementSize);
            UITransfromData* td = (UITransfromData*)fake.ip;
            td->insID = trans.GetInstanceID();
            td->localEulerAngles = trans.localEulerAngles;
            td->localPosition = trans.localPosition;
            td->localScale = trans.localScale;
            td->name = buffer.AddData(trans.name);
            td->tag = buffer.AddData(trans.tag);
            var coms = com.GetComponents<Component>();
            td->type = gameobjectBuffer.GetTypeID(coms);
            td->layer = trans.gameObject.layer;
            List<Int16> tmp = new List<short>();
            for(int i=0;i<coms.Length;i++)
            {
                if (coms[i] is UIHelper)
                {
                    (coms[i] as UIHelper).ToBufferData(buffer,td);
                }else
                if (!(coms[i] is Transform))
                {
                    Int32 type = gameobjectBuffer.GetTypeIndex(coms[i]);
                    if(type>=0)
                    {
                        var loader = gameobjectBuffer.GetDataLoader(type);
                        if (loader != null)
                        {
                            var fs = loader.LoadFromObject(coms[i], buffer);
                            tmp.Add((Int16)buffer.AddData(fs));
                        }
                        else tmp.Add(0);
                        tmp.Add((Int16)type);
                        var scr = coms[i] as UIElement;
                        if (scr != null)
                        {
                            td->size = scr.Content.SizeDelta;
                            td->pivot = scr.Content.Pivot;
                        }
                    }
                }
            }
            td->coms = buffer.AddData(tmp.ToArray());
            int c = trans.childCount;
            if (c > 0)
            {
                Int16[] buf = new short[c];
                for (int i = 0; i < c; i++)
                {
                    var fs = LoadFromObject(trans.GetChild(i), buffer);
                    buf[i] = (Int16)buffer.AddData(fs);
                }
                td->child = buffer.AddData(buf);
            }
            return fake;
        }
    }
}