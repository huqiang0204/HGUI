using huqiang;
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
        static bool DispatchEvent(GUIElement[] pipeLine, int index,Vector3 pos,Vector3 scale,Quaternion quate, UserAction action)
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
            var script = pipeLine[index].script;
            if (script != null)
            {
                var ue = script.userEvent;
                if (ue == null)
                {
                    int c = pipeLine[index].childCount;
                    if(c > 0)
                    {
                        int os = pipeLine[index].childOffset + c;
                        for (int i = 0; i < c; i++)
                        {
                            os--;
                            if (DispatchEvent(pipeLine, os, o, s, q, action))
                            {
                                return true;
                            }
                        }
                    }
                }
                else if (ue.forbid)
                {

                }
                else
                {

                }
            }
            return false;
        }
        int xTime;
        int yTime;
        float lastX;
        float lastY;
        public Vector2 boxSize;
        Vector2 maxVelocity;
        Vector2 sDistance;
        Vector2 mVelocity;
        public float ScrollDistanceX
        {
            get { return sDistance.x; }
            set
            {
                if (value == 0)
                    maxVelocity.x = 0;
                else
                    maxVelocity.x = MathH.DistanceToVelocity(DecayRateX, value);
                mVelocity.x = maxVelocity.x;
                sDistance.x = value;
                xTime = 0;
                lastX = 0;
            }
        }
        public float ScrollDistanceY
        {
            get { return sDistance.y; }
            set
            {
                if (value == 0)
                    maxVelocity.y = 0;
                else
                    maxVelocity.y = MathH.DistanceToVelocity(DecayRateY, value);
                mVelocity.y = maxVelocity.y;
                sDistance.y = value;
                yTime = 0;
                lastY = 0;
            }
        }
        public float DecayRateX = 0.998f;
        public float DecayRateY = 0.998f;
        public float speed = 1f;
        public Vector2 RawPosition { get; protected set; }
        Vector2 LastPosition;
        public int HoverTime { get; protected set; }
        public long PressTime { get; internal set; }
        public long EntryTime { get; protected set; }
        public long StayTime { get; protected set; }
        public bool Pressed { get; internal set; }
        public bool forbid;
    }
}
