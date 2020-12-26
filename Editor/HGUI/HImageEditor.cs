using huqiang.Core.HGUI;
using huqiang.Core.UIData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

[HGUIEditor(typeof(HImage))]
public class HimageEditor:UIEditor
{
    Sprite sprite;
    public override void OnEnable()
    {
        base.OnEnable();
        HImage img = Target as HImage;
        if (img != null)
        {
            sprite = img.Sprite;
        }
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var img = Target as HImage;
        if (img != null)
        {
            bool changed = false;
            if (sprite != img.Sprite)
            {
                sprite = img.Sprite;
                img.Sprite = sprite;
            }
            if (img.SprType == SpriteType.Filled)
            {
                var ori = img.FillMethod;
                img.FillMethod = (FillMethod)EditorGUILayout.EnumPopup("FillMethod", img.FillMethod);
                if (img.FillMethod != ori)
                    img.FillOrigin = 0;
                switch (img.FillMethod)
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
                img.FillAmount = EditorGUILayout.Slider("FillAmount", img.FillAmount, 0, 1);
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
            img.MainTexture = EditorGUILayout.ObjectField("Texture", img.MainTexture, typeof(Texture), true) as Texture;
            if (GUILayout.Button("Set Native Size"))
                img.SetNativeSize();
            if (GUILayout.Button("Set Circle Mask"))
                img.SetCircleMask();
            if (GUI.changed | changed)
            {
                img.m_dirty = true;
            }
        }
    }
}
