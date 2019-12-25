using Assets.Core.HGUI;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HCanvas), true)]
[CanEditMultipleObjects]
public class HCanvasEditor:Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUI.changed)
            Refresh(target as HCanvas);
    }
    void Refresh(HCanvas canvas)
    {
        if (canvas == null)
            return;
        canvas.Refresh();
        var mf = canvas.GetComponent<MeshFilter>();
        if (mf != null)
        {
            var mesh = mf.sharedMesh;
            mesh.triangles = null;
            mesh.vertices = null;
            mesh.uv = null;
            mesh.vertices = canvas.vertex.ToArray();
            mesh.uv = canvas.uv.ToArray();
            for (int i = 0; i < canvas.submesh.Count; i++)
                mesh.SetTriangles(canvas.submesh[i], i);
            mesh.subMeshCount = canvas.submesh.Count;
        }
        var mr = canvas.GetComponent<MeshRenderer>();
        if (mr != null)
        {
            mr.materials = canvas.materials.ToArray();
        }
    }
}
