using System;
using System.Collections.Generic;
using Newtonsoft;
using Newtonsoft.Json.Linq;
using UnityEngine;
using SerializedData;
using huqiang.UIComposite;

namespace SerializedData
{
     public partial class TestA
     {
        public void Load(JObject jo)
        {
            JArray ja_a= jo["a"] as JArray;
            if (ja_a != null)
            {
                int _tc = ja_a.Count;
                if (_tc > 0)
                {
                    a = new Int32[_tc];
                    for (int j = 0; j<_tc; j++)
                    {
                        var tk = ja_a[j] as JValue;
                        if (tk != null)
                        {
                             a[j] = (Int32)tk.Value;
                        }
                    }
                }
            }
            JArray ja_b= jo["b"] as JArray;
            if (ja_b != null)
            {
                int _tc = ja_b.Count;
                if (_tc > 0)
                {
                    b = new String[_tc];
                    for (int j = 0; j<_tc; j++)
                    {
                        var tk = ja_b[j] as JValue;
                        if (tk != null)
                        {
                            b[j] = tk.Value as String;
                        }
                    }
                }
            }
            JArray ja_c= jo["c"] as JArray;
            if (ja_c != null)
            {
                int _tc = ja_c.Count;
                if (_tc > 0)
                {
                    c = new TestB[_tc];
                    for (int j = 0; j<_tc; j++)
                    {
                        var tk = ja_c[j] as JObject;
                        if (tk != null)
                        {
                            var to = new TestB();
                            to.Load(tk);
                            c[j] = to;
                        }
                    }
                }
            }
            JArray ja_d = jo["d"] as JArray;
            if (ja_d != null)
            {
                int _tc = ja_d.Count;
                if (_tc > 0)
                {
                    d = new List<Int32>();
                    for (int j = 0; j<_tc; j++)
                    {
                        var tk = ja_d[j] as JValue;
                        if (tk != null)
                        {
                            d.Add((Int32)tk.Value);
                        }
                    }
                }
            }
            JArray ja_e = jo["e"] as JArray;
            if (ja_e != null)
            {
                int _tc = ja_e.Count;
                if (_tc > 0)
                {
                    e = new List<String>();
                    for (int j = 0; j<_tc; j++)
                    {
                        var tk = ja_e[j] as JValue;
                        if (tk != null)
                        {
                            e.Add(tk.Value as String);
                        }
                    }
                }
            }
            JArray ja_f = jo["f"] as JArray;
            if (ja_f != null)
            {
                int _tc = ja_f.Count;
                if (_tc > 0)
                {
                    f = new List<TestB>();
                    for (int j = 0; j<_tc; j++)
                    {
                        var tk = ja_f[j] as JValue;
                        if (tk != null)
                        {
                            var to = new TestB();
                            to.Load(tk.Value as JObject);
                            f.Add(to);
                        }
                    }
                }
            }
            var jv_scroll = jo["scroll"] as JValue;
            if (jv_scroll != null)
            {
                if(jv_scroll.HasValues)
                {
                    scroll = (ScrollType)((int) jv_scroll.Value);
                }
            }
            JArray ja_v2= jo["v2"] as JArray;
            if (ja_v2 != null)
            {
                int _tc = ja_v2.Count;
                if (_tc > 0)
                {
                    v2 = new Vector2[_tc];
                    for (int j = 0; j<_tc; j++)
                    {
                        var tk = ja_v2[j] as JObject;
                        if (tk != null)
                        {
                             v2[j] = JsonStructProc.ReadVector2(tk);
                        }
                    }
                }
            }
            JArray ja_v3= jo["v3"] as JArray;
            if (ja_v3 != null)
            {
                int _tc = ja_v3.Count;
                if (_tc > 0)
                {
                    v3 = new Vector3[_tc];
                    for (int j = 0; j<_tc; j++)
                    {
                        var tk = ja_v3[j] as JObject;
                        if (tk != null)
                        {
                             v3[j] = JsonStructProc.ReadVector3(tk);
                        }
                    }
                }
            }
            JArray ja_v4 = jo["v4"] as JArray;
            if (ja_v4 != null)
            {
                int _tc = ja_v4.Count;
                if (_tc > 0)
                {
                    v4 = new List<Vector4>();
                    for (int j = 0; j<_tc; j++)
                    {
                        var tk = ja_v4[j] as JValue;
                        if (tk != null)
                        {
                            v4.Add(JsonStructProc.ReadVector4(tk.Value as JObject));
                        }
                    }
                }
            }
        }
        public JObject Save()
        {
            JObject jo = new JObject();
            if (a != null)
            {
                int _tc = a.Length;
                if (_tc > 0)
                {
                    JArray ja_a = new JArray();
                    for (int i = 0; i < _tc; i++)
                    {
                        ja_a.Add(new JValue(a[i]));
                    }
                    jo.Add("a", ja_a);
                }
            }
            if (b != null)
            {
                int _tc = b.Length;
                if (_tc > 0)
                {
                    JArray ja_b = new JArray();
                    for (int i = 0; i < _tc; i++)
                    {
                        ja_b.Add(new JValue(b[i]));
                    }
                    jo.Add("b", ja_b);
                }
            }
            if (c != null)
            {
                int _tc = c.Length;
                if (_tc > 0)
                {
                    JArray ja_c = new JArray();
                    for (int i = 0; i < _tc; i++)
                    {
                        var obj = c[i];
                        if(obj!=null)
                           ja_c.Add(new JValue(obj.Save()));
                    }
                    jo.Add("c", ja_c);
                }
            }
            if (d != null)
            {
                int _tc = d.Count;
                if (_tc > 0)
                {
                    JArray ja_d = new JArray();
                    for (int i = 0; i < _tc; i++)
                    {
                        ja_d.Add(new JValue(d[i]));
                    }
                    jo.Add("d", ja_d);
                }
            }
            if (e != null)
            {
                int _tc = e.Count;
                if (_tc > 0)
                {
                    JArray ja_e = new JArray();
                    for (int i = 0; i < _tc; i++)
                    {
                        ja_e.Add(new JValue(e[i]));
                    }
                    jo.Add("e", ja_e);
                }
            }
            if (f != null)
            {
                int _tc = f.Count;
                if (_tc > 0)
                {
                    JArray ja_f = new JArray();
                    for (int i = 0; i < _tc; i++)
                    {
                        var obj = f[i];
                        if(obj!=null)
                           ja_f.Add(new JValue(obj.Save()));
                    }
                    jo.Add("f", ja_f);
                }
            }
            jo.Add("scroll", new JValue((int)scroll));
            if (v2 != null)
            {
                int _tc = v2.Length;
                if (_tc > 0)
                {
                    JArray ja_v2 = new JArray();
                    for (int i = 0; i < _tc; i++)
                    {
                        ja_v2.Add(JsonStructProc.WriteVector2(v2[i]));
                    }
                    jo.Add("v2", ja_v2);
                }
            }
            if (v3 != null)
            {
                int _tc = v3.Length;
                if (_tc > 0)
                {
                    JArray ja_v3 = new JArray();
                    for (int i = 0; i < _tc; i++)
                    {
                        ja_v3.Add(JsonStructProc.WriteVector3(v3[i]));
                    }
                    jo.Add("v3", ja_v3);
                }
            }
            if (v4 != null)
            {
                int _tc = v4.Count;
                if (_tc > 0)
                {
                    JArray ja_v4 = new JArray();
                    for (int i = 0; i < _tc; i++)
                    {
                        ja_v4.Add(new JValue(v4[i]));
                    }
                    jo.Add("v4", ja_v4);
                }
            }
            return jo;
        }
     }
}