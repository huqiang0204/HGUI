using huqiang.Data;
using huqiang.Helper.HGUI;
using huqiang.UIEvent;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PolygonEventHelper : UIHelper
{
    public Vector2[] Points;
    public unsafe override void ToBufferData(DataBuffer db, huqiang.Core.UIData.UIElementData* data)
    {
        if (Points != null)
        {
            FakeStruct fs = new FakeStruct(db, 2);
            fs[0] = (int)EventColliderType.Polygon;
            if (Points != null)
                if (Points.Length > 2)
                    fs[1] = db.AddArray<Vector2>(Points);
            int type = db.AddData("PolygonEventHelper");
            int index = db.AddData(fs);
            data->eve = type << 16 | index;
        }
    }
    public override unsafe void LoadFromBuffer(FakeStruct fake,Initializer initializer)
    {
        fake.buffer.GetArray<Vector2>(fake[1]);
    }
}
