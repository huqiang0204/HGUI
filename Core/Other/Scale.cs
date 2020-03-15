using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public enum ScaleType
{
    None,
    FillX,
    FillY,
    FillXY,
    Cover,
}
public enum AnchorType
{
    None,
    Anchor,
    Alignment
}
public enum AnchorPointType
{
    Center,
    Left,
    Right,
    Top,
    Down,
    LeftDown,
    LeftTop,
    RightDown,
    RightTop
}
public enum MarginType
{
    None,
    Margin,
    MarginRatio,
    MarginX,
    MarginY,
    MarginRatioX,
    MarginRatioY
}
public enum ParentType
{
    Tranfrom,Screen, BangsScreen
}
[Serializable]
public struct Margin
{
    /// <summary>
    /// pivot.x 0-1
    /// </summary>
    public float left;
    /// <summary>
    /// pivot.y 0-1
    /// </summary>
    public float down;
    /// <summary>
    /// size.x
    /// </summary>
    public float right;
    /// <summary>
    /// size.y
    /// </summary>
    public float top;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="l"></param>
    /// <param name="r"></param>
    /// <param name="d"></param>
    /// <param name="t"></param>
    public Margin(float l, float r, float t, float d) { left = l;right = r;down = d;top = t; }
    public Margin(Vector4 v) { left = v.x; right = v.z; top = v.w; down = v.y; }
    public Margin(Rect v) { left = v.x; right = v.width; top = v.height; down = v.y; }
}
[Serializable]
public struct Border
{
    /// <summary>
    /// pivot.x 0-1
    /// </summary>
    public float left;
    /// <summary>
    /// pivot.y 0-1
    /// </summary>
    public float down;
    /// <summary>
    /// size.x
    /// </summary>
    public float right;
    /// <summary>
    /// size.y
    /// </summary>
    public float top;
    public Border(float l, float r, float t, float d) { left = l; right = r; down = d; top = t; }
    public Border(Vector2 size,Vector2 pivot)
    {
        left = size.x * -pivot.x;
        right = size.x + left;
        down = size.y * -pivot.y;
        top = size.y + down;
    }
    public static Border GetBorder(ref Vector2 size,ref  Vector2 pivot)
    {
        float lx = size.x * -pivot.x;
        float rx = size.x + lx;
        float dy = size.y * -pivot.y;
        float ty = size.y + dy;
        return new Border(lx, rx, ty, dy);
    }
    public static Border operator *(Border a, float d)
    {
        a.left *= d;
        a.right *= d;
        a.top *= d;
        a.down *= d;
        return a;
    }
}

public class Scale
{
    public static float LayoutWidth=720;
    public static float LayoutHeight = 1280;
    public static float ScreenWidth=720;
    public static float ScreenHeight=1280;
    static float ScreenCurrentWidth ;
    static float ScreenCurrentHeight;

    public static float NormalDpi = 192;
    public static float ScreenDpi;
    public static float DpiRatio;
    public static void Initial()
    {
        ScreenDpi = Screen.dpi;
        DpiRatio = ScreenDpi / NormalDpi;
    }
    public static void MainUpdate()
    {
        ScreenDpi = Screen.dpi;
        DpiRatio = ScreenDpi / NormalDpi;
        ScreenCurrentWidth = Screen.width;
        ScreenCurrentHeight = Screen.height;
    }
    public static bool DpiScale;
    public static bool ScreenChanged()
    {
        if (ScreenCurrentWidth != ScreenWidth | ScreenCurrentHeight != ScreenHeight)
        {
            ScreenWidth = ScreenCurrentWidth;
            ScreenHeight = ScreenCurrentHeight;
            if(DpiScale)
            {
                LayoutWidth = ScreenWidth / DpiRatio;
                LayoutHeight = ScreenHeight / DpiRatio;
            }
            else
            {
                LayoutWidth = ScreenWidth;
                LayoutHeight = ScreenHeight;
            }
            return true;
        }
        return false;
    }
    public static float ScreenSize 
    {
        get
        {
            float w = Screen.width;
            float h = Screen.height;
            float r = Mathf.Sqrt(w * w + h * h);
#if UNITY_EDITOR
            return r / 334;
#else
            return r /Screen.dpi;
#endif
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="norSize">5</param>
    /// <param name="minSize">4</param>
    /// <param name="maxSize">7</param>
    /// <returns></returns>
    public static float GetScreenScale(float norSize,float minSize,float maxSize)
    {
        float s = Screen.width * Screen.width + Screen.height * Screen.height;
        float r = Mathf.Sqrt(s);
        if (r > maxSize)
            r = maxSize;
        else if (r < minSize)
            r = minSize;
        r /= norSize;
        return r;
    }
}