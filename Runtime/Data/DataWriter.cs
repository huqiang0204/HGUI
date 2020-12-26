using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace huqiang.Data
{
    public class DataWriter:TypeAnalysis
    {
        public DataBuffer Write(object intance)
        {
            DataBuffer db = new DataBuffer();
            FakeStruct fa = new FakeStruct(db, 2);
            fa[0] = db.AddData(WriteTables(db),DataType.FakeStructArray);
            fa[1]  = db.AddData(Write(types.Count - 1, intance, db), DataType.FakeStruct);
            db.fakeStruct = fa;
            return db;
        }
        FakeStructArray WriteTables(DataBuffer db)
        {
            int c = types.Count;
            FakeStructArray fsa = new FakeStructArray(db, 3, c);
            for (int i=0;i<c;i++)
            {
                fsa[i, 0] = types[i].dataLenth;
                fsa[i, 1] = db.AddData(types[i].name,DataType.String);
                fsa[i, 2] = db.AddData(WriteTableFields(db, types[i].dataTypes),DataType.FakeStructArray);
            }
            return fsa;
        }
        FakeStructArray WriteTableFields(DataBuffer db, FieldDataInfo[] fields)
        {
            int c = fields.Length;
            FakeStructArray fsa = new FakeStructArray(db,3,c);
            unsafe
            {
                for (int i = 0; i < c; i++)
                {
                    byte* ip = fsa[i];
                    Int16* bp = (Int16*)ip;
                    *bp = fields[i].Offset;
                    bp++;
                    *bp = fields[i].ElementLength;
                    bp++;
                    *bp = fields[i].DataType;
                    bp++;
                    *bp = (Int16)db.AddData(fields[i].TypeName, DataType.String);
                    bp++;
                    *bp = (Int16)db.AddData(fields[i].FieldName, DataType.String);
                    bp++;
                    *bp = (Int16)db.AddData(fields[i].ChildTypeName, DataType.String);
                }
            }
            return fsa;
        }
        FakeStruct Write(int typeIndex,object instance,DataBuffer db)
        {
            if (instance == null)
                return null;
            FakeStruct fake = new FakeStruct(db, types[typeIndex].ElementLength);
            var dts = types[typeIndex].dataTypes;
            for (int i = 0; i < dts.Length; i++)
            {
                WriteField(fake, 0, ref dts[i], instance);
            }
            return fake;
        }
        void Write(int typeIndex, object instance, FakeStruct fake)
        {
            var dts = types[typeIndex].dataTypes;
            for (int i = 0; i < dts.Length; i++)
            {
                WriteField(fake, 0, ref dts[i], instance);
            }
        }
        FakeStruct WriteObject(string type, object instance, DataBuffer db)
        {
            int index = -1;
            for (int j = 0; j < types.Count; j++)
            {
                if (types[j].name == type)
                {
                    index = j;
                    break;
                }
            }
            if (index >= 0)
            {
                FakeStruct fake = new FakeStruct(db, types[index].ElementLength);
                var dts = types[index].dataTypes;
                for (int i = 0; i < dts.Length; i++)
                {
                    WriteField(fake, 0, ref dts[i], instance);
                }
                return fake;
            }
            return null;
        }
        void WriteStruct(FakeStruct fake, int start, ref FieldDataInfo info, object instance)
        {
            string type = info.TypeName;
            int index = -1;
            for (int i = 0; i < types.Count; i++)
                if (types[i].name == type)
                {
                    index = i;
                    goto lable;
                }
            return;
            lable:;
            var dts = types[index].dataTypes;
            for (int i = 0; i < dts.Length; i++)
            {
                WriteField(fake, start, ref dts[i], instance);
            }
        }
        void WriteField(FakeStruct fake, int start, ref FieldDataInfo info,object instance)
        {
            int offset = start + info.Offset;
            if (info.TypeName== "Boolean")
            {
                if ((bool)info.field.GetValue(instance))
                    fake[offset] = 1;
            }
            else
            {
                var buffer = fake.buffer;
                int dt = info.DataType & 0xff;
                switch (dt)
                {
                    case DataType.Int:
                        fake[offset] = (Int32)info.field.GetValue(instance);
                        break;
                    case DataType.Float:
                        fake.SetFloat(offset, (float)info.field.GetValue(instance));
                        break;
                    case DataType.Long:
                        fake.SetInt64(offset, (Int64)info.field.GetValue(instance));
                        break;
                    case DataType.Double:
                        fake.SetDouble(offset, (double)info.field.GetValue(instance));
                        break;
                    case DataType.String:
                        {
                            var obj = info.field.GetValue(instance);
                            if (obj != null)
                                fake[offset] = buffer.AddData(obj, dt);
                        }
                        break;
                    case DataType.Struct:
                        WriteStruct(fake, offset, ref info, info.field.GetValue(instance));
                        break;
                    case DataType.Class:
                        {
                            var obj = info.field.GetValue(instance);
                            if (obj != null)
                            {
                                fake[offset] = buffer.AddData(WriteObject(info.field.FieldType.Name, obj, buffer), DataType.FakeStruct);
                            }
                        }
                        break;
                    case DataType.ByteArray:
                        if (info.TypeName == "List`1")
                        {
                            var list = info.field.GetValue(instance) as List<byte>;
                            if (list != null)
                            {
                                fake[offset] = buffer.AddData(list.ToArray(), dt);
                            }
                        }
                        else
                        {
                            var list = info.field.GetValue(instance) as byte[];
                            if (list != null)
                                fake[offset] = buffer.AddData(list, dt);
                        }
                        break;
                    case DataType.Int32Array:
                        if (info.TypeName == "List`1")
                        {
                            var list = info.field.GetValue(instance) as List<int>;
                            if (list != null)
                            {
                                fake[offset] = buffer.AddData(list.ToArray(), dt);
                            }
                        }
                        else
                        {
                            var list = info.field.GetValue(instance) as int[];
                            if (list != null)
                                fake[offset] = buffer.AddData(list, dt);
                        }
                        break;
                    case DataType.FloatArray:
                        if (info.TypeName == "List`1")
                        {
                            var list = info.field.GetValue(instance) as List<float>;
                            if (list != null)
                            {
                                fake[offset] = buffer.AddData(list.ToArray(), dt);
                            }
                        }
                        else
                        {
                            var list = info.field.GetValue(instance) as float[];
                            if (list != null)
                                fake[offset] = buffer.AddData(list, dt);
                        }
                        break;
                    case DataType.Int16Array:
                        if (info.TypeName == "List`1")
                        {
                            var list = info.field.GetValue(instance) as List<Int16>;
                            if (list != null)
                            {
                                fake[offset] = buffer.AddData(list.ToArray(), dt);
                            }
                        }
                        else
                        {
                            var list = info.field.GetValue(instance) as Int16[];
                            if (list != null)
                                fake[offset] = buffer.AddData(list, dt);
                        }
                        break;
                    case DataType.Int64Array:
                        if (info.TypeName == "List`1")
                        {
                            var list = info.field.GetValue(instance) as List<long>;
                            if (list != null)
                            {
                                fake[offset] = buffer.AddData(list.ToArray(), dt);
                            }
                        }
                        else
                        {
                            var list = info.field.GetValue(instance) as long[];
                            if (list != null)
                                fake[offset] = buffer.AddData(list, dt);
                        }
                        break;
                    case DataType.DoubleArray:
                        if (info.TypeName == "List`1")
                        {
                            var list = info.field.GetValue(instance) as List<double>;
                            if (list != null)
                            {
                                fake[offset] = buffer.AddData(list.ToArray(), dt);
                            }
                        }
                        else
                        {
                            var list = info.field.GetValue(instance) as double[];
                            if (list != null)
                                fake[offset] = buffer.AddData(list, dt);
                        }
                        break;
                    case DataType.FakeStringArray:
                        if (info.TypeName == "List`1")
                        {
                            var list = info.field.GetValue(instance) as List<string>;
                            if (list != null)
                            {
                                FakeStringArray fsa = new FakeStringArray(list.ToArray());
                                fake[offset] = buffer.AddData(fsa, dt);
                            }
                        }
                        else
                        {
                            var list = info.field.GetValue(instance) as string[];
                            if (list != null)
                            {
                                FakeStringArray fsa = new FakeStringArray(list);
                                fake[offset] = buffer.AddData(fsa, dt);
                            }
                        }
                        break;
                    case DataType.ClassArray:
                        {
                            var list = info.field.GetValue(instance) as IList;
                            if (list != null)
                            {
                                int index = -1;
                                for (int j = 0; j < types.Count; j++)
                                {
                                    if (types[j].name == info.ChildTypeName)
                                    {
                                        index = j;
                                        break;
                                    }
                                }
                                if (index >= 0)
                                {
                                    Int16[] aar = new Int16[list.Count];
                                    for (int i = 0; i < list.Count; i++)
                                    {
                                        var fs = Write(index, list[i], buffer);
                                        aar[i] = (Int16)buffer.AddData(fs, DataType.FakeStruct);
                                    }
                                    fake[offset] = buffer.AddData(aar, DataType.Int16Array);
                                }
                            }
                        }
                        break;
                    case DataType.StructArray:
                        {
                            var list = info.field.GetValue(instance) as IList;
                            if (list != null)
                            {
                                int index = -1;
                                for (int j = 0; j < types.Count; j++)
                                {
                                    if (types[j].name == info.ChildTypeName)
                                    {
                                        index = j;
                                        break;
                                    }
                                }
                                if (index >= 0)
                                {
                                    int el = types[index].ElementLength;
                                    FakeStructArray fsa = new FakeStructArray(buffer, el, list.Count);
                                    unsafe
                                    {
                                        FakeStruct fs = new FakeStruct(buffer, el, fsa.ip);
                                        for (int i = 0; i < list.Count; i++)
                                        {
                                            fs.SetPoint(fsa[i]);
                                            Write(index, list[i], fs);
                                        }
                                    }
                                    fake[offset] = buffer.AddData(fsa, DataType.FakeStructArray);
                                }
                            }
                        }
                        break;
                    case DataType.TwoDArray:
                    case DataType.TwoDList:
                        WriteTwoDList(fake, offset, ref info, instance);
                        break;
                    case DataType.Decimal:
                        fake.SetDecimal(offset, (Decimal)info.field.GetValue(instance));
                        break;
                }
            }
           
        }
        void WriteTwoDList(FakeStruct fake, int offset, ref FieldDataInfo info, object instance)
        {
            var buffer = fake.buffer;
            var list = info.field.GetValue(instance) as IList;
            if (list != null)
            {
                int sonType = ((int)info.DataType) >> 8;
                switch (sonType)
                {
                    case DataType.ByteArray:
                        if (info.TypeName == "List`1")
                        {
                            Int16[] aar = new Int16[list.Count];
                            for (int i = 0; i < list.Count; i++)
                            {
                                var a = list[i] as List<byte>;
                                if (a != null)
                                {
                                    aar[i] = (Int16)buffer.AddData(a.ToArray(),sonType);
                                }
                            }
                            fake[offset] = buffer.AddData(aar, DataType.Int16Array);
                        }
                        else
                        {
                            Int16[] aar = new Int16[list.Count];
                            for (int i = 0; i < list.Count; i++)
                            {
                                var a = list[i];
                                if (a != null)
                                {
                                    aar[i] = (Int16)buffer.AddData(a, sonType);
                                }
                            }
                            fake[offset] = buffer.AddData(aar, DataType.Int16Array);
                        }
                        break;
                    case DataType.Int16Array:
                        if (info.TypeName == "List`1")
                        {
                            Int16[] aar = new Int16[list.Count];
                            for (int i = 0; i < list.Count; i++)
                            {
                                var a = list[i] as List<Int16>;
                                if (a != null)
                                {
                                    aar[i] = (Int16)buffer.AddData(a.ToArray(),sonType);
                                }
                            }
                            fake[offset] = buffer.AddData(aar, DataType.Int16Array);
                        }
                        else
                        {
                            Int16[] aar = new Int16[list.Count];
                            for (int i = 0; i < list.Count; i++)
                            {
                                var a = list[i];
                                if (a != null)
                                {
                                    aar[i] = (Int16)buffer.AddData(a, sonType);
                                }
                            }
                            fake[offset] = buffer.AddData(aar, DataType.Int16Array);
                        }
                        break;
                    case DataType.Int32Array:
                        if (info.TypeName == "List`1")
                        {
                            Int16[] aar = new Int16[list.Count];
                            for (int i = 0; i < list.Count; i++)
                            {
                                var a = list[i] as List<Int32>;
                                if (a != null)
                                {
                                    aar[i] = (Int16)buffer.AddData(a.ToArray(), sonType);
                                }
                            }
                            fake[offset] = buffer.AddData(aar, DataType.Int16Array);
                        }
                        else
                        {
                            Int16[] aar = new Int16[list.Count];
                            for (int i = 0; i < list.Count; i++)
                            {
                                var a = list[i];
                                if (a != null)
                                {
                                    aar[i] = (Int16)buffer.AddData(a, sonType);
                                }
                            }
                            fake[offset] = buffer.AddData(aar, DataType.Int16Array);
                        }
                        break;
                    case DataType.FloatArray:
                        if (info.TypeName == "List`1")
                        {
                            Int16[] aar = new Int16[list.Count];
                            for (int i = 0; i < list.Count; i++)
                            {
                                var a = list[i] as List<float>;
                                if (a != null)
                                {
                                    aar[i] = (Int16)buffer.AddData(a.ToArray(), sonType);
                                }
                            }
                            fake[offset] = buffer.AddData(aar, DataType.Int16Array);
                        }
                        else
                        {
                            Int16[] aar = new Int16[list.Count];
                            for (int i = 0; i < list.Count; i++)
                            {
                                var a = list[i];
                                if (a != null)
                                {
                                    aar[i] = (Int16)buffer.AddData(a, sonType);
                                }
                            }
                            fake[offset] = buffer.AddData(aar, DataType.Int16Array);
                        }
                        break;
                    case DataType.Int64Array:
                        if (info.TypeName == "List`1")
                        {
                            Int16[] aar = new Int16[list.Count];
                            for (int i = 0; i < list.Count; i++)
                            {
                                var a = list[i] as List<Int64>;
                                if (a != null)
                                {
                                    aar[i] = (Int16)buffer.AddData(a.ToArray(), sonType);
                                }
                            }
                            fake[offset] = buffer.AddData(aar, DataType.Int16Array);
                        }
                        else
                        {
                            Int16[] aar = new Int16[list.Count];
                            for (int i = 0; i < list.Count; i++)
                            {
                                var a = list[i];
                                if (a != null)
                                {
                                    aar[i] = (Int16)buffer.AddData(a, sonType);
                                }
                            }
                            fake[offset] = buffer.AddData(aar, DataType.Int16Array);
                        }
                        break;
                    case DataType.DoubleArray:
                        if (info.TypeName == "List`1")
                        {
                            Int16[] aar = new Int16[list.Count];
                            for (int i = 0; i < list.Count; i++)
                            {
                                var a = list[i] as List<double>;
                                if (a != null)
                                {
                                    aar[i] = (Int16)buffer.AddData(a.ToArray(), sonType);
                                }
                            }
                            fake[offset] = buffer.AddData(aar, DataType.Int16Array);
                        }
                        else
                        {
                            Int16[] aar = new Int16[list.Count];
                            for (int i = 0; i < list.Count; i++)
                            {
                                var a = list[i];
                                if (a != null)
                                {
                                    aar[i] = (Int16)buffer.AddData(a, sonType);
                                }
                            }
                            fake[offset] = buffer.AddData(aar, DataType.Int16Array);
                        }
                        break;
                    case DataType.FakeStringArray:
                        if (info.TypeName == "List`1")
                        {
                            Int16[] aar = new Int16[list.Count];
                            for (int i = 0; i < list.Count; i++)
                            {
                                var a = list[i] as List<string>;
                                if (a != null)
                                {
                                    aar[i] = (Int16)buffer.AddData(new FakeStringArray(a.ToArray()), sonType);
                                }
                            }
                            fake[offset] = buffer.AddData(aar, DataType.Int16Array);
                        }
                        else
                        {
                            Int16[] aar = new Int16[list.Count];
                            for (int i = 0; i < list.Count; i++)
                            {
                                var a = list[i] as string[];
                                if (a != null)
                                {
                                    aar[i] = (Int16)buffer.AddData(new FakeStringArray(a), sonType);
                                }
                            }
                            fake[offset] = buffer.AddData(aar, DataType.Int16Array);
                        }
                        break;
                    case DataType.StructArray:
                        {
                            int index = -1;
                            for (int j = 0; j < types.Count; j++)
                            {
                                if (types[j].name == info.ChildTypeName)
                                {
                                    index = j;
                                    break;
                                }
                            }
                            if (index >= 0)
                            {
                                Int16[] aar = new Int16[list.Count];
                                int el = types[index].ElementLength;
                                for (int i = 0; i < list.Count; i++)
                                {
                                    var son = list[i] as IList;
                                    if (son != null)
                                    {
                                        FakeStructArray fsa = new FakeStructArray(buffer, el, son.Count);
                                        unsafe
                                        {
                                            FakeStruct fs = new FakeStruct(buffer, el, fsa.ip);
                                            for (int k = 0; k < son.Count; k++)
                                            {
                                                fs.SetPoint(fsa[k]);
                                                Write(index, son[k], fs);
                                            }
                                        }
                                        aar[i] =(Int16) buffer.AddData(fsa, DataType.FakeStructArray);
                                    }
                                }
                                fake[offset] = buffer.AddData(aar, DataType.Int16Array);
                            }
                        }
                        break;
                    case DataType.ClassArray:
                        {
                            int index = -1;
                            for (int j = 0; j < types.Count; j++)
                            {
                                if (types[j].name == info.ChildTypeName)
                                {
                                    index = j;
                                    break;
                                }
                            }
                            if (index >= 0)
                            {
                                Int16[] aar = new Int16[list.Count];
                                int el = types[index].ElementLength;
                                for (int i = 0; i < list.Count; i++)
                                {
                                    var son = list[i] as IList;
                                    if (son != null)
                                    {
                                        Int16[] tmp = new Int16[son.Count];
                                        unsafe
                                        {
                                            for (int k = 0; k < son.Count; k++)
                                            {
                                                if(son[k]!=null)
                                                {
                                                    var fs = Write(index, son[k], buffer);
                                                    tmp[k] = (Int16)buffer.AddData(fs,DataType.FakeStruct);
                                                }
                                            }
                                        }
                                        aar[i] = (Int16)buffer.AddData(tmp, DataType.Int16Array);
                                    }
                                }
                                fake[offset] = buffer.AddData(aar, DataType.Int16Array);
                            }
                        }
                        break;
                }
            }
        }
    }
}