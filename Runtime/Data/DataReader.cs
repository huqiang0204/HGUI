using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace huqiang.Data
{
    public class DataReader:TypeAnalysis
    {
        protected List<TypeInfo> oldTypes = new List<TypeInfo>();
        protected List<TypeInfo> compatible = new List<TypeInfo>();
        List<FieldDataInfo> tmp = new List<FieldDataInfo>();
        void MatchingTables()
        {
            int maxA = types.Count - 1;
            int maxB = oldTypes.Count - 1;
            MatchingTable(maxA,maxB);
        }
        void MatchingTable(string cur, string old)
        {
            for (int i = 0; i < compatible.Count; i++)
                if (compatible[i].name == cur)
                    return;
            int ci = -1;
            for (int i = 0; i < types.Count; i++)
            {
                if (types[i].name == cur)
                {
                    ci = i;
                    break;
                }
            }
            if (ci < 0)
                return;
            int oi = -1;
            for (int i = 0; i < oldTypes.Count; i++)
            {
                if (oldTypes[i].name == old)
                {
                    oi = i;
                    break;
                }
            }
            if (oi < 0)
                return;
            MatchingTable(ci, oi);
        }
        void MatchingTable(int cur, int old)
        {
            var type = types[cur];
            var ts = type.dataTypes = MatchingFields(type.dataTypes, oldTypes[old].dataTypes);
            compatible.Add(type);
            if (ts != null)
                for (int i = 0; i < ts.Length; i++)
                {
                    if(ts[i].HaveTable)
                    {
                        if (ts[i].IsArray)
                            MatchingTable(ts[i].ChildTypeName, ts[i].OldChildTypeName);
                        else MatchingTable(ts[i].TypeName, ts[i].OldTypeName);
                    }
                }
        }
        FieldDataInfo[] MatchingFields(FieldDataInfo[] cur, FieldDataInfo[] old)
        {
            tmp.Clear();
            for (int i = 0; i < cur.Length; i++)
            {
                string name = cur[i].FieldName;
                for (int j = 0; j < old.Length; j++)
                {
                    if (old[j].FieldName == name)
                    {
                        int a = old[j].DataType;
                        int b = cur[i].DataType;
                        if(CheckType(a,b))
                        {
                            FieldDataInfo info = cur[i];
                            info.Offset = old[j].Offset;
                            info.DataLength = old[j].DataLength;
                            info.ElementLength = old[j].ElementLength;
                            info.OldType = old[j].DataType;
                            info.OldTypeName = old[j].TypeName;
                            info.OldChildTypeName = old[j].ChildTypeName;
                            tmp.Add(info);
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
            int c = a & 0xff;
            int d = b & 0xff;
            switch (c)
            {
                case DataType.Struct:
                    if (d == DataType.Class)
                        return true;
                    break;
                case DataType.Class:
                    if (d == DataType.Struct)
                        return true;
                    break;
                case DataType.StructArray:
                    if (d == DataType.ClassArray)
                        goto lable;
                    break;
                case DataType.ClassArray:
                    if (d == DataType.StructArray)
                        goto lable;
                    break;
                case DataType.TwoDArray:
                    if (d == DataType.TwoDList)
                        goto lable;
                    break;
                case DataType.TwoDList:
                    if (d == DataType.TwoDArray)
                        goto lable;
                    break;
            }
            return false;
            lable:;//比较子元素
            int j = (a >> 8) & 0xff;
            int k = (b >> 8) & 0xff;
            if (j == k)//子元素类型相同
                return true;
            switch (j)
            {
                case DataType.Struct:
                    if (k == DataType.Class)
                        return true;
                    break;
                case DataType.Class:
                    if (k == DataType.Struct)
                        return true;
                    break;
                case DataType.StructArray:
                    if (k == DataType.ClassArray)
                        return true;
                    break;
                case DataType.ClassArray:
                    if (k == DataType.StructArray)
                        return true;
                    break;
            }
            return false;
        }
        public T Read<T>(DataBuffer db, bool Compatiblemode = true) where T : class, new()
        {
            var fs = db.fakeStruct;
            if (Compatiblemode)
            {
                var fsa = fs.GetData<FakeStructArray>(0);
                if (fsa != null)
                {
                    ReadTables(fsa);
                    MatchingTables();
                }
            }
            else
            {
                compatible = types;
            }
            var fake = fs.GetData<FakeStruct>(1);
            if (fake != null)
                return ReadObject(MainType, fake) as T;
            return null;
        }
        public void Read(DataBuffer db, object instance, bool Compatiblemode = true)
        {
            var fs = db.fakeStruct;
            if (Compatiblemode)
            {
                var fsa = fs.GetData<FakeStructArray>(0);
                if (fsa != null)
                {
                    ReadTables(fsa);
                    MatchingTables();
                }
            }
            else
            {
                compatible = types;
            }
            var fake = fs.GetData<FakeStruct>(1);
            if (fake != null)
                ReadObject(MainType, fake, instance);
        }
        void ReadTables(FakeStructArray fsa)
        {
            int c = fsa.Length;
            TypeInfo info = new TypeInfo();
            for (int i = 0; i < c; i++)
            {
                info.dataLenth = fsa[i, 0];
                info.name = fsa.GetData<string>(i, 1);
                var fs = fsa.GetData<FakeStructArray>(i, 2);
                if (fs != null)
                    info.dataTypes = ReadTableFields(fs);
                oldTypes.Add(info);
            }
        }
        FieldDataInfo[] ReadTableFields(FakeStructArray fsa)
        {
            int c = fsa.Length;
            FieldDataInfo[] fields = new FieldDataInfo[c];
            var db = fsa.buffer;
            unsafe
            {
                for (int i = 0; i < c; i++)
                {
                    byte* ip = fsa[i];
                    Int16* bp = (Int16*)ip;
                    fields[i].Offset= * bp;
                    bp++;
                    fields[i].ElementLength =* bp;
                    bp++;
                    fields[i].DataType = *bp;
                    bp++;
                    fields[i].TypeName = db.GetData(*bp)as string;
                    bp++;
                    fields[i].FieldName = db.GetData(*bp) as string;
                    bp++;
                    fields[i].ChildTypeName = db.GetData(*bp) as string;
                }
            }
            return fields;
        }
        void ReadObject(string type, FakeStruct fake, object instance)
        {
            int index = -1;
            for (int j = 0; j < compatible.Count; j++)
            {
                if (compatible[j].name == type)
                {
                    index = j;
                    break;
                }
            }
            if (index >= 0)
            {
                var dts = compatible[index].dataTypes;
                for (int i = 0; i < dts.Length; i++)
                {
                    ReadField(fake, 0, ref dts[i], instance);
                }
            }
        }
        object ReadObject(int index, FakeStruct fake)
        {
            var obj = Activator.CreateInstance(compatible[index].type);
            var dts = compatible[index].dataTypes;
            for (int i = 0; i < dts.Length; i++)
            {
                ReadField(fake, 0, ref dts[i], obj);
            }
            return obj;
        }
        object ReadObject(string type, FakeStruct fake)
        {
            int index = -1;
            for (int j = 0; j < compatible.Count; j++)
            {
                if (compatible[j].name == type)
                {
                    index = j;
                    break;
                }
            }
            if (index >= 0)
            {
                var obj = Activator.CreateInstance(compatible[index].type);
                var dts = compatible[index].dataTypes;
                for (int i = 0; i < dts.Length; i++)
                {
                    ReadField(fake, 0, ref dts[i], obj);
                }
                return obj;
            }
            return null;
        }
        object ReadStruct(FakeStruct fake, int start, ref FieldDataInfo info)
        {
            string type = info.TypeName;
            int index = -1;
            for (int i = 0; i < compatible.Count; i++)
                if (compatible[i].name == type)
                {
                    index = i;
                    goto lable;
                }
            return null;
            lable:;
            var obj = Activator.CreateInstance(compatible[index].type);
            var dts = compatible[index].dataTypes;
            for (int i = 0; i < dts.Length; i++)
            {
                ReadField(fake, start, ref dts[i], obj);
            }
            return obj;
        }
        void ReadField(FakeStruct fake, int start, ref FieldDataInfo info, object instance)
        {
            int offset = start + info.Offset;
            if (info.TypeName == "Boolean")
            {
                if (fake[offset] == 1)
                    info.field.SetValue(instance, true);
            }
            else
            {
                int dt = ((int)info.DataType) & 0xff;
                switch (dt)
                {
                    case DataType.Int:
                        info.field.SetValue(instance, fake[offset]);
                        break;
                    case DataType.Float:
                        info.field.SetValue(instance, fake.GetFloat(offset));
                        break;
                    case DataType.Long:
                        info.field.SetValue(instance, fake.GetInt64(offset));
                        break;
                    case DataType.Double:
                        info.field.SetValue(instance, fake.GetDouble(offset));
                        break;
                    case DataType.String:
                        var obj = fake.GetData(offset);
                        if (obj != null)
                            info.field.SetValue(instance, obj);
                        break;
                    case DataType.Struct:
                        info.field.SetValue(instance, ReadStruct(fake, offset, ref info));
                        break;
                    case DataType.Class:
                        {
                            var fs = fake.GetData<FakeStruct>(offset);
                            if (fs != null)
                            {
                                info.field.SetValue(instance, ReadObject(info.TypeName, fs));
                            }
                        }
                        break;
                    //case DataType.FakeStruct:
                    //    if (info.field.FieldType.IsValueType)//这是一个结构体
                    //    {
                    //        info.field.SetValue(instance, ReadStruct(fake, offset, ref info));
                    //    }
                    //    else//否则是一个类
                    //    {
                    //        var fs = fake.GetData<FakeStruct>(offset);
                    //        if (fs != null)
                    //        {
                    //            info.field.SetValue(instance, ReadObject(info.TypeName, fs));
                    //        }
                    //    }
                    //    break;
                    case DataType.ByteArray:
                        {
                            var arr = fake.GetData<byte[]>(offset);
                            if (arr != null)
                            {
                                if (info.TypeName == "List`1")
                                {
                                    info.field.SetValue(instance, new List<byte>(arr));
                                }
                                else
                                {
                                    info.field.SetValue(instance, arr);
                                }
                            }
                        }
                        break;
                    case DataType.Int32Array:
                        {
                            var arr = fake.GetData<int[]>(offset);
                            if (arr != null)
                            {
                                if (info.TypeName == "List`1")
                                {
                                    info.field.SetValue(instance, new List<int>(arr));
                                }
                                else
                                {
                                    info.field.SetValue(instance, arr);
                                }
                            }
                        }
                        break;
                    case DataType.FloatArray:
                        {
                            var arr = fake.GetData<float[]>(offset);
                            if (arr != null)
                            {
                                if (info.TypeName == "List`1")
                                {
                                    info.field.SetValue(instance, new List<float>(arr));
                                }
                                else
                                {
                                    info.field.SetValue(instance, arr);
                                }
                            }
                        }
                        break;
                    case DataType.Int16Array:
                        {
                            var arr = fake.GetData<Int16[]>(offset);
                            if (arr != null)
                            {
                                if (info.TypeName == "List`1")
                                {
                                    info.field.SetValue(instance, new List<Int16>(arr));
                                }
                                else
                                {
                                    info.field.SetValue(instance, arr);
                                }
                            }
                        }
                        break;
                    case DataType.Int64Array:
                        {
                            var arr = fake.GetData<Int64[]>(offset);
                            if (arr != null)
                            {
                                if (info.TypeName == "List`1")
                                {
                                    info.field.SetValue(instance, new List<Int64>(arr));
                                }
                                else
                                {
                                    info.field.SetValue(instance, arr);
                                }
                            }
                        }
                        break;
                    case DataType.DoubleArray:
                        {
                            var arr = fake.GetData<double[]>(offset);
                            if (arr != null)
                            {
                                if (info.TypeName == "List`1")
                                {
                                    info.field.SetValue(instance, new List<double>(arr));
                                }
                                else
                                {
                                    info.field.SetValue(instance, arr);
                                }
                            }
                        }
                        break;
                    case DataType.FakeStringArray:
                        {
                            var arr = fake.GetData<FakeStringArray>(offset);
                            if (arr != null)
                            {
                                if (info.TypeName == "List`1")
                                {
                                    info.field.SetValue(instance, new List<string>(arr.Data));
                                }
                                else
                                {
                                    info.field.SetValue(instance, arr.Data);
                                }
                            }
                        }
                        break;
                    case DataType.ClassArray:
                        ReadObjectArray(fake, offset, ref info, instance);
                        break;
                    case DataType.StructArray:
                        ReadStructArray(fake, offset, ref info, instance);
                        break;
                    case DataType.TwoDArray:
                    case DataType.TwoDList:
                        ReadTwoDArray(fake, offset, ref info, instance);
                        break;
                    case DataType.Decimal:
                        info.field.SetValue(instance, fake.GetDecimal(offset));
                        break;
                }
            }
        }
        void ReadObjectArray(FakeStruct fake, int offset, ref FieldDataInfo info, object instance)
        {
            if (info.OldType == DataType.StructArray)
            {
                var arr = fake.GetData<FakeStructArray>(offset);
                if (arr != null)
                {
                    int c = arr.Length;
                    int index = -1;
                    for (int i = 0; i < compatible.Count; i++)
                        if (compatible[i].name == info.ChildTypeName)
                        {
                            index = i;
                            break;
                        }
                    if (index >= 0)
                    {
                        unsafe
                        {
                            FakeStruct fs = new FakeStruct(fake.buffer, arr.StructSize, arr.ip);
                            if (info.TypeName == "List`1")
                            {
                                var list = Activator.CreateInstance(info.field.FieldType) as IList;
                                for (int i = 0; i < c; i++)
                                {
                                    fs.SetPoint(arr[i]);
                                    list.Add(ReadObject(index, fs));
                                }
                                info.field.SetValue(instance, list);
                            }
                            else
                            {
                                var list = Array.CreateInstance(info.ChildType, c);
                                for (int i = 0; i < c; i++)
                                {
                                    fs.SetPoint(arr[i]);
                                    list.SetValue(ReadObject(index, fs), i);
                                }
                                info.field.SetValue(instance, list);
                            }
                        }
                    }
                }
            }
            else
            {
                var arr = fake.GetData<Int16[]>(offset);
                if (arr != null)
                {
                    var buffer = fake.buffer;
                    int c = arr.Length;
                    int index = -1;
                    for (int i = 0; i < compatible.Count; i++)
                        if (compatible[i].name == info.ChildTypeName)
                        {
                            index = i;
                            break;
                        }
                    if (index >= 0)
                    {
                        if (info.TypeName == "List`1")
                        {
                            var list = Activator.CreateInstance(info.field.FieldType) as IList;
                            for (int i = 0; i < c; i++)
                            {
                                var fs = buffer.GetData(arr[i]) as FakeStruct;
                                if (fs != null)
                                {
                                    list.Add(ReadObject(index, fs));
                                }
                            }
                            info.field.SetValue(instance, list);
                        }
                        else
                        {
                            var list = Array.CreateInstance(info.ChildType, c);
                            for (int i = 0; i < c; i++)
                            {
                                var fs = buffer.GetData(arr[i]) as FakeStruct;
                                if (fs != null)
                                {
                                    list.SetValue(ReadObject(index, fs), i);
                                }
                            }
                            info.field.SetValue(instance, list);
                        }
                    }
                }
            }
        }
        void ReadStructArray(FakeStruct fake, int offset, ref FieldDataInfo info, object instance)
        {
            if(info.OldType==DataType.ClassArray)
            {
                var arr = fake.GetData<Int16[]>(offset);
                if (arr != null)
                {
                    var buffer = fake.buffer;
                    int c = arr.Length;
                    int index = -1;
                    for (int i = 0; i < compatible.Count; i++)
                        if (compatible[i].name == info.ChildTypeName)
                        {
                            index = i;
                            break;
                        }
                    if (index >= 0)
                    {
                        unsafe
                        {
                            if (info.TypeName == "List`1")
                            {
                                var list = Activator.CreateInstance(info.field.FieldType) as IList;
                                for (int i = 0; i < c; i++)
                                {
                                    var fs = buffer.GetData(arr[i]) as FakeStruct;
                                    if (fs != null)
                                    {
                                        list.Add(ReadObject(index, fs));
                                    }
                                }
                                info.field.SetValue(instance, list);
                            }
                            else
                            {
                                var list = Array.CreateInstance(info.ChildType, c);
                                for (int i = 0; i < c; i++)
                                {
                                    var fs = buffer.GetData(arr[i]) as FakeStruct;
                                    if (fs != null)
                                    {
                                        list.SetValue(ReadObject(index, fs), i);
                                    }
                                }
                                info.field.SetValue(instance, list);
                            }
                        }
                    }
                }
            }
            else
            {
                var arr = fake.GetData<FakeStructArray>(offset);
                if (arr != null)
                {
                    int c = arr.Length;
                    int index = -1;
                    for (int i = 0; i < compatible.Count; i++)
                        if (compatible[i].name == info.ChildTypeName)
                        {
                            index = i;
                            break;
                        }
                    if (index >= 0)
                    {
                        unsafe
                        {
                            FakeStruct fs = new FakeStruct(fake.buffer, arr.StructSize, arr.ip);
                            if (info.TypeName == "List`1")
                            {
                                var list = Activator.CreateInstance(info.field.FieldType) as IList;
                                for (int i = 0; i < c; i++)
                                {
                                    fs.SetPoint(arr[i]);
                                    if (fs != null)
                                    {
                                        list.Add(ReadObject(index, fs));
                                    }
                                }
                                info.field.SetValue(instance, list);
                            }
                            else
                            {
                                var list = Array.CreateInstance(info.ChildType, c);
                                for (int i = 0; i < c; i++)
                                {
                                    fs.SetPoint(arr[i]);
                                    if (fs != null)
                                    {
                                        list.SetValue(ReadObject(index, fs), i);
                                    }
                                }
                                info.field.SetValue(instance, list);
                            }
                        }
                    }
                }
            }
        }
        void ReadTwoDArray(FakeStruct fake, int offset, ref FieldDataInfo info, object instance)
        {
            var buffer = fake.buffer;
            Int16[] aar = fake.GetData<Int16[]>(offset);
            if (aar != null)
            {
                int sonType = ((int)info.DataType) >> 8;
                switch (sonType)
                {
                    case DataType.ByteArray:
                        if (info.TypeName == "List`1")
                        {
                            List<List<byte>> list = new List<List<byte>>();
                            for (int i = 0; i < aar.Length; i++)
                            {
                                var dat = buffer.GetData(aar[i]) as byte[];
                                if (dat != null)
                                {
                                    list.Add(new List<byte>(dat));
                                }
                            }
                            info.field.SetValue(instance, list);
                        }
                        else
                        {
                            byte[][] list = new byte[aar.Length][];
                            for (int i = 0; i < aar.Length; i++)
                            {
                                list[i] = buffer.GetData(aar[i]) as byte[];
                            }
                            info.field.SetValue(instance, list);
                        }
                        break;
                    case DataType.Int16Array:
                        if (info.TypeName == "List`1")
                        {
                            List<List<Int16>> list = new List<List<Int16>>();
                            for (int i = 0; i < aar.Length; i++)
                            {
                                var dat = buffer.GetData(aar[i]) as Int16[];
                                if (dat != null)
                                {
                                    list.Add(new List<Int16>(dat));
                                }
                            }
                            info.field.SetValue(instance, list);
                        }
                        else
                        {
                            Int16[][] list = new Int16[aar.Length][];
                            for (int i = 0; i < aar.Length; i++)
                            {
                                list[i] = buffer.GetData(aar[i]) as Int16[];
                            }
                            info.field.SetValue(instance, list);
                        }
                        break;
                    case DataType.Int32Array:
                        if (info.TypeName == "List`1")
                        {
                            List<List<Int32>> list = new List<List<Int32>>();
                            for (int i = 0; i < aar.Length; i++)
                            {
                                var dat = buffer.GetData(aar[i]) as Int32[];
                                if (dat != null)
                                {
                                    list.Add(new List<Int32>(dat));
                                }
                            }
                            info.field.SetValue(instance, list);
                        }
                        else
                        {
                            Int32[][] list = new Int32[aar.Length][];
                            for (int i = 0; i < aar.Length; i++)
                            {
                                list[i] = buffer.GetData(aar[i]) as Int32[];
                            }
                            info.field.SetValue(instance, list);
                        }
                        break;
                    case DataType.FloatArray:
                        if (info.TypeName == "List`1")
                        {
                            List<List<float>> list = new List<List<float>>();
                            for (int i = 0; i < aar.Length; i++)
                            {
                                var dat = buffer.GetData(aar[i]) as float[];
                                if (dat != null)
                                {
                                    list.Add(new List<float>(dat));
                                }
                            }
                            info.field.SetValue(instance, list);
                        }
                        else
                        {
                            float[][] list = new float[aar.Length][];
                            for (int i = 0; i < aar.Length; i++)
                            {
                                list[i] = buffer.GetData(aar[i]) as float[];
                            }
                            info.field.SetValue(instance, list);
                        }
                        break;
                    case DataType.Int64Array:
                        if (info.TypeName == "List`1")
                        {
                            List<List<Int64>> list = new List<List<Int64>>();
                            for (int i = 0; i < aar.Length; i++)
                            {
                                var dat = buffer.GetData(aar[i]) as Int64[];
                                if (dat != null)
                                {
                                    list.Add(new List<Int64>(dat));
                                }
                            }
                            info.field.SetValue(instance, list);
                        }
                        else
                        {
                            Int64[][] list = new Int64[aar.Length][];
                            for (int i = 0; i < aar.Length; i++)
                            {
                                list[i] = buffer.GetData(aar[i]) as Int64[];
                            }
                            info.field.SetValue(instance, list);
                        }
                        break;
                    case DataType.DoubleArray:
                        if (info.TypeName == "List`1")
                        {
                            List<List<double>> list = new List<List<double>>();
                            for (int i = 0; i < aar.Length; i++)
                            {
                                var dat = buffer.GetData(aar[i]) as double[];
                                if (dat != null)
                                {
                                    list.Add(new List<double>(dat));
                                }
                            }
                            info.field.SetValue(instance, list);
                        }
                        else
                        {
                            double[][] list = new double[aar.Length][];
                            for (int i = 0; i < aar.Length; i++)
                            {
                                list[i] = buffer.GetData(aar[i]) as double[];
                            }
                            info.field.SetValue(instance, list);
                        }
                        break;
                    case DataType.FakeStringArray:
                        if (info.TypeName == "List`1")
                        {
                            List<List<string>> list = new List<List<string>>();
                            for (int i = 0; i < aar.Length; i++)
                            {
                                var dat = buffer.GetData(aar[i]) as FakeStringArray;
                                if (dat != null)
                                {
                                    list.Add(new List<string>(dat.Data));
                                }
                            }
                            info.field.SetValue(instance, list);
                        }
                        else
                        {
                            string[][] list = new string[aar.Length][];
                            for (int i = 0; i < aar.Length; i++)
                            {
                               var dat = buffer.GetData(aar[i]) as FakeStringArray;
                                if (dat != null)
                                    list[i] = dat.Data;
                            }
                            info.field.SetValue(instance, list);
                        }
                        break;
                    case DataType.StructArray:
                    case DataType.ClassArray:
                        ReadTwoArray(fake, ref info,instance,aar);
                        break;
                }
            }
        }
        void ReadTwoArray(FakeStruct fake, ref FieldDataInfo info, object instance, Int16[] aar)
        {
            var buffer = fake.buffer;
            int index = -1;
            for (int j = 0; j < compatible.Count; j++)
            {
                if (compatible[j].name == info.ChildTypeName)
                {
                    index = j;
                    break;
                }
            }
            int old = info.OldType >> 8;
            if (index >= 0)
            {
                if (info.TypeName == "List`1")
                {
                    var list = Activator.CreateInstance(info.field.FieldType) as IList;
                    if (old == DataType.ClassArray)
                    {
                        for (int i = 0; i < aar.Length; i++)
                        {
                            var fsa = buffer.GetData(aar[i]) as Int16[];
                            if (fsa != null)
                            {
                                var son = Activator.CreateInstance(info.ChildType) as IList;
                                for (int j = 0; j < fsa.Length; j++)
                                {
                                    FakeStruct fs = buffer.GetData(fsa[i]) as FakeStruct;
                                    if (fs != null)
                                        son.Add(ReadObject(index, fs));
                                }
                                list.Add(son);
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < aar.Length; i++)
                        {
                            var fsa = buffer.GetData(aar[i]) as FakeStructArray;
                            if (fsa != null)
                            {
                                unsafe
                                {
                                    FakeStruct fs = new FakeStruct(buffer, fsa.StructSize, fsa.ip);
                                    var son = Activator.CreateInstance(info.ChildType) as IList;
                                    for (int j = 0; j < fsa.Length; j++)
                                    {
                                        fs.SetPoint(fsa[j]);
                                        son.Add(ReadObject(index, fs));
                                    }
                                    list.Add(son);
                                }
                            }
                        }
                    }
                    info.field.SetValue(instance, list);
                }
                else
                {
                    var list = Array.CreateInstance(info.ChildType, aar.Length);
                    if (old == DataType.ClassArray)
                    {
                        for (int i = 0; i < aar.Length; i++)
                        {
                            var fsa = buffer.GetData(aar[i]) as Int16[];
                            if (fsa != null)
                            {
                                var son = Array.CreateInstance(info.ArgType, fsa.Length);
                                for (int j = 0; j < fsa.Length; j++)
                                {
                                    FakeStruct fs = buffer.GetData(fsa[i]) as FakeStruct;
                                    if (fs != null)
                                        son.SetValue(ReadObject(index, fs), j); 
                                }
                                list.SetValue(son, i);
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < aar.Length; i++)
                        {
                            var fsa = buffer.GetData(aar[i]) as FakeStructArray;
                            if (fsa != null)
                            {
                                unsafe
                                {
                                    FakeStruct fs = new FakeStruct(buffer, fsa.StructSize, fsa.ip);
                                    var son = Array.CreateInstance(info.ArgType, fsa.Length);
                                    for (int j = 0; j < fsa.Length; j++)
                                    {
                                        fs.SetPoint(fsa[j]);
                                        son.SetValue(ReadObject(index, fs), j);
                                    }
                                    list.SetValue(son, i);
                                }
                            }
                        }
                    }
                    info.field.SetValue(instance, list);
                }
            }
        }
    }
}
