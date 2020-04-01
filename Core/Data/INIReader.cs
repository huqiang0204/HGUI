using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace huqiang.Data
{
    public class INIReader
    {
        List<INISection> sections = new List<INISection>();
        public string Name;
        public INISection FindSection(string sec)
        {
            for (int i = 0; i < sections.Count; i++)
                if (sections[i].name == sec)
                    return sections[i];
            return null;
        }
        public T Serializal<T>(string section) where T : class, new()
        {
            var ini = FindSection(section);
            if(ini!=null)
            {
                T t = new T();
                Serializal(typeof(T),t,ini);
                return t;
            }
            return null;
        }
        public void Serializal(Type typ, object t, INISection sec)
        {
            var fs = typ.GetFields();
            for (int i = 0; i < fs.Length; i++)
            {
                var f = fs[i];
                if (f.FieldType == typeof(int))
                {
                    int a = 0;
                    int.TryParse(sec.GetValue(f.Name), out a);
                    f.SetValue(t, a);
                }
                else if (f.FieldType == typeof(float))
                {
                    float a = 0;
                    float.TryParse(sec.GetValue(f.Name), out a);
                    f.SetValue(t, a);
                }
                else if (f.FieldType == typeof(long))
                {
                    long a = 0;
                    long.TryParse(sec.GetValue(f.Name), out a);
                    f.SetValue(t, a);
                }
                else if (f.FieldType == typeof(double))
                {
                    double a = 0;
                    double.TryParse(sec.GetValue(f.Name), out a);
                    f.SetValue(t, a);
                }
                else if (f.FieldType == typeof(string))
                {
                    f.SetValue(t, sec.GetValue(f.Name));
                }
                else
                {
                    var ini= FindSection(sec.GetValue(f.Name));
                    if(ini!=null)
                    {
                        var s =  Activator.CreateInstance(f.FieldType);
                        Serializal(f.FieldType,s,ini);
                        f.SetValue(t,s);
                    }
                }
            }
        }
        public void LoadFromFile(string path)
        {
            sections.Clear();
            if(File.Exists(path))
            {
                var dat = File.ReadAllBytes(path);
                var str= Encoding.UTF8.GetString(dat);
                str = str.Replace(" ","");
                var ss = str.Split('\r','\n');
                LoadData(ss);
            }
        }
        public void LoadFromFile(byte[] bytes)
        {
            sections.Clear();
            if (bytes!=null&&bytes.Length>0)
            {
                var str = Encoding.UTF8.GetString(bytes);

                var ss = str.Split('\r','\n');
                LoadData(ss);
            }
        }
        public void LoadData(string[] ss)
        {
            INISection sec = null;
            for (int i = 0; i < ss.Length; i++)
            {
                var str = ss[i];
                if(str!="")
                {
                    if (str[0] == '[')
                    {
                        var last = str.LastIndexOf(']');
                        var name = str.Substring(1, last - 1);
                        sec = new INISection();
                        sec.name = name;
                        sections.Add(sec);
                    }
                    else if (sec != null)
                    {
                        var kv = str.Split('=');
                        if (kv.Length > 1)
                        {
                            if (kv.Length > 2)
                            {
                                string t = "";
                                for (int j = 1; j < kv.Length - 1; j++)
                                {
                                    t += kv[j] + '=';
                                }
                                t += kv[kv.Length - 1];
                                sec.Add(kv[0], t);
                            }
                            else
                            {
                                sec.Add(kv[0], kv[1]);
                            }
                        }
                    }
                }
            }
        }
        public void RemoveSection(string name)
        {
            for(int i=0;i<sections.Count;i++)
                if(sections[i].name==name)
                {
                    sections.RemoveAt(i);
                    return;
                }
        }
        public void AddSection(INISection section)
        {
            sections.Add(section);
        }
        public void WriteToFile(string path)
        {
            if (File.Exists(path))
                File.Delete(path);
            StringBuilder sb = new StringBuilder();
            bool muti = false;
            for(int i=0;i<sections.Count;i++)
            {
                if (muti)
                    sb.Append("\r[");
                else sb.Append("[");
                sb.Append(sections[i].name);
                sb.Append("]");
                var v = sections[i].values;
                for(int j=0;j<v.Count;j++)
                {
                    var kv = v[j];
                    if (kv.value != null & kv.value != "")
                    {
                        sb.Append('\r');
                        sb.Append(kv.key);
                        sb.Append('=');
                        sb.Append(kv.value);
                    }
                }
                var m = sections[i].mates;
                for (int j = 0; j < m.Count; j++)
                {
                    var u = m[j];
                    v = u.values;
                    for(int k=0;k<v.Count;k++)
                    {
                        var kv = v[k];
                        if(kv.value!=null&kv.value!="")
                        {
                            sb.Append('\r');
                            sb.Append(u.index);
                            sb.Append('\\');
                            sb.Append(kv.key);
                            sb.Append('=');
                            sb.Append(kv.value);
                        }
                    }
                }
                muti = true;
            }
            byte[] dat = Encoding.UTF8.GetBytes(sb.ToString());
            File.WriteAllBytes(path, dat);
        }
    }
}