using System;
using System.Collections.Generic;
using Newtonsoft;
using Newtonsoft.Json.Linq;
using UnityEngine;
using SerializedData;

namespace SerializedData
{
     public partial class TestB
     {
        public void Load(JObject jo)
        {
            var jv_a = jo["a"] as JValue;
            if(jv_a != null)
            {
                a = jv_a.Value<Byte>();
            }
            var jv_b = jo["b"] as JValue;
            if(jv_b != null)
            {
                b = jv_b.Value<Int16>();
            }
            var jv_c = jo["c"] as JValue;
            if(jv_c != null)
            {
                c = jv_c.Value<Int32>();
            }
            var jv_d = jo["d"] as JValue;
            if(jv_d != null)
            {
                d = jv_d.Value<Int64>();
            }
            var jv_e = jo["e"] as JValue;
            if(jv_e != null)
            {
                e = jv_e.Value<Single>();
            }
            var jv_f = jo["f"] as JValue;
            if(jv_f != null)
            {
                f = jv_f.Value<Double>();
            }
            var jv_g = jo["g"] as JValue;
            if(jv_g != null)
            {
                g = jv_g.Value as String;
            }
            var jo_h = jo["h"] as JObject;
            if(jo_h!=null)
            {
                if(jo_h.HasValues)
                {
                    h = new TestA();
                    h.Load(jo_h);
                }
            }
            var jo_v = jo["v"] as JObject;
            if (jo_v != null)
            {
                v = JsonStructProc.ReadVector2(jo_v);
            }
            var jo_v3 = jo["v3"] as JObject;
            if (jo_v3 != null)
            {
                v3 = JsonStructProc.ReadVector3(jo_v3);
            }
            var jo_v4 = jo["v4"] as JObject;
            if (jo_v4 != null)
            {
                v4 = JsonStructProc.ReadVector4(jo_v4);
            }
            var jo_col = jo["col"] as JObject;
            if (jo_col != null)
            {
                col = JsonStructProc.ReadColor(jo_col);
            }
            var jo_col32 = jo["col32"] as JObject;
            if (jo_col32 != null)
            {
                col32 = JsonStructProc.ReadColor32(jo_col32);
            }
            var jo_q = jo["q"] as JObject;
            if (jo_q != null)
            {
                q = JsonStructProc.ReadQuaternion(jo_q);
            }
        }
        public JObject Save()
        {
            JObject jo = new JObject();
            jo.Add("a", new JValue(a));
            jo.Add("b", new JValue(b));
            jo.Add("c", new JValue(c));
            jo.Add("d", new JValue(d));
            jo.Add("e", new JValue(e));
            jo.Add("f", new JValue(f));
            jo.Add("g", new JValue(g));
            if (h != null)
                jo.Add("h", h.Save());
            jo.Add("v", JsonStructProc.WriteVector2(v));
            jo.Add("v3", JsonStructProc.WriteVector3(v3));
            jo.Add("v4", JsonStructProc.WriteVector4(v4));
            jo.Add("col", JsonStructProc.WriteColor(col));
            jo.Add("col32", JsonStructProc.WriteColor32(col32));
            jo.Add("q", JsonStructProc.WriteQuaternion(q));
            return jo;
        }
     }
}