using huqiang;
using huqiang.Core.HGUI;
using huqiang.Core.UIData;
using huqiang.Data;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class CompositeMenu
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
    //[MenuItem("HGUI/Add/Canvas", false, 0)]
    //static public void AddCanvas(UIElement menuCommand)
    //{
    //    GameObject parent = menuCommand.context as GameObject;
    //    var go = new GameObject("Canvas", typeof(HCanvas));
    //    var trans = go.transform;
    //    trans.localPosition = Vector3.zero;
    //    trans.localScale = Vector3.one;
    //    trans.localRotation = Quaternion.identity;
    //    go.AddComponent<MeshRenderer>();
    //    go.AddComponent<MeshFilter>();
    //}
    [MenuItem("HGUI/Add/Page", false, 1)]
    static public void AddPage(UIElement parent)
    {
        string pagename = "page";
        bool root = false;
        if(parent!=null)
        {
            if (parent is HCanvas)
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
            //var canvas = UnityEngine.Object.FindObjectOfType<HCanvas>();
            //var go = new GameObject(classname, typeof(UIElement));
            //var trans = go.transform;
            //if (canvas != null)
            //    trans.SetParent(canvas.transform);
            //trans.localPosition = Vector3.zero;
            //trans.localScale = Vector3.one;
            //trans.localRotation = Quaternion.identity;
        }
        else if(root)
        {
            var go = new UIElement();
            go.SetParent(parent);
            go.localPosition = Vector3.zero;
            go.localScale = Vector3.one;
            go.localRotation = Quaternion.identity;
            go.name = classname;
        }
        AssetDatabase.Refresh();
    }
    [MenuItem("HGUI/Add/UIElement", false, 2)]
    static public UIElement AddEmpty(UIElement parent)
    {
        var go = new UIElement();
        go.SetParent(parent);
        go.localPosition = Vector3.zero;
        go.localScale = Vector3.one;
        go.localRotation = Quaternion.identity;
        go.name = "UIElement";
        return go;
    }
    [MenuItem("HGUI/Add/Image", false, 3)]
    static public UIElement AddImage(UIElement parent)
    {
        HImage img = new HImage();
        img.name = "Image";
        img.SizeDelta = new Vector2(100,100);
        img.SetParent(parent);
        return img;
    }
    [MenuItem("HGUI/Add/Text", false, 4)]
    static public UIElement AddText(UIElement parent)
    {
        HText img = new HText();
        img.name = "Text";
        img.SizeDelta = new Vector2(160, 40);
        img.SetParent(parent);
        return img;
    }
    [MenuItem("HGUI/Add/TextBox", false, 5)]
    static public UIElement AddTextBox(UIElement parent)
    {
        TextBox img = new TextBox();
        img.name = "Text";
        img.SizeDelta = new Vector2(400, 400);
        img.SetParent(parent);
        return img;
    }
    [MenuItem("HGUI/Add/InputBox", false, 6)]
    static public UIElement AddInputBox(UIElement parent)
    {
        HImage img = new HImage();
        img.name = "InputBox";
        img.SetParent(parent);
        img.localPosition = Vector3.zero;
        img.localScale = Vector3.one;
        img.localRotation = Quaternion.identity;
        img.compositeType = CompositeType.InputBox;
        img.Sprite = EditorModelManager.FindSprite(icons, box);
        img.SprType = SpriteType.Sliced;
        img.SizeDelta = new Vector2(400,100);

        HText txt = new HText();
        txt.SetParent(img);
        txt.localPosition = Vector3.zero;
        txt.localScale = Vector3.one;
        txt.localRotation = Quaternion.identity;
        txt.SizeDelta = new Vector2(380,90);
       
        txt.marginType = MarginType.Margin;
        txt.margin = new Margin(5,5,5,5);
        txt.name = "InputText";

        HImage cur = new HImage();
        cur.name = "Cursor";
        cur.SizeDelta = new Vector2(2, 28);
        return img;
    }
    [MenuItem("HGUI/Add/UISliderH", false, 7)]
    static public UIElement AddSliderH(UIElement parent)
    {
        var rect = CreateSliderH();
        rect.SetParent(parent);
        return rect;
    }
    static UIElement CreateSliderH()
    {
        var image = new HImage();
        image.name = "SliderH";
        image.SizeDelta = new Vector2(400, 20);
        image.compositeType = CompositeType.Slider;

        //var help = image.AddComponent<SliderHelper>();
        //help.StartOffset.x = -15;
        //help.EndOffset.x = -15;
        image.Sprite = EditorModelManager.FindSprite(icons, background);
        image.SprType = SpriteType.Sliced;

        var Fill = new HImage();
        Fill.name = "FillImage";
        Fill.SizeDelta = new Vector2(400, 20);
        Fill.SetParent(image);
        Fill.localPosition = Vector3.zero;
        Fill.localScale = Vector3.one;
        Fill.localRotation = Quaternion.identity;
        Fill.Sprite = EditorModelManager.FindSprite(icons, background);
        Fill.SprType = SpriteType.Filled;
        Fill.FillMethod = FillMethod.Horizontal;
        Fill.MainColor = new Color32(94, 137, 197, 255);

        var Nob = new HImage();
        Nob.name = "Nob";
        Nob.SizeDelta = new Vector2(30, 30);
        Nob.SetParent(image);
        Nob.localPosition = new Vector3(200, 0, 0);
        Nob.localScale = Vector3.one;
        Nob.localRotation = Quaternion.identity;
        image.MainColor = Color.green;
        image.Sprite = EditorModelManager.FindSprite(icons, leaves);
        return image;
    }
    [MenuItem("HGUI/Add/UISliderV", false, 8)]
    static public UIElement AddSliderV(UIElement parent)
    {
        var rect = CreateSliderV();
        rect.SetParent(parent);
        return rect;
    }
    static UIElement CreateSliderV(float startOffset=-15,float endOffset=-15)
    {
        HImage image = new HImage();
        image.name = "SliderV";
        image.SizeDelta = new Vector2(20, 400);
        image.compositeType = CompositeType.Slider;

        //var help = go.AddComponent<SliderHelper>();
        //help.StartOffset.y = startOffset;
        //help.EndOffset.y = endOffset;
        //help.direction = huqiang.UIComposite.UISlider.Direction.Vertical;
        image.Sprite = EditorModelManager.FindSprite(icons, background);
        image.SprType = SpriteType.Sliced;

        var Fill = new HImage();
        Fill.name = "FillImage";
        Fill.SizeDelta = new Vector2(20, 400);
        Fill.SetParent(image);
        Fill.localPosition = Vector3.zero;
        Fill.localScale = Vector3.one;
        Fill.localRotation = Quaternion.identity;
        Fill.Sprite = EditorModelManager.FindSprite(icons, background);
        Fill.SprType = SpriteType.Filled;
        Fill.FillMethod = FillMethod.Vertical;
        Fill.MainColor = new Color32(94, 137, 197, 255);

        var Nob = new HImage();
        Nob.name = "Nob";
        Nob.SizeDelta = new Vector2(30, 30);
        Nob.SetParent(image);
        Nob.localPosition = new Vector3(0, 200+startOffset, 0);
        Nob.localScale = Vector3.one;
        Nob.localRotation = Quaternion.identity;
        image.MainColor = Color.green;
        image.Sprite = EditorModelManager.FindSprite(icons, ufo);
        return image;
    }
    [MenuItem("HGUI/Add/Scroll", false, 9)]
    static public UIElement AddScroll(UIElement parent)
    {
        HImage scroll = new HImage();
        scroll.name = "gridScroll";
        scroll.SizeDelta = new Vector2(400,400);
        scroll.SetParent(parent);
        scroll.Mask = true;
        scroll.MainColor = new Color32(224, 224, 224, 255);
        scroll.eventType = HEventType.UserEvent;
        scroll.compositeType = CompositeType.GridScroll;
        scroll.Sprite = EditorModelManager.FindSprite(icons, background);
        scroll.SprType = SpriteType.Sliced;

        CreateItem(scroll, "Item");
        return scroll;
    }
    [MenuItem("HGUI/Add/UIRocker", false, 10)]
    static public UIElement AddRocker(UIElement parent)
    {
        var scr = new HImage();//ss.AddComponent<HImage>();
        scr.name = "Rocker";
        scr.SizeDelta = new Vector2(300, 300);
        scr.eventType = HEventType.UserEvent;
        scr.compositeType = CompositeType.Rocker;
        scr.Sprite = EditorModelManager.FindSprite(icons, circleol);
        scr.SetParent(parent);

        var Item = new HImage();//new GameObject("Nob");
        Item.name = "Nob";
        Item.SizeDelta = new Vector2(100, 100);
        Item.Sprite = EditorModelManager.FindSprite(icons, circlesm);
        Item.SetParent(scr);
        return scr;
    }
    [MenuItem("HGUI/Add/TreeView", false, 11)]
    static public UIElement AddTreeView(UIElement parent)
    {
        var ss = new HImage();//new GameObject("TreeView");
        ss.name = "TreeView";
        ss.SizeDelta = new Vector2(400, 400);
        ss.Mask = true;
        ss.compositeType = CompositeType.TreeView;
        ss.SetParent(parent);

        var Item = new UIElement();//new GameObject("Item");
        Item.SetParent(ss);
        Item.SizeDelta = new Vector2(200, 40);

        var txt = new HText();//new GameObject("Text");
        txt.SetParent(Item);
        txt.SizeDelta = new Vector2(200, 40);
        txt.TextAnchor = TextAnchor.MiddleLeft;
        txt.FontSize = 24;
        return ss;
    }
    [MenuItem("HGUI/Add/UIDate", false, 12)]
    static public UIElement AddDate(UIElement parent)
    {
        var date = new UIElement();//new GameObject("Date");
        date.SizeDelta = new Vector2(400,200);
        date.eventType = HEventType.UserEvent;
        date.compositeType = CompositeType.UIDate;
        date.SetParent(parent);

        var label = new HText();//new GameObject("YearLabel", typeof(HText));
        label.name = "YearLabel";
        //var ht = label.GetComponent<HText>();
        label.SizeDelta = new Vector2(80, 40);
        label.FontSize = 24;
        label.TextAnchor = TextAnchor.MiddleCenter;
        label.Text = "Year";
        label.SetParent(date);
        label.localPosition = new Vector3(-80,0,0);

        label = new HText();//new GameObject("MonthLabel", typeof(HText));
        label.name = "MonthLabel";
        label.SizeDelta = new Vector2(80, 40);
        label.FontSize = 24;
        label.TextAnchor = TextAnchor.MiddleCenter;
        label.Text = "Month";
        label.SetParent(date);
        label.localPosition = new Vector3(40, 0, 0);

        label = new HText();//new GameObject("DayLabel", typeof(HText));
        label.name = "DayLabel";
        label.SizeDelta = new Vector2(80, 40);
        label.FontSize = 24;
        label.TextAnchor = TextAnchor.MiddleCenter;
        label.Text = "Year";
        label.SetParent(date);
        label.localPosition = new Vector3(160, 0, 0);

        var now = DateTime.Now;
        var Year = new UIElement(); //new GameObject("Year", typeof(UIElement));
        Year.name = "Year";
        Year.SizeDelta = new Vector2(80,400);
        Year.eventType = HEventType.UserEvent;
        Year.compositeType = CompositeType.ScrollY;
        Year.SetParent(date);
        Year.localPosition = new Vector3(-147, 0, 0);

        var Item = new HText();//new GameObject("Item");
        Item.name = "Item";
        Item.SetParent(Year);
        Item.SizeDelta = new Vector2(60, 40);
        Item.TextAnchor = TextAnchor.MiddleCenter;
        Item.Text = now.Year.ToString();
        Item.FontSize = 24;

        var Month = new UIElement();//new GameObject("Month",typeof(UIElement));
        Month.name = "Month";
        Month.SizeDelta = new Vector2(80, 400);
        Month.eventType = HEventType.UserEvent;
        Month.compositeType = CompositeType.ScrollY;
        Month.SetParent(date);
        Month.localPosition = new Vector3(-22,0,0);
        Month.localScale = Vector3.one;
        Month.localRotation = Quaternion.identity;

        Item = new HText();//new GameObject("Item");
        Item.name = "Item";
        Item.SetParent(Month);
        Item.SizeDelta = new Vector2(60, 40);
        Item.TextAnchor = TextAnchor.MiddleCenter;
        Item.Text = now.Month.ToString();
        Item.FontSize = 24;

        var Day = new UIElement();//new GameObject("Day", typeof(UIElement));
        Day.name = "Day";
        Day.SizeDelta = new Vector2(80, 400);
        Day.eventType = HEventType.UserEvent;
        Day.compositeType = CompositeType.ScrollY;
        Day.SetParent(date);
        Day.localPosition = new Vector3(107, 0, 0);

        Item = new HText();//new GameObject("Item");
        Item.name = "Item";
        Item.SetParent(Day);
        Item.SizeDelta = new Vector2(60, 40);
        Item.TextAnchor = TextAnchor.MiddleCenter;
        Item.Text = now.Day.ToString();
        Item.FontSize = 24;
        return date;
    }
    [MenuItem("HGUI/Add/UIPalette", false, 13)]
    static public UIElement AddPalette(UIElement parent)
    {
        var palette = new HImage();//new GameObject("Palette");
        palette.name = "Palette";
        palette.SizeDelta = new Vector2(500,500);
        palette.eventType = HEventType.UserEvent;
        palette.compositeType = CompositeType.UIPalette;
        palette.SetParent(parent);

        var Fill = new HImage();//new GameObject("HTemplate");
        Fill.name = "HTemplate";
        Fill.eventType = HEventType.UserEvent;
        Fill.SizeDelta = new Vector2(256, 256);
        Fill.SetParent(palette);

        var Nob = new HImage();//new GameObject("NobA");
        Nob.name = "NobA";
        Nob.SetParent(palette);
        Nob.localPosition = new Vector3(0, -220, 0);
        Nob.SizeDelta = new Vector2(44,44);
        var aim = Nob.Sprite = EditorModelManager.FindSprite(icons, Aim);

        Nob = new HImage();//new GameObject("NobB");
        Nob.name = "NobB";
        Nob.SetParent(palette);
        Nob.localPosition = new Vector3(-128, 128, 0);
        Nob.SizeDelta= new Vector2(24, 24);
        Nob.Sprite = aim;

        var Slider = new HImage();//new GameObject("Slider");
        Slider.name = "Slider";
        Slider.SetParent(palette);
        Slider.localPosition = new Vector3(0, -285, 0);
        Slider.eventType = HEventType.UserEvent;
        Slider.compositeType = CompositeType.Slider;
        Slider.SizeDelta = new Vector2(400, 20);

        Nob = new HImage();//new GameObject("Nob");
        Nob.name = "Nob";
        Nob.SetParent(Slider);
        Nob.localPosition = new Vector3(200, 0, 0);
        Nob.SizeDelta= new Vector2(30, 30);
        Nob.MainColor = new Color(1, 1, 1, 1f);
        Nob.Sprite = aim;

        //palette.AddComponent<PaletteHelper>().Initial();
        return palette;
    }
    [MenuItem("HGUI/Add/ScrollY", false, 14)]
    static public UIElement AddScrollY(UIElement parent)
    {
        UIElement p = UICreator.CreateElement(new Vector3(-10,200,0),new Vector2(420,400),"Scroll",parent);
        var scroll = UICreator.CreateHImage(Vector3.zero, new Vector2(400, 400), "ScrollY", p);
        scroll.marginType = MarginType.Margin;
        scroll.margin.right = 20;
        scroll.Mask = true;
        scroll.MainColor = new Color32(224, 224, 224, 255);
        scroll.eventType = HEventType.UserEvent;
        scroll.compositeType = CompositeType.ScrollY;
        scroll.Sprite = EditorModelManager.FindSprite(icons, background);
        scroll.SprType = SpriteType.Sliced;
        scroll.Pivot = new Vector2(0.5f,1);

        var item =CreateItem(scroll, "Item");
        item.Pivot = new Vector2(0.5f, 1);
        item.Find("Image").localPosition = new Vector3(0, -50, 0);
        item.Find("Text").localPosition = new Vector3(0, -50, 0);

        var go = CreateSliderV();
        go.SetParent(p);
        go.localPosition = new Vector3(200, 0, 0);

        go.anchorType = AnchorType.Alignment;
        go.anchorPointType = AnchorPointType.Right;
        go.marginType = MarginType.MarginY;
        //var help = scroll.gameObject.AddComponent<ScrollHelper>();
        //help.Slider = go.transform;
        return p;
    }
    [MenuItem("HGUI/Add/ScrollYExtand", false, 15)]
    static public UIElement AddScrollYExtand(UIElement parent)
    {
        var scroll = UICreator.CreateHImage(new Vector3(0,400,0), new Vector2(400, 800), "ScrollEx", parent);
        scroll.Mask = true;
        scroll.MainColor = new Color32(224, 224, 224, 255);
        scroll.eventType = HEventType.UserEvent;
        scroll.compositeType = CompositeType.ScrollYExtand;
        scroll.Sprite = EditorModelManager.FindSprite(icons, background);
        scroll.SprType = SpriteType.Sliced;
        scroll.Pivot = new Vector2(0.5f, 1);

        var bds = UICreator.CreateElement(Vector3.zero, new Vector2(400, 100), "Bodys",  scroll);
        bds.anchorType = AnchorType.Alignment;
        bds.anchorPointType = AnchorPointType.Top;
        bds.marginType = MarginType.MarginX;
        bds.Pivot = new Vector2(0.5f,1);

        bds = UICreator.CreateElement(Vector3.zero, new Vector2(400, 100), "Titles", scroll);
        bds.anchorType = AnchorType.Alignment;
        bds.anchorPointType = AnchorPointType.Top;
        bds.marginType = MarginType.MarginX;
        bds.Pivot = new Vector2(0.5f, 1);

        CreateItemE(scroll,"Title");
        var item= CreateItemE(scroll, "Item");
        item.localPosition = new Vector3(0,-100, 0);
        var tail = CreateItemE(scroll,"Tail");
        tail.localPosition = new Vector3(0,-200,0);

        var body = new UIElement();//new GameObject("Body");
        body.name = "Body";
        body.SetParent(scroll);
        body.localPosition = new Vector3(0, -300, 0);
        body.SizeDelta = new Vector2(400,100);
        body.Pivot = new Vector2(0.5f,1);
        body.Mask = true;
        return scroll;
    }
    static UIElement CreateItemE(UIElement parent, string name)
    {
        var mod = UICreator.CreateElement(Vector3.zero, new Vector2(400, 100), name, parent);
        mod.Pivot = new Vector2(0.5f,1);

        var img = UICreator.CreateHImage(Vector3.zero, new Vector2(400, 90), "Image", mod);
        img.Pivot = new Vector2(0.5f,1);
        img.MainColor = new Color32(68, 68, 68, 255);
        img.eventType = HEventType.UserEvent;
        img.Sprite = EditorModelManager.FindSprite(icons, background);
        img.SprType = SpriteType.Sliced;

        var txt = UICreator.CreateHText(Vector3.zero, new Vector2(400, 80), "Text", mod);
        txt.Pivot = new Vector2(0.5f,1);
        txt.Text = name;
        txt.FontSize = 30;
        txt.TextAnchor = TextAnchor.MiddleLeft;
        return mod;
    }
    [MenuItem("HGUI/Add/ScrollX", false, 16)]
    static public UIElement AddScrollX(UIElement parent)
    {
        UIElement p = UICreator.CreateElement(Vector3.zero, new Vector2(400, 420), "Scroll", parent);
        var scroll = UICreator.CreateHImage(Vector3.zero,new Vector2(400,400),"ScrollX",p);
        scroll.marginType = MarginType.Margin;
        scroll.margin.down = 20;
        scroll.Mask = true;
        scroll.MainColor = new Color32(224, 224, 224, 255);
        scroll.eventType = HEventType.UserEvent;
        scroll.compositeType = CompositeType.ScrollX;
        scroll.Sprite = EditorModelManager.FindSprite(icons, background);
        scroll.SprType = SpriteType.Sliced;

        CreateItem(scroll, "Item");

        var go = CreateSliderH();
        go.SetParent(p);
        go.localPosition = new Vector3(200, 0, 0);
        go.anchorType = AnchorType.Alignment;
        go.anchorPointType = AnchorPointType.Down;
        go.marginType = MarginType.MarginX;
        //var help = scroll.gameObject.AddComponent<ScrollHelper>();
        //help.Slider = go.transform;
        return p;
    }
    static UIElement CreateItem(UIElement parent,string name)
    {
        var mod = new UIElement();
        mod.name = name;
        mod.SizeDelta = new Vector2(100, 100);
        mod.SetParent(parent);//UICreator.CreateElement(Vector3.zero,new Vector2(100,100),name,parent);

        var img = new HImage();
        img.name = "Image";
        img.SizeDelta = new Vector2(90, 90);
        img.SetParent(mod);//UICreator.CreateHImage(Vector3.zero,new Vector2(90,90),"Image",mod.transform);
        img.MainColor = new Color32(68,68,68,255);
        img.eventType = HEventType.UserEvent;
        img.Sprite = EditorModelManager.FindSprite(icons, background);
        img.SprType = SpriteType.Sliced;

        var txt = new HText();
        txt.name = "Text";
        txt.SizeDelta = new Vector2(80, 80);
        txt.SetParent(mod);//UICreator.CreateHText(Vector3.zero,new Vector2(80,80),"Text",mod.transform);
        txt.Text = name;
        txt.FontSize = 30;
        txt.TextAnchor = TextAnchor.MiddleLeft;
        return mod;
    }
    [MenuItem("HGUI/Add/DropDown", false, 17)]
    static public UIElement AddDropDown(UIElement parent)
    {
        var drop = new HImage();//new GameObject("DropDown");
        drop.name = "DropDown";
        drop.SizeDelta=new Vector2(300,60);
        drop.Sprite = EditorModelManager.FindSprite(icons, background);
        drop.SprType = SpriteType.Sliced;
        drop.MainColor = new Color32(224,224,224,255);
        drop.eventType = HEventType.UserEvent;
        drop.compositeType = CompositeType.DropDown;
        drop.SetParent(parent);

        var txt = UICreator.CreateHText(Vector3.zero, Vector2.zero, "Label", drop);
        txt.TextAnchor = TextAnchor.MiddleLeft;
        txt.FontSize = 36;
        txt.marginType = MarginType.Margin;
        txt.margin = new Margin(20, 100, 5, 5);
        txt.Text = "Label";
        txt.MainColor = Color.black;

        var img = UICreator.CreateHImage(Vector3.zero, new Vector2(50, 50), "Arrow", drop);
        img.SizeDelta = new Vector2(50,50);
        img.Sprite = EditorModelManager.FindSprite(icons, diamond);
        img.anchorPointType = AnchorPointType.Right;
        img.anchorType = AnchorType.Alignment;
        img.MainColor = Color.black;

        var main = UICreator.CreateHImage(new Vector3(0,-34,0), new Vector2(300, 300), "Scroll", drop);
        main.Pivot = new Vector2(0.5f, 1);
        main.Mask = true;
        main.MainColor = new Color32(224, 224, 224, 255);
        main.eventType = HEventType.UserEvent;
        main.compositeType = CompositeType.ScrollY;
        main.Sprite = EditorModelManager.FindSprite(icons, background);
        main.SprType = SpriteType.Sliced;

        var item = UICreator.CreateElement(new Vector3(0, -150, 0), new Vector2(300, 60), "Item", main);
        item.eventType = HEventType.UserEvent;
        img = UICreator.CreateHImage(Vector3.zero,new Vector2(290,50),"Image",item);
        img.Sprite = EditorModelManager.FindSprite(icons, background);
        img.SprType = SpriteType.Sliced;
        txt = UICreator.CreateHText(Vector3.zero, Vector2.zero, "Text", item);
        txt.marginType = MarginType.Margin;
        txt.margin = new Margin(60,60,5,5);
        txt.MainColor = Color.black;
        txt.Text = "Option";
        txt.FontSize = 32;
        txt.TextAnchor = TextAnchor.MiddleLeft;
        img= UICreator.CreateHImage(Vector3.zero, new Vector2(30, 30), "Check", item);
        img.Sprite = EditorModelManager.FindSprite(icons, star);
        img.anchorType = AnchorType.Alignment;
        img.anchorOffset.x = 20;
        img.anchorPointType = AnchorPointType.Left;
        img.MainColor = Color.black;

        var slider = UICreator.CreateHImage(new Vector3(140,-150,0), new Vector2(20, 300), "Slider", main);
        slider.eventType = HEventType.UserEvent;
        slider.compositeType = CompositeType.Slider;
        slider.Sprite = EditorModelManager.FindSprite(icons, background);
        slider.SprType = SpriteType.Sliced;
        slider.MainColor = 0x295B7680.ToColor();

        //var help = slider.gameObject.AddComponent<SliderHelper>();
        //help.direction = huqiang.UIComposite.UISlider.Direction.Vertical;
    

        var Nob = UICreator.CreateHImage(new Vector3(0, 135, 0), new Vector2(20, 30), "Nob", slider);
        Nob.MainColor = 0x5F5263ff.ToColor();
        Nob.Sprite = EditorModelManager.FindSprite(icons, background);
        Nob.SprType = SpriteType.Sliced;
        return drop;
    }
    [MenuItem("HGUI/Add/TabControl", false, 18)]
    static public UIElement AddTabControl(UIElement parent)
    {
        //GameObject parent = menuCommand.context as GameObject;
        //Transform pt = null;
        //if (parent != null)
        //    pt = parent.transform;
       return CreateTabControl(parent);
    }
    static UIElement CreateTabControl(UIElement parent)
    {
        var tab = UICreator.CreateHImage(Vector3.zero, new Vector2(800, 800), "TabControl",parent);
        tab.Mask = true;
        tab.MainColor = new Color32(204, 204, 204, 255);
        tab.eventType = HEventType.UserEvent;
        tab.compositeType = CompositeType.TabControl;
        tab.Sprite = EditorModelManager.FindSprite(icons, background);
        tab.SprType = SpriteType.Sliced;

        var head = UICreator.CreateElement(Vector3.zero, new Vector2(100, 44), "Head", tab);
        head.marginType = MarginType.MarginX;
        head.anchorType = AnchorType.Alignment;
        head.anchorPointType = AnchorPointType.Top;

        var items = UICreator.CreateElement(Vector3.zero, new Vector2(800, 42), "Items", head);
        items.marginType = MarginType.Margin;
        items.margin.down = 2;
        items.compositeType = CompositeType.StackPanel;

        var line = UICreator.CreateHImage(Vector3.zero, new Vector2(100, 2), "Line", head);
        line.anchorType = AnchorType.Alignment;
        line.anchorPointType = AnchorPointType.Down;
        line.marginType = MarginType.MarginX;

        var item = UICreator.CreateElement(new Vector3(-350, 380), new Vector2(100, 40), "Item", tab);

        var img = UICreator.CreateHImage(Vector3.zero, new Vector2(100, 40), "Image", item);
        img.MainColor = new Color32(168, 168, 168, 255);

        var txt = UICreator.CreateHText(Vector3.zero, new Vector2(80, 40), "Text", item);
        txt.MainColor = Color.black;
        txt.Text = "Label";
        txt.FontSize = 24;
        txt.TextAnchor = TextAnchor.MiddleLeft;

        var content = UICreator.CreateElement(Vector3.zero, new Vector2(800, 756), "Content", tab);
        content.marginType = MarginType.Margin;
        content.margin.top = 44;
        return tab;
    }
    [MenuItem("HGUI/Add/DockPanel", false, 19)]
    static public UIElement AddDockPanel(UIElement parent)
    {
        //var game = menuCommand.context as GameObject;
        //Transform pt = null;
        //if (game != null)
        //    pt = game.transform;
        return CreateDockPanel(parent);
    }
    static UIElement CreateDockPanel(UIElement parent)
    {
        var dock = UICreator.CreateHImage(Vector3.zero, new Vector2(800, 800), "DockPanel", parent);
        dock.MainColor = new Color32(56, 56, 56, 255);
        dock.compositeType = CompositeType.DockPanel;
        dock.Sprite = EditorModelManager.FindSprite(icons, background);
        dock.SprType = SpriteType.Sliced;


        var AreaLevel = new UIElement();//new GameObject("AreaLevel");
        AreaLevel.name = "AreaLevel";
        AreaLevel.SetParent(dock);

        var LineLevel = new UIElement();//new GameObject("LineLevel");
        LineLevel.name = "LineLevel";
        LineLevel.SetParent(dock);

        var Line = UICreator.CreateHImage(Vector3.zero,new Vector2(800,8),"Line", dock);
        Line.MainColor = Color.black;
        UICreator.CreateHImage(new Vector3(0,-60,0), new Vector2(100, 100), "Area", dock);
 
        return dock;
    }
    [MenuItem("HGUI/Add/DesignedDockPanel", false, 20)]
    static public UIElement AddDesignedDockPanel(UIElement parent)
    {
        var dock = CreateDockPanel(parent);
        dock.name = "DesignedDockPanel";
        dock.compositeType = CompositeType.DesignedDockPanel;

        var drag = UICreator.CreateHImage(Vector3.zero, new Vector2(40, 40), "Drag", dock);
        drag.Sprite = EditorModelManager.FindSprite(icons, file);

        CreateAuxiliary(dock);
        return dock;
    }
    static void CreateAuxiliary(UIElement parent)
    {
        var aux = UICreator.CreateElement(Vector3.zero, new Vector2(800, 800), "Auxiliary", parent);
        aux.marginType = MarginType.Margin;

        var tab = CreateTabControl(aux);
        tab.marginType = MarginType.Margin;

        var item = tab.Find("Item");

        var clo = UICreator.CreateHImage(new Vector3(41.5f,0,0),new Vector2(30,30),"Close",item);
        clo.eventType = HEventType.UserEvent;
        clo.Sprite = EditorModelManager.FindSprite(icons, close);

        var Cover = UICreator.CreateHImage(Vector3.zero,new Vector2(30,30),"Cover",aux);
        Cover.MainColor= new Color32(128, 128, 128, 128);

        var Docker = UICreator.CreateElement(Vector3.zero, new Vector2(800, 800), "Docker",aux);

        var bk= EditorModelManager.FindSprite(icons, background);
        var Center = UICreator.CreateHImage(Vector3.zero, new Vector2(80, 80), "Center", Docker);
        Center.Sprite = bk;
        Center.MainColor = 0x69DCF8d0.ToColor();
        var left = UICreator.CreateHImage(new Vector3(-80,0,0), new Vector2(50, 100), "Left", Docker);
        left.Sprite = bk;
        left.MainColor = 0x69DCF8d0.ToColor();
        var top = UICreator.CreateHImage(new Vector3(0, 80, 0), new Vector2(100, 50), "Top", Docker);
        top.Sprite = bk;
        top.MainColor = 0x69DCF8d0.ToColor();
        var right = UICreator.CreateHImage(new Vector3(80, 0, 0), new Vector2(50, 100), "Right", Docker);
        right.Sprite = bk;
        right.MainColor = 0x69DCF8d0.ToColor();
        var down = UICreator.CreateHImage(new Vector3(0, -80, 0), new Vector2(100, 50), "Down", Docker);
        down.Sprite = bk;
        down.MainColor = 0x69DCF8d0.ToColor();
    }
    [MenuItem("HGUI/Add/DataGrid", false, 21)]
    static public UIElement AddDataGrid(UIElement parent)
    {
        var dg =  UICreator.CreateElement(Vector3.zero,new Vector2(1280,720),"DataGrid",parent);
        dg.Pivot = new Vector2(0f,1);
        dg.compositeType = CompositeType.DataGrid;
        var grid = UICreator.CreateElement(Vector3.zero,new Vector2(100,100),"Grid",dg);
        grid.Pivot = new Vector2(0f,1f);
        grid.marginType = MarginType.Margin;
        grid.Mask = true;
        grid.margin.top = 60;

        var items = UICreator.CreateElement(Vector3.zero,new Vector2(100,100),"Items",grid);
        items.Pivot = new Vector2(0f,1);

        var line = UICreator.CreateHLine(Vector3.zero, new Vector2(200, 60), "Line", grid);
        line.MainColor = new Color32(85, 85, 85, 255);
        line.marginType = MarginType.Margin;

        var heads = new UIElement();//new GameObject("Heads");
        heads.name = "Heads";
        heads.SetParent(dg);
        var drags = new UIElement();//new GameObject("Drags");
        drags.name = "Drags";
        drags.SetParent(dg);

        var head = UICreator.CreateElement(Vector3.zero,new Vector2(200,60),"Head",dg);
        head.Pivot = new Vector2(0f,1);
        var img = UICreator.CreateHImage(Vector3.zero, new Vector2(200, 60), "Image", head);
        img.marginType = MarginType.Margin;
        var txt = UICreator.CreateHText(Vector3.zero, new Vector2(200, 60), "Text", head);
        txt.marginType = MarginType.Margin;
        txt.TextAnchor = TextAnchor.MiddleLeft;
        txt.FontSize = 32;
        txt.Text = "字段名";
        txt.MainColor = Color.black;
        var item= UICreator.CreateElement(Vector3.zero, new Vector2(200, 60), "Item", dg);
        item.Pivot = new Vector2(0f,1);
        txt = UICreator.CreateHText(Vector3.zero, new Vector2(200, 60), "Text", item);
        txt.marginType = MarginType.Margin;
        txt.TextAnchor = TextAnchor.MiddleLeft;
        txt.FontSize =28;
        txt.Text = "数据";

        var drag = UICreator.CreateElement(Vector3.zero, new Vector2(40, 60), "Drag", dg);
        drag.eventType = HEventType.UserEvent;
        return dg;
    }
}