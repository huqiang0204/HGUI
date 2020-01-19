using huqiang;
using huqiang.Core.HGUI;
using huqiang.Data;
using huqiang.UIComposite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class UICompositeMenu
{
    private const string icons = "icons";
    private const string Aim = "Aim";
    private const string background = "Background2";
    private const string file= "Pinned-Notices";
    private const string close = "Close";
    private const string list = "list";
    private const string line = "Line";
    private const string leaves = "Leaves";
    private const string ufo = "Ufo";
    private const string circleol = "Circle-Outline";
    private const string circlesm = "Circle-Small";
    private const string magicstick = "Magic-Stick";
    private const string box = "Border2";

    [MenuItem("GameObject/HGUI/Canvas", false, 0)]
    static public void AddCanvas(MenuCommand menuCommand)
    {
        GameObject parent = menuCommand.context as GameObject;
        var go = new GameObject("Canvas", typeof(HCanvas));
        var trans = go.transform;
        trans.localPosition = Vector3.zero;
        trans.localScale = Vector3.one;
        trans.localRotation = Quaternion.identity;
        go.AddComponent<MeshRenderer>();
        go.AddComponent<MeshFilter>();
    }
    [MenuItem("GameObject/HGUI/Page", false, 1)]
    static public void AddPage(MenuCommand menuCommand)
    {
        string path = EditorUtility.SaveFilePanel("CreatePage", Application.dataPath,"page", "cs");
        FileStream fs1 = new FileStream(path, FileMode.Create, FileAccess.Write);//创建写入文件 
        StreamWriter sw = new StreamWriter(fs1);
        var paths = path.Split('/');
        var classname = paths[paths.Length - 1];
        classname = classname.Substring(0, classname.Length - 3);
        sw.WriteLine(UIPageModel.GetPageModel(classname));//开始写入值
        sw.Close();
        fs1.Close();
        Debug.Log(classname + ".cs Create done");
        GameObject parent = menuCommand.context as GameObject;
        var go = new GameObject(classname, typeof(UIElement));
        var trans = go.transform;
        if (parent != null)
        {
            HCanvas canvas = parent.GetComponent<HCanvas>();
            if (canvas != null)
                trans.SetParent(canvas.transform);
            else
            {
                canvas = UnityEngine.Object.FindObjectOfType<HCanvas>();
                if (canvas != null)
                    trans.SetParent(canvas.transform);
                else
                {
                    //创建Canvas并设置父级
                }
            }
        }
        else
        {
            var canvas = UnityEngine.Object.FindObjectOfType<HCanvas>();
            if (canvas != null)
                trans.SetParent(canvas.transform);
            else
            {
                //创建Canvas并设置父级
            }
        }

        trans.localPosition = Vector3.zero;
        trans.localScale = Vector3.one;
        trans.localRotation = Quaternion.identity;
    }
    [MenuItem("GameObject/HGUI/Empty", false, 1)]
    static public void AddEmpty(MenuCommand menuCommand)
    {
        GameObject parent = menuCommand.context as GameObject;
        var go = new GameObject("Empty", typeof(UIElement));
        var trans = go.transform;
        if (parent != null)
            trans.SetParent(parent.transform);
        trans.localPosition = Vector3.zero;
        trans.localScale = Vector3.one;
        trans.localRotation = Quaternion.identity;
    }
    [MenuItem("GameObject/HGUI/Image", false, 2)]
    static public void AddImage(MenuCommand menuCommand)
    {
        GameObject parent = menuCommand.context as GameObject;
        var go = new GameObject("Image", typeof(HImage));
        var trans = go.transform;
        if (parent != null)
            trans.SetParent(parent.transform);
        trans.localPosition = Vector3.zero;
        trans.localScale = Vector3.one;
        trans.localRotation = Quaternion.identity;
    }
    [MenuItem("GameObject/HGUI/Text", false, 3)]
    static public void AddText(MenuCommand menuCommand)
    {
        GameObject parent = menuCommand.context as GameObject;
        var go = new GameObject("Text", typeof(HText));
        var trans = go.transform;
        if (parent != null)
            trans.SetParent(parent.transform);
        trans.localPosition = Vector3.zero;
        trans.localScale = Vector3.one;
        trans.localRotation = Quaternion.identity;
    }
    [MenuItem("GameObject/HGUI/InputBox", false, 4)]
    static public void AddInputBox(MenuCommand menuCommand)
    {
        GameObject parent = menuCommand.context as GameObject;

        var go = new GameObject("InputBox", typeof(HImage));
        var trans = go.transform;
        if (parent != null)
            trans.SetParent(parent.transform);
        trans.localPosition = Vector3.zero;
        trans.localScale = Vector3.one;
        trans.localRotation = Quaternion.identity;
        var img = go.GetComponent<HImage>();
        img.Sprite = EditorModelManager.FindSprite(icons, box);
        img.SprType = SpriteType.Sliced;
        img.SizeDelta = new Vector2(400,100);

        go = new GameObject("InputText", typeof(HText));
        var son = go.transform;
        son.SetParent(trans);
        son.localPosition = Vector3.zero;
        son.localScale = Vector3.one;
        son.localRotation = Quaternion.identity;
        var txt = go.GetComponent<HText>();
        txt.SizeDelta = new Vector2(380,90);
        txt.eventType = huqiang.Core.HGUI.EventType.TextInput;
        txt.marginType = MarginType.Margin;
        txt.margin = new Margin(5,5,5,5);
        var help = go.AddComponent<TextInputHelper>();
        help.Refresh();
    }
    [MenuItem("GameObject/HGUI/UISliderH", false, 4)]
    static public void AddSliderH(MenuCommand menuCommand)
    {
        GameObject parent = menuCommand.context as GameObject;
        var go = CreateSliderH();
        if (parent != null)
        {
            var rect = go.transform;
            rect.SetParent(parent.transform);
            rect.localPosition = Vector3.zero;
            rect.localScale = Vector3.one;
            rect.localRotation = Quaternion.identity;
        }
    }
    static GameObject CreateSliderH()
    {
        var go = new GameObject("SliderH", typeof(HImage));
        HImage image = go.GetComponent<HImage>();
        image.SizeDelta = new Vector2(400, 20);
        image.compositeType = CompositeType.Slider;
        var rect = go.transform;

        var help = go.AddComponent<SliderHelper>();
        help.StartOffset.x = -15;
        help.EndOffset.x = -15;
        image.Sprite = EditorModelManager.FindSprite(icons, background);
        image.SprType = SpriteType.Sliced;

        var Fill = new GameObject("FillImage", typeof(HImage));
        image = Fill.GetComponent<HImage>();
        image.SizeDelta = new Vector2(400, 20);
        var son = image.transform;
        son.SetParent(rect);
        son.localPosition = Vector3.zero;
        son.localScale = Vector3.one;
        son.localRotation = Quaternion.identity;
        image.Sprite = EditorModelManager.FindSprite(icons, background);
        image.SprType = SpriteType.Filled;
        image.FillMethod = FillMethod.Horizontal;
        image.Chromatically = new Color32(94, 137, 197, 255);

        var Nob = new GameObject("Nob", typeof(HImage));
        image = Nob.GetComponent<HImage>();
        image.SizeDelta = new Vector2(30, 30);
        son = image.transform;
        son.SetParent(rect);
        son.localPosition = new Vector3(200, 0, 0);
        son.localScale = Vector3.one;
        son.localRotation = Quaternion.identity;
        image.Chromatically = Color.green;
        image.Sprite = EditorModelManager.FindSprite(icons, leaves);
        return go;
    }
    [MenuItem("GameObject/HGUI/UISliderV", false, 5)]
    static public void AddSliderV(MenuCommand menuCommand)
    {
        GameObject parent = menuCommand.context as GameObject;
        var go = CreateSliderV();
        if (parent != null)
        {
            var rect = go.transform;
            rect.SetParent(parent.transform);
            rect.localPosition = Vector3.zero;
            rect.localScale = Vector3.one;
            rect.localRotation = Quaternion.identity;
        }
    }
    static GameObject CreateSliderV()
    {
        var go = new GameObject("SliderV", typeof(HImage));
        HImage image = go.GetComponent<HImage>();
        image.SizeDelta = new Vector2(20, 400);
        image.compositeType = CompositeType.Slider;
        var rect = image.transform;

        var help = go.AddComponent<SliderHelper>();
        help.StartOffset.y = -15;
        help.EndOffset.y = -15;
        image.Sprite = EditorModelManager.FindSprite(icons, background);
        image.SprType = SpriteType.Sliced;

        var Fill = new GameObject("FillImage", typeof(HImage));
        image = Fill.GetComponent<HImage>();
        image.SizeDelta = new Vector2(20, 400);
        var son = image.transform;
        son.SetParent(rect);
        son.localPosition = Vector3.zero;
        son.localScale = Vector3.one;
        son.localRotation = Quaternion.identity;
        image.Sprite = EditorModelManager.FindSprite(icons, background);
        image.SprType = SpriteType.Filled;
        image.FillMethod = FillMethod.Vertical;
        image.Chromatically = new Color32(94, 137, 197, 255);

        var Nob = new GameObject("Nob", typeof(HImage));
        image = Nob.GetComponent<HImage>();
        image.SizeDelta = new Vector2(30, 30);
        son = image.transform;
        son.SetParent(rect);
        son.localPosition = new Vector3(0, 200, 0);
        son.localScale = Vector3.one;
        son.localRotation = Quaternion.identity;
        image.Chromatically = Color.green;
        image.Sprite = EditorModelManager.FindSprite(icons, ufo);
        return go;
    }
    [MenuItem("GameObject/HGUI/Scroll", false, 6)]
    static public void AddScroll(MenuCommand menuCommand)
    {
        GameObject parent = menuCommand.context as GameObject;
        var Scroll = CreateScroll();
        if (parent != null)
        {
            var rect = Scroll.transform;
            rect.SetParent(parent.transform);
            rect.localPosition = Vector3.zero;
            rect.localScale = Vector3.one;
            rect.localRotation = Quaternion.identity;
        }
    }
    static GameObject CreateScroll()
    {
        var ss = new GameObject("Scroll");
        var img = ss.AddComponent<HImage>();
        img.Mask = true;
        img.SizeDelta = new Vector2(400,400);
        img.compositeType = CompositeType.ScrollY;
        ss.AddComponent<ScrollHelper>();
        var rect = img.transform;
        var Item = new GameObject("Item");
        var fr = Item.AddComponent<UIElement>();
        fr.SizeDelta = new Vector2(80, 80);
        var son = fr.transform;
        son.SetParent(rect);
        son.localPosition = Vector3.zero;
        son.localScale = Vector3.one;
        son.localRotation = Quaternion.identity;
        return ss;
    }
    //[MenuItem("GameObject/UIComposite/ScrollY", false, 4)]
    static public void AddScrollY(MenuCommand menuCommand)
    {
        GameObject parent = menuCommand.context as GameObject;
        var Scroll = new GameObject("ScrollY", typeof(RectTransform));
        if (parent != null)
            Scroll.transform.SetParent(parent.transform);
        Scroll.transform.localPosition = Vector3.zero;
        Scroll.transform.localScale = Vector3.one;

        var ss = new GameObject("Scroll", typeof(RectTransform));
        RectTransform rect = ss.transform as RectTransform;
        rect.SetParent(Scroll.transform);
        rect.sizeDelta = new Vector2(400, 400);
        rect.localPosition = Vector3.zero;
        rect.localScale = Vector3.one;
        ss.AddComponent<RectMask2D>();
        var Item = new GameObject("Item", typeof(RectTransform));
        var fr = Item.transform as RectTransform;
        fr.sizeDelta = new Vector2(80, 80);
        fr.SetParent(rect);
        fr.localPosition = Vector3.zero;
        fr.localScale = Vector3.one;

        ss = new GameObject("Slider", typeof(RectTransform));
        var fn = ss.transform as RectTransform;
        fn.sizeDelta = new Vector2(20, 400);
        fn.SetParent(Scroll.transform);
        fn.localPosition = new Vector3(190, 0, 0);
        fn.localScale = Vector3.one;
        var img = ss.AddComponent<Image>();
        img.sprite = EditorModelManager.FindSprite(icons, background);
        img.type = Image.Type.Sliced;
        img.color = new Color32(152, 152, 152, 255);
        var help = ss.AddComponent<SliderHelper>();
        help.direction = UISlider.Direction.Vertical;
        help.StartOffset.y = 15;
        help.EndOffset.y = 15;

        var Nob = new GameObject("Nob", typeof(RectTransform));
        fn = Nob.transform as RectTransform;
        fn.sizeDelta = new Vector2(30, 30);
        fn.SetParent(ss.transform);
        fn.localPosition = new Vector3(0, -185, 0);
        fn.localScale = Vector3.one;
        img = Nob.AddComponent<Image>();
        img.sprite = EditorModelManager.FindSprite(icons, circlesm);
    }
    //[MenuItem("GameObject/UIComposite/UIRocker", false, 5)]
    static public void AddRocker(MenuCommand menuCommand)
    {
        GameObject parent = menuCommand.context as GameObject;
        var ss = new GameObject("Rocker", typeof(RectTransform));
        RectTransform rect = ss.transform as RectTransform;
        rect.sizeDelta = new Vector2(300, 300);
        if (parent != null)
            rect.SetParent(parent.transform);
        rect.localPosition = Vector3.zero;
        rect.localScale = Vector3.one;
        var img = ss.AddComponent<Image>();
        img.sprite = EditorModelManager.FindSprite(icons, circleol);
        var Item = new GameObject("Nob", typeof(RectTransform));
        var fr = Item.transform as RectTransform;
        fr.sizeDelta = new Vector2(100, 100);
        fr.SetParent(rect);
        fr.localPosition = Vector3.zero;
        fr.localScale = Vector3.one;
        img = Item.AddComponent<Image>();
        img.sprite = EditorModelManager.FindSprite(icons, circlesm);
    }
    //[MenuItem("GameObject/UIComposite/UIPalette", false, 6)]
    static public void AddPalette(MenuCommand menuCommand)
    {
        GameObject parent = menuCommand.context as GameObject;
        var palette = new GameObject("Palette", typeof(RectTransform));
        RectTransform rect = palette.transform as RectTransform;
        rect.sizeDelta = new Vector2(500, 500);
        if (parent != null)
            rect.SetParent(parent.transform);
        rect.localPosition = Vector3.zero;
        rect.localScale = Vector3.one;
        palette.AddComponent<RawImage>();


        var Fill = new GameObject("HTemplate", typeof(RectTransform));
        var fr = Fill.transform as RectTransform;
        fr.sizeDelta = new Vector2(256, 256);
        fr.SetParent(rect);
        fr.localPosition = Vector3.zero;
        fr.localScale = Vector3.one;
        Fill.AddComponent<RawImage>();

        var Nob = new GameObject("NobA", typeof(RectTransform));
        var fn = Nob.transform as RectTransform;
        fn.sizeDelta = new Vector2(44, 44);
        fn.SetParent(rect);
        fn.localPosition = new Vector3(0, -220, 0);
        fn.localScale = Vector3.one;
        var img = Nob.AddComponent<Image>();
        var aim = img.sprite = EditorModelManager.FindSprite(icons, Aim);

        Nob = new GameObject("NobB", typeof(RectTransform));
        fn = Nob.transform as RectTransform;
        fn.sizeDelta = new Vector2(24, 24);
        fn.SetParent(rect);
        fn.localPosition = new Vector3(-128, 128, 0);
        fn.localScale = Vector3.one;
        Nob.AddComponent<Image>().sprite = aim;

        var Slider = new GameObject("Slider", typeof(RectTransform));
        fn = Slider.transform as RectTransform;
        fn.sizeDelta = new Vector2(400, 20);
        fn.SetParent(rect);
        fn.localPosition = new Vector3(0, -285, 0);
        fn.localScale = Vector3.one;
        Slider.AddComponent<RawImage>();

        Nob = new GameObject("Nob", typeof(RectTransform));
        fn = Nob.transform as RectTransform;
        fn.sizeDelta = new Vector2(30, 30);
        fn.SetParent(Slider.transform);
        fn.localPosition = new Vector3(200, 0, 0);
        fn.localScale = Vector3.one;
        img = Nob.AddComponent<Image>();
        img.color = new Color(1, 1, 1, 1f);
        img.sprite = aim;

        palette.AddComponent<PaletteHelper>().Initial();
    }
    //[MenuItem("GameObject/UIComposite/UIDate", false, 7)]
    static public void AddDate(MenuCommand menuCommand)
    {
        GameObject parent = menuCommand.context as GameObject;
        var date = new GameObject("Date", typeof(RectTransform));
        RectTransform rect = date.transform as RectTransform;
        rect.sizeDelta = new Vector2(360, 210);
        if (parent != null)
            rect.SetParent(parent.transform);
        rect.localPosition = Vector3.zero;
        rect.localScale = Vector3.one;

        var now = DateTime.Now;
        var Year = new GameObject("Year", typeof(RectTransform));
        var fr = Year.transform as RectTransform;
        fr.sizeDelta = new Vector2(120, 210);
        fr.SetParent(rect);
        fr.localPosition = new Vector3(-120, 0, 0);
        fr.localScale = Vector3.one;
        Year.AddComponent<Image>();
        Year.AddComponent<Mask>().showMaskGraphic = false;

        var Item = new GameObject("Item", typeof(RectTransform));
        var fn = Item.transform as RectTransform;
        fn.sizeDelta = new Vector2(120, 30);
        fn.SetParent(fr);
        fn.localPosition = Vector3.zero;
        fn.localScale = Vector3.one;
        var txt = Item.AddComponent<Text>();
        txt.alignment = TextAnchor.MiddleCenter;
        txt.text = now.Year + " Year";
        txt.fontSize = 24;

        var Month = new GameObject("Month", typeof(RectTransform));
        fr = Month.transform as RectTransform;
        fr.sizeDelta = new Vector2(120, 210);
        fr.SetParent(rect);
        fr.localPosition = Vector3.zero;
        fr.localScale = Vector3.one;
        Month.AddComponent<RectMask2D>();

        Item = new GameObject("Item", typeof(RectTransform));
        fn = Item.transform as RectTransform;
        fn.sizeDelta = new Vector2(120, 30);
        fn.SetParent(fr);
        fn.localPosition = Vector3.zero;
        fn.localScale = Vector3.one;
        txt = Item.AddComponent<Text>();
        txt.alignment = TextAnchor.MiddleCenter;
        txt.text = now.Month + " Month";
        txt.fontSize = 24;

        var Day = new GameObject("Day", typeof(RectTransform));
        fr = Day.transform as RectTransform;
        fr.sizeDelta = new Vector2(120, 210);
        fr.SetParent(rect);
        fr.localPosition = new Vector3(120, 0, 0);
        fr.localScale = Vector3.one;
        Day.AddComponent<RectMask2D>();

        Item = new GameObject("Item", typeof(RectTransform));
        fn = Item.transform as RectTransform;
        fn.sizeDelta = new Vector2(120, 30);
        fn.SetParent(fr);
        fn.localPosition = Vector3.zero;
        fn.localScale = Vector3.one;
        txt = Item.AddComponent<Text>();
        txt.alignment = TextAnchor.MiddleCenter;
        txt.text = now.Day + " Day";
        txt.fontSize = 24;
    }
    [MenuItem("GameObject/HGUI/TreeView", false, 7)]
    static public void AddTreeView(MenuCommand menuCommand)
    {
        GameObject parent = menuCommand.context as GameObject;
        var ss = new GameObject("TreeView");
        Transform rect = ss.transform;
        UIElement uI = ss.AddComponent<UIElement>();
        uI.SizeDelta = new Vector2(400, 400);
        uI.Mask = true;
        uI.compositeType = CompositeType.TreeView;
        if (parent != null)
            rect.SetParent(parent.transform);
        rect.localPosition = Vector3.zero;
        rect.localScale = Vector3.one;
        var Item = new GameObject("Item");
        var fr = Item.transform;
        fr.SetParent(rect);
        fr.localPosition = Vector3.zero;
        fr.localScale = Vector3.one;
        var txt = Item.AddComponent<HText>();
        txt.SizeDelta = new Vector2(200,40);
        txt.TextAnchor = TextAnchor.MiddleLeft;
        txt.FontSize = 24;
    }
    //[MenuItem("GameObject/UIComposite/DropDown", false, 9)]
    static public void AddDropDown(MenuCommand menuCommand)
    {
        GameObject parent = menuCommand.context as GameObject;
        var ss = new GameObject("DropDownEx", typeof(RectTransform));
        RectTransform rect = ss.transform as RectTransform;
        rect.sizeDelta = new Vector2(400, 40);
        if (parent != null)
            rect.SetParent(parent.transform);

        var label = new GameObject("Label", typeof(RectTransform));
        var fr = label.transform as RectTransform;
        fr.sizeDelta = new Vector2(400, 40);
        fr.SetParent(rect);
        fr.localPosition = Vector3.zero;
        fr.localScale = Vector3.one;
        var txt = label.AddComponent<Text>();
        txt.alignment = TextAnchor.MiddleLeft;
        txt.fontSize = 24;

        var Scroll = new GameObject("Scroll", typeof(RectTransform));
        RectTransform scr = Scroll.transform as RectTransform;
        scr.sizeDelta = new Vector2(400, 400);
        scr.SetParent(rect);
        scr.localPosition = new Vector3(0, -220, 0);
        scr.localScale = Vector3.one;
        ss.AddComponent<RectMask2D>();

        var Item = new GameObject("Item", typeof(RectTransform));
        fr = Item.transform as RectTransform;
        fr.sizeDelta = new Vector2(400, 40);
        fr.SetParent(scr);
        fr.localPosition = Vector3.zero;
        fr.localScale = Vector3.one;
    }
    //[MenuItem("GameObject/UIComposite/TabControl", false, 10)]
    //static public void AddLayout(MenuCommand menuCommand)
    //{
    //    var parent = menuCommand.context as GameObject;
    //    if (parent == null)
    //        return;
    //    var tab = new GameObject("TabControl", typeof(RectTransform));
    //    tab.transform.SetParent(parent.transform);
    //    var ss = tab.AddComponent<SizeScaleEx>();
    //    ss.marginType = MarginType.Margin;

    //    var Head = new GameObject("Head", typeof(RectTransform));
    //    Head.transform.SetParent(tab.transform);
    //    (Head.transform as RectTransform).sizeDelta = new Vector2(100, 60);

    //    var Items = new GameObject("Items");
    //    Items.transform.SetParent(Head.transform);
    //    Items.transform.localPosition = Vector3.zero;
    //    ss = Items.AddComponent<SizeScaleEx>();
    //    ss.marginType = MarginType.MarginX;

    //    var Item = new GameObject("Item", typeof(RectTransform));
    //    Item.transform.SetParent(Head.transform);
    //    Item.transform.localPosition = Vector3.zero;
    //    (Item.transform as RectTransform).sizeDelta = new Vector2(100, 50);

    //    var back = new GameObject("Back", typeof(RectTransform));
    //    back.transform.SetParent(Item.transform);
    //    var img = back.AddComponent<Image>();
    //    img.color = 0x2555FFff.ToColor();
    //    ss = back.AddComponent<SizeScaleEx>();
    //    ss.marginType = MarginType.Margin;

    //    var label = new GameObject("Label", typeof(RectTransform));
    //    label.transform.SetParent(Item.transform);
    //    (label.transform as RectTransform).sizeDelta = new Vector2(100, 50);
    //    label.transform.localPosition = new Vector3(-20, 0, 0);
    //    var txt = label.AddComponent<Text>();
    //    txt.alignment = TextAnchor.MiddleLeft;
    //    txt.fontSize = 30;

    //    var line = new GameObject("Line", typeof(RectTransform));
    //    line.transform.SetParent(Head.transform);
    //    (line.transform as RectTransform).sizeDelta = new Vector2(100, 4);
    //    line.transform.localPosition = new Vector3(0, -24, 0);
    //    ss = line.AddComponent<SizeScaleEx>();
    //    ss.marginType = MarginType.MarginX;

    //    var content = new GameObject("Content", typeof(RectTransform));
    //    content.transform.SetParent(tab.transform);
    //    content.transform.localPosition = Vector3.zero;
    //}
    //[MenuItem("GameObject/UIComposite/DropdownEx", false, 11)]
    //static public void AddDropdownEx(MenuCommand menuCommand)
    //{
    //    GameObject parent = menuCommand.context as GameObject;
    //    var drop = new GameObject("DropdownEx", typeof(RectTransform));
    //    RectTransform dt = drop.transform as RectTransform;
    //    if (parent != null)
    //        dt.SetParent(parent.transform);
    //    dt.sizeDelta = new Vector2(400, 60);
    //    var Label = new GameObject("Label", typeof(RectTransform), typeof(Text));
    //    var st = Label.transform as RectTransform;
    //    st.SetParent(dt);
    //    st.localPosition = new Vector3(-30, 0, 0);
    //    st.localScale = Vector3.one;
    //    st.sizeDelta = new Vector2(340, 40);
    //    var txt = Label.GetComponent<Text>();
    //    txt.color = Color.white;
    //    txt.fontSize = 32;
    //    txt.text = "Label";
    //    txt.alignment = TextAnchor.MiddleCenter;

    //    var Close = new GameObject("Menu", typeof(RectTransform), typeof(Image));
    //    st = Close.transform as RectTransform;
    //    st.SetParent(dt);
    //    st.localPosition = new Vector3(170, 0, 0);
    //    st.localScale = new Vector3(1, 1, 1);
    //    st.sizeDelta = new Vector2(48, 36);
    //    var img = Close.GetComponent<Image>();
    //    img.color = Color.white;
    //    img.sprite = EditorModelManager.FindSprite(icons, list);

    //    var ss = new GameObject("Scroll", typeof(RectTransform));
    //    RectTransform rect = ss.transform as RectTransform;
    //    rect.sizeDelta = new Vector2(400, 400);
    //    rect.SetParent(dt);
    //    rect.localPosition = new Vector3(0, -230, 0);
    //    rect.localScale = Vector3.one;
    //    ss.AddComponent<RectMask2D>();
    //    var Item = new GameObject("Item", typeof(RectTransform));
    //    var fr = Item.transform as RectTransform;
    //    fr.sizeDelta = new Vector2(400, 80);
    //    fr.SetParent(rect);
    //    fr.localPosition = Vector3.zero;
    //    fr.localScale = Vector3.one;

    //    Label = new GameObject("Label", typeof(RectTransform), typeof(Text));
    //    st = Label.transform as RectTransform;
    //    st.SetParent(fr);
    //    st.localPosition = new Vector3(-30, 0, 0);
    //    st.localScale = Vector3.one;
    //    st.sizeDelta = new Vector2(400, 60);
    //    txt = Label.GetComponent<Text>();
    //    txt.color = Color.white;
    //    txt.fontSize = 32;
    //    txt.text = "Label";
    //    txt.alignment = TextAnchor.MiddleCenter;

    //    dt.localPosition = Vector3.zero;
    //    dt.localScale = Vector3.one;
    //}
    //[MenuItem("GameObject/UIComposite/DockPanel", false, 13)]
    //static public void AddDockPanel(MenuCommand menuCommand)
    //{
    //    var game = menuCommand.context as GameObject;
    //    if (game == null)
    //        return;
    //    CreateDockPanel(game);
    //}
    //[MenuItem("GameObject/UIComposite/DesignedDockPanel", false, 14)]
    //static public void AddDesignedDockPanel(MenuCommand menuCommand)
    //{
    //    var game = menuCommand.context as GameObject;
    //    if (game == null)
    //        return;
    //    var obj = CreateDockPanel(game);
    //    CreateAuxiliary(obj);

    //    var Drag = new GameObject("Drag", typeof(RectTransform), typeof(Image));
    //    var st = Drag.transform as RectTransform;
    //    st.SetParent(obj.transform);
    //    st.localScale = Vector3.one;
    //    st.sizeDelta = new Vector2(60, 60);
    //    var img = Drag.GetComponent<Image>();
    //    img.color = Color.green;
    //    img.sprite = EditorModelManager.FindSprite(icons, file);
    //}
    //static GameObject CreateDockPanel(GameObject parent)
    //{
    //    var dp = new GameObject("DockPanel", typeof(RectTransform));
    //    RectTransform rect = dp.transform as RectTransform;
    //    rect.sizeDelta = new Vector2(1920, 1080);
    //    if (parent != null)
    //        rect.SetParent(parent.transform);
    //    var sse = dp.AddComponent<SizeScaleEx>();
    //    sse.anchorPointType = AnchorPointType.Cneter;
    //    sse.marginType = MarginType.Margin;
    //    sse.parentType = ParentType.Tranfrom;
    //    sse.DesignSize = new Vector2(1920, 1080);

    //    var AreaLevel = new GameObject("AreaLevel", typeof(RectTransform));
    //    AreaLevel.transform.SetParent(rect);
    //    var LineLevel = new GameObject("LineLevel", typeof(RectTransform));
    //    LineLevel.transform.SetParent(rect);
    //    var Line = new GameObject("Line", typeof(RectTransform), typeof(Image));
    //    Line.transform.SetParent(rect);
    //    Line.GetComponent<Image>().color = new Color32(64, 64, 64, 255);
    //    var Area = new GameObject("Area", typeof(RectTransform), typeof(Image));
    //    Area.transform.SetParent(rect);
    //    return dp;
    //}
    //static void CreateAuxiliary(GameObject parent)
    //{
    //    Sprite bk = EditorModelManager.FindSprite(icons, background);
    //    var rect = parent.transform;
    //    var Auxiliary = new GameObject("Auxiliary", typeof(RectTransform));
    //    Auxiliary.transform.SetParent(rect);
    //    CreateDockTabControl(Auxiliary);
    //    var ss = Auxiliary.AddComponent<SizeScaleEx>();
    //    ss.marginType = MarginType.Margin;


    //    var Cover = new GameObject("Cover", typeof(RectTransform), typeof(RawImage));
    //    Cover.transform.SetParent(Auxiliary.transform);
    //    Cover.GetComponent<RawImage>().color = new Color32(128, 128, 128, 128);

    //    var Docker = new GameObject("Docker", typeof(RectTransform));
    //    Docker.transform.SetParent(Auxiliary.transform);

    //    var Center = new GameObject("Center", typeof(RectTransform), typeof(Image));
    //    var st = Center.transform as RectTransform;
    //    st.SetParent(Docker.transform);
    //    st.localPosition = Vector3.zero;
    //    st.localScale = Vector3.one;
    //    st.sizeDelta = new Vector2(100, 100);
    //    var img = Center.GetComponent<Image>();
    //    img.color = new Color32(59, 87, 255, 128);
    //    img.sprite = bk;

    //    var Left = new GameObject("Left", typeof(RectTransform), typeof(Image));
    //    st = Left.transform as RectTransform;
    //    st.SetParent(Docker.transform);
    //    st.localPosition = new Vector3(-90, 0, 0);
    //    st.localScale = Vector3.one;
    //    st.sizeDelta = new Vector2(60, 100);
    //    img = Left.GetComponent<Image>();
    //    img.color = new Color32(59, 87, 255, 128);
    //    img.sprite = bk;

    //    var Top = new GameObject("Top", typeof(RectTransform), typeof(Image));
    //    st = Top.transform as RectTransform;
    //    st.SetParent(Docker.transform);
    //    st.localPosition = new Vector3(0, 90, 0);
    //    st.localScale = Vector3.one;
    //    st.sizeDelta = new Vector2(100, 60);
    //    img = Top.GetComponent<Image>();
    //    img.color = new Color32(59, 87, 255, 128);
    //    img.sprite = bk;

    //    var Right = new GameObject("Right", typeof(RectTransform), typeof(Image));
    //    st = Right.transform as RectTransform;
    //    st.SetParent(Docker.transform);
    //    st.localPosition = new Vector3(90, 0, 0);
    //    st.localScale = Vector3.one;
    //    st.sizeDelta = new Vector2(60, 100);
    //    img = Right.GetComponent<Image>();
    //    img.color = new Color32(59, 87, 255, 128);
    //    img.sprite = bk;

    //    var Down = new GameObject("Down", typeof(RectTransform), typeof(Image));
    //    st = Down.transform as RectTransform;
    //    st.SetParent(Docker.transform);
    //    st.localPosition = new Vector3(0, -90, 0);
    //    st.localScale = Vector3.one;
    //    st.sizeDelta = new Vector2(100, 60);
    //    img = Down.GetComponent<Image>();
    //    img.color = new Color32(59, 87, 255, 128);
    //    img.sprite = bk;
    //}
    //static void CreateDockTabControl(GameObject parent)
    //{
    //    var tab = new GameObject("TabControl", typeof(RectTransform));
    //    tab.transform.SetParent(parent.transform);
    //    var ss = tab.AddComponent<SizeScaleEx>();
    //    ss.marginType = MarginType.Margin;

    //    var Head = new GameObject("Head", typeof(RectTransform));
    //    Head.transform.SetParent(tab.transform);
    //    (Head.transform as RectTransform).sizeDelta = new Vector2(100, 60);

    //    var Items = new GameObject("Items", typeof(RectTransform));
    //    Items.transform.SetParent(Head.transform);
    //    Items.transform.localPosition = Vector3.zero;
    //    (Items.transform as RectTransform).sizeDelta = new Vector2(100, 50);
    //    ss = Items.AddComponent<SizeScaleEx>();
    //    ss.marginType = MarginType.MarginX;
    //    Items.AddComponent<UILayout>().type = huqiang.UI.LayoutType.StackPanelH;
    //    Items.AddComponent<RectMask2D>();


    //    var Item = new GameObject("Item", typeof(RectTransform));
    //    Item.transform.SetParent(Head.transform);
    //    Item.transform.localPosition = Vector3.zero;
    //    (Item.transform as RectTransform).sizeDelta = new Vector2(100, 50);

    //    var back = new GameObject("Back", typeof(RectTransform));
    //    back.transform.SetParent(Item.transform);
    //    var img = back.AddComponent<Image>();
    //    img.color = 0x2555FFff.ToColor();
    //    ss = back.AddComponent<SizeScaleEx>();
    //    ss.marginType = MarginType.MarginY;

    //    var label = new GameObject("Label", typeof(RectTransform));
    //    label.transform.SetParent(Item.transform);
    //    (label.transform as RectTransform).sizeDelta = new Vector2(60, 50);
    //    label.transform.localPosition = new Vector3(-20, 0, 0);
    //    var txt = label.AddComponent<Text>();
    //    txt.alignment = TextAnchor.MiddleLeft;
    //    txt.fontSize = 30;

    //    var clo = new GameObject("Close", typeof(RectTransform));
    //    clo.transform.SetParent(Item.transform);
    //    (clo.transform as RectTransform).sizeDelta = new Vector2(40, 40);
    //    clo.transform.localPosition = new Vector3(30, 0, 0);
    //    img = clo.AddComponent<Image>();
    //    img.color = Color.white;
    //    img.sprite = EditorModelManager.FindSprite(icons, close);

    //    var line = new GameObject("Line", typeof(RectTransform));
    //    line.transform.SetParent(Head.transform);
    //    (line.transform as RectTransform).sizeDelta = new Vector2(100, 4);
    //    line.transform.localPosition = new Vector3(0, -24, 0);
    //    ss = line.AddComponent<SizeScaleEx>();
    //    ss.marginType = MarginType.MarginX;
    //    img = line.AddComponent<Image>();
    //    img.color = 0x5379FFff.ToColor();

    //    var content = new GameObject("Content", typeof(RectTransform));
    //    content.transform.SetParent(tab.transform);
    //    content.transform.localPosition = Vector3.zero;
    //}
}