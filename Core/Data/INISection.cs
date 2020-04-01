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
        public string name;
        public List<KeyValue> values = new List<KeyValue>();
        public List<IniMate> mates = new List<IniMate>();
        public void Add(string key, string value)
        {
            var ss = key.Split('\\');
            if (ss.Length > 1)
            {
                Add(ss[0],ss[1],value);
            }
            else
            {
                for (int i = 0; i < values.Count; i++)
                    if (values[i].key == key)
                    {
                        values.RemoveAt(i);
                        break;
                    }
                var kv = new KeyValue();
                kv.key = key;
                kv.value = value;
                values.Add(kv);
            }
        }
        public void Add(string index,string key, string value)
        {
            IniMate mate = null;
            for (int i = 0; i < mates.Count; i++)
                if (mates[i].index == index)
                {
                    mate = mates[i];
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
                mates.Add(mate);
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
            for (int i = 0; i < values.Count; i++)
                if (key == values[i].key)
                    return values[i].value;
            return null;
        }
    }
}
