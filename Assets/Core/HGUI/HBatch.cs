using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Core.HGUI
{
    internal class HBatch
    {
       internal class MeshCollector
        {
            public List<Vector3> vertex = new List<Vector3>();
            public List<Vector2> uv = new List<Vector2>();
            List<Material> materials = new List<Material>();
            List<int> ids = new List<int>();
            public List<int[]> submesh = new List<int[]>();
            public void AddMaterial(Material material, int id)
            {
                materials.Add(material);
                ids.Add(id);
            }
            public Material FindMaterial(int id)
            {
                for (int i = 0; i < ids.Count; i++)
                {
                    if (ids[i] == id)
                    {
                        return materials[i];
                    }
                }
                return null;
            }
        }
        public static void Batch(GUIElement[] pipeLine)
        {
            MeshCollector collector = new MeshCollector();
            GUIElement root = pipeLine[0];
            if (root.script != null)
            {
                int c = root.childCount;
                int os = root.childOffset;
                for (int i = 0; i < c; i++)
                {
                    Batch(pipeLine, os, collector, Vector3.zero, Vector3.one, Quaternion.identity);
                    os++;
                }
            }
        }
        static void Batch(GUIElement[] pipeLine,int index,MeshCollector collector, Vector3 pos, Vector3 scale, Quaternion quate)
        {
            GUIElement root = pipeLine[index];
            Vector3 p = quate * pipeLine[index].localPosition;
            Vector3 o = Vector3.zero;
            o.x = p.x * scale.x;
            o.y = p.y * scale.y;
            o.z = p.z * scale.z;
            o += pos;
            Vector3 s = pipeLine[index].localScale;
            Quaternion q = quate * pipeLine[index].localRotation;
     
            if (root.active)
            {
                if (root.script != null)
                {
                    var graphics = root.script as HGraphics;
                    var vs = collector.vertex;
                    var vc = vs.Count;
                    var vert = graphics.vertex;
                    for (int j = 0; j < vert.Length; j++)
                    {
                        var t = q * vert[j];
                        t.x *= s.x;
                        t.y *= s.y;
                        vs.Add(pos + t);
                    }
                    collector.uv.AddRange(graphics.uv);
                    var ms = graphics.SubMesh;
                    if (ms != null)
                    {
                        for (int j = 0; j < ms.Length; j++)
                        {
                            var src = ms[j];
                            if(src!=null)
                            {
                                if(src.Length>0)
                                {
                                    int[] tmp = new int[src.Length];
                                    for (int k = 0; k < tmp.Length; k++)
                                    {
                                        tmp[k] = src[k] + vc;
                                    }
                                    collector.submesh.Add(tmp);
                                    collector.AddMaterial(graphics.GetMaterial(j,collector),graphics.InstanceID);
                                }
                            }
                        }
                    }
                }
                s.x *= scale.x;
                s.y *= scale.y;
                int c = root.childCount;
                int os = root.childOffset;
                for (int i = 0; i < c; i++)
                {
                    Batch(pipeLine, os, collector, o, s, q);
                    os++;
                }
            }
        }
    }
}
