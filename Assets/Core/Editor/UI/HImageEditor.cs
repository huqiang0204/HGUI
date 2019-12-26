using Assets.Core.HGUI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HImage), true)]
[CanEditMultipleObjects]
public class HImageEditor:Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        HImage img = target as HImage;
        if(img!=null)
        {
            bool changed = false;
            img.SprType = (SpriteType)EditorGUILayout.EnumPopup("SpriteType",img.SprType);
            if(img.SprType==SpriteType.Filled)
            {
                var ori = img.FillMethod;
                img.FillMethod = (FillMethod)EditorGUILayout.EnumPopup("FillMethod", img.FillMethod);
                if (img.FillMethod != ori)
                    img.FillOrigin = 0;
                switch(img.FillMethod)
                {
                    case FillMethod.Horizontal:
                         img.FillOrigin = (int)(OriginHorizontal)EditorGUILayout.EnumPopup("FillOrigin", (OriginHorizontal)img.FillOrigin);
                        break;
                    case FillMethod.Vertical:
                        img.FillOrigin = (int)(OriginVertical)EditorGUILayout.EnumPopup("FillOrigin", (OriginVertical)img.FillOrigin);
                        break;
                    case FillMethod.Radial90:
                        img.FillOrigin = (int)(Origin90)EditorGUILayout.EnumPopup("FillOrigin", (Origin90)img.FillOrigin);
                        break;
                    case FillMethod.Radial180:
                        img.FillOrigin = (int)(Origin90)EditorGUILayout.EnumPopup("FillOrigin", (Origin180)img.FillOrigin);
                        break;
                    case FillMethod.Radial360:
                        img.FillOrigin = (int)(Origin90)EditorGUILayout.EnumPopup("FillOrigin", (Origin360)img.FillOrigin);
                        break;
                }
                img.FillAmount = EditorGUILayout.Slider("FillAmount",img.FillAmount,0,1);
            }
            if (img.SprType == SpriteType.Simple | img.SprType == SpriteType.Filled)
            {
                bool p = img.PreserveAspect;
                img.PreserveAspect = EditorGUILayout.Toggle("PreserveAspect", img.PreserveAspect);
                if (p != img.PreserveAspect)
                    changed = true;
            }
            else
            {
                img.FillCenter = EditorGUILayout.Toggle("FillCenter", img.FillCenter);
                img.PixelsPerUnitMultiplier = EditorGUILayout.FloatField("PixelsPerUnitMultiplier", img.PixelsPerUnitMultiplier);
            }
            if (GUILayout.Button("Set Native Size"))
                img.SetNativeSize();
            if(GUI.changed |changed)
            {
                var can = FindHCanvas(img.transform);
                if (can != null)
                    can.Refresh();
            }
        }
    }
    HCanvas FindHCanvas(Transform trans)
    {
        if (trans == null)
            return null;
        var can = trans.GetComponent<HCanvas>();
        if (can == null)
            return FindHCanvas(trans.parent);
        return can;
    }
}
