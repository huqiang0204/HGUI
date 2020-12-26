using huqiang.Core.UIData;
using huqiang.Helper.HGUI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HImage), true)]
[CanEditMultipleObjects]
public class HImageEditor:UIElementEditor
{
    //Vector3 pos;
    //Vector3 scale;
    //Vector3 angle;
    Sprite sprite;
    public override void OnEnable()
    {
        base.OnEnable();
        HImage img = target as HImage;
        if (img != null)
        {
            pos = img.transform.localPosition;
            scale = img.transform.localScale;
            angle = img.transform.localEulerAngles;
            sprite = img.Sprite;
        }
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        HImage img = target as HImage;
        if(img!=null)
        {
            bool changed = false;
            if(sprite!=img.Sprite)
            {
                sprite = img.Sprite;
                img.Sprite = sprite;
            }
            if(img.SprType == SpriteType.Filled)
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
            var t2d = EditorGUILayout.ObjectField("Texture", img.MainTexture, typeof(Texture), true) as Texture;
            if (img.MainTexture != t2d)
            {
                sprite = null;
                img.Sprite = null;
                img.MainTexture = t2d; 
            }
            if (GUILayout.Button("Set Native Size"))
                img.SetNativeSize();
            if (GUILayout.Button("Set Circle Mask"))
                img.SetCircleMask();
            if (GUI.changed |changed)
            {
                var tar = huqiang.Core.HGUI.UIElement.FindInstance(img.ContextID);
                if (tar != null)
                    img.ToHGUI2(tar, false);
            }
        }
    }
}
