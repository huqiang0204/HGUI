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
            a = (byte)fake[1];
            b = (Int16)fake[2];
            c = fake[3];
            d = fake.GetInt64(4);
            e = fake.GetFloat(6);
            f = fake.GetDouble(7);
            g = fake.GetData<string>(9);
            var h_t = fake.GetData<FakeStruct>(10);
            if (h_t != null)
            {
                h = new TestA();
                h.Load(h_t);
            }
            v = DBStructProc.LoadVector2(fake, 11);
            v3 = DBStructProc.LoadVector3(fake, 13);
            v4 = DBStructProc.LoadVector4(fake, 16);
            col = DBStructProc.LoadColor(fake, 20);
            col32 = DBStructProc.LoadColor32(fake, 24);
            q = DBStructProc.LoadQuaternion(fake, 25);
        }
        public FakeStruct Save(DataBuffer db)
        {
            FakeStruct fake = new FakeStruct(db, 29);
            fake[0] = db.AddData(this.GetType().Name, DataType.String);
            fake[1]= a;
            fake[2]= b;
            fake[3]= c;
            fake.SetInt64(4, d);
            fake.SetFloat(6, e);
            fake.SetDouble(7, f);
            fake.SetData(9, g);
            if (h != null)
                fake[10] = db.AddData(h.Save(db));
            v.Save(fake, 11);
            v3.Save(fake, 13);
            v4.Save(fake, 16);
            col.Save(fake, 20);
            col32.Save(fake, 24);
            q.Save(fake, 25);
            return fake;
        }
     }
}