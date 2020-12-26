using System;
using System.Collections.Generic;
using huqiang.Data;
using UnityEngine;
using SerializedData;
using huqiang.UIComposite;

namespace SerializedData
{
     public partial class TestA
     {
        public void Load(FakeStruct fake)
        {
            a = fake.GetData<Int32[]>(0);
            var b_t = fake.GetData<FakeStringArray>(1);
            if (b_t != null)
                b = b_t.Data;
            var c_t = fake.GetData<Int16[]>(2);
            if (c_t != null)
            {
                c = new TestB[c_t.Length];
                for (int i = 0; i < c_t.Length; i++)
                {
                    if (c_t[i] >= 0)
                    {
                        c[i] = new TestB();
                        c[i].Load(fake.buffer.GetData(c_t[i]) as FakeStruct);
                    }
                }
            }
            var d_t = fake.GetData<Int32[]>(3);
            if (d_t != null)
            {
                d = new List<Int32>();
                d.AddRange(d_t); 
            }
            var e_t = fake.GetData<FakeStringArray>(4);
            if (e_t != null)
                e.AddRange(e_t.Data);
            var f_t = fake.GetData<Int16[]>(5);
            if (f_t != null)
            {
                f = new List<TestB>();
                for (int i = 0; i < f_t.Length; i++)
                {
                    if (f_t[i] >= 0)
                    {
                        var f_s = new TestB();
                        f_s.Load(fake.buffer.GetData(f_t[i]) as FakeStruct);
                        f.Add(f_s);
                    }
                }
            }
            scroll = (ScrollType)fake[6];
            v2 = fake.buffer.GetArray<Vector2>(fake[7]);
            v3 = fake.buffer.GetArray<Vector3>(fake[8]);
            var v4_t = fake.buffer.GetArray<Vector4>(fake[9]);
            if (v4 != null)
            {
                v4 = new List<Vector4>();
                v4.AddRange(v4_t);
            }
        }
        public FakeStruct Save(DataBuffer db)
        {
            FakeStruct fake = new FakeStruct(db, 10);
            fake[0] = db.AddData(a, DataType.Int32Array);
            if (b != null)
                fake[1] = db.AddData(new FakeStringArray(b), DataType.FakeStringArray);
            if (c != null)
            {
                if(c.Length>0)
                {
                    Int16[] c_b = new Int16[c.Length];
                    for (int i = 0; i < c.Length; i++)
                    {
                        var c_t = c[i];
                        if (c_t != null)
                        {
                            c_b[i] = (Int16)db.AddData(c_t.Save(db));
                        }
                    }
                    fake[2] = db.AddData(c_b, DataType.Int16Array);
                }
            }
            if (d != null)
                if (d.Count > 0)
                    fake[3] = db.AddData(d.ToArray(), DataType.Int32Array);
            if (e != null)
                if (e.Count > 0)
                    fake[4] = db.AddData(new FakeStringArray(e.ToArray()), DataType.FakeStringArray);
            if (f != null)
            {
                if(f.Count>0)
                {
                    Int16[] f_b = new Int16[f.Count];
                    for (int i = 0; i < f.Count; i++)
                    {
                        var f_t = f[i];
                        if (f_t != null)
                        {
                            f_b[i] = (Int16)db.AddData(f_t.Save(db));
                        }
                    }
                    fake[5] = db.AddData(f_b, DataType.Int16Array);
                }
            }
            fake[6] = (int)scroll;
            if (v2 != null)
                fake[7] = db.AddArray<Vector2>(v2);
            if (v3 != null)
                fake[8] = db.AddArray<Vector3>(v3);
            if (v4 != null)
                fake[9] = db.AddArray<Vector4>(v4.ToArray());
            return fake;
        }
     }
}