using huqiang.Data;
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
        FakeStruct fs = new FakeStruct(data,2);
        fs.SetFloat(0,Radius);
        fs.SetFloat(1,Ratio);
        return fs;
    }
}
