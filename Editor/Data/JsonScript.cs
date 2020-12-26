using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

public class JsonScript : Editor
{
    [Serializable]
    class Asmdef
    {
        public string name;
        public string[] references;
        public bool allowUnsafeCode;
    }
    [MenuItem("Assets/ExportCS/Json")]
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
            string p = dic + "\\" + path + "\\JsonData";
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
                        CreateJsonScript(type, dic);
                }
            }
            string cs = p + "\\JsonStructProc.cs";
            if (!File.Exists(cs))
                File.WriteAllText(cs, StringAsset.StructProcCS);
            AssetDatabase.Refresh();
        }
    }
    static string claStart = @"
namespace SerializedData
{
     public partial class ";

    public static void CreateJsonScript(Type type, string dic)
    {
        StringBuilder sb = new StringBuilder();

        var fs = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
        List<string> nps = new List<string>();
        nps.Add("System");
        nps.Add("System.Collections.Generic");
        nps.Add("Newtonsoft");
        nps.Add("Newtonsoft.Json.Linq");
        nps.Add("UnityEngine");
        StringBuilder loader = new StringBuilder();
        StringBuilder writer = new StringBuilder();
        writer.Append("            JObject jo = new JObject();\r\n");
        if (fs != null)
        {
            for (int j = 0; j < fs.Length; j++)
            {
                Analysis(fs[j], loader, writer, nps);
            }
        }
        writer.Append("            return jo;\r\n");
        for (int i = 0; i < nps.Count; i++)
        {
            sb.Append("using ");
            sb.Append(nps[i]);
            sb.Append(";\r\n");
        }
        sb.Append(claStart);
        sb.Append(type.Name);
        sb.Append("\r\n     {\r\n        public void Load(JObject jo)\r\n        {\r\n");
        sb.Append(loader);
        sb.Append("        }\r\n        public JObject Save()\r\n        {\r\n");
        if (writer.Length == 0)
            sb.Append("            return null;\r\n");
        else sb.Append(writer);
        sb.Append("        }\r\n     }\r\n}");
        string path = dic + "/" + type.Name + ".cs";
        if (File.Exists(path))
            File.Delete(path);
        File.WriteAllText(path, sb.ToString());
    }
    static string LoadString = @"            var jv_--- = jo[""---""] as JValue;
            if(jv_--- != null)
            {
                --- = jv_---.Value as String;
            }
";
    static string SaveObject = @"            if (--- != null)
                jo.Add(""---"", ---.Save());
";
    static string LoadObject = @"            var jo_--- = jo[""---""] as JObject;
            if(jo_---!=null)
            {
                if(jo_---.HasValues)
                {
                    --- = new +++();
                    ---.Load(jo_---);
                }
            }
";
    static string LoadEnum = @"            var jv_--- = jo[""---""] as JValue;
            if (jv_--- != null)
            {
                if(jv_---.HasValues)
                {
                    --- = (+++)((int) jv_---.Value);
                }
            }
";
    static string SaveEnum = "            jo.Add(\"---\", new JValue((int)---));\r\n";
    static void Analysis(FieldInfo field, StringBuilder loader, StringBuilder writer, List<string> nps)
    {
        var type = field.FieldType;
        if (type.IsArray)
        {
            LoadArrayType(field, loader);
            SaveArrayType(field, writer);
            var son = type.GetElementType();
            string ss = son.Namespace;
            if (!nps.Contains(ss))
                nps.Add(ss);
        }
        else if (type.IsClass)
        {
            if (type.Name == "String")
            {
                string str = LoadString.Replace("---", field.Name);
                loader.Append(str);
                str = SaveValue.Replace("---", field.Name);
                writer.Append(str);
                return;
            }
            else if (type.IsGenericType)//泛型类
            {
                if (type.Name == "List`1")
                {
                    LoadListType(field, loader);
                    SaveListType(field, writer);
                    var tps = type.GetGenericArguments();
                    var son = tps[0];
                    string ss = son.Namespace;
                    if (!nps.Contains(ss))
                        nps.Add(ss);
                    return;
                }
            }
            string to = LoadObject.Replace("---", field.Name);
            to = to.Replace("+++", type.Name);
            loader.Append(to);
            to = SaveObject.Replace("---", field.Name);
            writer.Append(to);
            string ns = type.Namespace;
            if (!nps.Contains(ns))
                nps.Add(ns);
        }
        else if (type.IsEnum)
        {
            string str = LoadEnum.Replace("---", field.Name);
            str = str.Replace("+++", type.Name);
            loader.Append(str);
            writer.Append(SaveEnum.Replace("---", field.Name));
            string ns = type.Namespace;
            if (!nps.Contains(ns))
                nps.Add(ns);
        }
        else if (type.IsValueType)
        {
            LoadValueType(field, loader);
            SaveValueType(field, writer);
        }
    }
    static string LoadValue = @"            var jv_--- = jo[""---""] as JValue;
            if(jv_--- != null)
            {
                --- = jv_---.Value<+++>();
            }
";
    static string LoadStruct = @"            var jo_--- = jo[""---""] as JObject;
            if (jo_--- != null)
            {
                --- = JsonStructProc.+++(jo_---);
            }
";
    static void LoadValueType(FieldInfo field, StringBuilder loader)
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
            case "Boolean":
            case "Int32":
            case "UInt32":
            case "Single":
            case "Int64":
            case "Double":
            case "UInt64":
            case "Decimal":
                string str = LoadValue.Replace("---", field.Name);
                str = str.Replace("+++", name);
                loader.Append(str);
                break;
            case "Vector2":
                string a = LoadStruct.Replace("---", field.Name);
                a = a.Replace("+++", "ReadVector2");
                loader.Append(a);
                break;
            case "Vector3":
                a = LoadStruct.Replace("---", field.Name);
                a = a.Replace("+++", "ReadVector3");
                loader.Append(a);
                break;
            case "Vector4":
                a = LoadStruct.Replace("---", field.Name);
                a = a.Replace("+++", "ReadVector4");
                loader.Append(a);
                break;
            case "Quaternion":
                a = LoadStruct.Replace("---", field.Name);
                a = a.Replace("+++", "ReadQuaternion");
                loader.Append(a);
                break;
            case "Color":
                a = LoadStruct.Replace("---", field.Name);
                a = a.Replace("+++", "ReadColor");
                loader.Append(a);
                break;
            case "Color32":
                a = LoadStruct.Replace("---", field.Name);
                a = a.Replace("+++", "ReadColor32");
                loader.Append(a);
                break;
            default://这是一个结构体

                //Analysis(type);

                break;
        }

    }
    static string SaveValue = "            jo.Add(\"---\", new JValue(---));\r\n";
    static string SaveStruct = "            jo.Add(\"---\", JsonStructProc.+++(---));\r\n";
    static void SaveValueType(FieldInfo field, StringBuilder writer)
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
            case "Boolean":
            case "Int32":
            case "UInt32":
            case "Single":
            case "Int64":
            case "Double":
            case "UInt64":
            case "Decimal":
                string str = SaveValue.Replace("---", field.Name);
                writer.Append(str);
                break;
            case "Vector2":
                string a = SaveStruct.Replace("---", field.Name);
                a = a.Replace("+++", "WriteVector2");
                writer.Append(a);
                break;
            case "Vector3":
                a = SaveStruct.Replace("---", field.Name);
                a = a.Replace("+++", "WriteVector3");
                writer.Append(a);
                break;
            case "Vector4":
                a = SaveStruct.Replace("---", field.Name);
                a = a.Replace("+++", "WriteVector4");
                writer.Append(a);
                break;
            case "Color":
                a = SaveStruct.Replace("---", field.Name);
                a = a.Replace("+++", "WriteColor");
                writer.Append(a);
                break;
            case "Color32":
                a = SaveStruct.Replace("---", field.Name);
                a = a.Replace("+++", "WriteColor32");
                writer.Append(a);
                break;
            case "Quaternion":
                a = SaveStruct.Replace("---", field.Name);
                a = a.Replace("+++", "WriteQuaternion");
                writer.Append(a);
                break;

            default://这是一个结构体

                //Analysis(type);

                break;
        }
    }
    static string LoadArrayStart = @"            JArray ja_---= jo[""---""] as JArray;
            if (ja_--- != null)
            {
                int _tc = ja_---.Count;
                if (_tc > 0)
                {
                    --- = new +++[_tc];
                    for (int j = 0; j<_tc; j++)
                    {
";
    static string ArrayValueEnd = @"                        var tk = ja_---[j] as JValue;
                        if (tk != null)
                        {
                             ---[j] = (Int32)tk.Value;
                        }
                    }
                }
            }
";
    static string ArrayStructEnd = @"                        var tk = ja_---[j] as JObject;
                        if (tk != null)
                        {
                             ---[j] = JsonStructProc.Read+++(tk);
                        }
                    }
                }
            }
";
    static string ArrayStringEnd = @"                        var tk = ja_---[j] as JValue;
                        if (tk != null)
                        {
                            ---[j] = tk.Value as String;
                        }
                    }
                }
            }
";
    static string ArrayObjectEnd = @"                        var tk = ja_---[j] as JObject;
                        if (tk != null)
                        {
                            var to = new +++();
                            to.Load(tk);
                            ---[j] = to;
                        }
                    }
                }
            }
";
    static void LoadArrayType(FieldInfo field, StringBuilder loader)
    {
        var type = field.FieldType;
        var son = type.GetElementType();
        string str = LoadArrayStart.Replace("---", field.Name);
        str = str.Replace("+++", son.Name);
        loader.Append(str);
        if (son.IsValueType)
        {
            switch (son.Name)
            {
                case "Byte":
                case "SByte":
                case "Int16":
                case "UInt16":
                case "Char":
                case "Boolean":
                case "Int32":
                case "UInt32":
                case "Single":
                case "Int64":
                case "Double":
                case "UInt64":
                case "Decimal":
                    loader.Append(ArrayValueEnd.Replace("---", field.Name));
                    break;
                case "Vector2":
                case "Vector3":
                case "Vector4":
                case "Color":
                case "Color32":
                case "Quaternion":
                    loader.Append(ArrayStructEnd.Replace("---", field.Name).Replace("+++",son.Name));
                    break;

                default://这是一个结构体

                    //Analysis(type);

                    break;
            }
            
        }
        else
        {
            if (son.Name == "String")
            {
                loader.Append(ArrayStringEnd.Replace("---", field.Name));
            }
            else
            {
                str = ArrayObjectEnd.Replace("---", field.Name);
                str = str.Replace("+++", son.Name);
                loader.Append(str);
            }
        }
    }
    static string saveValueArray = @"            if (--- != null)
            {
                int _tc = ---.Length;
                if (_tc > 0)
                {
                    JArray ja_--- = new JArray();
                    for (int i = 0; i < _tc; i++)
                    {
                        ja_---.Add(new JValue(---[i]));
                    }
                    jo.Add(""---"", ja_---);
                }
            }
";
    static string saveStructArray = @"            if (--- != null)
            {
                int _tc = ---.Length;
                if (_tc > 0)
                {
                    JArray ja_--- = new JArray();
                    for (int i = 0; i < _tc; i++)
                    {
                        ja_---.Add(JsonStructProc.Write+++(---[i]));
                    }
                    jo.Add(""---"", ja_---);
                }
            }
";
    static string saveObjArray = @"            if (--- != null)
            {
                int _tc = ---.Length;
                if (_tc > 0)
                {
                    JArray ja_--- = new JArray();
                    for (int i = 0; i < _tc; i++)
                    {
                        var obj = ---[i];
                        if(obj!=null)
                           ja_---.Add(new JValue(obj.Save()));
                    }
                    jo.Add(""---"", ja_---);
                }
            }
";
    static void SaveArrayType(FieldInfo field, StringBuilder writer)
    {
        var type = field.FieldType;
        var son = type.GetElementType();
        if (son.IsValueType)
        {
            switch (son.Name)
            {
                case "Byte":
                case "SByte":
                case "Int16":
                case "UInt16":
                case "Char":
                case "Boolean":
                case "Int32":
                case "UInt32":
                case "Single":
                case "Int64":
                case "Double":
                case "UInt64":
                case "Decimal":
                    writer.Append(saveValueArray.Replace("---", field.Name));
                    break;
                case "Vector2":
                case "Vector3":
                case "Vector4":
                case "Color":
                case "Color32":
                case "Quaternion":
                    writer.Append(saveStructArray.Replace("---", field.Name).Replace("+++", son.Name));
                    break;
                default://这是一个结构体
                    break;
            }
        }
        else
        {
            if (son.Name == "String")
            {
                writer.Append(saveValueArray.Replace("---", field.Name));
            }
            else
            {
                writer.Append(saveObjArray.Replace("---", field.Name));
            }
        }
    }
    static string LoadListStart = @"            JArray ja_--- = jo[""---""] as JArray;
            if (ja_--- != null)
            {
                int _tc = ja_---.Count;
                if (_tc > 0)
                {
                    --- = new List<+++>();
                    for (int j = 0; j<_tc; j++)
                    {
                        var tk = ja_---[j] as JValue;
                        if (tk != null)
                        {
";
    static string ListValueEnd = @"                            ---.Add((Int32)tk.Value);
                        }
                    }
                }
            }
";
    static string ListStructEnd = @"                            ---.Add(JsonStructProc.Read+++(tk.Value as JObject));
                        }
                    }
                }
            }
";
    static string ListStringEnd = @"                            ---.Add(tk.Value as String);
                        }
                    }
                }
            }
";
    static string ListObjectEnd = @"                            var to = new +++();
                            to.Load(tk.Value as JObject);
                            ---.Add(to);
                        }
                    }
                }
            }
";
    static void LoadListType(FieldInfo field, StringBuilder loader)
    {
        var type = field.FieldType;
        var tps = type.GetGenericArguments();
        var son = tps[0];
        string str = LoadListStart.Replace("---", field.Name);
        str = str.Replace("+++", son.Name);
        loader.Append(str);
        if (son.IsValueType)
        {
            switch (son.Name)
            {
                case "Byte":
                case "SByte":
                case "Int16":
                case "UInt16":
                case "Char":
                case "Boolean":
                case "Int32":
                case "UInt32":
                case "Single":
                case "Int64":
                case "Double":
                case "UInt64":
                case "Decimal":
                    loader.Append(ListValueEnd.Replace("---", field.Name));
                    break;
                case "Vector2":
                case "Vector3":
                case "Vector4":
                case "Color":
                case "Color32":
                case "Quaternion":
                    loader.Append(ListStructEnd.Replace("---", field.Name).Replace("+++", son.Name));
                    break;
                default://这是一个结构体
                    break;
            }
        }
        else
        {
            if (son.Name == "String")
            {
                loader.Append(ListStringEnd.Replace("---", field.Name));
            }
            else
            {
                str = ListObjectEnd.Replace("---", field.Name);
                str = str.Replace("+++", son.Name);
                loader.Append(str);
            }
        }
    }
    static string saveValueList = @"            if (--- != null)
            {
                int _tc = ---.Count;
                if (_tc > 0)
                {
                    JArray ja_--- = new JArray();
                    for (int i = 0; i < _tc; i++)
                    {
                        ja_---.Add(new JValue(---[i]));
                    }
                    jo.Add(""---"", ja_---);
                }
            }
";
    static string saveObjList = @"            if (--- != null)
            {
                int _tc = ---.Count;
                if (_tc > 0)
                {
                    JArray ja_--- = new JArray();
                    for (int i = 0; i < _tc; i++)
                    {
                        var obj = ---[i];
                        if(obj!=null)
                           ja_---.Add(new JValue(obj.Save()));
                    }
                    jo.Add(""---"", ja_---);
                }
            }
";
    static void SaveListType(FieldInfo field, StringBuilder writer)
    {
        var type = field.FieldType;
        var tps = type.GetGenericArguments();
        var son = tps[0];
        if (son.IsValueType)
        {
            writer.Append(saveValueList.Replace("---", field.Name));
        }
        else
        {
            if (son.Name == "String")
            {
                writer.Append(saveValueList.Replace("---", field.Name));
            }
            else
            {
                writer.Append(saveObjList.Replace("---", field.Name));
            }
        }
    }
}
