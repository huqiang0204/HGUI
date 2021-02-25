using huqiang;
using huqiang.Helper.HGUI;
using huqiang.Core.UIData;
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
    [UnityEditor.MenuItem("GameObject/HGUI/UISystem", false, -2)]
    static public void AddUISystem(MenuCommand menuCommand)
    {
        var ui = SceneAsset.FindObjectsOfType(typeof(huqiang.Core.HGUI.UISystem));
        if(ui!=null)
        {
            if(ui.Length>0)
            {
                Debug.Log("场景中已存在");
                return;
            }
        }
        new GameObject("UISystem",typeof(huqiang.Core.HGUI.UISystem));
    }
    [UnityEditor.MenuItem("GameObject/HGUI/HGUIRender", false, -1)]
    static public void AddHGUIRender(MenuCommand menuCommand)
    {
        GameObject parent = menuCommand.context as GameObject;
        var go = new GameObject("HGUIRender");
        var trans = go.transform;
        trans.localPosition = Vector3.zero;
        trans.localScale = Vector3.one;
        trans.localRotation = Quaternion.identity;
        go.AddComponent<MeshRenderer>();
        go.AddComponent<MeshFilter>();
        go.AddComponent<huqiang.Core.HGUI.HGUIRender>();
    }
    [UnityEditor.MenuItem("GameObject/HGUI/Canvas", false, 0)]
    static public void AddCanvas(MenuCommand menuCommand)
    {
        GameObject parent = menuCommand.context as GameObject;
        var go = new GameObject("Canvas", typeof(HCanvas));
        var trans = go.transform;
        trans.localPosition = Vector3.zero;
        trans.localScale = Vector3.one;
        trans.localRotation = Quaternion.identity;
        var ui = SceneAsset.FindObjectsOfType(typeof(huqiang.Core.HGUI.UISystem));//GameObject.FindObjectOfType<huqiang.Core.HGUI.UISystem>();
        if (ui == null)
            new GameObject("UISystem",typeof(huqiang.Core.HGUI.UISystem));
    }
    [UnityEditor.MenuItem("GameObject/HGUI/Page", false, 1)]
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
    [UnityEditor.MenuItem("GameObject/HGUI/Empty", false, 2)]
    static public void AddEmpty(MenuCommand menuCommand)
    {
        GameObject parent = menuCommand.context as GameObject;
        UICreator.CreateElement(Vector3.zero, new Vector2(100,100),"Empty", parent.transform);
    }
    [UnityEditor.MenuItem("GameObject/HGUI/Image", false, 3)]
    static public void AddImage(MenuCommand menuCommand)
    {
        GameObject parent = menuCommand.context as GameObject;
        UICreator.CreateHImage(Vector3.zero, new Vector2(100, 100), "Image", parent.transform);
    }
    [UnityEditor.MenuItem("GameObject/HGUI/Text", false, 4)]
    static public void AddText(MenuCommand menuCommand)
    {
        GameObject parent = menuCommand.context as GameObject;
        UICreator.CreateHText(Vector3.zero, new Vector2(160, 40), "Text", parent.transform);
    }
    [UnityEditor.MenuItem("GameObject/HGUI/TextBox", false, 5)]
    static public void AddTextBox(MenuCommand menuCommand)
    {
        GameObject parent = menuCommand.context as GameObject;
        UICreator.CreateTextBox(Vector3.zero, new Vector2(400, 400), "TextBox", parent.transform);
    }
    [UnityEditor.MenuItem("GameObject/HGUI/InputBox", false, 6)]
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
        var img = go.GetComponent<HImage>().Content;
        img.Mask = true;
        img.compositeType = CompositeType.InputBox;
        img.Sprite = EditorModelManager.FindSprite(icons, box);
        img.SprType = SpriteType.Sliced;
        img.SizeDelta = new Vector2(400,100);
        var help = go.AddComponent<TextInputHelper>();
     
        go = new GameObject("InputText", typeof(TextBox));
        var son = go.transform;
        son.SetParent(trans);
        son.localPosition = Vector3.zero;
        son.localScale = Vector3.one;
        son.localRotation = Quaternion.identity;
        var txt = go.GetComponent<HText>().Content;
        txt.SizeDelta = new Vector2(380,90);
       
        txt.marginType = MarginType.Margin;
        txt.margin = new Margin(5,5,5,5);
       

        UICreator.CreateHImage(Vector3.zero, new Vector2(2, 28), "Cursor", son);
        help.Refresh();
    }
    [UnityEditor.MenuItem("GameObject/HGUI/UISliderH", false, 7)]
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
        var image = go.GetComponent<HImage>().Content;
        image.SizeDelta = new Vector2(400, 20);
        image.compositeType = CompositeType.Slider;
        var rect = go.transform;

        var help = go.AddComponent<SliderHelper>();
        help.StartOffset.x = -15;
        help.EndOffset.x = -15;
        image.Sprite = EditorModelManager.FindSprite(icons, background);
        image.SprType = SpriteType.Sliced;

        var Fill = new GameObject("FillImage", typeof(HImage));
        image = Fill.GetComponent<HImage>().Content;
        image.SizeDelta = new Vector2(400, 20);
        var son = Fill.transform;
        son.SetParent(rect);
        son.localPosition = Vector3.zero;
        son.localScale = Vector3.one;
        son.localRotation = Quaternion.identity;
        image.Sprite = EditorModelManager.FindSprite(icons, background);
        image.SprType = SpriteType.Filled;
        image.FillMethod = FillMethod.Horizontal;
        image.MainColor = new Color32(94, 137, 197, 255);

        var Nob = new GameObject("Nob", typeof(HImage));
        image = Nob.GetComponent<HImage>().Content;
        image.SizeDelta = new Vector2(30, 30);
        son = Nob.transform;
        son.SetParent(rect);
        son.localPosition = new Vector3(200, 0, 0);
        son.localScale = Vector3.one;
        son.localRotation = Quaternion.identity;
        image.MainColor = Color.green;
        image.Sprite = EditorModelManager.FindSprite(icons, leaves);
        return go;
    }
    [UnityEditor.MenuItem("GameObject/HGUI/UISliderV", false, 8)]
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
        var image = go.GetComponent<HImage>().Content;
        image.SizeDelta = new Vector2(20, 400);
        image.compositeType = CompositeType.Slider;
        var rect = go.transform;

        var help = go.AddComponent<SliderHelper>();
        help.StartOffset.y = startOffset;
        help.EndOffset.y = endOffset;
        help.direction = UISlider.Direction.Vertical;
        image.Sprite = EditorModelManager.FindSprite(icons, background);
        image.SprType = SpriteType.Sliced;

        var Fill = new GameObject("FillImage", typeof(HImage));
        image = Fill.GetComponent<HImage>().Content;
        image.SizeDelta = new Vector2(20, 400);
        var son = Fill.transform;
        son.SetParent(rect);
        son.localPosition = Vector3.zero;
        son.localScale = Vector3.one;
        son.localRotation = Quaternion.identity;
        image.Sprite = EditorModelManager.FindSprite(icons, background);
        image.SprType = SpriteType.Filled;
        image.FillMethod = FillMethod.Vertical;
        image.MainColor = new Color32(94, 137, 197, 255);

        var Nob = new GameObject("Nob", typeof(HImage));
        image = Nob.GetComponent<HImage>().Content;
        image.SizeDelta = new Vector2(30, 30);
        son = Nob.transform;
        son.SetParent(rect);
        son.localPosition = new Vector3(0, 200+startOffset, 0);
        son.localScale = Vector3.one;
        son.localRotation = Quaternion.identity;
        image.MainColor = Color.green;
        image.Sprite = EditorModelManager.FindSprite(icons, ufo);
        return go;
    }
    [UnityEditor.MenuItem("GameObject/HGUI/Scroll", false, 9)]
    static public void AddScroll(MenuCommand menuCommand)
    {
        GameObject parent = menuCommand.context as GameObject;
        Transform pt = null;
        if (parent != null)
            pt = parent.transform;
        var s = UICreator.CreateHImage(Vector3.zero, new Vector2(400, 400), "gridScroll", pt);
        var scroll = s.Content;
        scroll.Mask = true;
        scroll.MainColor = new Color32(224, 224, 224, 255);
        scroll.eventType = HEventType.UserEvent;
        scroll.compositeType = CompositeType.GridScroll;
        scroll.Sprite = EditorModelManager.FindSprite(icons, background);
        scroll.SprType = SpriteType.Sliced;

        CreateItem(s.transform, "Item");
    }
    [UnityEditor.MenuItem("GameObject/HGUI/UIRocker", false, 10)]
    static public void AddRocker(MenuCommand menuCommand)
    {
        GameObject parent = menuCommand.context as GameObject;
        var ss = new GameObject("Rocker");
        var scr = ss.AddComponent<HImage>().Content;
        scr.SizeDelta = new Vector2(300, 300);
        scr.eventType = HEventType.UserEvent;
        scr.compositeType = CompositeType.Rocker;
        scr.Sprite = EditorModelManager.FindSprite(icons, circleol);
        var rect = ss.transform;
        if (parent != null)
            rect.SetParent(parent.transform);
        rect.localPosition = Vector3.zero;
        rect.localScale = Vector3.one;
        rect.localRotation = Quaternion.identity;
       
        var Item = new GameObject("Nob");
        var fr = Item.AddComponent<HImage>().Content;
        fr.SizeDelta = new Vector2(100, 100);
        fr.Sprite = EditorModelManager.FindSprite(icons, circlesm);
        var son = Item.transform;
        son.SetParent(rect);
        son.localPosition = Vector3.zero;
        son.localScale = Vector3.one;
       
    }
    [UnityEditor.MenuItem("GameObject/HGUI/TreeView", false, 11)]
    static public void AddTreeView(MenuCommand menuCommand)
    {
        GameObject parent = menuCommand.context as GameObject;
        var ss = new GameObject("TreeView");
        Transform rect = ss.transform;
        var uI = ss.AddComponent<UIElement>().Content;
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
        var e = Item.AddComponent<UIElement>().Content;
        e.SizeDelta = new Vector2(200, 40);

        var txt = new GameObject("Text");
        var t = txt.transform;
        t.SetParent(fr);
        t.localPosition = Vector3.zero;
        t.localScale = Vector3.one;
        var ht = txt.AddComponent<HText>().Content;
        ht.SizeDelta = new Vector2(200, 40);
        ht.TextAnchor = TextAnchor.MiddleLeft;
        ht.FontSize = 24;
    }
    [UnityEditor.MenuItem("GameObject/HGUI/UIDate", false, 12)]
    static public void AddDate(MenuCommand menuCommand)
    {
        GameObject parent = menuCommand.context as GameObject;
        var date = new GameObject("Date");
        var main = date.AddComponent<UIElement>().Content;
        main.SizeDelta = new Vector2(400,200);
        main.eventType = HEventType.UserEvent;
        main.compositeType = CompositeType.UIDate;
        Transform rect = date.transform;
        if (parent != null)
            rect.SetParent(parent.transform);
        rect.localPosition = Vector3.zero;
        rect.localScale = Vector3.one;
        rect.localRotation = Quaternion.identity;

        var label = new GameObject("YearLabel", typeof(HText));
        var ht = label.GetComponent<HText>().Content;
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
        ht = label.GetComponent<HText>().Content;
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
        ht = label.GetComponent<HText>().Content;
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
        var ui = Year.GetComponent<UIElement>().Content;
        ui.SizeDelta = new Vector2(80,400);
        ui.eventType = HEventType.UserEvent;
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
        var txt = Item.AddComponent<HText>().Content;
        txt.SizeDelta = new Vector2(60, 40);
        txt.TextAnchor = TextAnchor.MiddleCenter;
        txt.Text = now.Year.ToString();
        txt.FontSize = 24;

        var Month = new GameObject("Month",typeof(UIElement));
        ui = Month.GetComponent<UIElement>().Content;
        ui.SizeDelta = new Vector2(80, 400);
        ui.eventType = HEventType.UserEvent;
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
        txt = Item.AddComponent<HText>().Content;
        txt.SizeDelta = new Vector2(60,40);
        txt.TextAnchor = TextAnchor.MiddleCenter;
        txt.Text = now.Month.ToString();
        txt.FontSize = 24;

        var Day = new GameObject("Day", typeof(UIElement));
        ui = Day.GetComponent<UIElement>().Content;
        ui.SizeDelta = new Vector2(80, 400);
        ui.eventType = HEventType.UserEvent;
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
        txt = Item.AddComponent<HText>().Content;
        txt.SizeDelta = new Vector2(60,40);
        txt.TextAnchor = TextAnchor.MiddleCenter;
        txt.Text = now.Day.ToString();
        txt.FontSize = 24;
    }
    [UnityEditor.MenuItem("GameObject/HGUI/UIPalette", false, 13)]
    static public void AddPalette(MenuCommand menuCommand)
    {
        GameObject parent = menuCommand.context as GameObject;
        var palette = new GameObject("Palette");
        var main = palette.AddComponent<HImage>().Content;
        main.SizeDelta = new Vector2(500,500);
        main.eventType = HEventType.UserEvent;
        main.compositeType = CompositeType.UIPalette;
        Transform rect = palette.transform;
        if (parent != null)
            rect.SetParent(parent.transform);
        rect.localPosition = Vector3.zero;
        rect.localScale = Vector3.one;
        rect.localRotation = Quaternion.identity;

        var Fill = new GameObject("HTemplate");
        var fr = Fill.transform;
        var ht = Fill.AddComponent<HImage>().Content;
        ht.eventType = HEventType.UserEvent;
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
        var img = Nob.AddComponent<HImage>().Content;
        img.SizeDelta = new Vector2(44,44);
        var aim = img.Sprite = EditorModelManager.FindSprite(icons, Aim);

        Nob = new GameObject("NobB");
        fn = Nob.transform ;
        fn.SetParent(rect);
        fn.localPosition = new Vector3(-128, 128, 0);
        fn.localScale = Vector3.one;
        fn.localRotation = Quaternion.identity;
        img = Nob.AddComponent<HImage>().Content;
        img.SizeDelta= new Vector2(24, 24);
        img.Sprite = aim;

        var Slider = new GameObject("Slider");
        fn = Slider.transform;
        fn.SetParent(rect);
        fn.localPosition = new Vector3(0, -285, 0);
        fn.localScale = Vector3.one;
        fn.localRotation = Quaternion.identity;
        img= Slider.AddComponent<HImage>().Content;
        img.eventType = HEventType.UserEvent;
        img.compositeType = CompositeType.Slider;
        img.SizeDelta = new Vector2(400, 20);

        Nob = new GameObject("Nob");
        fn = Nob.transform;
        fn.SetParent(Slider.transform);
        fn.localPosition = new Vector3(200, 0, 0);
        fn.localScale = Vector3.one;
        img = Nob.AddComponent<HImage>().Content;
        img.SizeDelta= new Vector2(30, 30);
        img.MainColor = new Color(1, 1, 1, 1f);
        img.Sprite = aim;

        palette.AddComponent<PaletteHelper>().Initial();
    }
    [UnityEditor.MenuItem("GameObject/HGUI/ScrollY", false, 14)]
    static public void AddScrollY(MenuCommand menuCommand)
    {
        GameObject parent = menuCommand.context as GameObject;
        Transform pt = null;
        if (parent != null)
            pt = parent.transform;
        UIElement p = UICreator.CreateElement(new Vector3(-10,200,0),new Vector2(420,400),"Scroll",pt);
        var sl = UICreator.CreateHImage(Vector3.zero, new Vector2(400, 400), "ScrollY", p.transform);
        var scroll = sl.Content;
        scroll.marginType = MarginType.Margin;
        scroll.margin.right = 20;
        scroll.Mask = true;
        scroll.MainColor = new Color32(224, 224, 224, 255);
        scroll.eventType = HEventType.UserEvent;
        scroll.compositeType = CompositeType.ScrollY;
        scroll.Sprite = EditorModelManager.FindSprite(icons, background);
        scroll.SprType = SpriteType.Sliced;
        scroll.Pivot = new Vector2(0.5f,1);

        var item =CreateItem(sl.transform, "Item");
        item.Content.Pivot = new Vector2(0.5f, 1);
        item.transform.localPosition = new Vector3(0, 0, 0);
        item.transform.Find("Image").localPosition = new Vector3(0, -50, 0);
        item.transform.Find("Text").localPosition = new Vector3(0, -50, 0);

        var go = CreateSliderV();
        if (parent != null)
        {
            var rect = go.transform;
            rect.SetParent(p.transform);
            rect.localPosition = new Vector3(200,0,0);
            rect.localScale = Vector3.one;
            rect.localRotation = Quaternion.identity;
        }
        var slider = go.GetComponent<UIElement>().Content;
        slider.anchorType = AnchorType.Alignment;
        slider.anchorPointType = AnchorPointType.Right;
        slider.marginType = MarginType.MarginY;
        var help = sl.gameObject.AddComponent<ScrollHelper>();
        help.Slider = go.transform;
    }
    [UnityEditor.MenuItem("GameObject/HGUI/ScrollYExtand", false, 15)]
    static public void AddScrollYExtand(MenuCommand menuCommand)
    {
        GameObject parent = menuCommand.context as GameObject;
        Transform pt = null;
        if (parent != null)
            pt = parent.transform;
        var sl = UICreator.CreateHImage(new Vector3(0,400,0), new Vector2(400, 800), "ScrollEx", pt);
        var scroll = sl.Content;
        scroll.Mask = true;
        scroll.MainColor = new Color32(224, 224, 224, 255);
        scroll.eventType = HEventType.UserEvent;
        scroll.compositeType = CompositeType.ScrollYExtand;
        scroll.Sprite = EditorModelManager.FindSprite(icons, background);
        scroll.SprType = SpriteType.Sliced;
        scroll.Pivot = new Vector2(0.5f, 1);

        var bds = UICreator.CreateElement(Vector3.zero, new Vector2(400, 100), "Bodys",  sl.transform).Content;
        bds.anchorType = AnchorType.Alignment;
        bds.anchorPointType = AnchorPointType.Top;
        bds.marginType = MarginType.MarginX;
        bds.Pivot = new Vector2(0.5f,1);

        bds = UICreator.CreateElement(Vector3.zero, new Vector2(400, 100), "Titles", sl.transform).Content;
        bds.anchorType = AnchorType.Alignment;
        bds.anchorPointType = AnchorPointType.Top;
        bds.marginType = MarginType.MarginX;
        bds.Pivot = new Vector2(0.5f, 1);

        CreateItemE(sl.transform,"Title");
        var item= CreateItemE(sl.transform, "Item");
        item.transform.localPosition = new Vector3(0,-100, 0);
        var tail = CreateItemE(sl.transform,"Tail");
        tail.transform.localPosition = new Vector3(0,-200,0);

        var body = new GameObject("Body");
        var son = body.transform;
        son.SetParent(sl.transform);
        son.localPosition = new Vector3(0, -300, 0);
        son.localScale = Vector3.one;
        son.localRotation = Quaternion.identity;
        var ui = body.AddComponent<UIElement>().Content;
        ui.SizeDelta = new Vector2(400,100);
        ui.Pivot = new Vector2(0.5f,1);
        ui.Mask = true;
    }
    static UIElement CreateItemE(Transform parent, string name)
    {
        var mod = UICreator.CreateElement(Vector3.zero, new Vector2(400, 100), name, parent);
        mod.Content.Pivot = new Vector2(0.5f,1);

        var im = UICreator.CreateHImage(Vector3.zero, new Vector2(400, 90), "Image", mod.transform);
        var img = im.Content;
        img.Pivot = new Vector2(0.5f,1);
        img.MainColor = new Color32(68, 68, 68, 255);
        img.eventType = HEventType.UserEvent;
        img.Sprite = EditorModelManager.FindSprite(icons, background);
        img.SprType = SpriteType.Sliced;

        var txt = UICreator.CreateHText(Vector3.zero, new Vector2(400, 80), "Text", mod.transform).Content;
        txt.Pivot = new Vector2(0.5f,1);
        txt.Text = name;
        txt.FontSize = 30;
        txt.TextAnchor = TextAnchor.MiddleLeft;
        return mod;
    }
    [UnityEditor.MenuItem("GameObject/HGUI/ScrollX", false, 16)]
    static public void AddScrollX(MenuCommand menuCommand)
    {
        GameObject parent = menuCommand.context as GameObject;
        Transform pt = null;
        if (parent != null)
            pt = parent.transform;
        UIElement p = UICreator.CreateElement(Vector3.zero, new Vector2(400, 420), "Scroll", pt);
        var sl = UICreator.CreateHImage(Vector3.zero,new Vector2(400,400),"ScrollX",p.transform);
        var scroll = sl.Content;
        scroll.marginType = MarginType.Margin;
        scroll.margin.down = 20;
        scroll.Mask = true;
        scroll.MainColor = new Color32(224, 224, 224, 255);
        scroll.eventType = HEventType.UserEvent;
        scroll.compositeType = CompositeType.ScrollX;
        scroll.Sprite = EditorModelManager.FindSprite(icons, background);
        scroll.SprType = SpriteType.Sliced;

        CreateItem(sl.transform, "Item");

        var go = CreateSliderH();
        if (parent != null)
        {
            var rect = go.transform;
            rect.SetParent(p.transform);
            rect.localPosition = new Vector3(200, 0, 0);
            rect.localScale = Vector3.one;
            rect.localRotation = Quaternion.identity;
        }
        var slider = go.GetComponent<UIElement>().Content;
        slider.anchorType = AnchorType.Alignment;
        slider.anchorPointType = AnchorPointType.Down;
        slider.marginType = MarginType.MarginX;
        var help = sl.gameObject.AddComponent<ScrollHelper>();
        help.Slider = go.transform;
    }
    static UIElement CreateItem(Transform parent,string name)
    {
        var mod = UICreator.CreateElement(Vector3.zero,new Vector2(100,100),name,parent);

        var img = UICreator.CreateHImage(Vector3.zero,new Vector2(90,90),"Image",mod.transform).Content;
        img.MainColor = new Color32(68,68,68,255);
        img.eventType = HEventType.UserEvent;
        img.Sprite = EditorModelManager.FindSprite(icons, background);
        img.SprType = SpriteType.Sliced;

        var txt = UICreator.CreateHText(Vector3.zero,new Vector2(80,80),"Text",mod.transform).Content;
        txt.Text = name;
        txt.FontSize = 30;
        txt.TextAnchor = TextAnchor.MiddleLeft;
        return mod;
    }
    [UnityEditor.MenuItem("GameObject/HGUI/DropDown", false, 17)]
    static public void AddDropDown(MenuCommand menuCommand)
    {
        GameObject parent = menuCommand.context as GameObject;

        var ss = new GameObject("DropDown");
        Transform rect = ss.transform;
        var drop = ss.AddComponent<HImage>().Content;
        drop.SizeDelta=new Vector2(300,60);
        drop.Sprite = EditorModelManager.FindSprite(icons, background);
        drop.SprType = SpriteType.Sliced;
        drop.MainColor = new Color32(224,224,224,255);
        drop.eventType = HEventType.UserEvent;
        drop.compositeType = CompositeType.DropDown;
        if (parent != null)
            rect.SetParent(parent.transform);
        rect.localPosition = Vector3.zero;
        rect.localScale = Vector3.one;
        rect.localRotation = Quaternion.identity;

        var txt = UICreator.CreateHText(Vector3.zero, Vector2.zero, "Label", rect).Content;
        txt.TextAnchor = TextAnchor.MiddleLeft;
        txt.FontSize = 36;
        txt.marginType = MarginType.Margin;
        txt.margin = new Margin(20, 100, 5, 5);
        txt.Text = "Label";
        txt.MainColor = Color.black;

        var img = UICreator.CreateHImage(Vector3.zero, new Vector2(50, 50), "Arrow", rect).Content;
        img.SizeDelta = new Vector2(50,50);
        img.Sprite = EditorModelManager.FindSprite(icons, diamond);
        img.anchorPointType = AnchorPointType.Right;
        img.anchorType = AnchorType.Alignment;
        img.MainColor = Color.black;

        var sl = UICreator.CreateHImage(new Vector3(0,-34,0), new Vector2(300, 300), "Scroll", rect);
        var main = sl.Content;
        main.Pivot = new Vector2(0.5f, 1);
        main.Mask = true;
        main.MainColor = new Color32(224, 224, 224, 255);
        main.eventType = HEventType.UserEvent;
        main.compositeType = CompositeType.ScrollY;
        main.Sprite = EditorModelManager.FindSprite(icons, background);
        main.SprType = SpriteType.Sliced;

        var item = UICreator.CreateElement(new Vector3(0, -150, 0), new Vector2(300, 60), "Item", sl.transform);
        item.Content.eventType = HEventType.UserEvent;
        img = UICreator.CreateHImage(Vector3.zero,new Vector2(290,50),"Image",item.transform).Content;
        img.Sprite = EditorModelManager.FindSprite(icons, background);
        img.SprType = SpriteType.Sliced;
        txt = UICreator.CreateHText(Vector3.zero, Vector2.zero, "Text", item.transform).Content;
        txt.marginType = MarginType.Margin;
        txt.margin = new Margin(60,60,5,5);
        txt.MainColor = Color.black;
        txt.Text = "Option";
        txt.FontSize = 32;
        txt.TextAnchor = TextAnchor.MiddleLeft;
        img= UICreator.CreateHImage(Vector3.zero, new Vector2(30, 30), "Check", item.transform).Content;
        img.Sprite = EditorModelManager.FindSprite(icons, star);
        img.anchorType = AnchorType.Alignment;
        img.anchorOffset.x = 20;
        img.anchorPointType = AnchorPointType.Left;
        img.MainColor = Color.black;

        var sd = UICreator.CreateHImage(new Vector3(140,-150,0), new Vector2(20, 300), "Slider", sl.transform);
        var slider = sd.Content;
        slider.eventType = HEventType.UserEvent;
        slider.compositeType = CompositeType.Slider;
        slider.Sprite = EditorModelManager.FindSprite(icons, background);
        slider.SprType = SpriteType.Sliced;
        slider.MainColor = 0x295B7680.ToColor();

        var help = sd.gameObject.AddComponent<SliderHelper>();
        help.direction = UISlider.Direction.Vertical;
    

        var Nob = UICreator.CreateHImage(new Vector3(0, 135, 0), new Vector2(20, 30), "Nob", sd.transform).Content;
        Nob.MainColor = 0x5F5263ff.ToColor();
        Nob.Sprite = EditorModelManager.FindSprite(icons, background);
        Nob.SprType = SpriteType.Sliced;
    }
    [UnityEditor.MenuItem("GameObject/HGUI/TabControl", false, 18)]
    static public void AddTabControl(MenuCommand menuCommand)
    {
        GameObject parent = menuCommand.context as GameObject;
        Transform pt = null;
        if (parent != null)
            pt = parent.transform;
        CreateTabControl(pt);
    }
    static HImage CreateTabControl(Transform parent)
    {
        var tab = UICreator.CreateHImage(Vector3.zero, new Vector2(800, 800), "TabControl",parent);
        tab.Content.Mask = true;
        tab.Content.MainColor = new Color32(204, 204, 204, 255);
        tab.Content.eventType = HEventType.UserEvent;
        tab.Content.compositeType = CompositeType.TabControl;
        tab.Content.Sprite = EditorModelManager.FindSprite(icons, background);
        tab.Content.SprType = SpriteType.Sliced;

        var head = UICreator.CreateElement(Vector3.zero, new Vector2(100, 44), "Head", tab.transform);
        head.Content.marginType = MarginType.MarginX;
        head.Content.anchorType = AnchorType.Alignment;
        head.Content.anchorPointType = AnchorPointType.Top;

        var items = UICreator.CreateElement(Vector3.zero, new Vector2(800, 42), "Items", head.transform);
        items.Content.marginType = MarginType.Margin;
        items.Content.margin.down = 2;
        items.Content.compositeType = CompositeType.StackPanel;

        var line = UICreator.CreateHImage(Vector3.zero, new Vector2(100, 2), "Line", head.transform);
        line.Content.anchorType = AnchorType.Alignment;
        line.Content.anchorPointType = AnchorPointType.Down;
        line.Content.marginType = MarginType.MarginX;

        var item = UICreator.CreateElement(new Vector3(-350, 380), new Vector2(100, 40), "Item", tab.transform);

        var img = UICreator.CreateHImage(Vector3.zero, new Vector2(100, 40), "Image", item.transform);
        img.Content.MainColor = new Color32(168, 168, 168, 255);

        var txt = UICreator.CreateHText(Vector3.zero, new Vector2(80, 40), "Text", item.transform);
        txt.Content.MainColor = Color.black;
        txt.Content.Text = "Label";
        txt.Content.FontSize = 24;
        txt.Content.TextAnchor = TextAnchor.MiddleLeft;

        var content = UICreator.CreateElement(Vector3.zero, new Vector2(800, 756), "Content", tab.transform);
        content.Content.marginType = MarginType.Margin;
        content.Content.margin.top = 44;
        return tab;
    }
    [UnityEditor.MenuItem("GameObject/HGUI/DockPanel", false, 19)]
    static public void AddDockPanel(MenuCommand menuCommand)
    {
        var game = menuCommand.context as GameObject;
        Transform pt = null;
        if (game != null)
            pt = game.transform;
        CreateDockPanel(pt);
    }
    static HImage CreateDockPanel(Transform parent)
    {
        var dock = UICreator.CreateHImage(Vector3.zero, new Vector2(800, 800), "DockPanel", parent);
        dock.Content.MainColor = new Color32(56, 56, 56, 255);
        dock.Content.compositeType = CompositeType.DockPanel;
        dock.Content.Sprite = EditorModelManager.FindSprite(icons, background);
        dock.Content.SprType = SpriteType.Sliced;


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
        Line.Content.MainColor = Color.black;
        UICreator.CreateHImage(new Vector3(0,-60,0), new Vector2(100, 100), "Area", dock.transform);
 
        return dock;
    }
    [UnityEditor.MenuItem("GameObject/HGUI/DesignedDockPanel", false, 20)]
    static public void AddDesignedDockPanel(MenuCommand menuCommand)
    {
        var game = menuCommand.context as GameObject;
        Transform pt = null;
        if (game != null)
            pt = game.transform;
        var dock = CreateDockPanel(pt);
        dock.name = "DesignedDockPanel";
        dock.Content.compositeType = CompositeType.DesignedDockPanel;

        var drag = UICreator.CreateHImage(Vector3.zero, new Vector2(40, 40), "Drag", dock.transform);
        drag.Content.Sprite = EditorModelManager.FindSprite(icons, file);

        CreateAuxiliary(dock.transform);
    }
    static void CreateAuxiliary(Transform parent)
    {
        var aux = UICreator.CreateElement(Vector3.zero, new Vector2(800, 800), "Auxiliary", parent);
        aux.Content.marginType = MarginType.Margin;

        var tab = CreateTabControl(aux.transform);
        tab.Content.marginType = MarginType.Margin;

        var item = tab.transform.Find("Item");

        var clo = UICreator.CreateHImage(new Vector3(41.5f,0,0),new Vector2(30,30),"Close",item);
        clo.Content.eventType = HEventType.UserEvent;
        clo.Content.Sprite = EditorModelManager.FindSprite(icons, close);

        var Cover = UICreator.CreateHImage(Vector3.zero,new Vector2(30,30),"Cover",aux.transform);
        Cover.Content.MainColor= new Color32(128, 128, 128, 128);

        var Docker = UICreator.CreateElement(Vector3.zero, new Vector2(800, 800), "Docker",aux.transform);

        var bk= EditorModelManager.FindSprite(icons, background);
        var Center = UICreator.CreateHImage(Vector3.zero, new Vector2(80, 80), "Center", Docker.transform);
        Center.Content.Sprite = bk;
        Center.Content.MainColor = 0x69DCF8d0.ToColor();
        var left = UICreator.CreateHImage(new Vector3(-80,0,0), new Vector2(50, 100), "Left", Docker.transform);
        left.Content.Sprite = bk;
        left.Content.MainColor = 0x69DCF8d0.ToColor();
        var top = UICreator.CreateHImage(new Vector3(0, 80, 0), new Vector2(100, 50), "Top", Docker.transform);
        top.Content.Sprite = bk;
        top.Content.MainColor = 0x69DCF8d0.ToColor();
        var right = UICreator.CreateHImage(new Vector3(80, 0, 0), new Vector2(50, 100), "Right", Docker.transform);
        right.Content.Sprite = bk;
        right.Content.MainColor = 0x69DCF8d0.ToColor();
        var down = UICreator.CreateHImage(new Vector3(0, -80, 0), new Vector2(100, 50), "Down", Docker.transform);
        down.Content.Sprite = bk;
        down.Content.MainColor = 0x69DCF8d0.ToColor();
    }
    [UnityEditor.MenuItem("GameObject/HGUI/DataGrid", false, 21)]
    static public void AddDataGrid(MenuCommand menuCommand)
    {
        GameObject parent = menuCommand.context as GameObject;
        var dg =  UICreator.CreateElement(Vector3.zero,new Vector2(1280,720),"DataGrid",parent.transform);
        dg.Content.Pivot = new Vector2(0f,1);
        dg.Content.compositeType = CompositeType.DataGrid;
        var grid = UICreator.CreateElement(Vector3.zero,new Vector2(100,100),"Grid",dg.transform);
        grid.Content.Pivot = new Vector2(0f,1f);
        grid.Content.marginType = MarginType.Margin;
        grid.Content.Mask = true;
        grid.Content.margin.top = 60;

        var items = UICreator.CreateElement(Vector3.zero,new Vector2(100,100),"Items",grid.transform);
        items.Content.Pivot = new Vector2(0f,1);

        var line = UICreator.CreateHLine(Vector3.zero, new Vector2(200, 60), "Line", grid.transform);
        line.Content.MainColor = new Color32(85, 85, 85, 255);
        line.Content.marginType = MarginType.Margin;

        var heads = new GameObject("Heads");
        heads.transform.SetParent(dg.transform);
        heads.transform.localScale = Vector3.one;
        heads.transform.localPosition = Vector3.zero;
        heads.transform.localRotation = Quaternion.identity;
        var drags = new GameObject("Drags");
        drags.transform.SetParent(dg.transform);
        drags.transform.localScale = Vector3.one;
        drags.transform.localPosition = Vector3.zero;
        drags.transform.localRotation = Quaternion.identity;

        var head = UICreator.CreateElement(Vector3.zero,new Vector2(200,60),"Head",dg.transform);
        head.Content.Pivot = new Vector2(0f,1);
        var img = UICreator.CreateHImage(Vector3.zero, new Vector2(200, 60), "Image", head.transform);
        img.Content.marginType = MarginType.Margin;
        var txt = UICreator.CreateHText(Vector3.zero, new Vector2(200, 60), "Text", head.transform);
        txt.Content.marginType = MarginType.Margin;
        txt.Content.TextAnchor = TextAnchor.MiddleLeft;
        txt.Content.FontSize = 32;
        txt.Content.Text = "字段名";
        txt.Content.MainColor = Color.black;
        var item= UICreator.CreateElement(Vector3.zero, new Vector2(200, 60), "Item", dg.transform);
        item.Content.Pivot = new Vector2(0f,1);
        txt = UICreator.CreateHText(Vector3.zero, new Vector2(200, 60), "Text", item.transform);
        txt.Content.marginType = MarginType.Margin;
        txt.Content.TextAnchor = TextAnchor.MiddleLeft;
        txt.Content.FontSize =28;
        txt.Content.Text = "数据";

        var drag = UICreator.CreateElement(Vector3.zero, new Vector2(40, 60), "Drag", dg.transform);
        drag.Content.eventType = HEventType.UserEvent;
      
    }
    [UnityEditor.MenuItem("GameObject/HGUI/PopMenu", false, 22)]
    static public void AddPopMenu(MenuCommand menuCommand)
    {
        GameObject parent = menuCommand.context as GameObject;
        var dg = UICreator.CreateElement(Vector3.zero, new Vector2(300, 80), "PopMenu", parent.transform);
        dg.Content.compositeType = CompositeType.PopMenu;
        var grid = UICreator.CreateHImage(Vector3.zero, new Vector2(300, 80), "Content", dg.transform);
        grid.Content.MainColor = new Color32(40,40,40,255);
        grid.Content.compositeType = CompositeType.PopMenu;
        var helper = grid.gameObject.AddComponent<StackPanelHelper>();
        helper.direction = Direction.Vertical;

        var item = UICreator.CreateElement(Vector3.zero, new Vector2(300, 60), "Item", dg.transform);
        item.Content.marginType = MarginType.MarginX;

        var img = UICreator.CreateHImage(Vector3.zero, new Vector2(300, 60), "Image", item.transform);
        img.Content.MainColor = new Color32(64,64,64,255);
        img.Content.marginType = MarginType.Margin;

        var  txt = UICreator.CreateHText(Vector3.zero, new Vector2(200, 60), "Text", item.transform);
        txt.Content.marginType = MarginType.Margin;
        txt.Content.margin = new Margin(5, 32, 5, 5);
        txt.Content.TextAnchor = TextAnchor.MiddleCenter;
        txt.Content.FontSize = 32;
        txt.Content.Text = "Item";

        img = UICreator.CreateHImage(new Vector3(138,0,0), new Vector2(24, 32), "Expand", item.transform);
        img.Content.anchorType = AnchorType.Alignment;
        img.Content.anchorPointType = AnchorPointType.Right;
        img.Content.Sprite = EditorModelManager.FindSprite(icons, "Next");
    }
}