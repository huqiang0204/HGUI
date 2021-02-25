using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace huqiang.Data
{
    public class UnsafeDataWriter
    {
        /// <summary>
        /// 是否有派生类，有派生类则需要额外的反射开销
        /// </summary>
        public bool HaveDerived = false;
        public DataInfo dataInfo;
        List<DataTypeInfo> types;
        protected List<ClassContext> objList = new List<ClassContext>();
        /// <summary>
        /// 是否有实际的派生数据
        /// </summary>
        bool DerivedData;
        /// <summary>
        ///  可在返回数据中添加附加信息，预留字段为FakeStruct[4]
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public DataBuffer Write<T>(object instance, DataInfo info)
        {
#if ENABLE_IL2CPP
            UnityEngine.Debug.LogError("IL2CPP Not supported");
            return null;
#endif
            if (instance == null)
                return null;
            if (info == null)
            {
                if (dataInfo == null)
                {
                    dataInfo = new DataInfo();
                    dataInfo.Analysis(instance.GetType());
                }
            }
            else dataInfo = info;
            types = dataInfo.dataTypes;
            DataBuffer db = new DataBuffer();
            FakeStruct fa = new FakeStruct(db, 5);//预留一个字段，用于扩展信息
            string str = instance.ToString();
            fa[0] = db.AddData(str, DataType.String);
            fa[1] = WriteObject(str, instance, db);
            fa[2] = db.AddData(dataInfo.WriteTables(db), DataType.FakeStructArray);
            fa[3] = DerivedData ? 1 : 0;
            db.fakeStruct = fa;
            objList.Clear();
            return db;
        }
        /// <summary>
        /// 写入结构体的数据
        /// </summary>
        /// <param name="typeIndex"></param>
        /// <param name="instance"></param>
        /// <param name="fake"></param>
        void WriteStruct(DataTypeInfo info, IntPtr ptr, FakeStruct fake)
        {
            var dts = info.dataFeilds;
            for (int i = 0; i < dts.Count; i++)
            {
                WriteField(fake, 0, dts[i], ptr);
            }
        }
        /// <summary>
        /// 写入类的数据
        /// </summary>
        /// <param name="type"></param>
        /// <param name="instance"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        int WriteObject(string type, object instance, DataBuffer db)
        {
            return WriteObject(dataInfo.FindTypeInfo2(type), instance, db);
        }
        /// <summary>
        /// 写入类的数据
        /// </summary>
        /// <param name="typeIndex"></param>
        /// <param name="instance"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        int WriteObject(DataTypeInfo dt, object instance, DataBuffer db)
        {
            if (instance == null)
                return 0;
            for (int i = 0; i < objList.Count; i++)
                if (objList[i].instance == instance)
                    return objList[i].index;
            int typeIndex = dt.typeIndex;
            if (HaveDerived)
            {
                string fullName = instance.ToString();
                if (fullName != dt.fullName)
                    dt = dataInfo.FindTypeInfo2(fullName);
                if (dt == null)
                    return 0;
                else
                {
                    DerivedData = true;
                    typeIndex = dt.typeIndex;
                }
            }
            FakeStruct fake = new FakeStruct(db, types[typeIndex].ElementLength);
            int r = db.AddData(fake, DataType.FakeStruct);
            ClassContext cc = new ClassContext();
            cc.instance = instance;
            cc.index = typeIndex << 16 | r;
            objList.Add(cc);
            var dts = dt.dataFeilds;
            IntPtr ptr = UnsafeOperation.GetObjectAddr(instance);
            for (int i = 0; i < dts.Count; i++)
            {
                WriteField(fake, 0, dts[i], ptr);
            }
            return cc.index;
        }
        void WriteField(FakeStruct fake, int start, DataFieldInfo info, IntPtr ptr)
        {
            int offset = start + info.DataOffset;
            if (info.DBType < 32)
            {
                if (info.DBType < 16)//数值
                {
                    WriteValue(fake, offset, info, ptr);
                }
                else//引用类型
                {
                    if (info.DBType == DataType2.String)
                    {
                        var obj = UnsafeOperation.GetObject(ptr + info.MemOffset);
                        if (obj != null)
                            fake[offset] = fake.buffer.AddData(obj, DataType.String);
                    }
                    else
                    {
                        var obj = UnsafeOperation.GetObject(ptr + info.MemOffset);
                        if (obj != null)
                        {
                            fake[offset] = WriteObject(info.TypeFullName, obj, fake.buffer);
                        }
                    }
                }
            }
            else if (info.DBType < 64)//数组
            {
                WriteArray(fake, offset, info, ptr);
            }
            else//二维数组
            {
                WriteArray2(fake, offset, info, ptr);
            }
        }
        void WriteValue(FakeStruct fake, int offset, DataFieldInfo info, IntPtr ptr)
        {
            if (info.DBType < 6)
            {
                if(info.DataLength==1)
                {
                    unsafe { fake[offset] = *(byte*)(ptr + info.MemOffset); }
                }
                else if(info.DataLength==2)
                {
                    unsafe { fake[offset] = *(Int16*)(ptr + info.MemOffset); }
                }
                else
                {
                    unsafe { fake[offset] = *(Int32*)(ptr + info.MemOffset); }
                }
            }
            else if (info.DBType == DataType2.Float)
            {
                unsafe { fake.SetFloat(offset, *(float*)(ptr + info.MemOffset)); }
            }
            else if (info.DBType < 8)
            {
                unsafe { fake.SetInt64(offset, *(Int64*)(ptr + info.MemOffset)); }
            }
            else if (info.DBType == DataType2.Double)
            {
                unsafe { fake.SetDouble(offset, *(Double*)(ptr + info.MemOffset)); }
            }
            else if (info.DBType == DataType2.Decimal)
            {
                unsafe { fake.SetDecimal(offset, *(Decimal*)(ptr + info.MemOffset)); }
            }
            else if (info.DBType == DataType2.Boolean)
            {
                unsafe { fake[offset] = *(byte*)(ptr + info.MemOffset); }
            }
            else if (info.DBType == 12)
            {
                WriteStruct(fake, offset, info, ptr + info.MemOffset);
            }
        }
        void WriteStruct(FakeStruct fake, int start, DataFieldInfo info, IntPtr ptr)
        {
            var dts = info.typeInfo.dataFeilds;
            for (int i = 0; i < dts.Count; i++)
            {
                WriteField(fake, start, dts[i], ptr);
            }
        }
        void WriteArray(FakeStruct fake, int offset, DataFieldInfo info, IntPtr ptr)
        {
            var buffer = fake.buffer;
            object value;
            value = UnsafeOperation.GetObject(ptr + info.MemOffset);
            if (value == null)
                return;
            int a = info.DBType & 31;
            if (info.TypeName == DataType2.List)
            {
                switch (a)
                {
                    case DataType2.Byte:
                        fake[offset] = buffer.AddData(ToArray<byte>(value), DataType.ByteArray);
                        break;
                    case DataType2.Int16:
                        fake[offset] = buffer.AddData(ToArray<Int16>(value), DataType.Int16Array);
                        break;
                    case DataType2.Int32:
                        fake[offset] = buffer.AddData(ToArray<Int32>(value), DataType.Int32Array);
                        break;
                    case DataType2.Float:
                        fake[offset] = buffer.AddData(ToArray<float>(value), DataType.FloatArray);
                        break;
                    case DataType2.String:
                        {
                            string[] str = ToArray<string>(value);
                            if (str != null)
                            {
                                fake[offset] = buffer.AddData(new FakeStringArray(str), DataType.FakeStringArray);
                            }
                        }
                        break;
                    case DataType2.Int64:
                        fake[offset] = buffer.AddData(ToArray<Int64>(value), DataType.Int64Array);
                        break;
                    case DataType2.Double:
                        fake[offset] = buffer.AddData(ToArray<Double>(value), DataType.DoubleArray);
                        break;
                    case DataType2.Class:
                        fake[offset] = WriteClassArray(value, fake.buffer, info.typeInfo);
                        break;
                    case DataType2.Struct:
                        fake[offset] = WriteStructList(value, fake.buffer, info.typeInfo);
                        break;
                }
            }
            else
            {
                switch (a)
                {
                    case DataType2.Byte:
                        fake[offset] = buffer.AddData(value, DataType.ByteArray);
                        break;
                    case DataType2.Int16:
                        fake[offset] = buffer.AddData(value, DataType.Int16Array);
                        break;
                    case DataType2.Int32:
                        fake[offset] = buffer.AddData(value, DataType.Int32Array);
                        break;
                    case DataType2.Float:
                        fake[offset] = buffer.AddData(value, DataType.FloatArray);
                        break;
                    case DataType2.String:
                        fake[offset] = buffer.AddData(new FakeStringArray(value as string[]), DataType.FakeStringArray);
                        break;
                    case DataType2.Int64:
                        fake[offset] = buffer.AddData(value, DataType.Int64Array);
                        break;
                    case DataType2.Double:
                        fake[offset] = buffer.AddData(value, DataType.DoubleArray);
                        break;
                    case DataType2.Class:
                        fake[offset] = WriteClassArray(value, fake.buffer, info.typeInfo);
                        break;
                    case DataType2.Struct:
                        fake[offset] = WriteStructArray(value, fake.buffer, info.typeInfo);
                        break;
                }
            }
        }
        T[] ToArray<T>(object value)
        {
            var list = value as List<T>;
            if (list != null)
                return list.ToArray();
            return null;
        }
        int WriteClassArray(object value, DataBuffer db, DataTypeInfo info)
        {
            var list = value as IList;
            if (list != null)
            {
                if (list.Count > 0)
                {
                    if (HaveDerived)
                    {
                        Int32[] aar = new Int32[list.Count];
                        for (int i = 0; i < list.Count; i++)
                        {
                            var obj = list[i];
                            if (obj != null)
                            {
                                string fullName = obj.ToString();
                                if (fullName == info.fullName)
                                    aar[i] = WriteObject(info, obj, db);
                                else
                                {
                                    var dt = dataInfo.FindTypeInfo2(fullName);
                                    if (dt != null)
                                        aar[i] = WriteObject(dt, obj, db);
                                }
                            }
                        }
                        return db.AddData(aar, DataType.Int32Array);
                    }
                    else
                    {
                        Int16[] aar = new Int16[list.Count];
                        for (int i = 0; i < list.Count; i++)
                        {
                            aar[i] = (Int16)WriteObject(info, list[i], db);
                        }
                        return db.AddData(aar, DataType.Int16Array);
                    }
                }
            }
            return 0;
        }
        int WriteStructArray(object value, DataBuffer db, DataTypeInfo info)
        {
            var list = value as IList;
            if (list != null)
            {
                if (list.Count > 0)
                {
                    int el = info.ElementLength;
                    int dl = info.DataLength;
                    IntPtr ptr = Marshal.UnsafeAddrOfPinnedArrayElement(list as Array, 0);

                    FakeStructArray fsa = new FakeStructArray(db, el, list.Count);
                    unsafe
                    {
                        FakeStruct fs = new FakeStruct(db, el, fsa.ip);
                        for (int i = 0; i < list.Count; i++)
                        {
                            fs.SetPoint(fsa[i]);
                            WriteStruct(info, ptr, fs);
                            ptr += dl;
                        }
                    }
                    return db.AddData(fsa, DataType.FakeStructArray);
                }
            }
            return 0;
        }
        int WriteStructList(object value, DataBuffer db, DataTypeInfo info)
        {
            var list = value as IList;
            if (list != null)
            {
                if (list.Count > 0)
                {
                    int el = info.ElementLength;
                    int dl = info.DataLength;
                    IntPtr ptr = UnsafeOperation.GetObjectAddr(list);
                    ptr = UnsafeOperation.GetListElement(ptr, 0);

                    FakeStructArray fsa = new FakeStructArray(db, el, list.Count);
                    unsafe
                    {
                        FakeStruct fs = new FakeStruct(db, el, fsa.ip);
                        for (int i = 0; i < list.Count; i++)
                        {
                            fs.SetPoint(fsa[i]);
                            WriteStruct(info, ptr, fs);
                            ptr += dl;
                        }
                    }
                    return db.AddData(fsa, DataType.FakeStructArray);
                }
            }
            return 0;
        }
        void WriteArray2(FakeStruct fake, int offset, DataFieldInfo info, IntPtr ptr)
        {
            var buffer = fake.buffer;
            object value;
            value = UnsafeOperation.GetObject(ptr + info.MemOffset);
            if (value == null)
                return;
            int a = info.DBType & 31;
            switch (a)
            {
                case DataType2.Byte:
                    fake[offset] = WriteArray2<byte>(value, buffer, DataType.ByteArray);
                    break;
                case DataType2.Int16:
                    fake[offset] = WriteArray2<Int16>(value, buffer, DataType.ByteArray);
                    break;
                case DataType2.Int32:
                    fake[offset] = WriteArray2<Int32>(value, buffer, DataType.ByteArray);
                    break;
                case DataType2.Float:
                    fake[offset] = WriteArray2<float>(value, buffer, DataType.ByteArray);
                    break;
                case DataType2.String:
                    fake[offset] = WriteStringArray2(value, buffer, DataType.ByteArray);
                    break;
                case DataType2.Int64:
                    fake[offset] = WriteArray2<Int64>(value, buffer, DataType.ByteArray);
                    break;
                case DataType2.Double:
                    fake[offset] = WriteArray2<Double>(value, buffer, DataType.ByteArray);
                    break;
                case DataType2.Class:
                    fake[offset] = WriteClassArray2(value, fake.buffer, info);
                    break;
                case DataType2.Struct:
                    fake[offset] = WriteStructArray2(value, fake.buffer, info);
                    break;
            }
        }
        int WriteArray2<T>(object value, DataBuffer db, int dataType)
        {
            //List<List<T>>
            //List<T[]>
            //T[][]
            var list = value as IList;
            if (list != null)
            {
                int c = list.Count;
                if (c > 0)
                {
                    Int16[] addr = new short[c];
                    for (int i = 0; i < c; i++)
                    {
                        var obj = list[i];
                        T[] tmp = obj as T[];
                        if (tmp == null)
                        {
                            List<T> lt = obj as List<T>;
                            if (lt != null)
                                tmp = lt.ToArray();
                        }
                        if (tmp != null)
                            addr[i] = (Int16)db.AddData(tmp, dataType);
                    }
                    return db.AddData(addr, DataType.Int16Array);
                }
            }
            return 0;
        }
        int WriteStringArray2(object value, DataBuffer db, int dataType)
        {
            //List<List<String>>
            //List<String[]>
            //String[][]
            var list = value as IList;
            if (list != null)
            {
                int c = list.Count;
                if (c > 0)
                {
                    Int16[] addr = new short[c];
                    for (int i = 0; i < c; i++)
                    {
                        var obj = list[i];
                        string[] tmp = obj as string[];
                        if (tmp == null)
                        {
                            List<string> lt = obj as List<string>;
                            if (lt != null)
                                tmp = lt.ToArray();
                        }
                        if (tmp != null)
                            addr[i] = (Int16)db.AddData(new FakeStringArray(tmp), DataType.FakeStringArray);
                    }
                    return db.AddData(addr, DataType.Int16Array);
                }
            }
            return 0;
        }
        int WriteClassArray2(object value, DataBuffer db, DataFieldInfo info)
        {
            var list = value as IList;
            if (list != null)
            {
                int c = list.Count;
                if (c > 0)
                {
                    Int16[] addr = new short[c];
                    for (int i = 0; i < c; i++)
                    {
                        var obj = list[i];
                        if (obj != null)
                        {
                            addr[i] = (Int16)WriteClassArray(obj, db, info.typeInfo);
                        }
                    }
                    return db.AddData(addr, DataType.Int16Array);
                }
            }
            return 0;
        }
        int WriteStructArray2(object value, DataBuffer db, DataFieldInfo info)
        {
            var list = value as IList;
            if (list != null)
            {
                int c = list.Count;
                if (c > 0)
                {
                    Int16[] addr = new short[c];
                    if(info.ChildTypeName == DataType2.List)
                    {
                        for (int i = 0; i < c; i++)
                        {
                            var obj = list[i];
                            if (obj != null)
                            {
                                addr[i] = (Int16)WriteStructList(obj, db, info.typeInfo);
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < c; i++)
                        {
                            var obj = list[i];
                            if (obj != null)
                            {
                                addr[i] = (Int16)WriteStructArray(obj, db, info.typeInfo);
                            }
                        }
                    }
                    return db.AddData(addr, DataType.Int16Array);
                }
            }
            return 0;
        }
    }
}
