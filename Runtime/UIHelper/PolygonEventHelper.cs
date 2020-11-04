using huqiang.Core.HGUI;
using huqiang.Data;
using huqiang.UIEvent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PolygonEventHelper : UIHelper
{
    public Vector2[] Points;
    public unsafe override void ToBufferData(DataBuffer db,UITransfromData* data)
    {
        if (Points != null)
        {
            FakeStruct fs = new FakeStruct(db, 2);
            fs[0] = (int)EventColliderType.Polygon;
            fs[1] = db.AddArray<Vector2>(Points);
            data->eve = db.AddData(fs);
        }
    }
}
