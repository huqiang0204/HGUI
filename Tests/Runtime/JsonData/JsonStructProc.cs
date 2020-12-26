using Newtonsoft.Json.Linq;
using UnityEngine;

public class JsonStructProc
{
    public static Vector2 ReadVector2(JObject jo)
    {
        Vector2 v= new Vector2();
        if (jo.HasValues)
        {
            var jv = jo["x"] as JValue;
            if (jv != null)
            {
                v.x = jv.Value<float>();
            }
            jv = jo["y"] as JValue;
            if (jv != null)
            {
                v.y = jv.Value<float>();
            }
        }
        return v;
    }
    public static Vector3 ReadVector3(JObject jo)
    {
        Vector3 v = new Vector3();
        if (jo.HasValues)
        {
            var jv = jo["x"] as JValue;
            if (jv != null)
            {
                v.x = jv.Value<float>();
            }
            jv = jo["y"] as JValue;
            if (jv != null)
            {
                v.y = jv.Value<float>();
            }
            jv = jo["z"] as JValue;
            if (jv != null)
            {
                v.z = jv.Value<float>();
            }
        }
        return v;
    }
    public static Vector4 ReadVector4(JObject jo)
    {
        Vector4 v = new Vector4();
        if (jo.HasValues)
        {
            var jv = jo["x"] as JValue;
            if (jv != null)
            {
                v.x = jv.Value<float>();
            }
            jv = jo["y"] as JValue;
            if (jv != null)
            {
                v.y = jv.Value<float>();
            }
            jv = jo["z"] as JValue;
            if (jv != null)
            {
                v.z = jv.Value<float>();
            }
            jv = jo["w"] as JValue;
            if (jv != null)
            {
                v.w = jv.Value<float>();
            }
        }
        return v;
    }
    public static Quaternion ReadQuaternion(JObject jo)
    {
        Quaternion v = new Quaternion();
        if (jo.HasValues)
        {
            var jv = jo["x"] as JValue;
            if (jv != null)
            {
                v.x = jv.Value<float>();
            }
            jv = jo["y"] as JValue;
            if (jv != null)
            {
                v.y = jv.Value<float>();
            }
            jv = jo["z"] as JValue;
            if (jv != null)
            {
                v.z = jv.Value<float>();
            }
            jv = jo["w"] as JValue;
            if (jv != null)
            {
                v.w = jv.Value<float>();
            }
        }
        return v;
    }
    public static Color ReadColor(JObject jo)
    {
        Color v = new Color();
        if (jo.HasValues)
        {
            var jv = jo["r"] as JValue;
            if (jv != null)
            {
                v.r = jv.Value<float>();
            }
            jv = jo["g"] as JValue;
            if (jv != null)
            {
                v.g = jv.Value<float>();
            }
            jv = jo["b"] as JValue;
            if (jv != null)
            {
                v.b = jv.Value<float>();
            }
            jv = jo["a"] as JValue;
            if (jv != null)
            {
                v.a = jv.Value<float>();
            }
        }
        return v;
    }
    public static Color32 ReadColor32(JObject jo)
    {
        Color32 v = new Color32();
        if (jo.HasValues)
        {
            var jv = jo["r"] as JValue;
            if (jv != null)
            {
                v.r = jv.Value<byte>();
            }
            jv = jo["g"] as JValue;
            if (jv != null)
            {
                v.g = jv.Value<byte>();
            }
            jv = jo["b"] as JValue;
            if (jv != null)
            {
                v.b = jv.Value<byte>();
            }
            jv = jo["a"] as JValue;
            if (jv != null)
            {
                v.a = jv.Value<byte>();
            }
        }
        return v;
    }
    public static JObject WriteVector2(Vector2 v)
    {
        JObject jo = new JObject();
        jo.Add("x", new JValue(v.x));
        jo.Add("y", new JValue(v.y));
        return jo;
    }
    public static JObject WriteVector3(Vector3 v)
    {
        JObject jo = new JObject();
        jo.Add("x", new JValue(v.x));
        jo.Add("y", new JValue(v.y));
        jo.Add("z", new JValue(v.z));
        return jo;
    }
    public static JObject WriteVector4(Vector4 v)
    {
        JObject jo = new JObject();
        jo.Add("x", new JValue(v.x));
        jo.Add("y", new JValue(v.y));
        jo.Add("z", new JValue(v.z));
        jo.Add("w", new JValue(v.w));
        return jo;
    }
    public static JObject WriteQuaternion(Quaternion v)
    {
        JObject jo = new JObject();
        jo.Add("x", new JValue(v.x));
        jo.Add("y", new JValue(v.y));
        jo.Add("z", new JValue(v.z));
        jo.Add("w", new JValue(v.w));
        return jo;
    }
    public static JObject WriteColor(Color v)
    {
        JObject jo = new JObject();
        jo.Add("r", new JValue(v.r));
        jo.Add("g", new JValue(v.g));
        jo.Add("b", new JValue(v.b));
        jo.Add("a", new JValue(v.a));
        return jo;
    }
    public static JObject WriteColor32(Color32 v)
    {
        JObject jo = new JObject();
        jo.Add("r", new JValue(v.r));
        jo.Add("g", new JValue(v.g));
        jo.Add("b", new JValue(v.b));
        jo.Add("a", new JValue(v.a));
        return jo;
    }
}