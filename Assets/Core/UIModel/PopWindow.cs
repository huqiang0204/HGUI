using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PopWindow : UIBase
{
    public Func<bool> Back { get; set; }
    protected UIPage mainPage;
    public virtual void Initial(Transform parent, UIPage page, object obj = null)
    {
        base.Initial(parent, page, obj);
        mainPage = page;
        if (model != null)
            if (page != null)
                Main.SetParent(parent);
    }
    public virtual void Show(object obj = null) { if (model != null) model.activeSelf = true; }
    public virtual void Hide()
    {
        if (model != null)
            model.activeSelf = false;
    }
    public virtual bool Handling(string cmd, object dat)
    {
        return false;
    }
}
