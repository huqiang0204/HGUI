using huqiang.Data;
using huqiang.UIEvent;
using System;
using UnityEngine;
using huqiang.Helper.HGUI;
/// <summary>
/// 圆形事件区域帮助类
/// </summary>
public class CircleEventHelper : UIHelper
{
    /// <summary>
    /// 半径
    /// </summary>
    public float Radius;
    [Range(0,10)]
    public float Ratio = 1;
    public unsafe override void ToBufferData(DataBuffer db, huqiang.Core.UIData.UIElementData* data)
    {
        FakeStruct fs = new FakeStruct(db,3);
        fs[0] = (int)EventColliderType.Circle;
        fs.SetFloat(1,Radius);
        fs.SetFloat(2,Ratio);
        int type = db.AddData("CircleEventHelper");
        int index= db.AddData(fs);
        data->eve = type << 16 | index;
    }
    public override unsafe void LoadFromBuffer(FakeStruct fake,Initializer initializer)
    {
        Radius = fake.GetFloat(1);
        Ratio = fake.GetFloat(2);
    }
}
