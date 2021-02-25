using huqiang.Data;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace huqiang.Data
{
    public class DataType2
    {
        public static int[] ElementLength = new int[] {
            1,1,1,1,1,1,1,2,2,2,4,1,1,0,0,0,1,1
        };
        public static int[] DataLength = new int[] {
            1,1,2,2,4,4,4,8,8,8,16,1,4,0,0,0,4,4
        };
        /// <summary>
        /// 基本类型
        /// </summary>
        public const short Byte = 0;
        public const short SByte = 1;
        public const short Int16 = 2;
        public const short UInt16 = 3;
        public const short Int32 = 4;
        public const short UInt32 = 5;
        public const short Float = 6;
        public const short Int64 = 7;
        public const short UInt64 = 8;
        public const short Double = 9;
        public const short Decimal = 10;
        public const short Boolean = 11;
        public const short Struct = 12;
        public const short String = 16;
        public const short Class = 17;
        /// <summary>
        /// 一维数组
        /// </summary>
        public const short ByteArray = 32;
        public const short SByteArray = 33;
        public const short Int16Array = 34;
        public const short UInt16Array = 35;
        public const short Int32Array = 36;
        public const short UInt32Array = 37;
        public const short FloatArray = 38;
        public const short Int64Array = 39;
        public const short UInt64Array = 40;
        public const short DoubleArray = 41;
        public const short DecimalArray = 42;
        public const short StructArray = 44;
        public const short StringArray = 48;
        public const short ClassArray = 49;
        /// <summary>
        /// 二维数组
        /// </summary>
        public const short ByteArray2 = 64;
        public const short SByteArray2 = 65;
        public const short Int16Array2 = 66;
        public const short UInt16Array2 = 67;
        public const short Int32Array2 = 68;
        public const short UInt32Array2 = 69;
        public const short FloatArray2 = 70;
        public const short Int64Array2 = 71;
        public const short UInt64Array2 = 72;
        public const short DoubleArray2 = 73;
        public const short DecimalArray2 = 74;
        public const short StructArray2 = 76;
        public const short StringArray2 = 80;
        public const short ClassArray2 = 81;

        public const string List = "List`1";
        public const string Action = "Action`1";
    }
    public class DataInfo
    {
        public static Func<object> GetCreateStructFunc(Type type)
        {
            var c = Expression.Convert(Expression.New(type), typeof(object));
            return Expression.Lambda<Func<object>>(c, null).Compile();
        }
        public static Func<object> GetCreateFunc(Type type)
        {
            var newExpression = Expression.New(type);
            return Expression.Lambda<Func<object>>(newExpression, null).Compile();
        }
        public static Func<int, object> GetCreateFuncArray(Type type)
        {
            var cons = Expression.Parameter(typeof(int), "a");
            var newExpression = Expression.NewArrayBounds(type, cons);
            return Expression.Lambda<Func<int, object>>(newExpression, cons).Compile();
        }
        bool _unsafe;
        public bool Unsafe { get=>_unsafe; set {
#if ENABLE_IL2CPP
            UnityEngine.Debug.LogError("IL2CPP Not supported");
            return;
#endif
                _unsafe = value;
            }
        }
        public bool CreateConstruction;
        public List<DataTypeInfo> dataTypes = new List<DataTypeInfo>();
        public Assembly assembly;
        public DataTypeInfo Analysis(string fullName)
        {
            if (assembly == null)
                return null;
            var type = assembly.GetType(fullName);
            if (type != null)
                return Analysis(type);
            return null;
        }
        public DataTypeInfo Analysis(Type typ)
        {
            for (int i = 0; i < dataTypes.Count; i++)
            {
                if (dataTypes[i].fullName == typ.FullName)
                    return dataTypes[i];
            }
            DataTypeInfo dataType = new DataTypeInfo();
            dataType.typeIndex = dataTypes.Count;
            dataTypes.Add(dataType);
            dataType.Analysis(this, typ);
            return dataType;
        }
        public DataTypeInfo Analysis<T>() where T : new()
        {
            var t = typeof(T);
            assembly = t.Assembly;
            return Analysis(t);
        }
        /// <summary>
        /// 写入类的声明信息
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public FakeStructArray WriteTables(DataBuffer db)
        {
            int c = dataTypes.Count;
            FakeStructArray fsa = new FakeStructArray(db, 4, c);
            for (int i = 0; i < c; i++)
            {
                fsa[i, 0] = dataTypes[i].DataLength;
                fsa[i, 1] = db.AddData(dataTypes[i].fullName, DataType.String);
                fsa[i, 2] = db.AddData(dataTypes[i].name, DataType.String);
                fsa[i, 3] = db.AddData(WriteTableFields(db, dataTypes[i].dataFeilds), DataType.FakeStructArray);
            }
            return fsa;
        }
        /// <summary>
        /// 写入字段声明信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        FakeStructArray WriteTableFields(DataBuffer db, List<DataFieldInfo> fields)
        {
            int c = fields.Count;
            FakeStructArray fsa = new FakeStructArray(db, 3, c);
            unsafe
            {
                for (int i = 0; i < c; i++)
                {
                    byte* ip = fsa[i];
                    Int16* bp = (Int16*)ip;
                    *bp = fields[i].DataOffset;
                    bp++;
                    *bp = fields[i].ElementLength;
                    bp++;
                    *bp = fields[i].DBType;
                    bp++;
                    *bp = (Int16)db.AddData(fields[i].TypeFullName, DataType.String);
                    bp++;
                    *bp = (Int16)db.AddData(fields[i].FieldName, DataType.String);
                    bp++;
                    *bp = (Int16)db.AddData(fields[i].ChildTypeFullName, DataType.String);
                }
            }
            return fsa;
        }
        public DataTypeInfo FindTypeInfo2(string fullName)
        {
            for (int i = 0; i < dataTypes.Count; i++)
                if (fullName == dataTypes[i].fullName)
                    return dataTypes[i];
            return null;
        }
        public DataTypeInfo[] ReadTables(FakeStructArray fsa)
        {
            int c = fsa.Length;
            DataTypeInfo[] tmp = new DataTypeInfo[c];
            for (int i = 0; i < c; i++)
            {
                DataTypeInfo info = new DataTypeInfo();
                info.DataLength = fsa[i, 0];
                info.fullName = fsa.GetData<string>(i, 1);
                info.name = fsa.GetData<string>(i, 2);
                var fs = fsa.GetData<FakeStructArray>(i, 3);
                if (fs != null)
                    info.oldDataFeilds = ReadTableFields(fs);
                tmp[i] = info;
            }
            return tmp;
        }
        /// <summary>
        /// 读取字段声明
        /// </summary>
        /// <param name="fsa"></param>
        /// <returns></returns>
        DataFieldInfo[] ReadTableFields(FakeStructArray fsa)
        {
            int c = fsa.Length;
            DataFieldInfo[] fields = new DataFieldInfo[c];
            var db = fsa.buffer;
            unsafe
            {
                for (int i = 0; i < c; i++)
                {
                    var df = fields[i] = new DataFieldInfo();
                    byte* ip = fsa[i];
                    Int16* bp = (Int16*)ip;
                    df.DataOffset = *bp;
                    bp++;
                    df.ElementLength = *bp;
                    bp++;
                    df.DBType = *bp;
                    bp++;
                    df.TypeFullName = db.GetData(*bp) as string;
                    bp++;
                    df.FieldName = db.GetData(*bp) as string;
                    bp++;
                    df.ChildTypeFullName = db.GetData(*bp) as string;
                }
            }
            return fields;
        }
    }
}
