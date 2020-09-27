using huqiang.Data;
using huqiang.UIEvent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PolygonEventHelper : UICompositeHelp
{
    public Vector2[] Points;
    public override object ToBufferData(DataBuffer data)
    {
        if (Points != null)
        {
            FakeStruct fs = new FakeStruct(data, 2);
            fs[0] = (int)EventColliderType.Polygon;
            fs[1] = data.AddArray<Vector2>(Points);
            return fs;
        }
        return null;
    }
}
