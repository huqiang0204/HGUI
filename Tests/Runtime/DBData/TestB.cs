using System;
using System.Collections.Generic;
using huqiang.Data;
using UnityEngine;
using SerializedData;

namespace SerializedData
{
     public partial class TestB
     {
        public void Load(FakeStruct fake)
        {
            a = (byte)fake[0];
            b = (Int16)fake[1];
            c = fake[2];
            d = fake.GetInt64(3);
            e = fake.GetFloat(5);
            f = fake.GetDouble(6);
            g = fake.GetData<string>(8);
            var h_t = fake.GetData<FakeStruct>(9);
            if (h_t != null)
            {
                h = new TestA();
                h.Load(h_t);
            }
            v = DBStructProc.LoadVector2(fake, 10);
            v3 = DBStructProc.LoadVector3(fake, 12);
            v4 = DBStructProc.LoadVector4(fake, 15);
            col = DBStructProc.LoadColor(fake, 19);
            col32 = DBStructProc.LoadColor32(fake, 23);
            q = DBStructProc.LoadQuaternion(fake, 24);
        }
        public FakeStruct Save(DataBuffer db)
        {
            FakeStruct fake = new FakeStruct(db, 28);
            fake[0]= a;
            fake[1]= b;
            fake[2]= c;
            fake.SetInt64(3, d);
            fake.SetFloat(5, e);
            fake.SetDouble(6, f);
            fake.SetData(8, g);
            if (h != null)
                fake[9] = db.AddData(h.Save(db));
            v.Save(fake, 10);
            v3.Save(fake, 12);
            v4.Save(fake, 15);
            col.Save(fake, 19);
            col32.Save(fake, 23);
            q.Save(fake, 24);
            return fake;
        }
     }
}