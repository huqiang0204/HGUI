using huqiang.UIEvent;
using System;
using UnityEngine;

namespace Assets.Core.HGUI
{
    public class UserEvent
    {
        internal static void DispatchEvent(UserAction action, GUIElement[] pipeLine)
        {
            GUIElement root = pipeLine[0];
            if (root.script != null)
            {
                 int c = root.childCount;
                for (int i = c; i >= 1; i--)
                {
                    try
                    {
                        if (DispatchEvent(pipeLine, c, Vector3.zero, Vector3.one, Quaternion.identity, action))
                            return;
                    }
                    catch (Exception ex)
                    {
                        Debug.Log(ex);
                    }
                }
            }
        }
        static bool DispatchEvent(GUIElement[] pipeLine,int index,Vector3 pos,Vector3 scale,Quaternion quate, UserAction action)
        {
            Vector3 p = quate * pipeLine[index].localPosition;
            Vector3 o = Vector3.zero;
            o.x = p.x * scale.x;
            o.y = p.y * scale.y;
            o.z = p.z * scale.z;
            o += pos;
            Vector3 s = pipeLine[index].localScale;
            Quaternion q = quate * pipeLine[index].localRotation;
            s.x *= scale.x;
            s.y *= scale.y;

            return false;
        }
    }
}
