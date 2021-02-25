using huqiang;
using huqiang.Helper.HGUI;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UIContext), true)]
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
    public virtual void OnEnable()
    {
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
        DrawBorder(target as UIContext);
    }
    public void DrawBorder(UIContext txt)
    {
        if (txt == null)
            return;
        var can = txt.transform.root.GetComponent<HCanvas>();
        if (can == null)
            return;
        can.ApplyToCamera();
        Handles.color = Color.red;
        Vector3[] verts = new Vector3[8];
        var p = txt.transform.position;
        var ui = txt.GetUIData();
        var size = ui.SizeDelta;
        size.x *= txt.transform.lossyScale.x;
        size.y *= txt.transform.lossyScale.y;
        var bor = new Border(size, ui.Pivot);
        var q = txt.transform.rotation;
        var lt = p + q * new Vector3(bor.left, bor.top, 0);
        var rt = p + q * new Vector3(bor.right, bor.top, 0);
        var rd = p + q * new Vector3(bor.right, bor.down, 0);
        var ld = p + q * new Vector3(bor.left, bor.down, 0);
        verts[0] = lt;
        verts[1] = rt;
        verts[2] = rt;
        verts[3] = rd;
        verts[4] = rd;
        verts[5] = ld;
        verts[6] = ld;
        verts[7] = lt;
        Handles.DrawLines(verts);
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var ui = target as UIContext;
        var dat = ui.GetUIData();
        var trans = ui.transform;
        dat.activeSelf = ui.gameObject.activeSelf;
        dat.localPosition = trans.localPosition;
        dat.localRotation = trans.localRotation;
        dat.localScale = trans.localScale;
    }
}