using huqiang.Data;
using huqiang.UIEvent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CircleEventHelper : UICompositeHelp
{
    public float Radius;
    [Range(0,10)]
    public float Ratio = 1;
    public override object ToBufferData(DataBuffer data)
    {
        FakeStruct fs = new FakeStruct(data,3);
        fs[0] = (int)EventColliderType.Circle;
        fs.SetFloat(1,Radius);
        fs.SetFloat(2,Ratio);
        return fs;
    }
}
