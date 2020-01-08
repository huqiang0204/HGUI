using huqiang.Core.HGUI;
using UGUI;
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
                    m_Caret.SizeDelta = Vector2.zero;
                }
                else if (m_Caret.name == "buff")
                {
                    var g = new GameObject("m_caret", typeof(HImage));
                    m_Caret = g.GetComponent<HImage>();
                    m_Caret.SizeDelta = Vector2.zero;
                }
                return m_Caret;
            }
        }
        static float time;
        public static void ChangeCaret(TextControll info)
        {
            //if (m_Caret != null)
            //{
            //    int c = info.selectVertex.Count;
            //    Color32 col;
            //    if (info.CaretStyle == 1)
            //        col = info.caretColor;
            //    else col = info.areaColor;
            //    HVertex[] hv = new HVertex[c];
            //    for (int i = 0; i < c; i++)
            //    {
            //        hv[i].position = info.selectVertex[i].position;
            //        hv[i].color = col;
            //    }
            //    m_Caret.vertices = hv;
            //    m_Caret.tris = info.selectTri.ToArray();
            //    time = 0;
            //    m_Caret.gameObject.SetActive(true);
            //}
            //CaretStyle = info.CaretStyle;
        }
        public static int CaretStyle;
        public static void UpdateCaret()
        {
            switch (CaretStyle)
            {
                case 1:
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
                    break;
                case 2:

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
        public static void Active()
        {
            if (m_Caret != null)
            {
                m_Caret.gameObject.SetActive(true);
            }
        }
        public static void Hide()
        {
            CaretStyle = 0;
            if (m_Caret != null)
            {
                m_Caret.gameObject.SetActive(false);
            }
        }
    }
}
