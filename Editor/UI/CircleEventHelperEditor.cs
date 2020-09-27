using huqiang.Core.HGUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CircleEventHelper), true)]
public class CircleEventHelperEditor:Editor
{
    public virtual void OnEnable()
    {
        var scr = target as CircleEventHelper;
#if UNITY_2019_1_OR_NEWER
        SceneView.duringSceneGui += DuringSceneGui;
#else
        SceneView.onSceneGUIDelegate += DuringSceneGui;
#endif
    }
    public virtual void OnDisable()
    {
#if UNITY_2019_1_OR_NEWER
        SceneView.duringSceneGui -= DuringSceneGui;
#else
        SceneView.onSceneGUIDelegate -= DuringSceneGui;
#endif
    }
    void DuringSceneGui(SceneView view)
    {
        DrawCircle(target as CircleEventHelper);
    }
    void DrawCircle(CircleEventHelper circle)
    {
        var trans = circle.transform;
        float r = circle.Radius* trans.lossyScale.x;
        Vector2 pivot = new Vector2(0.5f,0.5f);
        var ui = circle.GetComponent<UIElement>();
        Vector3 os = Vector3.zero;
        if (ui != null)
        {
            pivot = ui.Pivot;
            var size = ui.SizeDelta;
            if (r==0)
            {
                size.x *= trans.lossyScale.x;
                size.y *= trans.lossyScale.y;
                r = size.x;
                if (r > size.y)
                    r = size.y;
                r *= 0.5f;
                r *= circle.Ratio;
            }
            os.x = (0.5f - pivot.x) * size.x;
            os.y = (0.5f - pivot.y) * size.y;
            os.x *= trans.lossyScale.x;
            os.y *= trans.lossyScale.y;
            os = trans.rotation * os;
        }
        if (r <= 0)
            return;
        os.z = 0;
        Handles.color = Color.green;     
        Handles.Disc(trans.rotation, trans.position+os,
            trans.forward, r, false, 1);
    }
}
