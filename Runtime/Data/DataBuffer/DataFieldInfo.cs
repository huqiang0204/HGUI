using huqiang.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace huqiang.Data
{
    public class DataFieldInfo
    {
        /// <summary>
        /// 数据在内存中的偏移地址
        /// </summary>
        public Int16 MemOffset;
        /// <summary>
        /// 数据存储偏移地址
        /// </summary>
        public Int16 DataOffset;
        /// <summary>
        /// 数据长度
        /// </summary>
        public Int16 DataLength;
        /// <summary>
        /// 元素长度
        /// </summary>
        public Int16 ElementLength;
        /// <summary>
        /// 数据类型
        /// </summary>
        public Int16 DBType;
        /// <summary>
        /// 类型名称
        /// </summary>
        public string TypeName;
        /// <summary>
        /// 类型名称
        /// </summary>
        public string TypeFullName;
        /// <summary>
        /// 字段名称
        /// </summary>
        public string FieldName;
        public string ChildTypeName;//子元素类型
        public string ChildTypeName2;//子元素类型
        /// <summary>
        /// 子元素类型
        /// </summary>
        public string ChildTypeFullName;//子元素类型
        public FieldInfo field;
        public DataFieldInfo OldFieldInfo;
        /// <summary>
        /// 是否是数组类型
        /// </summary>
        public bool IsArray;
        /// <summary>
        /// 是否有类型声明表
        /// </summary>
        public bool HaveTable;
        public Func<object> Construction;
        public Func<object> Construction2;
        /// <summary>
        /// 一维数组的构造函数
        /// </summary>
        public Func<int, object> ArrayConstruction;
        /// <summary>
        /// 二维数组的构造函数
        /// </summary>
        public Func<int, object> ArrayConstruction2;
        DataInfo dataInfo;
        public DataTypeInfo typeInfo;
        Type type, type1;
        public virtual void Analysis(DataInfo analysis, FieldInfo fi)
        {
            dataInfo = analysis;
            field = fi;
            type = field.FieldType;
            TypeName = type.Name;
            TypeFullName = type.FullName;
            FieldName = field.Name;
            DBType = GetFieldType(type, 0);
            if(DBType<32)
            {
                if (DBType != DataType2.Struct)
                {
                    DataLength = (Int16)DataType2.DataLength[DBType];
                    ElementLength = (Int16)DataType2.ElementLength[DBType];
                }
            }
            else
            {
                ElementLength = 1;
                DataLength = 4;//(Int16)DataType2.DataLength[a];
            }
#if ENABLE_IL2CPP
            if (dataInfo.CreateConstruction)
            {
                if(DBType>64)
                {
                    ArrayConstruction = (o) => { return Array.CreateInstance(type1, o); };
                    ArrayConstruction2 = (o) => { return Array.CreateInstance(type, o); };
                    Construction = () => { return Activator.CreateInstance(type1); };
                    Construction2 = () => { return Activator.CreateInstance(type); };
                }
                else if(DBType > 32)
                {
                    ArrayConstruction = (o) => { return Array.CreateInstance(type, o); };
                    Construction = () => { return Activator.CreateInstance(type); };
                }
            }
#endif
        }
        protected Int16 GetFieldType(Type tar, int c)
        {
            if(c==1)
            {
                ChildTypeName = tar.Name;
                type1 = tar;
            }else if(c==2)
            {
                ChildTypeName2 = tar.Name;
            }
            c++;
            if (tar.IsArray)
            {
                var son = tar.GetElementType();
                Int16 typ = GetFieldType(son, c);
#if ENABLE_MONO
                if (dataInfo.CreateConstruction)
                {
                    int a = typ;
                    if (a < 32)
                    {
                        if (typ == DataType2.Struct | typ == DataType2.Class)//[]
                            ArrayConstruction = DataInfo.GetCreateFuncArray(son);
                    }
                    else
                    {
                        ArrayConstruction2 = DataInfo.GetCreateFuncArray(son);
                    }
                }
#endif
                return (Int16)(typ + 32);
            }
            else if (tar.IsClass)
            {
                if (tar.Name == "String")
                {
                    return DataType2.String;
                }
                else if (tar.IsGenericType)//泛型类
                {
                    if (tar.Name == DataType2.List)
                    {
                        DBType = DataType2.Int16Array;
                        var tps = tar.GetGenericArguments();
                        var son = tps[0];
                        Int16 typ = GetFieldType(son, c);
#if ENABLE_MONO
                        if (dataInfo.CreateConstruction)
                        {
                            if (typ == DataType2.Struct | typ==DataType2.Class)//List<T[]>
                            { 
                                Construction = DataInfo.GetCreateFunc(tar); 
                            }
                            else if (typ > 31)
                            {
                                Construction2 = DataInfo.GetCreateFunc(tar);
                            }
                        }
#endif
                        return (Int16)(typ + 32);
                    }
                }
                typeInfo = dataInfo.Analysis(tar);
                HaveTable = true;
                ChildTypeFullName = typeInfo.fullName;
                return DataType2.Class;
            }
            else if (tar.IsEnum)
            {
                return DataType2.Int32;
            }
            else if (tar.IsValueType)
            {
                switch (tar.Name)
                {
                    case "Byte":
                    case "SByte":
                        return DataType2.Byte;
                    case "Boolean":
                        return DataType2.Boolean;
                    case "Int16":
                    case "UInt16":
                    case "Char":
                        return DataType2.Int16;
                    case "Int32":
                    case "UInt32":
                        return DataType2.Int32;
                    case "Single":
                        return DataType2.Float;
                    case "Int64":
                    case "UInt64":
                        return DataType2.Int64;
                    case "Double":
                        return DataType2.Double;
                    case "Decimal":
                        return DataType2.Decimal;
                    default://这是一个结构体
                        typeInfo = dataInfo.Analysis(tar);
                        ElementLength = typeInfo.ElementLength;
                        DataLength =(Int16)typeInfo.DataLength;
                        HaveTable = true;
                        ChildTypeFullName = typeInfo.fullName;
                        return DataType2.Struct;
                }
            }
            return DataType2.Int32;
        }
    }
}
