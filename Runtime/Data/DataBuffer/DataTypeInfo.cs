using huqiang.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace huqiang.Data
{
    public class DataTypeInfo
    {
        public string fullName;
        public string name;
        public int DataLength;
        public Int16 ElementLength;
        public Type type;
        public List<DataFieldInfo> dataFeilds;
        public DataFieldInfo[] oldDataFeilds;
        public DataFieldInfo[] matchedFields;
        public int newType;
        public bool isStruct;
        public int typeIndex;
        public Func<object> Construction;
        public DataTypeInfo NewType;
        public DataTypeInfo OldType;
        public void Analysis(DataInfo analysis, Type typ)
        {
            type = typ;
            name = type.Name;
            fullName = type.FullName;
            isStruct = typ.IsValueType;
            var fs = typ.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            int len = 0;
            int dlen = 0;
            dataFeilds = new List<DataFieldInfo>();
            for (int i = 0; i < fs.Length; i++)
            {
                if (fs[i].IsPublic | fs[i].IsDefined(typeof(SerializeField)))
                {
                    if (!fs[i].IsStatic)
                    {
                        if (!fs[i].IsNotSerialized)
                        {
                            if(fs[i].FieldType.Name!=DataType2.Action)
                            {
                                DataFieldInfo df = new DataFieldInfo();
                                df.Analysis(analysis, fs[i]);
                                if (analysis.Unsafe)
                                {
                                    df.MemOffset = UnsafeOperation.GetFeildOffset(df.field.FieldHandle.Value, isStruct);
                                }
                                df.DataOffset = (Int16)len;
                                len += df.ElementLength;
                                dlen += df.DataLength;
                                dataFeilds.Add(df);
                            }
                        }
                    }
                }
            }
            DataLength = dlen;
            ElementLength = (Int16)len;
            if (analysis.CreateConstruction)
            {
#if ENABLE_MONO
                try
                {
                    if (isStruct)
                        Construction = DataInfo.GetCreateStructFunc(typ);
                    else
                        Construction = DataInfo.GetCreateFunc(typ);
                }catch
                {
                    Debug.LogError("Construction Error ! "+ typ.Name);
                    throw (new Exception());
                }
#else
                Construction = () => { return Activator.CreateInstance(type); };
#endif
            }

        }
    }
}
