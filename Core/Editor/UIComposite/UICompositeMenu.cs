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
        GameObject parent = menuCommand.context as GameObject;
        string pagename = "page";
        bool root = false;
        if(parent!=null)
        {
            if (parent.GetComponent<HCanvas>() == null)
            {
                pagename = parent.name;
            }
            else root = true;
        }
        string path = EditorUtility.SaveFilePanel("CreatePage", Application.dataPath,pagename, "cs");
        FileStream fs1 = new FileStream(path, FileMode.Create, FileAccess.Write);//创建写入文件 
        StreamWriter sw = new StreamWriter(fs1);
        var paths = path.Split('/');
        var classname = paths[paths.Length - 1];
        classname = classname.Substring(0, classname.Length - 3);
        if(root)
            sw.WriteLine(UIPageModel.GetPageModel(classname, classname));//开始写入值
        else sw.WriteLine(UIPageModel.GetPageModel(classname,pagename));//开始写入值
        sw.Close();
        fs1.Close();
        Debug.Log(classname + ".cs Create done");
     
        if(parent==null)
        {
            var canvas = UnityEngine.Object.FindObjectOfType<HCanvas>();
            var go = new GameObject(classname, typeof(UIElement));
            var trans = go.transform;
            if (canvas != null)
                trans.SetParent(canvas.transform);
            trans.localPosition = Vector3.zero;
            trans.localScale = Vector3.one;
            trans.localRotation = Quaternion.identity;
        }
        else if(root)
        {
            var go = new GameObject(classname, typeof(UIElement));
            var trans = go.transform;
            trans.SetParent(parent.transform);
            trans.localPosition = Vector3.zero;
            trans.localScale = Vector3.one;
            trans.localRotation = Quaternion.identity;
        }
        AssetDatabase.Refresh();
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
        image.MainColor = new Color32(94, 137, 197, 255);

        var Nob = new GameObject("Nob", typeof(HImage));
        image = Nob.GetComponent<HImage>();
        image.SizeDelta = new Vector2(30, 30);
        son = image.transform;
        son.SetParent(rect);
        son.localPosition = new Vector3(200, 0, 0);
        son.localScale = Vector3.one;
        son.localRotation = Quaternion.identity;
        image.MainColor = Color.green;
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
        image.MainColor = new Color32(94, 137, 197, 255);

        var Nob = new GameObject("Nob", typeof(HImage));
        image = Nob.GetComponent<HImage>();
        image.SizeDelta = new Vector2(30, 30);
        son = image.transform;
        son.SetParent(rect);
        son.localPosition = new Vector3(0, 200+startOffset, 0);
        son.localScale = Vector3.one;
        son.localRotation = Quaternion.identity;
        image.MainColor = Color.green;
        image.Sprite = EditorModelManager.FindSprite(icons, ufo);
        return go;
    }
    [MenuItem("GameObject/HGUI/Scroll", false, 8)]
    static public void AddScroll(MenuCommand menuCommand)
    {
        GameObject parent = menuCommand.context as GameObject;
        Transform pt = null;
        if (parent != null)
            pt = parent.transform;
        var scroll = UICreator.CreateHImage(Vector3.zero, new Vector2(400, 400), "gridScroll", pt);
        scroll.Mask = true;
        scroll.MainColor = new Color32(224, 224, 224, 255);
        scroll.eventType = huqiang.Core.HGUI.EventType.UserEvent;
        scroll.compositeType = CompositeType.GridScroll;
        scroll.Sprite = EditorModelManager.FindSprite(icons, background);
        scroll.SprType = SpriteType.Sliced;

        CreateItem(scroll.transform, "Item");
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
        img.MainColor = new Color(1, 1, 1, 1f);
        img.Sprite = aim;

        palette.AddComponent<PaletteHelper>().Initial();
    }
    [MenuItem("GameObject/HGUI/ScrollY", false, 13)]
    static public void AddScrollY(MenuCommand menuCommand)
    {
        GameObject parent = menuCommand.context as GameObject;
        Transform pt = null;
        if (parent != null)
            pt = parent.transform;
        var scroll = UICreator.CreateHImage(Vector3.zero, new Vector2(400, 400), "ScrollY", pt);
        scroll.Mask = true;
        scroll.MainColor = new Color32(224, 224, 224, 255);
        scroll.eventType = huqiang.Core.HGUI.EventType.UserEvent;
        scroll.compositeType = CompositeType.ScrollY;
        scroll.Sprite = EditorModelManager.FindSprite(icons, background);
        scroll.SprType = SpriteType.Sliced;
        scroll.Pivot = new Vector2(0.5f,1);

        var item =CreateItem(scroll.transform, "Item");
        item.transform.localPosition = new Vector3(0,-200,0);

        var slider = UICreator.CreateHImage(new Vector3(190, -200, 0), new Vector2(20, 400), "Slider", scroll.transform);
        slider.eventType = huqiang.Core.HGUI.EventType.UserEvent;
        slider.compositeType = CompositeType.Slider;
        slider.Sprite = scroll.Sprite;
        slider.SprType = SpriteType.Sliced;
        slider.MainColor = 0x295B7680.ToColor();

        var help = slider.gameObject.AddComponent<SliderHelper>();
        help.direction = UISlider.Direction.Vertical;

        var Nob = UICreator.CreateHImage(new Vector3(0, 185, 0), new Vector2(20, 30), "Nob", slider.transform);
        Nob.MainColor = 0x5F5263ff.ToColor();
        Nob.Sprite = EditorModelManager.FindSprite(icons, background);
        Nob.SprType = SpriteType.Sliced;
    }
    [MenuItem("GameObject/HGUI/ScrollYExtand", false, 14)]
    static public void AddScrollYExtand(MenuCommand menuCommand)
    {
        GameObject parent = menuCommand.context as GameObject;
        Transform pt = null;
        if (parent != null)
            pt = parent.transform;
        var scroll = UICreator.CreateHImage(new Vector3(0,400,0), new Vector2(400, 800), "ScrollEx", pt);
        scroll.Mask = true;
        scroll.MainColor = new Color32(224, 224, 224, 255);
        scroll.eventType = huqiang.Core.HGUI.EventType.UserEvent;
        scroll.compositeType = CompositeType.ScrollYExtand;
        scroll.Sprite = EditorModelManager.FindSprite(icons, background);
        scroll.SprType = SpriteType.Sliced;
        scroll.Pivot = new Vector2(0.5f, 1);

        var bds = UICreator.CreateElement(Vector3.zero, new Vector2(400, 100), "Bodys",  scroll.transform);
        bds.anchorType = AnchorType.Alignment;
        bds.anchorPointType = AnchorPointType.Top;
        bds.marginType = MarginType.MarginX;
        bds.Pivot = new Vector2(0.5f,1);

        bds = UICreator.CreateElement(Vector3.zero, new Vector2(400, 100), "Titles", scroll.transform);
        bds.anchorType = AnchorType.Alignment;
        bds.anchorPointType = AnchorPointType.Top;
        bds.marginType = MarginType.MarginX;
        bds.Pivot = new Vector2(0.5f, 1);

        CreateItemE(scroll.transform,"Title");
        var item= CreateItemE(scroll.transform, "Item");
        item.transform.localPosition = new Vector3(0,-100, 0);
        var tail = CreateItemE(scroll.transform,"Tail");
        tail.transform.localPosition = new Vector3(0,-200,0);

        var body = new GameObject("Body");
        var son = body.transform;
        son.SetParent(scroll.transform);
        son.localPosition = new Vector3(0, -300, 0);
        son.localScale = Vector3.one;
        son.localRotation = Quaternion.identity;
        var ui = body.AddComponent<UIElement>();
        ui.SizeDelta = new Vector2(400,100);
        ui.Pivot = new Vector2(0.5f,1);
        ui.Mask = true;
    }
    static UIElement CreateItemE(Transform parent, string name)
    {
        var mod = UICreator.CreateElement(Vector3.zero, new Vector2(400, 100), name, parent);
        mod.Pivot = new Vector2(0.5f,1);

        var img = UICreator.CreateHImage(Vector3.zero, new Vector2(400, 90), "Image", mod.transform);
        img.Pivot = new Vector2(0.5f,1);
        img.MainColor = new Color32(68, 68, 68, 255);
        img.eventType = huqiang.Core.HGUI.EventType.UserEvent;
        img.Sprite = EditorModelManager.FindSprite(icons, background);
        img.SprType = SpriteType.Sliced;

        var txt = UICreator.CreateHText(Vector3.zero, new Vector2(400, 80), "Text", mod.transform);
        txt.Pivot = new Vector2(0.5f,1);
        txt.Text = name;
        txt.FontSize = 30;
        txt.TextAnchor = TextAnchor.MiddleLeft;
        return mod;
    }
    [MenuItem("GameObject/HGUI/ScrollX", false, 15)]
    static public void AddScrollX(MenuCommand menuCommand)
    {
        GameObject parent = menuCommand.context as GameObject;
        Transform pt = null;
        if (parent != null)
            pt = parent.transform;
        var scroll = UICreator.CreateHImage(Vector3.zero,new Vector2(400,400),"ScrollX",pt);
        scroll.Mask = true;
        scroll.MainColor = new Color32(224, 224, 224, 255);
        scroll.eventType = huqiang.Core.HGUI.EventType.UserEvent;
        scroll.compositeType = CompositeType.ScrollX;
        scroll.Sprite = EditorModelManager.FindSprite(icons, background);
        scroll.SprType = SpriteType.Sliced;

        CreateItem(scroll.transform, "Item");

        var slider = UICreator.CreateHImage(new Vector3(0, -190, 0),new Vector2(400,20),"Slider",scroll.transform);
        slider.eventType = huqiang.Core.HGUI.EventType.UserEvent;
        slider.compositeType = CompositeType.Slider;
        slider.Sprite = scroll.Sprite;
        slider.SprType = SpriteType.Sliced;
        slider.MainColor = 0x295B7680.ToColor();

        var help = slider.gameObject.AddComponent<SliderHelper>();

        var Nob = UICreator.CreateHImage(new Vector3(-185, 0, 0), new Vector2(30, 20), "Nob", slider.transform);
        Nob.MainColor = 0x5F5263ff.ToColor();
        Nob.Sprite = EditorModelManager.FindSprite(icons, background);
        Nob.SprType = SpriteType.Sliced;
    }
    static UIElement CreateItem(Transform parent,string name)
    {
        var mod = UICreator.CreateElement(Vector3.zero,new Vector2(100,100),name,parent);

        var img = UICreator.CreateHImage(Vector3.zero,new Vector2(90,90),"Image",mod.transform);
        img.MainColor = new Color32(68,68,68,255);
        img.eventType = huqiang.Core.HGUI.EventType.UserEvent;
        img.Sprite = EditorModelManager.FindSprite(icons, background);
        img.SprType = SpriteType.Sliced;

        var txt = UICreator.CreateHText(Vector3.zero,new Vector2(80,80),"Text",mod.transform);
        txt.Text = name;
        txt.FontSize = 30;
        txt.TextAnchor = TextAnchor.MiddleLeft;
        return mod;
    }
    [MenuItem("GameObject/HGUI/DropDown", false, 16)]
    static public void AddDropDown(MenuCommand menuCommand)
    {
        GameObject parent = menuCommand.context as GameObject;

        var ss = new GameObject("DropDown");
        Transform rect = ss.transform;
        var drop = ss.AddComponent<HImage>();
        drop.SizeDelta=new Vector2(300,60);
        drop.Sprite = EditorModelManager.FindSprite(icons, background);
        drop.SprType = SpriteType.Sliced;
        drop.MainColor = new Color32(224,224,224,255);
        drop.eventType = huqiang.Core.HGUI.EventType.UserEvent;
        drop.compositeType = CompositeType.DropDown;
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
        txt.MainColor = Color.black;

        var img = UICreator.CreateHImage(Vector3.zero, new Vector2(50, 50), "Arrow", rect);
        img.SizeDelta = new Vector2(50,50);
        img.Sprite = EditorModelManager.FindSprite(icons, diamond);
        img.anchorPointType = AnchorPointType.Right;
        img.anchorType = AnchorType.Alignment;
        img.MainColor = Color.black;

        var main = UICreator.CreateHImage(new Vector3(0,-34,0), new Vector2(300, 300), "Scroll", rect);
        main.Pivot = new Vector2(0.5f, 1);
        main.Mask = true;
        main.MainColor = new Color32(224, 224, 224, 255);
        main.eventType = huqiang.Core.HGUI.EventType.UserEvent;
        main.compositeType = CompositeType.ScrollY;
        main.Sprite = EditorModelManager.FindSprite(icons, background);
        main.SprType = SpriteType.Sliced;

        var item = UICreator.CreateElement(new Vector3(0, -150, 0), new Vector2(300, 60), "Item", main.transform);
        item.eventType = huqiang.Core.HGUI.EventType.UserEvent;
        img = UICreator.CreateHImage(Vector3.zero,new Vector2(290,50),"Image",item.transform);
        img.Sprite = EditorModelManager.FindSprite(icons, background);
        img.SprType = SpriteType.Sliced;
        txt = UICreator.CreateHText(Vector3.zero, Vector2.zero, "Text", item.transform);
        txt.marginType = MarginType.Margin;
        txt.margin = new Margin(60,60,5,5);
        txt.MainColor = Color.black;
        txt.Text = "Option";
        txt.FontSize = 32;
        txt.TextAnchor = TextAnchor.MiddleLeft;
        img= UICreator.CreateHImage(Vector3.zero, new Vector2(30, 30), "Check", item.transform);
        img.Sprite = EditorModelManager.FindSprite(icons, star);
        img.anchorType = AnchorType.Alignment;
        img.anchorOffset.x = 20;
        img.anchorPointType = AnchorPointType.Left;
        img.MainColor = Color.black;

        var slider = UICreator.CreateHImage(new Vector3(140,-150,0), new Vector2(20, 300), "Slider", main.transform);
        slider.eventType = huqiang.Core.HGUI.EventType.UserEvent;
        slider.compositeType = CompositeType.Slider;
        slider.Sprite = EditorModelManager.FindSprite(icons, background);
        slider.SprType = SpriteType.Sliced;
        slider.MainColor = 0x295B7680.ToColor();

        var help = slider.gameObject.AddComponent<SliderHelper>();
        help.direction = UISlider.Direction.Vertical;
    

        var Nob = UICreator.CreateHImage(new Vector3(0, 135, 0), new Vector2(20, 30), "Nob", slider.transform);
        Nob.MainColor = 0x5F5263ff.ToColor();
        Nob.Sprite = EditorModelManager.FindSprite(icons, background);
        Nob.SprType = SpriteType.Sliced;
    }
    [MenuItem("GameObject/HGUI/TabControl", false, 17)]
    static public void AddTabControl(MenuCommand menuCommand)
    {
        GameObject parent = menuCommand.context as GameObject;
        Transform pt = null;
        if (parent != null)
            pt = parent.transform;
        CreateTabControl(pt);
    }
    static UIElement CreateTabControl(Transform parent)
    {
        var tab = UICreator.CreateHImage(Vector3.zero, new Vector2(800, 800), "TabControl",parent);
        tab.Mask = true;
        tab.MainColor = new Color32(204, 204, 204, 255);
        tab.eventType = huqiang.Core.HGUI.EventType.UserEvent;
        tab.compositeType = CompositeType.TabControl;
        tab.Sprite = EditorModelManager.FindSprite(icons, background);
        tab.SprType = SpriteType.Sliced;

        var head = UICreator.CreateElement(Vector3.zero, new Vector2(100, 44), "Head", tab.transform);
        head.marginType = MarginType.MarginX;
        head.anchorType = AnchorType.Alignment;
        head.anchorPointType = AnchorPointType.Top;

        var items = UICreator.CreateElement(Vector3.zero, new Vector2(800, 42), "Items", head.transform);
        items.marginType = MarginType.Margin;
        items.margin.down = 2;
        items.compositeType = CompositeType.StackPanel;

        var line = UICreator.CreateHImage(Vector3.zero, new Vector2(100, 2), "Line", head.transform);
        line.anchorType = AnchorType.Alignment;
        line.anchorPointType = AnchorPointType.Down;
        line.marginType = MarginType.MarginX;

        var item = UICreator.CreateElement(new Vector3(-350, 380), new Vector2(100, 40), "Item", tab.transform);

        var img = UICreator.CreateHImage(Vector3.zero, new Vector2(100, 40), "Image", item.transform);
        img.MainColor = new Color32(168, 168, 168, 255);

        var txt = UICreator.CreateHText(Vector3.zero, new Vector2(80, 40), "Text", item.transform);
        txt.MainColor = Color.black;
        txt.Text = "Label";
        txt.FontSize = 24;
        txt.TextAnchor = TextAnchor.MiddleLeft;

        var content = UICreator.CreateElement(Vector3.zero, new Vector2(800, 756), "Content", tab.transform);
        content.marginType = MarginType.Margin;
        content.margin.top = 44;
        return tab;
    }
    [MenuItem("GameObject/HGUI/DockPanel", false, 18)]
    static public void AddDockPanel(MenuCommand menuCommand)
    {
        var game = menuCommand.context as GameObject;
        Transform pt = null;
        if (game != null)
            pt = game.transform;
        CreateDockPanel(pt);
    }
    static UIElement CreateDockPanel(Transform parent)
    {
        var dock = UICreator.CreateHImage(Vector3.zero, new Vector2(800, 800), "DockPanel", parent);
        dock.MainColor = new Color32(56, 56, 56, 255);
        dock.compositeType = CompositeType.DockPanel;
        dock.Sprite = EditorModelManager.FindSprite(icons, background);
        dock.SprType = SpriteType.Sliced;


        var AreaLevel = new GameObject("AreaLevel");
        var trans = AreaLevel.transform;
        trans.SetParent(dock.transform);
        trans.localPosition = Vector3.zero;
        trans.localScale = Vector3.one;
        trans.localRotation = Quaternion.identity;

        var LineLevel = new GameObject("LineLevel");
        trans = LineLevel.transform;
        trans.SetParent(dock.transform);
        trans.localPosition = Vector3.zero;
        trans.localScale = Vector3.one;
        trans.localRotation = Quaternion.identity;

        var Line = UICreator.CreateHImage(Vector3.zero,new Vector2(800,8),"Line", dock.transform);
        Line.MainColor = Color.black;
        UICreator.CreateHImage(new Vector3(0,-60,0), new Vector2(100, 100), "Area", dock.transform);
 
        return dock;
    }
    [MenuItem("GameObject/HGUI/DesignedDockPanel", false, 19)]
    static public void AddDesignedDockPanel(MenuCommand menuCommand)
    {
        var game = menuCommand.context as GameObject;
        Transform pt = null;
        if (game != null)
            pt = game.transform;
        var dock = CreateDockPanel(pt);
        dock.name = "DesignedDockPanel";
        dock.compositeType = CompositeType.DesignedDockPanel;

        var drag = UICreator.CreateHImage(Vector3.zero, new Vector2(40, 40), "Drag", dock.transform);
        drag.Sprite = EditorModelManager.FindSprite(icons, file);

        CreateAuxiliary(dock.transform);
    }
    static void CreateAuxiliary(Transform parent)
    {
        var aux = UICreator.CreateElement(Vector3.zero, new Vector2(800, 800), "Auxiliary", parent);
        aux.marginType = MarginType.Margin;

        var tab = CreateTabControl(aux.transform);
        tab.marginType = MarginType.Margin;

        var item = tab.transform.Find("Item");

        var clo = UICreator.CreateHImage(new Vector3(41.5f,0,0),new Vector2(30,30),"Close",item);
        clo.eventType = huqiang.Core.HGUI.EventType.UserEvent;
        clo.Sprite = EditorModelManager.FindSprite(icons, close);

        var Cover = UICreator.CreateHImage(Vector3.zero,new Vector2(30,30),"Cover",aux.transform);
        Cover.MainColor= new Color32(128, 128, 128, 128);

        var Docker = UICreator.CreateElement(Vector3.zero, new Vector2(800, 800), "Docker",aux.transform);

        var bk= EditorModelManager.FindSprite(icons, background);
        var Center = UICreator.CreateHImage(Vector3.zero, new Vector2(80, 80), "Center", Docker.transform);
        Center.Sprite = bk;
        Center.MainColor = 0x69DCF8d0.ToColor();
        var left = UICreator.CreateHImage(new Vector3(-80,0,0), new Vector2(50, 100), "Left", Docker.transform);
        left.Sprite = bk;
        left.MainColor = 0x69DCF8d0.ToColor();
        var top = UICreator.CreateHImage(new Vector3(0, 80, 0), new Vector2(100, 50), "Top", Docker.transform);
        top.Sprite = bk;
        top.MainColor = 0x69DCF8d0.ToColor();
        var right = UICreator.CreateHImage(new Vector3(80, 0, 0), new Vector2(50, 100), "Right", Docker.transform);
        right.Sprite = bk;
        right.MainColor = 0x69DCF8d0.ToColor();
        var down = UICreator.CreateHImage(new Vector3(0, -80, 0), new Vector2(100, 50), "Down", Docker.transform);
        down.Sprite = bk;
        down.MainColor = 0x69DCF8d0.ToColor();
    }
}