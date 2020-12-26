using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace huqiang.Data
{
    /// <summary>
    /// 用于DataBuffer数据序列化于反序列化
    /// 支持的数据类型有
    ///  Int32,Float,Int64,Double,Decimal,byte[],Int16[],Int32[],Int64[],Float[],Double[],List<T>,List<List<T>>,Array[T],Array[][T]
    /// 示例代码
    ///    DataWriter dw = new DataWriter();
    ///    dw.Analysis<A>();
    ///    A a = new A();
    ///    DataReader dr = new DataReader();
    ///    dr.Analysis<B>();
    ///    var b = dr.Read<B>(db);
    /// </summary>
    public class TypeAnalysis
    {
        protected struct FieldDataInfo
        {
            public Int16 Offset;
            public Int16 DataLength;
            public Int16 ElementLength;
            public Int16 DataType;
            public string TypeName;
            public string FieldName;
            public string ChildTypeName;//子元素类型
            public FieldInfo field;
            public Type ChildType;
            public Type ArgType;
            public Int16 OldType;
            public string OldTypeName;
            public string OldChildTypeName;//子元素类型
            public bool IsArray;
            public bool HaveTable;
        }
        protected struct TypeInfo
        {
            public string name;
            public int dataLenth;
            public Int16 ElementLength;
            public Type type;
            public FieldDataInfo[] dataTypes;
        }
        protected List<TypeInfo> types = new List<TypeInfo>();
        protected string MainType;
        public void Analysis<T>() where T : new()
        {
            types.Clear();
            var t = Analysis(typeof(T));
            MainType = t.name;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns>数据类占用内存长度</returns>
        TypeInfo Analysis(Type type)
        {
            if (type.Name == "String")
            {
                return new TypeInfo();
            }
            if (type.IsGenericType)//泛型类
            {
                if (type.Name == "List`1")
                {
                    var tps = type.GetGenericArguments();
                    return Analysis(tps[0]);
                }
            }
            if (type.IsArray)
            {
                return Analysis(type.GetElementType());
            }
            string name = type.Name;
            for (int i = 0; i < types.Count; i++)
            {
                if (types[i].name == name)
                    return types[i];
            }
            var fs = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
            int len = 0;
            int dlen = 0;
            List<FieldDataInfo> fdis = new List<FieldDataInfo>();
            for (int i = 0; i < fs.Length; i++)
            {
                var non =  fs[i].IsDefined(typeof(NonSerializedAttribute));
                if(!non)
                {
                    FieldDataInfo fd = new FieldDataInfo();
                    fd.field = fs[i];
                    fd.Offset = (Int16)len;
                    Analysis(fs[i], ref fd);
                    len += fd.ElementLength;
                    dlen += fd.DataLength;
                    fdis.Add(fd);
                }
            }
            TypeInfo info = new TypeInfo();
            info.name = name;
            info.dataLenth = dlen;
            info.ElementLength = (Int16)len;
            info.dataTypes = fdis.ToArray();
            info.type = type;
            types.Add(info);
            return info;
        }
        void Analysis(FieldInfo field, ref FieldDataInfo info)
        {
            var type = field.FieldType;
            info.TypeName = type.Name;
            info.FieldName = field.Name;
            if (type.IsArray)
            {
                info.DataLength = 2;
                info.ElementLength = 1;
                var son = type.GetElementType();
                AnalysisArrayType(son, ref info);
            }
            else if (type.IsClass)
            {
                if (type.Name == "String")
                {
                    info.DataLength = 2;
                    info.ElementLength = 1;
                    info.DataType = DataType.String;
                    return;
                }
                else if (type.IsGenericType)//泛型类
                {
                    if (type.Name == "List`1")
                    {
                        info.DataLength = 2;
                        info.ElementLength = 1;
                        info.DataType = DataType.Int16Array;
                        var tps = type.GetGenericArguments();
                        var son = tps[0];
                        AnalysisArrayType(son, ref info);
                        return;
                    }
                }
                info.DataLength = 2;
                info.ElementLength = 1;
                info.DataType = DataType.Class;
                info.HaveTable = true;
                info.ChildTypeName = info.FieldName;
                Analysis(type);
            }
            else if (type.IsValueType)
            {
                AnalysisValueType(type, ref info);
            }
            else if (type.IsEnum)
            {
                info.DataLength = 4;
                info.ElementLength = 1;
                info.DataType = DataType.Int;
            }
        }
        void AnalysisValueType(Type type, ref FieldDataInfo info)
        {
            string name = type.Name;
            switch (name)
            {
                case "Byte":
                case "SByte":
                    info.DataLength = 1;
                    info.ElementLength = 1;
                    info.DataType = DataType.Int;
                    break;
                case "Int16":
                case "UInt16":
                case "Char":
                    info.DataLength = 2;
                    info.ElementLength = 1;
                    info.DataType = DataType.Int;
                    break;
                case "Boolean":
                case "Int32":
                case "UInt32":
                    info.DataLength = 4;
                    info.ElementLength = 1;
                    info.DataType = DataType.Int;
                    break;
                case "Single":
                    info.DataLength = 4;
                    info.ElementLength = 1;
                    info.DataType = DataType.Float;
                    break;
                case "Int64":
                    info.DataLength = 8;
                    info.ElementLength = 2;
                    info.DataType = DataType.Long;
                    break;
                case "Double":
                case "UInt64":
                    info.DataLength = 8;
                    info.ElementLength = 2;
                    info.DataType = DataType.Double;
                    break;
                case "Decimal":
                    info.DataLength = 16;
                    info.ElementLength = 4;
                    info.DataType = DataType.Double;
                    break;
                default://这是一个结构体
                    info.DataType = DataType.Struct;
                    info.HaveTable = true;
                    var dt = Analysis(type);
                    info.DataLength = (Int16)dt.dataLenth;
                    info.ElementLength = dt.ElementLength;
                    info.ChildTypeName = name;
                    break;
            }
        }
        void AnalysisArrayType(Type type, ref FieldDataInfo info)
        {
            info.IsArray = true;
            string name = type.Name;
            info.ChildTypeName = name;
            info.ChildType = type;
            if (type.IsValueType)
            {
                switch (name)
                {
                    case "Byte":
                        info.DataType = DataType.ByteArray;
                        break;
                    case "Int16":
                        info.DataType = DataType.Int16Array;
                        break;
                    case "Int32":
                        info.DataType = DataType.Int32Array;
                        break;
                    case "Single":
                        info.DataType = DataType.FloatArray;
                        break;
                    case "Int64":
                        info.DataType = DataType.Int64Array;
                        break;
                    case "Double":
                        info.DataType = DataType.DoubleArray;
                        break;
                    default:
                        info.DataType = DataType.StructArray;
                        info.HaveTable = true;
                        Analysis(type);//这是一个结构体数组
                        break;
                }
            }
            else
            {
                if (type.IsGenericType)//泛型类
                {
                    if (name == "List`1")//二维列表
                    {
                        info.ArgType = type;
                        info.DataType = DataType.TwoDList;
                        var tps = type.GetGenericArguments();
                        FieldDataInfo fd = new FieldDataInfo();
                        info.ChildTypeName = tps[0].Name;
                        info.ArgType = tps[0];
                        AnalysisArrayType(tps[0], ref fd);
                        info.DataType = (Int16)(((int)fd.DataType << 8) | (int)info.DataType);
                        info.HaveTable = fd.HaveTable;
                    }
                    else info.DataType = DataType.ClassArray;
                }
                else if (type.IsArray)//二维数组
                {
                    info.DataType = DataType.TwoDArray;
                    FieldDataInfo fd = new FieldDataInfo();
                    var ele = type.GetElementType();
                    info.ChildTypeName = ele.Name;
                    info.ArgType = ele;
                    AnalysisArrayType(ele, ref fd);
                    info.DataType = (Int16)(((int)fd.DataType << 8) | (int)info.DataType);
                    info.HaveTable = fd.HaveTable;
                }
                else if (name == "String")
                {
                    info.DataType = DataType.FakeStringArray;
                }
                else
                {
                    info.DataType = DataType.ClassArray;
                    info.HaveTable = true;
                    Analysis(type);//这是一个类数组
                }
            }
        }
    }
}
