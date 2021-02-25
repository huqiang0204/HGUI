using System;
using System.Collections.Generic;
using System.Text;

namespace huqiang.Data
{
    /// <summary>
    /// 普通键值对
    /// </summary>
    public class KeyValue
    {
        public string key;
        public string value;
    }
    /// <summary>
    /// ini数组数据
    /// </summary>
    public class IniMate
    {
        public string index;
        public List<KeyValue> values = new List<KeyValue>();
    }

    /// <summary>
    /// ini节点
    /// </summary>
    public class INISection
    {
        /// <summary>
        /// 节点名称
        /// </summary>
        public string Name;
        /// <summary>
        /// 键值对数据集合
        /// </summary>
        public List<KeyValue> Values = new List<KeyValue>();
        /// <summary>
        /// 数组数据集合
        /// </summary>
        public List<IniMate> Mates = new List<IniMate>();
        /// <summary>
        /// 添加一组数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
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
        /// <summary>
        /// 添加数组的值
        /// </summary>
        /// <param name="index">元素索引</param>
        /// <param name="key"></param>
        /// <param name="value"></param>
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
        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public string GetValue(string key)
        {
            for (int i = 0; i < Values.Count; i++)
                if (key == Values[i].key)
                    return Values[i].value;
            return null;
        }
    }
}
