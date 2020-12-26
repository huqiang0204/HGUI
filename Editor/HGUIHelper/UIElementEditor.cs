using huqiang;
using huqiang.Helper.HGUI;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UIElement), true)]
public class UIElementEditor : Editor
{
    bool activeSelf;
    protected Vector3 pos;
    protected  Vector3 scale;
    protected Vector3 angle;
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
        var scr = target as UIElement;
        if(scr!=null)
        {
            activeSelf = scr.gameObject.activeSelf;
            var trans = scr.transform;
            pos = trans.localPosition;
            scale = trans.localScale;
            angle = trans.localEulerAngles;
        }
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
        DrawBorder(target as UIElement);
    }
    public void DrawBorder(UIElement txt)
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
        var size = txt.SizeDelta;
        size.x *= txt.transform.lossyScale.x;
        size.y *= txt.transform.lossyScale.y;
        var bor = new Border(size, txt.Pivot);
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
        var ui = target as UIElement;
        var trans = ui.transform;
        bool changed = false;
        if (activeSelf != trans.gameObject.activeSelf)
            changed = true;
        if (pos != trans.localPosition)
            changed = true;
        else if (scale != trans.localScale)
            changed = true;
        else if (angle != trans.localEulerAngles)
            changed = true;
        pos = trans.localPosition;
        scale = trans.localScale;
        angle = trans.localEulerAngles;
        if (GUI.changed | changed)
        {
            var tar = huqiang.Core.HGUI.UIElement.FindInstance(ui.ContextID);
            if (tar != null)
                ui.ToHGUI2(tar, false);
        }
    }
    protected void ApplyToHGUI2(UIElement element)
    {
        var ele = huqiang.Core.HGUI.UIElement.FindInstance(element.ContextID);
        if (ele != null)
        {
            element.ToHGUI2(ele,true);
        }
        else
        {
            ele = element.ToHGUI2(true,false);
            if(element.transform.parent!=null)
            {
                var pe = element.transform.parent.GetComponent<UIElement>();
                var le = huqiang.Core.HGUI.UIElement.FindInstance(pe.ContextID);
                ele.SetParent(ele);
            }
        }
    }
}