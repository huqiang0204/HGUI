using huqiang.Core.HGUI;
using System.Collections.Generic;
using UnityEngine;

namespace huqiang.UIEvent
{
    public class InputCaret
    {
        static HImage m_Caret;
        public static HImage Caret
        {
            get
            {
                if (m_Caret == null)
                {
                    var g = new GameObject("m_caret",typeof(HImage));
                    m_Caret = g.GetComponent<HImage>();
                    m_Caret.transform.SetParent(App.UIRoot);
                    m_Caret.SizeDelta = new Vector2(2,24);
                }
                else if (m_Caret.name == "buff")
                {
                    var g = new GameObject("m_caret", typeof(HImage));
                    m_Caret = g.GetComponent<HImage>();
                    m_Caret.SizeDelta = new Vector2(2, 24);
                }
                return m_Caret;
            }
        }
        static float time;
        public static int Styles = 0;
        public static int CaretStyle = 0;
        public static Color32 PointerColor = Color.white;
        public static Color32 AreaColor = new Color(0.65882f, 0.8078f, 1, 0.2f);
        static List<HVertex> hs = new List<HVertex>();
        static List<int> tris = new List<int>();
        public static void UpdateCaret()
        {
            switch (TextOperation.Style)
            {
                case 1:
                    if ((Styles & 1) > 0)
                    {
                        time += Time.deltaTime;
                        if (time > 2f)
                        {
                            time = 0;
                        }
                        else if (time > 1f)
                        {
                            Caret.gameObject.SetActive(false);
                        }
                        else
                        {
                            Caret.gameObject.SetActive(true);
                        }
                    }
                    else Caret.gameObject.SetActive(false);
                    break;
                case 2:
                    if ((Styles & 2) > 0)
                    {
                        Caret.gameObject.SetActive(true);
                    }
                    else Caret.gameObject.SetActive(false);
                    break;
                default:
                    Caret.gameObject.SetActive(false);
                    break;
            }
        }
        public static void SetParent(Transform rect)
        {
            var t = Caret.transform;
            t.SetParent(rect);
            t.localPosition = Vector3.zero;
            t.localScale = Vector3.one;
            t.localRotation = Quaternion.identity;
        }
        public static void Hide()
        {
            CaretStyle = 0;
            if (m_Caret != null)
            {
                m_Caret.gameObject.SetActive(false);
            }
        }
        public static void ChangeCaret()
        {
            hs.Clear();
            tris.Clear();
            TextOperation.GetSelectArea(tris,hs,AreaColor,PointerColor);
            Caret.LoadFromMesh(hs,tris);
        }
    }
}
