using System;
using System.Collections.Generic;
using System.Text;

namespace huqiang.Data
{
    public class KeyValue
    {
        public string key;
        public string value;
    }
    public class IniMate
    {
        public string index;
        public List<KeyValue> values = new List<KeyValue>();
    }
    public class INISection
    {
        public string Name;
        public List<KeyValue> Values = new List<KeyValue>();
        public List<IniMate> Mates = new List<IniMate>();
        public void Add(string key, string value)
        {
            var ss = key.Split('\\');
            if (ss.Length > 1)
            {
                Add(ss[0],ss[1],value);//数组
            }
            else
            {
                for (int i = 0; i < Values.Count; i++)
                    if (Values[i].key == key)
                    {
                        Values[i].value = value;
                        return;
                    }
                var kv = new KeyValue();
                kv.key = key;
                kv.value = value;
                Values.Add(kv);
            }
        }
        //添加数组的值
        public void Add(string index,string key, string value)
        {
            IniMate mate = null;
            for (int i = 0; i < Mates.Count; i++)
                if (Mates[i].index == index)
                {
                    mate = Mates[i];
                    break;
                }
            if (mate == null)
            {
                mate = new IniMate();
                mate.index = index;
                var kv = new KeyValue();
                kv.key = key;
                kv.value = value;

                mate.values.Add(kv);
                Mates.Add(mate);
            }
            else
            {
                var v = mate.values;
                for (int i = 0; i < v.Count; i++)
                    if (v[i].key == key)
                    {
                        v.RemoveAt(i);
                        break;
                    }
                var kv = new KeyValue();
                kv.key = key;
                kv.value = value;
                mate.values.Add(kv);
            }
        }
        public string GetValue(string key)
        {
            for (int i = 0; i < Values.Count; i++)
                if (key == Values[i].key)
                    return Values[i].value;
            return null;
        }
    }
}
