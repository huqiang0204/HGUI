using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace huqiang.Data
{
    public class JsonExtand
    {
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
