using huqiang;
using huqiang.Core.HGUI;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UIElement), true)]
public class UIElementEditor : Editor
{
    HCanvas FindHCanvas(Transform trans)
    {
        if (trans == null)
            return null;
        var can = trans.GetComponent<HCanvas>();
        if (can == null)
            return FindHCanvas(trans.parent);
        return can;
    }
    public void OnEnable()
    {
        var scr = target as UIElement;
        if(scr!=null)
        {
            var can = FindHCanvas(scr.transform);
            if (can != null)
                can.Refresh();
        }
    }
    public virtual void OnSceneGUI()
    {
        var txt = target as UIElement;
        if (txt == null)
            return;
        Handles.color = Color.red;
        Vector3[] verts = new Vector3[8];
        var p = txt.transform.position;
        var r = txt.transform.right;
        var t = txt.transform.up;
        float x = txt.SizeDelta.x * 0.5f;
        x *= txt.transform.lossyScale.x;
        float y = txt.SizeDelta.y * 0.5f;
        y *= txt.transform.lossyScale.y;
        Vector3 ox = r.Move(x);
        Vector3 oy = t.Move(y);
        var lx = p - ox;
        var rx = p + ox;
        var dy = p - oy;
        var ty = p + oy;
        verts[0] = lx + oy;
        verts[1] = rx + oy;
        verts[2] = rx + oy;
        verts[3] = rx - oy;
        verts[4] = rx - oy;
        verts[5] = lx - oy;
        verts[6] = lx - oy;
        verts[7] = lx + oy;
        Handles.DrawLines(verts);
    }
}