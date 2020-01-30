using huqiang;
using huqiang.Core.HGUI;
using huqiang.Data;
using huqiang.UIComposite;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

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
    private const string diamond = "Diamond-Outline5";
    private const string star = "Star";

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
    [MenuItem("GameObject/HGUI/Empty", false, 2)]
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
    [MenuItem("GameObject/HGUI/Image", false, 3)]
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
    [MenuItem("GameObject/HGUI/Text", false, 4)]
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
    [MenuItem("GameObject/HGUI/InputBox", false, 5)]
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
    [MenuItem("GameObject/HGUI/UISliderH", false, 6)]
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
    [MenuItem("GameObject/HGUI/UISliderV", false, 7)]
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
    static GameObject CreateSliderV(float startOffset=-15,float endOffset=-15)
    {
        var go = new GameObject("SliderV", typeof(HImage));
        HImage image = go.GetComponent<HImage>();
        image.SizeDelta = new Vector2(20, 400);
        image.compositeType = CompositeType.Slider;
        var rect = image.transform;

        var help = go.AddComponent<SliderHelper>();
        help.StartOffset.y = startOffset;
        help.EndOffset.y = endOffset;
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
        son.localPosition = new Vector3(0, 200+startOffset, 0);
        son.localScale = Vector3.one;
        son.localRotation = Quaternion.identity;
        image.Chromatically = Color.green;
        image.Sprite = EditorModelManager.FindSprite(icons, ufo);
        return go;
    }
    [MenuItem("GameObject/HGUI/Scroll", false, 8)]
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
   
    [MenuItem("GameObject/HGUI/UIRocker", false, 9)]
    static public void AddRocker(MenuCommand menuCommand)
    {
        GameObject parent = menuCommand.context as GameObject;
        var ss = new GameObject("Rocker");
        var scr = ss.AddComponent<HImage>();
        scr.SizeDelta = new Vector2(300, 300);
        scr.eventType = huqiang.Core.HGUI.EventType.UserEvent;
        scr.compositeType = CompositeType.Rocker;
        scr.Sprite = EditorModelManager.FindSprite(icons, circleol);
        var rect = ss.transform;
        if (parent != null)
            rect.SetParent(parent.transform);
        rect.localPosition = Vector3.zero;
        rect.localScale = Vector3.one;
        rect.localRotation = Quaternion.identity;
       
        var Item = new GameObject("Nob");
        var fr = Item.AddComponent<HImage>();
        fr.SizeDelta = new Vector2(100, 100);
        fr.Sprite = EditorModelManager.FindSprite(icons, circlesm);
        var son = fr.transform;
        son.SetParent(rect);
        son.localPosition = Vector3.zero;
        son.localScale = Vector3.one;
       
    }
    [MenuItem("GameObject/HGUI/TreeView", false, 10)]
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
    [MenuItem("GameObject/HGUI/UIDate", false, 11)]
    static public void AddDate(MenuCommand menuCommand)
    {
        GameObject parent = menuCommand.context as GameObject;
        var date = new GameObject("Date");
        var main = date.AddComponent<UIElement>();
        main.SizeDelta = new Vector2(400,200);
        main.eventType = huqiang.Core.HGUI.EventType.UserEvent;
        main.compositeType = CompositeType.UIDate;
        Transform rect = date.transform;
        if (parent != null)
            rect.SetParent(parent.transform);
        rect.localPosition = Vector3.zero;
        rect.localScale = Vector3.one;
        rect.localRotation = Quaternion.identity;

        var label = new GameObject("YearLabel", typeof(HText));
        var ht = label.GetComponent<HText>();
        ht.SizeDelta = new Vector2(80, 40);
        ht.FontSize = 24;
        ht.TextAnchor = TextAnchor.MiddleCenter;
        ht.Text = "Year";
        var lt = label.transform;
        lt.SetParent(rect);
        lt.localPosition = new Vector3(-80,0,0);
        lt.localScale = Vector3.one;
        lt.localRotation = Quaternion.identity;

        label = new GameObject("MonthLabel", typeof(HText));
        ht = label.GetComponent<HText>();
        ht.SizeDelta = new Vector2(80, 40);
        ht.FontSize = 24;
        ht.TextAnchor = TextAnchor.MiddleCenter;
        ht.Text = "Month";
        lt = label.transform;
        lt.SetParent(rect);
        lt.localPosition = new Vector3(40, 0, 0);
        lt.localScale = Vector3.one;
        lt.localRotation = Quaternion.identity;

        label = new GameObject("DayLabel", typeof(HText));
        ht = label.GetComponent<HText>();
        ht.SizeDelta = new Vector2(80, 40);
        ht.FontSize = 24;
        ht.TextAnchor = TextAnchor.MiddleCenter;
        ht.Text = "Year";
        lt = label.transform;
        lt.SetParent(rect);
        lt.localPosition = new Vector3(160, 0, 0);
        lt.localScale = Vector3.one;
        lt.localRotation = Quaternion.identity;

        var now = DateTime.Now;
        var Year = new GameObject("Year", typeof(UIElement));
        var ui = Year.GetComponent<UIElement>();
        ui.SizeDelta = new Vector2(80,400);
        ui.eventType = huqiang.Core.HGUI.EventType.UserEvent;
        ui.compositeType = CompositeType.ScrollY;
        var fr = Year.transform;
        fr.SetParent(rect);
        fr.localPosition = new Vector3(-147, 0, 0);
        fr.localScale = Vector3.one;
        fr.localRotation = Quaternion.identity;

        var Item = new GameObject("Item");
        var fn = Item.transform;
        fn.SetParent(fr);
        fn.localPosition = Vector3.zero;
        fn.localScale = Vector3.one;
        fn.localRotation = Quaternion.identity;
        var txt = Item.AddComponent<HText>();
        txt.SizeDelta = new Vector2(60, 40);
        txt.TextAnchor = TextAnchor.MiddleCenter;
        txt.Text = now.Year.ToString();
        txt.FontSize = 24;

        var Month = new GameObject("Month",typeof(UIElement));
        ui = Month.GetComponent<UIElement>();
        ui.SizeDelta = new Vector2(80, 400);
        ui.eventType = huqiang.Core.HGUI.EventType.UserEvent;
        ui.compositeType = CompositeType.ScrollY;
        fr = Month.transform;
        fr.SetParent(rect);
        fr.localPosition = new Vector3(-22,0,0);
        fr.localScale = Vector3.one;
        fr.localRotation = Quaternion.identity;

        Item = new GameObject("Item");
        fn = Item.transform;
        fn.SetParent(fr);
        fn.localPosition = Vector3.zero;
        fn.localScale = Vector3.one;
        fn.localRotation = Quaternion.identity;
        txt = Item.AddComponent<HText>();
        txt.SizeDelta = new Vector2(60,40);
        txt.TextAnchor = TextAnchor.MiddleCenter;
        txt.Text = now.Month.ToString();
        txt.FontSize = 24;

        var Day = new GameObject("Day", typeof(UIElement));
        ui = Day.GetComponent<UIElement>();
        ui.SizeDelta = new Vector2(80, 400);
        ui.eventType = huqiang.Core.HGUI.EventType.UserEvent;
        ui.compositeType = CompositeType.ScrollY;
        fr = Day.transform;
        fr.SetParent(rect);
        fr.localPosition = new Vector3(107, 0, 0);
        fr.localScale = Vector3.one;
        fr.localRotation = Quaternion.identity;

        Item = new GameObject("Item");
        fn = Item.transform;
        fn.SetParent(fr);
        fn.localPosition = Vector3.zero;
        fn.localScale = Vector3.one;
        fn.localRotation = Quaternion.identity;
        txt = Item.AddComponent<HText>();
        txt.SizeDelta = new Vector2(60,40);
        txt.TextAnchor = TextAnchor.MiddleCenter;
        txt.Text = now.Day.ToString();
        txt.FontSize = 24;
    }
    [MenuItem("GameObject/HGUI/UIPalette", false, 12)]
    static public void AddPalette(MenuCommand menuCommand)
    {
        GameObject parent = menuCommand.context as GameObject;
        var palette = new GameObject("Palette");
        var main = palette.AddComponent<HImage>();
        main.SizeDelta = new Vector2(500,500);
        main.eventType = huqiang.Core.HGUI.EventType.UserEvent;
        main.compositeType = CompositeType.UIPalette;
        Transform rect = palette.transform;
        if (parent != null)
            rect.SetParent(parent.transform);
        rect.localPosition = Vector3.zero;
        rect.localScale = Vector3.one;
        rect.localRotation = Quaternion.identity;

        var Fill = new GameObject("HTemplate");
        var fr = Fill.transform;
        var ht = Fill.AddComponent<HImage>();
        ht.eventType = huqiang.Core.HGUI.EventType.UserEvent;
        ht.SizeDelta = new Vector2(256, 256);
        fr.SetParent(rect);
        fr.localPosition = Vector3.zero;
        fr.localScale = Vector3.one;
        fr.localRotation = Quaternion.identity;

        var Nob = new GameObject("NobA");
        var fn = Nob.transform;
        fn.SetParent(rect);
        fn.localPosition = new Vector3(0, -220, 0);
        fn.localScale = Vector3.one;
        fn.localRotation = Quaternion.identity;
        var img = Nob.AddComponent<HImage>();
        img.SizeDelta = new Vector2(44,44);
        var aim = img.Sprite = EditorModelManager.FindSprite(icons, Aim);

        Nob = new GameObject("NobB");
        fn = Nob.transform ;
        fn.SetParent(rect);
        fn.localPosition = new Vector3(-128, 128, 0);
        fn.localScale = Vector3.one;
        fn.localRotation = Quaternion.identity;
        img = Nob.AddComponent<HImage>();
        img.SizeDelta= new Vector2(24, 24);
        img.Sprite = aim;

        var Slider = new GameObject("Slider");
        fn = Slider.transform;
        fn.SetParent(rect);
        fn.localPosition = new Vector3(0, -285, 0);
        fn.localScale = Vector3.one;
        fn.localRotation = Quaternion.identity;
        img= Slider.AddComponent<HImage>();
        img.eventType = huqiang.Core.HGUI.EventType.UserEvent;
        img.compositeType = CompositeType.Slider;
        img.SizeDelta = new Vector2(400, 20);

        Nob = new GameObject("Nob");
        fn = Nob.transform;
        fn.SetParent(Slider.transform);
        fn.localPosition = new Vector3(200, 0, 0);
        fn.localScale = Vector3.one;
        img = Nob.AddComponent<HImage>();
        img.SizeDelta= new Vector2(30, 30);
        img.Chromatically = new Color(1, 1, 1, 1f);
        img.Sprite = aim;

        palette.AddComponent<PaletteHelper>().Initial();
    }
    [MenuItem("GameObject/HGUI/ScrollY", false, 13)]
    static public void AddScrollY(MenuCommand menuCommand)
    {
        GameObject parent = menuCommand.context as GameObject;
        var scroll = new GameObject("ScrollY");
        var main = scroll.AddComponent<HImage>();
        main.SizeDelta = new Vector2(400, 400);
        main.Pivot = new Vector2(0.5f, 1);
        main.Mask = true;
        main.Chromatically = new Color32(54, 54, 54, 255);
        main.eventType = huqiang.Core.HGUI.EventType.UserEvent;
        main.compositeType = CompositeType.ScrollY;
        var trans = scroll.transform;
        if (parent != null)
            trans.SetParent(parent.transform);
        trans.localPosition = Vector3.zero;
        trans.localScale = Vector3.one;
        trans.localRotation = Quaternion.identity;

        CreateItem(trans, "Item");

        var go = new GameObject("Slider", typeof(HImage));
        HImage image = go.GetComponent<HImage>();
        image.SizeDelta = new Vector2(20, 400);
        image.compositeType = CompositeType.Slider;
        var rect = image.transform;
        rect.SetParent(trans);
        rect.localPosition = new Vector3(190,-200,0);
        rect.localScale = Vector3.one;
        rect.localRotation = Quaternion.identity;

        var help = go.AddComponent<SliderHelper>();
        help.direction = UISlider.Direction.Vertical;
        image.Sprite = EditorModelManager.FindSprite(icons, background);
        image.SprType = SpriteType.Sliced;

        var Nob = new GameObject("Nob", typeof(HImage));
        image = Nob.GetComponent<HImage>();
        image.SizeDelta = new Vector2(20, 30);
        var son = image.transform;
        son.SetParent(rect);
        son.localPosition = new Vector3(0, 185, 0);
        son.localScale = Vector3.one;
        son.localRotation = Quaternion.identity;
        image.Chromatically = Color.green;
        image.Sprite = EditorModelManager.FindSprite(icons, background);
        image.SprType = SpriteType.Sliced;
    }
    [MenuItem("GameObject/HGUI/ScrollYExtand", false, 14)]
    static public void AddScrollYExtand(MenuCommand menuCommand)
    {
        GameObject parent = menuCommand.context as GameObject;
        var scroll = new GameObject("scrollEx");
        var main = scroll.AddComponent<HImage>();
        main.SizeDelta = new Vector2(400,800);
        main.Pivot = new Vector2(0.5f,1);
        main.Mask = true;
        main.Chromatically = new Color32(54,54,54,255);
        main.eventType = huqiang.Core.HGUI.EventType.UserEvent;
        main.compositeType = CompositeType.ScrollYExtand;
        var trans = scroll.transform;
        if (parent != null)
            trans.SetParent(parent.transform);
        trans.localPosition = new Vector3(0,400,0);
        trans.localScale = Vector3.one;
        trans.localRotation = Quaternion.identity;

        CreateItem(trans,"Title");
        var item= CreateItem(trans, "Item");
        item.localPosition = new Vector3(0,-100, 0);
        var tail = CreateItem(trans,"Tail");
        tail.localPosition = new Vector3(0,-200,0);

        var body = new GameObject("Body");
        var son = body.transform;
        son.SetParent(trans);
        son.localPosition = new Vector3(0, -300, 0);
        son.localScale = Vector3.one;
        son.localRotation = Quaternion.identity;
        var ui = body.AddComponent<UIElement>();
        ui.SizeDelta = new Vector2(400,100);
        ui.Pivot = new Vector2(0.5f,1);
        ui.Mask = true;
    }
    static Transform CreateItem(Transform parent,string name)
    {
        var mod = new GameObject(name);
        var trans = mod.transform;
        trans.SetParent(parent);
        trans.localPosition = Vector3.zero;
        trans.localScale = Vector3.one;
        trans.localRotation = Quaternion.identity;
        var ui = mod.AddComponent<UIElement>();
        ui.SizeDelta = new Vector2(400,100);
        ui.Pivot = new Vector2(0.5f, 1);

        var img = new GameObject("Image");
        var son = img.transform;
        son.SetParent(trans);
        son.localPosition = new Vector3(0,-50, 0);
        son.localScale = Vector3.one;
        son.localRotation = Quaternion.identity;
        ui = img.AddComponent<HImage>();
        ui.SizeDelta = new Vector2(400,90);
        ui.Chromatically = new Color32(4,30,65,255);
        ui.eventType = huqiang.Core.HGUI.EventType.UserEvent;

        var txt = new GameObject("Text");
        son = txt.transform;
        son.SetParent(trans);
        son.localPosition = new Vector3(0, -50, 0);
        son.localScale = Vector3.one;
        son.localRotation = Quaternion.identity;
        var ht = txt.AddComponent<HText>();
        ht.SizeDelta = new Vector2(400, 80);
        ht.Text = name;
        ht.FontSize = 30;

        return trans;
    }
    [MenuItem("GameObject/HGUI/DropDown", false, 15)]
    static public void AddDropDown(MenuCommand menuCommand)
    {
        GameObject parent = menuCommand.context as GameObject;

        var ss = new GameObject("DropDown");
        Transform rect = ss.transform;
        var drop = ss.AddComponent<HImage>();
        drop.SizeDelta=new Vector2(300,60);
        drop.Sprite = EditorModelManager.FindSprite(icons, background);
        drop.SprType = SpriteType.Sliced;
        drop.Chromatically = new Color32(224,224,224,255);
        if (parent != null)
            rect.SetParent(parent.transform);
        rect.localPosition = Vector3.zero;
        rect.localScale = Vector3.one;
        rect.localRotation = Quaternion.identity;

        var txt = UICreator.CreateHText(Vector3.zero, Vector2.zero, "Label", rect);
        txt.TextAnchor = TextAnchor.MiddleLeft;
        txt.FontSize = 36;
        txt.marginType = MarginType.Margin;
        txt.margin = new Margin(20, 100, 5, 5);
        txt.Text = "Label";
        txt.Chromatically = Color.black;

        var img = UICreator.CreateHImage(Vector3.zero, new Vector2(50, 50), "Arrow", rect);
        img.SizeDelta = new Vector2(50,50);
        img.Sprite = EditorModelManager.FindSprite(icons, diamond);
        img.anchorPointType = AnchorPointType.Right;
        img.anchorType = AnchorType.Alignment;
        img.Chromatically = Color.black;

        var main = UICreator.CreateHImage(new Vector3(0,-34,0), new Vector2(300, 300), "Scroll", rect);
        main.Pivot = new Vector2(0.5f, 1);
        main.Mask = true;
        main.Chromatically = new Color32(224, 224, 224, 255);
        main.eventType = huqiang.Core.HGUI.EventType.UserEvent;
        main.compositeType = CompositeType.ScrollY;
        main.Sprite = EditorModelManager.FindSprite(icons, background);
        main.SprType = SpriteType.Sliced;

        var item = UICreator.CreateElement(new Vector3(0, -150, 0), new Vector2(300, 60), "Item", main.transform);
        img = UICreator.CreateHImage(Vector3.zero,new Vector2(290,50),"Image",item.transform);
        img.Sprite = EditorModelManager.FindSprite(icons, background);
        img.SprType = SpriteType.Sliced;
        txt = UICreator.CreateHText(Vector3.zero, Vector2.zero, "Text", item.transform);
        txt.marginType = MarginType.Margin;
        txt.margin = new Margin(60,60,5,5);
        txt.Chromatically = Color.black;
        txt.Text = "Option";
        txt.FontSize = 32;
        txt.TextAnchor = TextAnchor.MiddleLeft;
        img= UICreator.CreateHImage(Vector3.zero, new Vector2(30, 30), "Check", item.transform);
        img.Sprite = EditorModelManager.FindSprite(icons, star);
        img.anchorType = AnchorType.Alignment;
        img.anchorOffset.x = 20;
        img.anchorPointType = AnchorPointType.Left;
        img.Chromatically = Color.black;

        var image = UICreator.CreateHImage(new Vector3(140,-150,0), new Vector2(20, 300), "Slider", main.transform);
        image.eventType = huqiang.Core.HGUI.EventType.UserEvent;
        image.compositeType = CompositeType.Slider;
        image.Sprite = EditorModelManager.FindSprite(icons, background);
        image.SprType = SpriteType.Sliced;
        image.Chromatically = 0x295B7680.ToColor();

        var help = image.gameObject.AddComponent<SliderHelper>();
        help.direction = UISlider.Direction.Vertical;
    

        var Nob = UICreator.CreateHImage(new Vector3(0, 135, 0), new Vector2(20, 30), "Nob", image.transform);
        Nob.Chromatically = 0x5F5263ff.ToColor();
        Nob.Sprite = EditorModelManager.FindSprite(icons, background);
        Nob.SprType = SpriteType.Sliced;
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