using UnityEngine;

namespace huqiang.Data
{
    public static class DBStructProc
    {
        public static Vector2 LoadVector2(FakeStruct fake, int offset)
        {
            Vector2 v = new Vector2();
            v.x = fake.GetFloat(offset);
            v.y = fake.GetFloat(offset + 1);
            return v;
        }
        public static Vector3 LoadVector3(FakeStruct fake, int offset)
        {
            Vector3 v = new Vector3();
            v.x = fake.GetFloat(offset);
            v.y = fake.GetFloat(offset + 1);
            v.z = fake.GetFloat(offset + 2);
            return v;
        }
        public static Vector4 LoadVector4(FakeStruct fake, int offset)
        {
            Vector4 v = new Vector4();
            v.x = fake.GetFloat(offset);
            v.y = fake.GetFloat(offset + 1);
            v.z = fake.GetFloat(offset + 2);
            v.w = fake.GetFloat(offset + 3);
            return v;
        }
        public static Quaternion LoadQuaternion(FakeStruct fake, int offset)
        {
            Quaternion v = new Quaternion();
            v.x = fake.GetFloat(offset);
            v.y = fake.GetFloat(offset + 1);
            v.z = fake.GetFloat(offset + 2);
            v.w = fake.GetFloat(offset + 3);
            return v;
        }
        public static Color LoadColor(FakeStruct fake, int offset)
        {
            Color v = new Color();
            v.r = fake.GetFloat(offset);
            v.g = fake.GetFloat(offset + 1);
            v.b = fake.GetFloat(offset + 2);
            v.a = fake.GetFloat(offset + 3);
            return v;
        }
        public static Color32 LoadColor32(FakeStruct fake, int offset)
        {
            return fake[offset].ToColor();
        }
        public static void Save(this Vector2 v, FakeStruct fake, int offset)
        {
            fake.SetFloat(offset, v.x);
            fake.SetFloat(offset + 1, v.y);
        }
        public static void Save(this Vector3 v, FakeStruct fake, int offset)
        {
            fake.SetFloat(offset, v.x);
            fake.SetFloat(offset + 1, v.y);
            fake.SetFloat(offset + 2, v.z);
        }
        public static void Save(this Vector4 v, FakeStruct fake, int offset)
        {
            fake.SetFloat(offset, v.x);
            fake.SetFloat(offset + 1, v.y);
            fake.SetFloat(offset + 2, v.z);
            fake.SetFloat(offset + 3, v.w);
        }
        public static void Save(this Quaternion v, FakeStruct fake, int offset)
        {
            fake.SetFloat(offset, v.x);
            fake.SetFloat(offset + 1, v.y);
            fake.SetFloat(offset + 2, v.z);
            fake.SetFloat(offset + 3, v.w);
        }
        public static void Save(this Color v, FakeStruct fake, int offset)
        {
            fake.SetFloat(offset, v.r);
            fake.SetFloat(offset + 1, v.g);
            fake.SetFloat(offset + 2, v.b);
            fake.SetFloat(offset + 3, v.a);
        }
        public static void Save(this Color32 v, FakeStruct fake, int offset)
        {
            fake[offset] = v.ToInt();
        }
    }
}

