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
            img.sprite = EditorGUILayout.ObjectField("Sprite",img.sprite,typeof(Sprite),true) as Sprite;
            img.type = (SpriteType)EditorGUILayout.EnumPopup("SpriteType",img.type);
            if(img.type==SpriteType.Filled)
            {
                var ori = img.fillMethod;
                img.fillMethod = (FillMethod)EditorGUILayout.EnumPopup("FillMethod", img.fillMethod);
                if (img.fillMethod != ori)
                    img.fillOrigin = 0;
                switch(img.fillMethod)
                {
                    case FillMethod.Horizontal:
                         img.fillOrigin = (int)(OriginHorizontal)EditorGUILayout.EnumPopup("FillOrigin", (OriginHorizontal)img.fillOrigin);
                        break;
                    case FillMethod.Vertical:
                        img.fillOrigin = (int)(OriginVertical)EditorGUILayout.EnumPopup("FillOrigin", (OriginVertical)img.fillOrigin);
                        break;
                    case FillMethod.Radial90:
                        img.fillOrigin = (int)(Origin90)EditorGUILayout.EnumPopup("FillOrigin", (Origin90)img.fillOrigin);
                        break;
                    case FillMethod.Radial180:
                        img.fillOrigin = (int)(Origin90)EditorGUILayout.EnumPopup("FillOrigin", (Origin180)img.fillOrigin);
                        break;
                    case FillMethod.Radial360:
                        img.fillOrigin = (int)(Origin90)EditorGUILayout.EnumPopup("FillOrigin", (Origin360)img.fillOrigin);
                        break;
                }
                img.fillAmount = EditorGUILayout.Slider("FillAmount",img.fillAmount,0,1);
            }
            if (img.type == SpriteType.Simple | img.type == SpriteType.Filled)
                img.preserveAspect = EditorGUILayout.Toggle("PreserveAspect", img.preserveAspect);
            else
            {
                img.fillCenter = EditorGUILayout.Toggle("FillCenter", img.fillCenter);
                img.pixelsPerUnitMultiplier = EditorGUILayout.FloatField("PixelsPerUnitMultiplier", img.pixelsPerUnitMultiplier);
            }
            if (GUILayout.Button("Set Native Size"))
                img.SetNativeSize();
            else if(GUI.changed)
                img.Test();
        }
    }
}
