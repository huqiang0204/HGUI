using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace huqiang.Data
{
    /// <summary>
    /// 用于解决unity自带json无法直接序列化数组对象的问题
    /// </summary>
    public class JsonExtand
    {
        /// <summary>
        /// 将数组对象转换成json串
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static string ToJson(object[] array)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('[');
            int l = array.Length;
            for (int i = 0; i < l; i++)
            {
                if (i > 0)
                    sb.Append(',');
                sb.Append(JsonUtility.ToJson(array[i]));
            }
            sb.Append(']');
            return sb.ToString();
        }
        /// <summary>
        /// 将json串转换成数组对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonArray"></param>
        /// <returns></returns>
        public static T[] FromJson<T>(string jsonArray) where T : class, new()
        {
            string str = "{\"d\":" + jsonArray + "}";
            Vessel<T> vessel = JsonUtility.FromJson<Vessel<T>>(str);
            return vessel.d;
        }
    }
    [Serializable]
    class Vessel<T> where T : class, new()
    {
        public T[] d;
    }
}
