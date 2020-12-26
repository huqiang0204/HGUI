using huqiang.Helper.HGUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PolygonEventHelper), true)]
public class PolygonEventHelperEditor:Editor
{
    public virtual void OnEnable()
    {
        var scr = target as PolygonEventHelper;
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
        DrawPolygon(target as PolygonEventHelper);
    }
    Vector3 GetOffset(Transform trans)
    {
        Vector2 pivot = new Vector2(0.5f, 0.5f);
        var ui = trans.GetComponent<UIElement>();
        Vector3 os = Vector3.zero;
        if (ui != null)
        {
            var size = ui.SizeDelta;
            pivot = ui.Pivot;
            os.x = (0.5f - pivot.x) * size.x;
            os.y = (0.5f - pivot.y) * size.y;
            os.x *= trans.lossyScale.x;
            os.y *= trans.lossyScale.y;
            os = trans.rotation * os;
        }
        return os;
    }
    void DrawPolygon(PolygonEventHelper pol)
    {
        if (pol.Points == null)
            return;
        if (pol.Points.Length <3)
            return;
        var trans = pol.transform;
        var os = GetOffset(trans);

        Vector3[] verts = new Vector3[pol.Points.Length];

        var p = trans.position;
        var z = p.z;
        var q = trans.rotation;
        var lx = trans.lossyScale.x;
        var ly = trans.lossyScale.y;
        for(int i=0;i<verts.Length;i++)
        {
            Vector3 v = pol.Points[i];
            v.x *= lx;
            v.y *= ly;
            verts[i] = p + q * v + os;
            verts[i].z = z;
        }
        Handles.color = Color.green;
        Handles.DrawPolyLine(verts);
        Handles.DrawLine(verts[0], verts[verts.Length - 1]);
        Quaternion f = Quaternion.Inverse(q);
        for (int i = 0; i < verts.Length; i++)
        {
            Vector3 r = Handles.FreeMoveHandle(verts[i], q, 0.05f, Vector3.one, Handles.SphereHandleCap);
            if (r != verts[i])
            {
                r = r - p - os;
                Vector3 v = f * r;
                v.x /= lx;
                v.y /= ly;
                v.z = 0;
                pol.Points[i] = v;
            }
        }
    }
}
