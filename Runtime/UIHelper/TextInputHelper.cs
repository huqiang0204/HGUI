using System;
using huqiang.Data;
using huqiang.UIEvent;
using UnityEngine;
using huqiang.Helper.HGUI;

public class TextInputHelper:UIHelper
{
    public Color inputColor = Color.white;
    public Color tipColor = Color.gray;
    public Color pointColor=Color.white;
    public Color selectColor= new Color(0.65882f, 0.8078f, 1, 0.2f);
    [TextArea(3, 10)]
    [SerializeField]
    public string InputString;
    public string TipString = "请输入...";
    public int CharacterLimit;
    public bool ReadyOnly;
    public ContentType contentType;
    public LineType lineType;
    public unsafe override void ToBufferData(DataBuffer db, huqiang.Core.UIData.UIElementData* data)
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
        int type = db.AddData("TextInputHelper");
        int index = db.AddData(fake);
        data->eve = type << 16 | index;
    }
    public unsafe override void LoadFromBuffer(FakeStruct fake,Initializer initializer)
    {
        TextInputData* sp = (TextInputData*)fake.ip;
        inputColor = sp->inputColor;
        tipColor = sp->tipColor;
        pointColor = sp->pointColor;
        selectColor = sp->selectColor;
        InputString = fake.buffer.GetData(sp->inputString) as string;
        TipString = fake.buffer.GetData(sp->tipString) as string;
        CharacterLimit = sp->CharacterLimit;
        ReadyOnly = sp->ReadyOnly;
        contentType = sp->contentType;
        lineType = sp->lineType;
    }
    public override void Refresh()
    {
        var ui = GetComponentInChildren<HText>();
        if (ui == null)
            return;
        var txt = ui.Content;
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
