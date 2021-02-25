using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace huqiang.Data
{
    public class UnsafeDataReader
    {
        /// <summary>
        /// 是否有派生类，有派生类则需要额外的反射开销
        /// </summary>
        bool HaveDerived = false;
        /// <summary>
        /// 旧版的类声明
        /// </summary>
        protected DataTypeInfo[] oldTypes;
        List<DataFieldInfo> tmp = new List<DataFieldInfo>();
        protected List<ClassContext> objList = new List<ClassContext>();
        public DataInfo dataInfo;
        public T Read<T>(DataBuffer db, DataInfo info) where T : class, new()
        {
#if ENABLE_IL2CPP
            UnityEngine.Debug.LogError("IL2CPP Not supported");
            return null;
#endif
            if (info == null)
            {
                if (dataInfo == null)
                {
                    dataInfo = new DataInfo();
                    dataInfo.Analysis(typeof(T));
                }
            }
            else dataInfo = info;
            objList.Clear();
            string fullName = typeof(T).FullName;
            var fs = db.fakeStruct;
            string str = fs.GetData<string>(0);
            HaveDerived = fs[3] == 1;
            var fsa = fs.GetData<FakeStructArray>(2);
            if (fsa != null)
            {
                oldTypes = dataInfo.ReadTables(fsa);
            }
            if (HaveDerived)
            {
                if (MatchingTables2() == false)
                {
                    return null;
                }
            }
            else
            {
                MatchingTable(fullName, str);
            }
            var fake = fs.GetData<FakeStruct>(1);
            if (fake != null)
                return ReadObject(fullName, fake, fs[1]) as T;
            return null;
        }
        /// <summary>
        /// 匹配类型表
        /// </summary>
        /// <param name="cur"></param>
        /// <param name="old"></param>
        void MatchingTable(string cur, string old)
        {
            var a = dataInfo.FindTypeInfo2(cur);
            if (a == null)
                return;
            if (a.OldType != null)//已经匹配过了,不能重复匹配
                return;
            for (int i = 0; i < oldTypes.Length; i++)
                if (old == oldTypes[i].fullName)
                {
                    MatchingTable(a, oldTypes[i]);
                    break;
                }
        }
        /// <summary>
        /// 匹配类型表
        /// </summary>
        /// <param name="cur"></param>
        /// <param name="old"></param>
        void MatchingTable(DataTypeInfo cur, DataTypeInfo old)
        {
            old.NewType = cur;
            cur.OldType = old;
            var ts = MatchingFields(cur.dataFeilds, old.oldDataFeilds);
            cur.matchedFields = ts;
            if (ts != null)
                for (int i = 0; i < ts.Length; i++)
                {
                    if (ts[i].HaveTable)
                    {
                        if (ts[i].DBType>=32)
                        {
                            MatchingTable(ts[i].ChildTypeFullName, ts[i].OldFieldInfo.ChildTypeFullName);
                        }
                        else
                        {
                            MatchingTable(ts[i].TypeFullName, ts[i].OldFieldInfo.TypeFullName);
                        }
                    }
                }
        }
        /// <summary>
        /// 匹配字段
        /// </summary>
        /// <param name="cur"></param>
        /// <param name="old"></param>
        /// <returns></returns>
        DataFieldInfo[] MatchingFields(List<DataFieldInfo> cur, DataFieldInfo[] old)
        {
            tmp.Clear();
            for (int i = 0; i < cur.Count; i++)
            {
                string name = cur[i].FieldName;
                for (int j = 0; j < old.Length; j++)
                {
                    if (old[j].FieldName == name)
                    {
                        int a = cur[i].DBType;
                        int b = old[j].DBType;
                        if (CheckType(a, b))
                        {
                            cur[i].OldFieldInfo = old[j];
                            tmp.Add(cur[i]);
                        }
                        break;
                    }
                }
            }
            if (tmp.Count == 0)
                return null;
            return tmp.ToArray();
        }
        bool CheckType(int a, int b)
        {
            if (a == b)
            {
                return true;
            }
            if (a < 11 & b < 11)
                return true;
            int c = a & ~0x1f;
            int d = b & ~0x1f;
            if (c != d)
                return false;
            c = a & 0x1f;
            d = b & 0x1f;
            switch (c)
            {
                case DataType2.Struct:
                    if (d == DataType2.Class)
                        return true;
                    return false;
                case DataType2.Class:
                    if (d == DataType2.Struct)
                        return true;
                    return false;
            }
            return false;
        }
        /// <summary>
        /// 匹配字段
        /// </summary>
        /// <returns></returns>
        bool MatchingTables2()
        {
            for (int i = 0; i < oldTypes.Length; i++)
            {
                var name = oldTypes[i].fullName;
                var dt = dataInfo.FindTypeInfo2(name);
                if (dt == null)
                {
                    dt = dataInfo.Analysis(name);
                }
                if (dt == null)
                    return false;
                dt.OldType = oldTypes[i];
                oldTypes[i].NewType = dt;
                dt.matchedFields = MatchingFields(dt.dataFeilds, oldTypes[i].oldDataFeilds);
            }
            return true;
        }
        object ReadObject(string typeName, FakeStruct fake, int dataIndex)
        {
            for (int i = 0; i < objList.Count; i++)
                if (objList[i].index == dataIndex)
                    return objList[i].instance;
            var dt = dataInfo.FindTypeInfo2(typeName);
            if (dt != null)
            {
                var obj = dt.Construction();
                ClassContext cc = new ClassContext();
                cc.instance = obj;
                cc.index = dataIndex;
                objList.Add(cc);
                IntPtr ptr = UnsafeOperation.GetObjectAddr(obj);
                var mf = dt.matchedFields;
                if (mf != null)
                    for (int i = 0; i < mf.Length; i++)
                    {
                        ReadField(fake, 0, mf[i], ptr);
                    }
                return obj;
            }
            return null;
        }
        object ReadObject(DataTypeInfo dt, FakeStruct fake, int dataIndex)
        {
            for (int i = 0; i < objList.Count; i++)
                if (objList[i].index == dataIndex)
                    return objList[i].instance;
            var obj = dt.Construction();
            ClassContext cc = new ClassContext();
            cc.instance = obj;
            cc.index = dataIndex;
            objList.Add(cc);
            IntPtr ptr = UnsafeOperation.GetObjectAddr(obj);
            var mf = dt.matchedFields;
            if (mf != null)
                for (int i = 0; i < mf.Length; i++)
                {
                    ReadField(fake, 0, mf[i], ptr);
                }
            return obj;
        }
        object ReadStructToObject(DataTypeInfo dt, FakeStruct fake, int start)
        {
            var obj = dt.Construction();
            IntPtr ptr = UnsafeOperation.GetObjectAddr(obj);
            var mf = dt.matchedFields;
            if (mf != null)
                for (int i = 0; i < mf.Length; i++)
                {
                    ReadField(fake, start, mf[i], ptr);
                }
            return obj;
        }
        void ReadStruct(DataTypeInfo dt, FakeStruct fake, int offset, IntPtr ptr)
        {
            var mf = dt.matchedFields;
            if (mf != null)
                for (int i = 0; i < mf.Length; i++)
                {
                    ReadField(fake, offset, mf[i], ptr);
                }
        }
        void ReadField(FakeStruct fake, int start, DataFieldInfo info, IntPtr ptr)
        {
            int offset = start + info.OldFieldInfo.DataOffset;
            if (info.DBType < 11)
            {
                CompatibleReadValue(info, fake, offset, ptr);
            }
            else if (info.DBType < 32)
            {
                switch (info.DBType)
                {
                    case DataType2.String:
                        string str = fake.GetData<string>(offset);
                        if (str != null)
                            UnsafeOperation.SetObject(ptr + info.MemOffset, str);
                        break;
                    case DataType2.Class:
                        if (info.OldFieldInfo.DBType == DataType2.Struct)
                        {
                            UnsafeOperation.SetObject(ptr + info.MemOffset, ReadStructToObject(info.typeInfo, fake, offset));
                        }
                        else
                        {
                            FakeStruct fs = fake.GetData<FakeStruct>(offset);
                            if (fs != null)
                            {
                                if (HaveDerived)
                                {
                                    int a = fake[offset];
                                    int dbtype = a >> 16;
                                    int index = a & 0xffff;
                                    var dt = oldTypes[dbtype].NewType;
                                    if (dt != null)
                                        UnsafeOperation.SetObject(ptr + info.MemOffset, ReadObject(dt, fs, index));
                                }
                                else UnsafeOperation.SetObject(ptr + info.MemOffset, ReadObject(info.typeInfo, fs, fake[offset] & 0xffff));
                            }
                        }
                        break;
                    case DataType2.Struct:
                        if (info.OldFieldInfo.DBType == DataType2.Class)
                        {
                            var fs = fake.GetData<FakeStruct>(offset);
                            if (fs != null)
                                ReadStruct(info.typeInfo, fs, 0, ptr + info.MemOffset);
                        }
                        else
                        {
                            ReadStruct(info.typeInfo, fake, offset, ptr + info.MemOffset);
                        }
                        break;
                    case DataType2.Boolean:
                        unsafe { *(byte*)(ptr + info.MemOffset) = (byte)fake[offset]; }
                        break;
                }
            }
            else if (info.DBType < 64)
            {
                if (info.TypeName == "List`1")
                    UnsafeOperation.SetObject(ptr + info.MemOffset, ReadList(info, fake, offset));
                else UnsafeOperation.SetObject(ptr + info.MemOffset, ReadArray(info, fake, offset));
            }
            else
            {
                if (info.TypeName == "List`1")
                    UnsafeOperation.SetObject(ptr + info.MemOffset, ReadList2(info, fake, offset));
                else UnsafeOperation.SetObject(ptr + info.MemOffset, ReadArray2(info, fake, offset));
            }
        }
        void CompatibleReadValue(DataFieldInfo info, FakeStruct fake, int offset, IntPtr ptr)
        {
            switch (info.DBType)
            {
                case DataType2.Byte:
                    unsafe { *(byte*)(ptr + info.MemOffset) = (byte)fake[offset]; }
                    break;
                case DataType2.SByte:
                    unsafe { *(sbyte*)(ptr + info.MemOffset) = (sbyte)fake[offset]; }
                    break;
                case DataType2.Int16:
                    unsafe { *(Int16*)(ptr + info.MemOffset) = (Int16)fake[offset]; }
                    break;
                case DataType2.UInt16:
                    unsafe { *(UInt16*)(ptr + info.MemOffset) = (UInt16)fake[offset]; }
                    break;
                case DataType2.Int32:
                    unsafe { *(Int32*)(ptr + info.MemOffset) = (Int32)fake[offset]; }
                    break;
                case DataType2.UInt32:
                    unsafe { *(UInt32*)(ptr + info.MemOffset) = (UInt32)fake[offset]; }
                    break;
                case DataType2.Float:
                    unsafe { *(float*)(ptr + info.MemOffset) = fake.GetFloat(offset); }
                    break;
                case DataType2.Int64:
                case DataType2.UInt64:
                    if (info.OldFieldInfo.DataLength == 4)
                        unsafe { *(Int32*)(ptr + info.MemOffset) = (Int32)fake[offset]; }
                    else unsafe { *(Int64*)(ptr + info.MemOffset) = fake.GetInt64(offset); }
                    break;
                case DataType2.Double:
                    if (info.OldFieldInfo.DataLength == 4)
                        unsafe { *(float*)(ptr + info.MemOffset) = fake.GetFloat(offset); }
                    else unsafe { *(double*)(ptr + info.MemOffset) = fake.GetDouble(offset); }
                    break;
                case DataType2.Decimal:
                    if (info.OldFieldInfo.DataLength == 4)
                        unsafe { *(float*)(ptr + info.MemOffset) = fake.GetFloat(offset); }
                    else if (info.OldFieldInfo.DataLength == 8)
                        unsafe { *(double*)(ptr + info.MemOffset) = fake.GetDouble(offset); }
                    else unsafe { *(decimal*)(ptr + info.MemOffset) = fake.GetDecimal(offset); }
                    break;
            }
        }
        object ReadArray(DataFieldInfo info, FakeStruct fake, int offset)
        {
            int a = info.DBType;
            a &= 0x1f;
            switch (a)
            {
                case DataType2.Class:
                    return ReadObjectArray(info, fake.buffer, fake.GetData(offset));
                case DataType2.Struct:
                    return ReadStructArray(info, fake.buffer, fake.GetData(offset));
                case DataType2.String:
                    return fake.GetData<FakeStringArray>(offset).Data;
                default: return fake.GetData(offset);
            }
        }
        object ReadList(DataFieldInfo info, FakeStruct fake, int offset)
        {
            int a = info.DBType;
            a &= 0x1f;
            switch (a)
            {
                case DataType2.Byte:
                    return ReadArray<byte>(fake.GetData(offset));
                case DataType2.Int16:
                    return ReadArray<Int16>(fake.GetData(offset));
                case DataType2.Int32:
                    return ReadArray<Int32>(fake.GetData(offset));
                case DataType2.Int64:
                    return ReadArray<Int64>(fake.GetData(offset));
                case DataType2.Float:
                    return ReadArray<float>(fake.GetData(offset));
                case DataType2.Double:
                    return ReadArray<Double>(fake.GetData(offset));
                case DataType2.Class:
                    return ReadObjectList(info, fake.buffer, fake.GetData(offset));
                case DataType2.Struct:
                    return ReadStructList(info, fake.buffer, fake.GetData(offset));
                case DataType2.String:
                    {
                        var fsa = fake.GetData<FakeStringArray>(offset);
                        if (fsa != null)
                        {
                            List<string> list = new List<string>();
                            list.AddRange(fsa.Data);
                            return list;
                        }
                    }
                    return null;
                default: return fake.GetData(offset);
            }
        }
        List<T> ReadArray<T>(object dat)
        {
            T[] t = dat as T[];
            if (t != null)
            {
                List<T> list = new List<T>();
                list.AddRange(t);
                return list;
            }
            return null;
        }
        object ReadStructArray(DataFieldInfo info, DataBuffer buffer, object obj)
        {
            if (obj != null)
            {
                Int16[] addr16 = obj as Int16[];
                if (addr16 != null)
                {
                    if(addr16.Length>0)
                    {
                        var con = info.ArrayConstruction(addr16.Length);
                        Array arry = con as Array;
                        if (arry != null)
                        {
                            IntPtr ptr = Marshal.UnsafeAddrOfPinnedArrayElement(arry, 0);
                            for (int i = 0; i < addr16.Length; i++)
                            {
                                var fs = buffer.GetData(addr16[i]) as FakeStruct;
                                if (fs != null)
                                {
                                    ReadStruct(info.typeInfo, fs,0, ptr);
                                }
                                ptr += info.DataLength;
                            }
                        }
                        return con;
                    }
                }
                else
                {
                    Int32[] addr32 = obj as Int32[];
                    if (addr32 != null)
                    {
                        bool isStruct = info.typeInfo.isStruct;
                        var con = info.ArrayConstruction(addr32.Length);
                        Array arry = con as Array;
                        if (arry == null)
                        {
                            IntPtr ptr = Marshal.UnsafeAddrOfPinnedArrayElement(arry, 0);
                            for (int i = 0; i < addr32.Length; i++)
                            {
                                int a = addr32[i];
                                int index = a & 0xffff;
                                var fs = buffer.GetData(index) as FakeStruct;
                                if (fs != null)
                                {
                                    ReadStruct(info.typeInfo, fs, 0, ptr);
                                }
                                ptr += info.DataLength;
                            }
                        }
                        return con;
                    }
                    else
                    {
                        FakeStructArray fsa = obj as FakeStructArray;
                        if (fsa != null)
                        {
                            FakeStruct fs;
                            unsafe
                            {
                                fs = new FakeStruct(buffer, fsa.StructSize, fsa.ip);
                            }
                            var con = info.ArrayConstruction(fsa.Length);
                            Array arry = con as Array;
                            if (arry != null)
                            {
                                IntPtr ptr = Marshal.UnsafeAddrOfPinnedArrayElement(arry, 0);
                                for (int i = 0; i < fsa.Length; i++)
                                {
                                    unsafe { fs.SetPoint(fsa[i]); }
                                    ReadStruct(info.typeInfo, fs, 0, ptr);
                                    ptr += info.DataLength;
                                }
                            }
                            return con;
                        }
                    }
                }
            }
            return null;
        }
        object ReadObjectArray(DataFieldInfo info, DataBuffer buffer,object obj)
        {
            if (obj != null)
            {
                Int16[] addr16 = obj as Int16[];
                if (addr16 != null)
                {
                    var con = info.ArrayConstruction(addr16.Length);
                    Array arry = con as Array;
                    if (arry != null)
                    {
                        for (int i = 0; i < addr16.Length; i++)
                        {
                            var fs = buffer.GetData(addr16[i]) as FakeStruct;
                            if (fs != null)
                            {
                                arry.SetValue(ReadObject(info.typeInfo, fs, addr16[i]), i);
                            }
                        }
                    }
                    return con;
                }
                else
                {
                    Int32[] addr32 = obj as Int32[];
                    if (addr32 != null)
                    {
                        bool isStruct = info.typeInfo.isStruct;
                        var con = info.ArrayConstruction(addr32.Length);
                        Array arry = con as Array;
                        if (arry == null)
                        {
                            for (int i = 0; i < addr32.Length; i++)
                            {
                                int a = addr32[i];
                                int index = a & 0xffff;
                                var fs = buffer.GetData(index) as FakeStruct;
                                if (fs != null)
                                {
                                    if (isStruct)//结构体无法使用继承
                                    {
                                        arry.SetValue(ReadObject(info.typeInfo, fs, index), i);
                                    }
                                    else
                                    {
                                        var dt = oldTypes[a >> 16].NewType;//使用继承类型
                                        if (dt == null)
                                            dt = info.typeInfo;
                                        arry.SetValue(ReadObject(dt, fs, index), i);
                                    }
                                }
                            }
                        }
                        return con;
                    }
                    else
                    {
                        FakeStructArray fsa = obj as FakeStructArray;
                        if (fsa != null)
                        {
                            FakeStruct fs;
                            unsafe
                            {
                                fs = new FakeStruct(buffer, fsa.StructSize, fsa.ip);
                            }
                            var con = info.ArrayConstruction(fsa.Length);
                            Array arry = con as Array;
                            if (arry != null)
                            {

                                for (int i = 0; i < fsa.Length; i++)
                                {
                                    unsafe { fs.SetPoint(fsa[i]); }
                                    arry.SetValue(ReadStructToObject(info.typeInfo, fs, 0), i);
                                }
                            }
                            return con;
                        }
                    }
                }
            }
            return null;
        }
        object ReadStructList(DataFieldInfo info, DataBuffer buffer, object obj)
        {
            if (obj != null)
            {
                Int16[] addr16 = obj as Int16[];
                if (addr16 != null)
                {
                    var con = info.Construction();
                    IList list = con as IList;
                    var tmp = info.typeInfo.Construction();
                    IntPtr ptr = UnsafeOperation.GetStructAddr(tmp);
                    for (int i = 0; i < addr16.Length; i++)
                    {
                        unsafe
                        {
                            byte* bp = (byte*)ptr;
                            for (int j = 0; j < info.typeInfo.DataLength; j++)
                            {
                                bp[j] = 0;
                            }
                        }
                        var fs = buffer.GetData(addr16[i]) as FakeStruct;
                        if (fs != null)
                        {
                            ReadStruct(info.typeInfo, fs, 0, ptr);
                        }
                        list.Add(tmp);
                    }
                    return con;
                }
                else
                {
                    Int32[] addr32 = obj as Int32[];
                    if (addr32 != null)
                    {
                        bool isStruct = info.typeInfo.isStruct;
                        var con = info.Construction();
                        IList list = con as IList;
                        var tmp = info.typeInfo.Construction();
                        IntPtr ptr = UnsafeOperation.GetStructAddr(tmp);
                        for (int i = 0; i < addr32.Length; i++)
                        {
                            unsafe
                            {
                                byte* bp = (byte*)ptr;
                                for (int j = 0; j < info.typeInfo.DataLength; j++)
                                {
                                    bp[j] = 0;
                                }
                            }
                            int a = addr32[i];
                            int index = a & 0xffff;
                            var fs = buffer.GetData(index) as FakeStruct;
                            if (fs != null)
                            {
                                ReadStruct(info.typeInfo, fs, 0, ptr);
                            }
                            list.Add(tmp);
                        }
                        return con;
                    }
                    else
                    {
                        FakeStructArray fsa = obj as FakeStructArray;
                        if (fsa != null)
                        {
                            FakeStruct fs;
                            unsafe
                            {
                                fs = new FakeStruct(buffer, fsa.StructSize, fsa.ip);
                            }
                            var con = info.Construction();
                            IList list = con as IList;
                            var tmp = info.typeInfo.Construction();
                            IntPtr ptr = UnsafeOperation.GetStructAddr(tmp);
                            for (int i = 0; i < fsa.Length; i++)
                            {
                                unsafe
                                {
                                    byte* bp = (byte*)ptr;
                                    for (int j = 0; j < info.typeInfo.DataLength; j++)
                                    {
                                        bp[j] = 0;
                                    }
                                }
                                unsafe { fs.SetPoint(fsa[i]); }
                                ReadStruct(info.typeInfo, fs, 0, ptr);
                                list.Add(tmp);
                            }
                            return con;
                        }
                    }
                }
            }
            return null;
        }
        object ReadObjectList(DataFieldInfo info, DataBuffer buffer, object obj)
        {
            if (obj != null)
            {
                Int16[] addr16 = obj as Int16[];
                if (addr16 != null)
                {
                    var con = info.Construction();
                    IList list = con as IList;
                    for (int i = 0; i < addr16.Length; i++)
                    {
                        var fs = buffer.GetData(addr16[i]) as FakeStruct;
                        if (fs != null)
                        {
                            list.Add(ReadObject(info.typeInfo, fs, addr16[i]));
                        }
                        else list.Add(null);
                    }
                    return con;
                }
                else
                {
                    Int32[] addr32 = obj as Int32[];
                    if (addr32 != null)
                    {
                        bool isStruct = info.typeInfo.isStruct;
                        var con = info.Construction();
                        IList list = con as IList;
                        for (int i = 0; i < addr32.Length; i++)
                        {
                            int a = addr32[i];
                            int index = a & 0xffff;
                            var fs = buffer.GetData(index) as FakeStruct;
                            if (fs != null)
                            {
                                if (isStruct)//结构体无法使用继承
                                {
                                    list.Add(ReadObject(info.typeInfo, fs, index));
                                }
                                else
                                {
                                    var dt = oldTypes[a >> 16].NewType;//使用继承类型
                                    if (dt == null)//没有找到继承类型则使用默认类型
                                        list.Add(null);
                                    else list.Add(ReadObject(dt, fs, index));
                                }
                            }
                            else list.Add(null);
                        }
                        return con;
                    }
                    else
                    {
                        FakeStructArray fsa = obj as FakeStructArray;
                        if (fsa != null)
                        {
                            FakeStruct fs;
                            unsafe
                            {
                                fs = new FakeStruct(buffer, fsa.StructSize, fsa.ip);
                            }
                            var con = info.Construction();
                            IList list = con as IList;
                            for (int i = 0; i < fsa.Length; i++)
                            {
                                unsafe { fs.SetPoint(fsa[i]); }
                                list.Add(ReadStructToObject(info.typeInfo, fs, 0));
                            }
                            return con;
                        }
                    }
                }
            }
            return null;
        }
        object ReadArray2(DataFieldInfo info, FakeStruct fake, int offset)
        {
            Int16[] tmp = fake.GetData<Int16[]>(offset);
            if (tmp == null)
                return null;
            var buf = fake.buffer;
            Array arry = info.ArrayConstruction2(tmp.Length) as Array;
            int a = info.DBType;
            a &= 0x1f;
            switch (a)
            {
                case DataType2.Class:
                case DataType2.Struct:
                    for (int i = 0; i < tmp.Length; i++)
                    {
                        var v = buf.GetData(tmp[i]);
                        if (v != null)
                        {
                            var obj = ReadObjectArray(info, buf, v);
                            if (obj != null)
                                arry.SetValue(obj, i);
                        }
                    }
                    break;
                case DataType2.String:
                    for (int i = 0; i < tmp.Length; i++)
                    {
                        var v = buf.GetData(tmp[i]) as FakeStringArray;
                        if (v != null)
                            arry.SetValue(v.Data, i);
                    }
                    break;
                default:
                    for (int i = 0; i < tmp.Length; i++)
                    {
                        var v = buf.GetData(tmp[i]);
                        if (v != null)
                            arry.SetValue(v, i);
                    }
                    break;
            }
            return arry;
        }
        object ReadList2(DataFieldInfo info, FakeStruct fake, int offset)
        {
            Int16[] tmp = fake.GetData<Int16[]>(offset);
            if (tmp == null)
                return null;
            var buf = fake.buffer;
            IList arry = info.Construction2() as IList;
            int a = info.DBType;
            a &= 0x1f;
            switch (a)
            {
                case DataType2.Class:
                    for (int i = 0; i < tmp.Length; i++)
                    {
                        var v = buf.GetData(tmp[i]);
                        if (v != null)
                        {
                            object obj;
                            if (info.ChildTypeName == DataType2.List)
                                obj = ReadObjectList(info, buf, v);
                            else obj = ReadObjectArray(info, buf, v);
                            arry.Add(obj);
                        }
                        else arry.Add(null);
                    }
                    break;
                case DataType2.Struct:
                    for (int i = 0; i < tmp.Length; i++)
                    {
                        var v = buf.GetData(tmp[i]);
                        if (v != null)
                        {
                            object obj;
                            if (info.ChildTypeName == DataType2.List)
                                obj = ReadStructList(info, buf, v);
                            else obj = ReadStructArray(info, buf, v);
                            arry.Add(obj);
                        }
                        else arry.Add(null);
                    }
                    break;
                case DataType2.String:
                    if (info.ChildTypeName == DataType2.List)
                    {
                        for (int i = 0; i < tmp.Length; i++)
                        {
                            var v = buf.GetData(tmp[i]) as FakeStringArray;
                            if (v != null)
                            {
                                List<string> t = new List<string>();
                                t.AddRange(v.Data);
                                arry.Add(t); 
                            }
                            else arry.Add(null);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < tmp.Length; i++)
                        {
                            var v = buf.GetData(tmp[i]) as FakeStringArray;
                            if (v != null)
                                arry.Add(v.Data);
                            else arry.Add(null);
                        }
                    }
                    break;
                default:
                    if (info.ChildTypeName == DataType2.List)
                    {
                        switch (a)
                        {
                            case DataType2.Byte:
                                for (int i = 0; i < tmp.Length; i++)
                                    arry.Add(ReadArray<byte>(buf.GetData(tmp[i])));
                                break;
                            case DataType2.Int16:
                                for (int i = 0; i < tmp.Length; i++)
                                    arry.Add(ReadArray<Int16>(buf.GetData(tmp[i])));
                                break;
                            case DataType2.Int32:
                                for (int i = 0; i < tmp.Length; i++)
                                    arry.Add(ReadArray<Int32>(buf.GetData(tmp[i])));
                                break;
                            case DataType2.Int64:
                                for (int i = 0; i < tmp.Length; i++)
                                    arry.Add(ReadArray<Int64>(buf.GetData(tmp[i])));
                                break;
                            case DataType2.Float:
                                for (int i = 0; i < tmp.Length; i++)
                                    arry.Add(ReadArray<float>(buf.GetData(tmp[i])));
                                break;
                            case DataType2.Double:
                                for (int i = 0; i < tmp.Length; i++)
                                    arry.Add(ReadArray<double>(buf.GetData(tmp[i])));
                                break;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < tmp.Length; i++)
                        {
                            arry.Add(buf.GetData(tmp[i]));
                        }
                    }
                    break;
            }
            return arry;
        }
    }
}
