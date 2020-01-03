using huqiang;
using huqiang.UIEvent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huqiang.Core.HGUI
{
    public class VirtualEntity
    {
        public static Coordinates GetGlobaInfo(VirtualEntity entity, bool Includeroot = true)
        {
            VirtualEntity[] buff = new VirtualEntity[32];
            buff[0] = entity;
            var parent = entity.parent;
            int max = 1;
            if (parent != null)
                for (; max < 32; max++)
                {
                    buff[max] = parent;
                    parent = parent.parent;
                    if (parent == null)
                        break;
                }
            Vector3 pos, scale;
            Quaternion quate;
            if (Includeroot)
            {
                var p = buff[max];
                pos = p.localPosition;
                scale = p.localScale;
                quate = p.localRotation;
                max--;
            }
            else
            {
                pos = Vector3.zero;
                scale = Vector3.one;
                quate = Quaternion.identity;
                max--;
            }
            for (; max >= 0; max--)
            {
                var rt = buff[max];
                Vector3 p = rt.localPosition;
                Vector3 o = Vector3.zero;
                o.x = p.x * scale.x;
                o.y = p.y * scale.y;
                o.z = p.z * scale.z;
                pos += quate * o;
                quate *= rt.localRotation;
                Vector3 s = rt.localScale;
                scale.x *= s.x;
                scale.y *= s.y;
            }
            Coordinates coord = new Coordinates();
            coord.Postion = pos;
            coord.quaternion = quate;
            coord.Scale = scale;
            return coord;
        }
        public string name;
        Vector3 m_localPosition;
        Vector3 m_position;
        Vector3 m_localScale = Vector3.one;
        Vector3 m_lossyScale;
        Quaternion m_localRotation = Quaternion.identity;
        Quaternion m_rotation;
        bool m_changed;
        public Vector3 localPosition {
            get => m_localPosition;
            set
            {
                m_position = value;
            }
        }
        public Vector3 localEulerAngles {
            get => m_localRotation.eulerAngles;
            set
            {
                m_rotation = Quaternion.Euler(value);
            }
        }
        public Quaternion localRotation {
            get => m_localRotation;
            set
            {
                m_localRotation = value;
            }
        }

        public Vector3 localScale {
            get => m_localScale;
            set
            {
                m_localScale = value;
            }
        }
        public Vector3 position {
            get
            {
                return GetGlobaInfo(this).Postion;
            }
            set
            {
                var coord = GetGlobaInfo(this);
                var pos = coord.Postion;
                var offset = value - pos;
                offset.x *= coord.Scale.x / m_localScale.x;
                offset.y *= coord.Scale.y / m_localScale.y;
                offset.z *= coord.Scale.z / m_localScale.z;
                m_localPosition = offset;
            }
        }
        public Vector3 eulerAngles
        {
            get => m_localRotation.eulerAngles;
            set
            {
                rotation = Quaternion.Euler(value);
            }
        }
        public Quaternion rotation { get
            {
                return GetGlobaInfo(this).quaternion;
            }
            set
            {
                var coord = GetGlobaInfo(this);
                var rotate = coord.quaternion;
                rotate *= Quaternion.Inverse(m_localRotation);//父级的世界旋转,value 为自己世界旋转
                m_localRotation = Quaternion.Euler(value.eulerAngles - rotate.eulerAngles);
            }
        }
        public Vector3 lossyScale { get => GetGlobaInfo(this).Scale; }

        public List<VirtualEntity> Child = new List<VirtualEntity>();
        public VirtualEntity parent { get; private set; }
        public void SetParent(VirtualEntity entity)
        {
            if (parent != null)
                parent.Child.Remove(this);
            if (entity != null)
                entity.Child.Add(this);
            parent = entity;
            m_changed = true;
        }
        public Vector2 SizeDelta = new Vector2(100, 100);
        public Vector2 pivot = new Vector2(0.5f, 0.5f);
        private ScaleType lastScaleType;
        private AnchorType lastSizeType;
        private AnchorPointType lastAnchorType;
        private Margin lastmargin;
        public UserEvent userEvent;
        internal int PipelineIndex;
        public bool Mask;
    }
}