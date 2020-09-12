using huqiang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[ExecuteInEditMode]
public class BezierPath : MonoBehaviour
{
    public List<BezierNode> nodes = new List<BezierNode>();
}
[Serializable]
public class BezierNode
{
    /// <summary>
    /// 节点
    /// </summary>
    public Vector3 NodePos;
    /// <summary>
    /// 终点方向
    /// </summary>
    public Vector3 LastDir;
    /// <summary>
    /// 起点方向
    /// </summary>
    public Vector3 NextDir;
}
