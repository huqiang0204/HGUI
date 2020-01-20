using huqiang.Core.HGUI;
using huqiang.Other;
using huqiang.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class PaletteHelper:UICompositeHelp
{
    Palette palette;
    public void Initial()
    {
        palette = new Palette();
        palette.LoadHSVT(1);
        var hc = transform.Find("HTemplate");
        if (hc != null)
        {
            var template = hc.GetComponent<HImage>();
            if(template!=null)
            template.MainTexture = palette.texture;
        }
        var htemp = transform.GetComponent<HImage>();
        if(htemp!=null)
            htemp.MainTexture = Palette.LoadCTemplate();
        var sli= transform.Find("Slider");
        if(sli!=null)
        {
            var slider= sli.GetComponent<HImage>();
            if(slider!=null)
                slider.MainTexture = Palette.AlphaTemplate();
        }
        
    }
}