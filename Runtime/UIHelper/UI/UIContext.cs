using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace huqiang.Helper.HGUI
{
    public class UIContext:MonoBehaviour
    {
        public virtual Core.HGUI.UIElement GetUIData()
        {
            return null;
        }
#if UNITY_EDITOR
        public void MakeContext()
        {
            var trans = transform;
            var c = trans.childCount;
            var p = GetUIData();
            p.localPosition = trans.localPosition;
            p.localRotation = trans.localRotation;
            p.localScale = trans.localScale;
            p.name = trans.name;
            p.activeSelf = gameObject.activeSelf;
            for (int i = 0; i < c; i++)
            {
                var son = trans.GetChild(i);
                var uc = son.GetComponent<UIContext>();
                if (uc != null)
                {
                    var ud = uc.GetUIData();
                    if (ud != null)
                    {
                        ud.Context = son;
                        ud.SetParent(p);
                        uc.MakeContext();
                    }
                }
            }
        }
#endif
    }
}
