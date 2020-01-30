using System;
using huqiang.Core.HGUI;
using huqiang.Data;
using huqiang.UIEvent;
using UnityEngine;

[RequireComponent(typeof(HText))]
public class TextInputHelper:UICompositeHelp
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
    public unsafe override object ToBufferData(DataBuffer data)
    {
        FakeStruct fake = new FakeStruct(data, TextInputData.ElementSize);
        TextInputData* sp = (TextInputData*)fake.ip;
        sp->inputColor = inputColor;
        sp->tipColor = tipColor;
        sp->pointColor = pointColor;
        sp->selectColor = selectColor;
        sp->inputString = data.AddData(InputString);
        sp->tipString = data.AddData(TipString);
        sp->CharacterLimit = CharacterLimit;
        sp->ReadyOnly = ReadyOnly;
        sp->contentType = contentType;
        sp->lineType = lineType;
        return fake;
    }
    public override void Refresh()
    {
        if(InputString==null|InputString=="")
        {
            var txt = GetComponent<HText>();
            txt.Text = TipString;
            txt.MainColor = tipColor;
        }
        else
        {
            var txt = GetComponent<HText>();
            txt.Text = InputString;
            txt.MainColor = inputColor;
        }
    }
}
