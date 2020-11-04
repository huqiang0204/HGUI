using huqiang.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace huqiang.Data2D
{
    public unsafe struct TransfromData
    {
        /// <summary>
        /// 对象类型, 所有组件的位或值
        /// </summary>
        public Int64 type;
        /// <summary>
        /// 实例ID
        /// </summary>
        public Int32 insID;
        public Vector3 localEulerAngles;
        public Vector3 localPosition;
        public Vector3 localScale;
        /// <summary>
        /// 名称
        /// </summary>
        public StringPoint name;
        /// <summary>
        /// 标记
        /// </summary>
        public StringPoint tag;
        /// <summary>
        /// int32数组,高16位为索引,低16位为类型
        /// </summary>
        public IntArrayPoint coms;
        /// <summary>
        /// int16数组
        /// </summary>
        public Int16ArrayPoint child;
        public int layer;
        /// <summary>
        /// 附加信息,用于存储helper中写入的数据
        /// </summary>
        public FakeStrcutPoint ex;
        public static int Size = sizeof(TransfromData);
        public static int ElementSize = Size / 4;
    }
    public class TransfromLoader : DataLoader
    {
        public static FakeStruct GetEx(FakeStruct fake)
        {
            unsafe
            {
                var trans = (TransfromData*)fake.ip;
                return fake.buffer.GetData(trans->ex) as FakeStruct;
            }
        }
        /// <summary>
        /// 将假结构体中的数据载入到组件实体中
        /// </summary>
        /// <param name="fake">假结构体数据</param>
        /// <param name="com">unity组件</param>
        /// <param name="initializer">初始化器,用于反射</param>
        public unsafe override void LoadToObject(FakeStruct fake, Component com, Initializer initializer)
        {
            //TransfromData src = new TransfromData();
            //unsafe { transHelper.LoadData((byte*)&src,fake.ip); }
            var src = (TransfromData*)fake.ip;
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
                initializer.Initialiezd(fake, trans);
                initializer.AddContext(trans, src->insID);
            }
        }
        /// <summary>
        ///  将到组件实体中的数据载入假结构体中
        /// </summary>
        /// <param name="com">unity组件</param>
        /// <param name="buffer">数据缓存器</param>
        /// <returns></returns>
        public unsafe override FakeStruct LoadFromObject(Component com, DataBuffer buffer)
        {
            var trans = com as Transform;
            if (trans == null)
                return null;
            FakeStruct fake = new FakeStruct(buffer, TransfromData.ElementSize);
            TransfromData* td = (TransfromData*)fake.ip;
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
            for (int i = 0; i < coms.Length; i++)
            {
                if (!(coms[i] is Transform))
                {
                    Int32 type = gameobjectBuffer.GetTypeIndex(coms[i]);
                    if (type >= 0)
                    {
                        var loader = gameobjectBuffer.GetDataLoader(type);
                        if (loader != null)
                        {
                            var fs = loader.LoadFromObject(coms[i], buffer);
                            tmp.Add((Int16)buffer.AddData(fs));
                        }
                        else tmp.Add(0);
                        tmp.Add((Int16)type);
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
