using huqiang.Data;
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
            FakeStruct fs = new FakeStruct(data, 1);
            fs[0] = data.AddArray<Vector2>(Points);
            return fs;
        }
        return null;
    }
}
