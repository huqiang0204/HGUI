using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

public class DataBufferScript : Editor
{
    [Serializable]
    class Asmdef
    {
        public string name;
        public string[] references;
        public bool allowUnsafeCode;
    }
    [MenuItem("Assets/ExportCS/DataBuffer")]
    static void ExportJsonScript()
    {
        var o = Selection.activeObject;
        var ta = o as TextAsset;
        if (ta != null)
        {
            string path = AssetDatabase.GetAssetPath(o);
            int end = path.LastIndexOf("/");
            path = path.Substring(0, end);
            var dic = Environment.CurrentDirectory;
            string p = dic + "\\" + path + "\\DBData";
            if (!Directory.Exists(p))
                Directory.CreateDirectory(p);
            dic = p + "\\";
            var asm = JsonUtility.FromJson<Asmdef>(ta.text);
            var ass = Assembly.Load(asm.name);
            var tps = ass.GetExportedTypes();
            for (int i = 0; i < tps.Length; i++)
            {
                var type = tps[i];
                if (type.Namespace == "SerializedData")
                {
                    if (type.IsSerializable)
                        CreateDBScript(type, dic);
                }
            }
            AssetDatabase.Refresh();
        }
    }
    static string claStart = @"
namespace SerializedData
{
     public partial class ";
    public static void CreateDBScript(Type type, string dic)
    {
        StringBuilder sb = new StringBuilder();

        var fs = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
        List<string> nps = new List<string>();
        nps.Add("System");
        nps.Add("System.Collections.Generic");
        nps.Add("huqiang.Data");
        nps.Add("UnityEngine");
        StringBuilder loader = new StringBuilder();
        StringBuilder writer = new StringBuilder();
        int offset = 0;
        if (fs != null)
        {
            for (int j = 0; j < fs.Length; j++)
            {
                offset += Analysis(fs[j], loader, writer, nps, offset);
            }
        }
        writer.Append("            return fake;\r\n");
        for (int i = 0; i < nps.Count; i++)
        {
            sb.Append("using ");
            sb.Append(nps[i]);
            sb.Append(";\r\n");
        }
        sb.Append(claStart);
        sb.Append(type.Name);
        sb.Append("\r\n     {\r\n        public void Load(FakeStruct fake)\r\n        {\r\n");
        sb.Append(loader);
        sb.Append("        }\r\n        public FakeStruct Save(DataBuffer db)\r\n        {\r\n");
        sb.Append("            FakeStruct fake = new FakeStruct(db, &&&);\r\n".Replace("&&&",offset.ToString()));
        if (writer.Length == 0)
            sb.Append("            return null;\r\n");
        else sb.Append(writer);
        sb.Append("        }\r\n     }\r\n}");
        string path = dic + "/" + type.Name + ".cs";
        if (File.Exists(path))
            File.Delete(path);
        File.WriteAllText(path, sb.ToString());
    }
    static string LoadObject = @"            var ---_t = fake.GetData<FakeStruct>(&&&);
            if (---_t != null)
            {
                --- = new +++();
                ---.Load(---_t);
            }
";
    static int Analysis(FieldInfo field, StringBuilder loader, StringBuilder writer, List<string> nps, int offset)
    {
        var type = field.FieldType;
        if (type.IsArray)
        {
            LoadArrayType(field, loader,offset);
            SaveArrayType(field, writer, offset);
            var son = type.GetElementType();
            string ss = son.Namespace;
            if (!nps.Contains(ss))
                nps.Add(ss);
        }
        else if (type.IsClass)
        {
            if (type.Name == "String")
            {
                string str = "            --- = fake.GetData<string>(&&&);\r\n".Replace("---",field.Name);
                str = str.Replace("&&&",offset.ToString());
                loader.Append(str);
                str = "            fake.SetData(&&&, ---);\r\n".Replace("---", field.Name);
                str = str.Replace("&&&", offset.ToString());
                writer.Append(str);
                return 1;
            }
            else if (type.IsGenericType)//泛型类
            {
                if (type.Name == "List`1")
                {
                    LoadListType(field, loader, offset);
                    SaveListType(field, writer, offset);
                    var tps = type.GetGenericArguments();
                    var son = tps[0];
                    string ss = son.Namespace;
                    if (!nps.Contains(ss))
                        nps.Add(ss);
                    return 1;
                }
            }
            string to = LoadObject.Replace("---", field.Name);
            to = to.Replace("+++", type.Name);
            to = to.Replace("&&&", offset.ToString());
            loader.Append(to);
            to = "            if (--- != null)\r\n                fake[&&&] = db.AddData(---.Save(db));\r\n"
                .Replace("---", field.Name);
            to = to.Replace("&&&", offset.ToString());
            writer.Append(to);
            string ns = type.Namespace;
            if (!nps.Contains(ns))
                nps.Add(ns);
        }
        else if (type.IsEnum)
        {
            string str = "            --- = (+++)fake[&&&];\r\n".Replace("---", field.Name);
            str = str.Replace("+++", type.Name);
            str = str.Replace("&&&", offset.ToString());
            loader.Append(str);
            str = "            fake[&&&] = (int)---;\r\n".Replace("---", field.Name);
            str = str.Replace("&&&", offset.ToString());
            writer.Append(str);
            string ns = type.Namespace;
            if (!nps.Contains(ns))
                nps.Add(ns);
        }
        else if (type.IsValueType)
        {
            SaveValueType(field, writer, offset);
            return LoadValueType(field, loader, offset);
        }
        return 1;
    }
    static string StringArrayLoad = @"            var ---_t = fake.GetData<FakeStringArray>(&&&);
            if (---_t != null)
                --- = ---_t.Data;
";
    static string ObjectArrayLoad = @"            var ---_t = fake.GetData<Int16[]>(&&&);
            if (---_t != null)
            {
                --- = new +++[---_t.Length];
                for (int i = 0; i < ---_t.Length; i++)
                {
                    if (---_t[i] >= 0)
                    {
                        ---[i] = new +++();
                        ---[i].Load(fake.buffer.GetData(---_t[i]) as FakeStruct);
                    }
                }
            }
";
    static void LoadArrayType(FieldInfo field, StringBuilder loader,int offset)
    {
        var type = field.FieldType;
        var son = type.GetElementType();
        if (son.IsValueType)
        {
            switch(son.Name)
            {
                case "Byte":
                case "Int16":
                case "Int32":
                case "Single":
                case "Int64":
                case "Double":
                    string str= "            --- = fake.GetData<+++[]>(&&&);\r\n".Replace("---", field.Name);
                    str = str.Replace("+++", son.Name);
                    str = str.Replace("&&&", offset.ToString());
                    loader.Append(str);
                    break;
                case "SByte":
                case "UInt16":
                case "Char":
                case "Boolean":
                case "UInt32":
                case "UInt64":
                case "Decimal":
                case "Vector2":
                case "Vector3":
                case "Vector4":
                case "Quaternion":
                case "Color":
                case "Color32":
                    str = "            --- = fake.buffer.GetArray<+++>(fake[&&&]);\r\n".Replace("---", field.Name);
                    str = str.Replace("+++", son.Name);
                    str = str.Replace("&&&", offset.ToString());
                    loader.Append(str);
                    break;
                default:
                    
                    break;
            }
        }
        else
        {
            if (son.Name == "String")
            {
                string str = StringArrayLoad.Replace("---", field.Name);
                str = str.Replace("&&&", offset.ToString());
                loader.Append(str);
            }
            else
            {
                string str = ObjectArrayLoad.Replace("---", field.Name);
                str = str.Replace("+++", son.Name);
                str = str.Replace("&&&", offset.ToString());
                loader.Append(str);
            }
        }
    }
    static string ObjectArraySave = @"            if (--- != null)
            {
                if(---.Length>0)
                {
                    Int16[] ---_b = new Int16[---.Length];
                    for (int i = 0; i < ---.Length; i++)
                    {
                        var ---_t = ---[i];
                        if (---_t != null)
                        {
                            ---_b[i] = (Int16)db.AddData(---_t.Save(db));
                        }
                    }
                    fake[&&&] = db.AddData(---_b, DataType.Int16Array);
                }
            }
";
    static void SaveArrayType(FieldInfo field, StringBuilder writer, int offset)
    {
        var type = field.FieldType;
        var son = type.GetElementType();
        if (son.IsValueType)
        {
            switch (son.Name)
            {
                case "Byte":
                    string str = "            fake[&&&] = db.AddData(---, DataType.ByteArray);\r\n".Replace("---", field.Name);
                    str = str.Replace("&&&", offset.ToString());
                    writer.Append(str);
                    break;
                case "Int16":
                    str = "            fake[&&&] = db.AddData(---, DataType.Int16Array);\r\n".Replace("---", field.Name);
                    str = str.Replace("&&&", offset.ToString());
                    writer.Append(str);
                    break;
                case "Int32":
                    str = "            fake[&&&] = db.AddData(---, DataType.Int32Array);\r\n".Replace("---", field.Name);
                    str = str.Replace("&&&", offset.ToString());
                    writer.Append(str);
                    break;
                case "Single":
                    str = "            fake[&&&] = db.AddData(---, DataType.FloatArray);\r\n".Replace("---", field.Name);
                    str = str.Replace("&&&", offset.ToString());
                    writer.Append(str);
                    break;
                case "Int64":
                    str = "            fake[&&&] = db.AddData(---, DataType.Int64Array);\r\n".Replace("---", field.Name);
                    str = str.Replace("&&&", offset.ToString());
                    writer.Append(str);
                    break;
                case "Double":
                    str = "            fake[&&&] = db.AddData(---, DataType.DoubleArray);\r\n".Replace("---", field.Name);
                    str = str.Replace("&&&", offset.ToString());
                    writer.Append(str);
                    break;
                case "SByte":
                case "UInt16":
                case "Char":
                case "Boolean":
                case "UInt32":
                case "UInt64":
                case "Decimal":
                case "Vector2":
                case "Vector3":
                case "Vector4":
                case "Quaternion":
                case "Color":
                case "Color32":
                    str = "            if (--- != null)\r\n                fake[&&&] = db.AddArray<+++>(---);\r\n"
                        .Replace("---", field.Name);
                    str = str.Replace("&&&", offset.ToString());
                    str = str.Replace("+++", son.Name);
                    writer.Append(str);
                    break;
                default:

                    break;
            }
        }
        else
        {
            if (son.Name == "String")
            {
                string str = "            if (--- != null)\r\n                fake[&&&] = db.AddData(new FakeStringArray(---), DataType.FakeStringArray);\r\n"
                    .Replace("---", field.Name);
                str = str.Replace("&&&", offset.ToString());
                writer.Append(str);
            }
            else
            {
                string str = ObjectArraySave.Replace("---", field.Name);
                str = str.Replace("&&&", offset.ToString());
                writer.Append(str);
            }
        }
    }
    static string ListValueLoad = @"            var ---_t = fake.GetData<+++[]>(&&&);
            if (---_t != null)
            {
                --- = new List<+++>();
                ---.AddRange(---_t); 
            }
";
    static string StringListLoad = @"            var ---_t = fake.GetData<FakeStringArray>(&&&);
            if (---_t != null)
                ---.AddRange(---_t.Data);
";
    static string ObjectListLoad = @"            var ---_t = fake.GetData<Int16[]>(&&&);
            if (---_t != null)
            {
                --- = new List<+++>();
                for (int i = 0; i < ---_t.Length; i++)
                {
                    if (---_t[i] >= 0)
                    {
                        var ---_s = new +++();
                        ---_s.Load(fake.buffer.GetData(---_t[i]) as FakeStruct);
                        ---.Add(---_s);
                    }
                }
            }
";
    static string ListStruct = @"            var ---_t = fake.buffer.GetArray<+++>(fake[&&&]);
            if (--- != null)
            {
                --- = new List<+++>();
                ---.AddRange(---_t);
            }
";
    static void LoadListType(FieldInfo field, StringBuilder loader, int offset)
    {
        var type = field.FieldType;
        var tps = type.GetGenericArguments();
        var son = tps[0];
        if (son.IsValueType)
        {
            switch (son.Name)
            {
                case "Byte":
                case "Int16":
                case "Int32":
                case "Single":
                case "Int64":
                case "Double":
                    string str = ListValueLoad.Replace("---", field.Name);
                    str = str.Replace("+++", son.Name);
                    str = str.Replace("&&&", offset.ToString());
                    loader.Append(str);
                    break;
                case "SByte":
                case "UInt16":
                case "Char":
                case "Boolean":
                case "UInt32":
                case "UInt64":
                case "Decimal":
                case "Vector2":
                case "Vector3":
                case "Vector4":
                case "Quaternion":
                case "Color":
                case "Color32":
                    str = ListStruct.Replace("---", field.Name);
                    str = str.Replace("+++", son.Name);
                    str = str.Replace("&&&", offset.ToString());
                    loader.Append(str);
                    break;
                default:

                    break;
            }
        }
        else
        {
            if (son.Name == "String")
            {
                string str = StringListLoad.Replace("---", field.Name);
                str = str.Replace("&&&", offset.ToString());
                loader.Append(str);
            }
            else
            {
                string str = ObjectListLoad.Replace("---", field.Name);
                str = str.Replace("+++", son.Name);
                str = str.Replace("&&&", offset.ToString());
                loader.Append(str);
            }
        }
    }
    static string ObjectListSave = @"            if (--- != null)
            {
                if(---.Count>0)
                {
                    Int16[] ---_b = new Int16[---.Count];
                    for (int i = 0; i < ---.Count; i++)
                    {
                        var ---_t = ---[i];
                        if (---_t != null)
                        {
                            ---_b[i] = (Int16)db.AddData(---_t.Save(db));
                        }
                    }
                    fake[&&&] = db.AddData(---_b, DataType.Int16Array);
                }
            }
";
    static string ValueListSave = @"            if (--- != null)
                if (---.Count > 0)
                    fake[&&&] = db.AddData(---.ToArray(), DataType.+++Array);
";
    static string StringListSave = @"            if (--- != null)
                if (---.Count > 0)
                    fake[&&&] = db.AddData(new FakeStringArray(---.ToArray()), DataType.FakeStringArray);
";
    static void SaveListType(FieldInfo field, StringBuilder writer, int offset)
    {
        var type = field.FieldType;
        var tps = type.GetGenericArguments();
        var son = tps[0];
        if (son.IsValueType)
        {
            switch (son.Name)
            {
                case "Byte":
                case "Int16":
                case "Int32":
                case "Int64":
                case "Double":
                    string str = ValueListSave.Replace("---", field.Name);
                    str = str.Replace("&&&", offset.ToString());
                    str = str.Replace("+++", son.Name);
                    writer.Append(str);
                    break;
                case "Single":
                    str = ValueListSave.Replace("---", field.Name);
                    str = str.Replace("&&&", offset.ToString());
                    str = str.Replace("+++", "Float");
                    writer.Append(str);
                    break;
                case "SByte":
                case "UInt16":
                case "Char":
                case "Boolean":
                case "UInt32":
                case "UInt64":
                case "Decimal":
                case "Vector2":
                case "Vector3":
                case "Vector4":
                case "Quaternion":
                case "Color":
                case "Color32":
                    str = "            if (--- != null)\r\n                fake[&&&] = db.AddArray<+++>(---.ToArray());\r\n"
                        .Replace("---", field.Name);
                    str = str.Replace("&&&", offset.ToString());
                    str = str.Replace("+++", son.Name);
                    writer.Append(str);
                    break;
                default:

                    break;
            }
        }
        else
        {
            if (son.Name == "String")
            {
                string str = StringListSave.Replace("---", field.Name);
                str = str.Replace("&&&", offset.ToString());
                writer.Append(str);
            }
            else
            {
                string str = ObjectListSave.Replace("---", field.Name);
                str = str.Replace("&&&", offset.ToString());
                writer.Append(str);
            }
        }
    }
    static int LoadValueType(FieldInfo field, StringBuilder loader, int offset)
    {
        var type = field.FieldType;
        string name = type.Name;
        switch (name)
        {
            case "Byte":
                string str = "            --- = (byte)fake[&&&];\r\n".Replace("---",field.Name);
                str = str.Replace("&&&",offset.ToString());
                loader.Append(str);
                break;
            case "SByte":
                str = "            --- = (sbyte)fake[&&&];\r\n".Replace("---", field.Name);
                str = str.Replace("&&&", offset.ToString());
                loader.Append(str);
                break;
            case "Int16":
                str = "            --- = (Int16)fake[&&&];\r\n".Replace("---", field.Name);
                str = str.Replace("&&&", offset.ToString());
                loader.Append(str);
                break;
            case "UInt16":
                str = "            --- = (UInt16)fake[&&&];\r\n".Replace("---", field.Name);
                str = str.Replace("&&&", offset.ToString());
                loader.Append(str);
                break;
            case "Char":
                str = "            --- = (Char)fake[&&&];\r\n".Replace("---", field.Name);
                str = str.Replace("&&&", offset.ToString());
                loader.Append(str);
                break;
            case "Boolean":
                str = "            --- =  = fake[&&&] == 1;\r\n".Replace("---", field.Name);
                str = str.Replace("&&&", offset.ToString());
                loader.Append(str);
                break;
            case "Int32":
                str = "            --- = fake[&&&];\r\n".Replace("---", field.Name);
                str = str.Replace("&&&", offset.ToString());
                loader.Append(str);
                break;
            case "UInt32":
                str = "            --- = (UInt32)fake[&&&];\r\n".Replace("---", field.Name);
                str = str.Replace("&&&", offset.ToString());
                loader.Append(str);
                break;
            case "Single":
                str = "            --- = fake.GetFloat(&&&);\r\n".Replace("---", field.Name);
                str = str.Replace("&&&", offset.ToString());
                loader.Append(str);
                break;   
            case "Int64":
                str = "            --- = fake.GetInt64(&&&);\r\n".Replace("---", field.Name);
                str = str.Replace("&&&", offset.ToString());
                loader.Append(str);
                return 2;
            case "Double":
                str = "            --- = fake.GetDouble(&&&);\r\n".Replace("---", field.Name);
                str = str.Replace("&&&", offset.ToString());
                loader.Append(str);
                return 2;
            case "UInt64":
                str = "            --- = (UInt64)fake.GetInt64(&&&);\r\n".Replace("---", field.Name);
                str = str.Replace("&&&", offset.ToString());
                loader.Append(str);
                return 2;
            case "Decimal":
                str = "            --- = fake.GetDecimal(&&&);\r\n".Replace("---", field.Name);
                str = str.Replace("&&&", offset.ToString());
                loader.Append(str);
                return 4;
            case "Vector2":
                str = "            --- = DBStructProc.LoadVector2(fake, &&&);\r\n".Replace("---", field.Name);
                str = str.Replace("&&&", offset.ToString());
                loader.Append(str);
                return 2;
            case "Vector3":
                str = "            --- = DBStructProc.LoadVector3(fake, &&&);\r\n".Replace("---", field.Name);
                str = str.Replace("&&&", offset.ToString());
                loader.Append(str);
                return 3;
            case "Vector4":
                str = "            --- = DBStructProc.LoadVector4(fake, &&&);\r\n".Replace("---", field.Name);
                str = str.Replace("&&&", offset.ToString());
                loader.Append(str);
                return 4;
            case "Quaternion":
                str = "            --- = DBStructProc.LoadQuaternion(fake, &&&);\r\n".Replace("---", field.Name);
                str = str.Replace("&&&", offset.ToString());
                loader.Append(str);
                return 4;
            case "Color":
                str = "            --- = DBStructProc.LoadColor(fake, &&&);\r\n".Replace("---", field.Name);
                str = str.Replace("&&&", offset.ToString());
                loader.Append(str);
                return 4;
            case "Color32":
                str = "            --- = DBStructProc.LoadColor32(fake, &&&);\r\n".Replace("---", field.Name);
                str = str.Replace("&&&", offset.ToString());
                loader.Append(str);
                return 1;
            default://这是一个结构体

                //Analysis(type);

                break;
        }
        return 1;
    }
    static void SaveValueType(FieldInfo field, StringBuilder writer, int offset)
    {
        var type = field.FieldType;
        string name = type.Name;
        switch (name)
        {
            case "Byte":
            case "SByte":
            case "Int16":
            case "UInt16":
            case "Char":
            case "Int32":
                string str = "            fake[&&&]= ---;\r\n".Replace("---", field.Name);
                str = str.Replace("&&&", offset.ToString());
                writer.Append(str);
                break;
            case "Boolean":
                str = "            if (---)\r\n                fake[&&&] = 1;\r\n".Replace("---", field.Name);
                str = str.Replace("&&&", offset.ToString());
                writer.Append(str);
                break;
            case "UInt32":
                str = "            fake[&&&] = (int)---;\r\n".Replace("---", field.Name);
                str = str.Replace("&&&", offset.ToString());
                writer.Append(str);
                break;
            case "Single":
                str = "            fake.SetFloat(&&&, ---);\r\n".Replace("---", field.Name);
                str = str.Replace("&&&", offset.ToString());
                writer.Append(str);
                break;
            case "Int64":
                str = "            fake.SetInt64(&&&, ---);\r\n".Replace("---", field.Name);
                str = str.Replace("&&&", offset.ToString());
                writer.Append(str);
                break;
            case "Double":
                str = "            fake.SetDouble(&&&, ---);\r\n".Replace("---", field.Name);
                str = str.Replace("&&&", offset.ToString());
                writer.Append(str);
                break;
            case "UInt64":
                str = "            fake.SetInt64(&&&, (Int64)---);\r\n".Replace("---", field.Name);
                str = str.Replace("&&&", offset.ToString());
                writer.Append(str);
                break;
            case "Decimal":
                str = "            fake.SetDecimal(&&&, ---);\r\n".Replace("---", field.Name);
                str = str.Replace("&&&", offset.ToString());
                writer.Append(str);
                break;
            case "Vector2":
            case "Vector3":
            case "Vector4":
            case "Quaternion":
            case "Color":
            case "Color32":
                str = "            ---.Save(fake, &&&);\r\n".Replace("---", field.Name);
                str = str.Replace("&&&", offset.ToString());
                writer.Append(str);
                break;
            default://这是一个结构体
                break;
        }
    }
}
