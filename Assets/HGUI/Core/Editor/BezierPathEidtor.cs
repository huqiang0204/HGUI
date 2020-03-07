using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using huqiang;

[CustomEditor(typeof(BezierPath), true)]
[CanEditMultipleObjects]
public class BezierPathEidtor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Add Node"))
        {
            AddNode((target as BezierPath).nodes);
        }
    }
    void AddNode(List<BezierNode> nodes)
    {
        if (nodes.Count > 0)
        {
            var last = nodes[nodes.Count - 1];
            BezierNode node = new BezierNode();
            node.NodePos = last.LastDir;
            node.LastDir = last.LastDir;
            node.NextDir = last.NextDir;
            node.NodePos.x += 20;
            node.NodePos.y += 20;
            nodes.Add(node);
        }
        else
        {
            BezierNode node = new BezierNode();
            node.LastDir.y = -10;
            node.NextDir.x = 10;
            nodes.Add(node);
        }
    }

    public void OnSceneGUI()
    {
        var bp = target as BezierPath;
        if (bp != null)
            DrawNodes(bp.nodes, bp.transform);
    }
    void DrawNodes(List<BezierNode> nodes, Transform transform)
    {
        if (nodes.Count == 0)
            return;
        var p = transform.position;
        var q = transform.rotation;
        for (int i = 0; i < nodes.Count; i++)
        {
            Handles.color = Color.green;
            var s = p + nodes[i].NodePos;
            Handles.SphereHandleCap(0, p + nodes[i].NodePos, q, 2, EventType.Repaint);
            nodes[i].NodePos = Handles.DoPositionHandle(s, q) - p;

            Handles.color = Color.white;
            Handles.DrawLine(s, s + nodes[i].LastDir);
            Handles.SphereHandleCap(0, s + nodes[i].LastDir, q, 2, EventType.Repaint);
            nodes[i].LastDir = Handles.DoPositionHandle(s + nodes[i].LastDir, q) - s;

            Handles.DrawLine(s, s + nodes[i].NextDir);
            Handles.SphereHandleCap(0, s + nodes[i].NextDir, q, 2, EventType.Repaint);
            nodes[i].NextDir = Handles.DoPositionHandle(s + nodes[i].NextDir, q) - s;
        }
        if (nodes.Count > 1)
        {
            Handles.color = Color.green;
            var s = nodes[0];
            for (int i = 1; i < nodes.Count; i++)
            {
                var e = nodes[i];
                DrawNode(s, e, p);
                s = e;
            }
        }
    }
    void DrawNode(BezierNode start, BezierNode end, Vector3 position, int linecount = 16)
    {
        var a = start.NodePos + position;
        var b = a + start.NextDir;
        var d = end.NodePos + position;
        var c = d + end.LastDir;
        var last = a;
        for (int i = 1; i <= linecount; i++)
        {
            float r = i;
            r /= linecount;
            var p = MathH.BezierPoint(r, ref a, ref b, ref c, ref d);
            Handles.DrawLine(last, p);
            last = p;
        }
    }
}
