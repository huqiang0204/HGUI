using System;
using huqiang.Core.HGUI;
using huqiang.Data;
using huqiang.UIEvent;
using UnityEngine;

public class TextInputHelper:UIHelper
{
    public Color inputColor = Color.white;
    public Color tipColor = Color.gray;
    public Color pointColor=Color.white;
    public Color selectColor= new Color(0.65882f, 0.8078f, 1, 0.2f);
    public string InputString;
    public string TipString = "请输入...";
    public int CharacterLimit;
    public bool ReadyOnly;
    public ContentType contentType;
    public LineType lineType;
    public unsafe override void ToBufferData(DataBuffer db, UITransfromData* data)
    {
        FakeStruct fake = new FakeStruct(db, TextInputData.ElementSize);
        TextInputData* sp = (TextInputData*)fake.ip;
        sp->inputColor = inputColor;
        sp->tipColor = tipColor;
        sp->pointColor = pointColor;
        sp->selectColor = selectColor;
        sp->inputString = db.AddData(InputString);
        sp->tipString = db.AddData(TipString);
        sp->CharacterLimit = CharacterLimit;
        sp->ReadyOnly = ReadyOnly;
        sp->contentType = contentType;
        sp->lineType = lineType;
        data->eve = db.AddData(fake);
    }
    public override void Refresh()
    {
        var txt = GetComponentInChildren<HText>();
        if (txt == null)
            return;
        if (InputString==null|InputString=="")
        {
            txt.Text = TipString;
            txt.MainColor = tipColor;
        }
        else
        {
            txt.Text = InputString;
            txt.MainColor = inputColor;
        }
    }
}
