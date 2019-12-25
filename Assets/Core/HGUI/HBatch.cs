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
        public static void Batch(GUIElement[] pipeLine)
        {
            List<Vector3> vertex = new List<Vector3>();
            List<Vector2> uv = new List<Vector2>();
            List<Material> materials = new List<Material>();
            List<int[]> submesh = new List<int[]>();
            GUIElement root = pipeLine[0];
            if (root.script != null)
            {
                int c = root.childCount;
                int os = root.childOffset;
                for (int i = 0; i < c; i++)
                {
                    Batch(pipeLine, os);
                    os++;
                }
            }
        }
        static void Batch(GUIElement[] pipeLine,int index)
        {
            GUIElement root = pipeLine[index];
            if(root.active)
            {
                if (root.script != null)
                {
                    var graphics = root.script as HGraphics;
                }
                int c = root.childCount;
                int os = root.childOffset;
                for (int i = 0; i < c; i++)
                {
                    Batch(pipeLine, os);
                    os++;
                }
            }
        }
    }
}
