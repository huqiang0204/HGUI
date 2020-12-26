using huqiang.Helper.HGUI;
using huqiang.Core.UIData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CustomFontHelper : UIHelper
{
    [Serializable]
    public struct CharInfo
    {
        public char cha;
        public Sprite sprite;
    }
    [SerializeField]
    public List<CharInfo> chars = new List<CharInfo>();
    public int NormalFontSize = 14;
    public override void Refresh()
    {
        base.Refresh();
        var txt = GetComponent<HText>();
        if (txt != null)
        {
            CustomFont font = null;
            for (int i = 0; i < chars.Count; i++)
            {
                if(chars[i].sprite!=null)
                {
                    if (font == null)
                        font = new CustomFont(chars[i].sprite.texture);
                    font.AddCharMap(chars[i].cha,chars[i].sprite, NormalFontSize);
                }
            }
            txt.customFont = font;
        }
    }
}