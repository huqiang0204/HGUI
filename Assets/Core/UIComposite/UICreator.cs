using huqiang.UI;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace huqiang.UIComposite
{
    public class UICreator
    {
        private const string icons = "icons";
        private const string Aim = "Aim";
        private const string background = "Background2";
        private const string file = "Pinned-Notices";
        private const string close = "Close";
        private const string list = "list";
        private const string line = "Line";
        private const string leaves = "Leaves";
        private const string ufo = "Ufo";
        private const string circleol = "Circle-Outline";
        private const string circlesm = "Circle-Small";
        private const string magicstick = "Magic-Stick";
        static public UISlider CreateSliderH(string name)
        {
            var mod = ModelElement.CreateNew(name);
            mod.data.sizeDelta = new Vector2(400, 20);

            var image = mod.AddComponent<ImageElement>();
            image.textureName = icons;
            image.spriteName = background;
            image.data.type = Image.Type.Sliced;

            var Fill = ModelElement.CreateNew("FillImage");
            Fill.data.sizeDelta = new Vector2(400, 20);
            Fill.SetParent(mod);
            image = Fill.AddComponent<ImageElement>();
            image.textureName = icons;
            image.spriteName = background;
            image.data.type = Image.Type.Sliced;
            image.data.fillMethod = Image.FillMethod.Horizontal;
            image.color =  image.data.color = new Color32(94, 137, 197, 255);

            var Nob = ModelElement.CreateNew("Nob");
            Nob.data.sizeDelta = new Vector2(30, 30);
            Nob.SetParent(mod);
            Nob.data.localPosition = new Vector3(200, 0, 0);
            image = Nob.AddComponent<ImageElement>();
            image.color = image.data.color = Color.green;
            image.textureName = icons;
            image.spriteName = leaves;

            var slider = new UISlider();
            slider.Initial(mod);
            slider.info.direction = UISlider.Direction.Horizontal;
            return slider;
        }
        static public UISlider CreateSliderV(string name)
        {
            var mod = ModelElement.CreateNew(name);
            mod.data.sizeDelta = new Vector2(400, 20);

            var image = mod.AddComponent<ImageElement>();
            image.textureName = icons;
            image.spriteName = background;
            image.data.type = Image.Type.Sliced;

            var Fill = ModelElement.CreateNew("FillImage");
            Fill.data.sizeDelta = new Vector2(20, 400);
            Fill.SetParent(mod);
            image = Fill.AddComponent<ImageElement>();
            image.textureName = icons;
            image.spriteName = background;
            image.data.type = Image.Type.Sliced;
            image.data.fillMethod = Image.FillMethod.Horizontal;
            image.color = image.data.color = new Color32(94, 137, 197, 255);

            var Nob = ModelElement.CreateNew("Nob");
            Nob.data.sizeDelta = new Vector2(30, 30);
            Nob.SetParent(mod);
            Nob.data.localPosition = new Vector3(0, 200, 0);
            image = Nob.AddComponent<ImageElement>();
            image.color = image.data.color = Color.green;
            image.textureName = icons;
            image.spriteName = leaves;

            var slider = new UISlider();
            slider.Initial(mod);
            slider.info.direction = UISlider.Direction.Vertical;
            return slider;
        }


        static public UIRocker CreateRocker(string name)
        {
            var mod = ModelElement.CreateNew(name);
            mod.data.sizeDelta = new Vector2(300, 300);
            var img = mod.AddComponent<ImageElement>();
            img.textureName = icons;
            img.spriteName = circleol;

            var Item = ModelElement.CreateNew("Nob");
            Item.data.sizeDelta = new Vector2(100, 100);
            Item.SetParent(mod);
            img = Item.AddComponent<ImageElement>();
            img.textureName = icons;
            img.spriteName = circlesm;

            UIRocker rocker = new UIRocker();
            rocker.Initial(mod);
            return rocker;
        }
        static public UIPalette CreatePalette(string name)
        {
            var mod = ModelElement.CreateNew(name);
            mod.data.sizeDelta = new Vector2(500, 500);
            mod.AddComponent<RawImageElement>();

            var Fill = ModelElement.CreateNew("HTemplate");
            Fill.data.sizeDelta = new Vector2(256, 256);
            Fill.SetParent(mod);
            Fill.AddComponent<RawImageElement>();

            var Nob = ModelElement.CreateNew("NobA");
            Nob.data.sizeDelta = new Vector2(44, 44);
            Nob.SetParent(mod);
            Nob.data.localPosition = new Vector3(0, -220, 0);
            var img = Nob.AddComponent<ImageElement>();
            img.textureName = icons;
            img.spriteName = Aim;

            Nob = ModelElement.CreateNew("NobB");
            Nob.data.sizeDelta = new Vector2(24, 24);
            Nob.SetParent(mod);
            Nob.data.localPosition = new Vector3(-128, 128, 0);
            img = Nob.AddComponent<ImageElement>();
            img.textureName = icons;
            img.spriteName = Aim;

            var Slider = ModelElement.CreateNew("Slider");
            Slider.data.sizeDelta = new Vector2(400, 20);
            Slider.SetParent(mod);
           Slider.data.localPosition = new Vector3(0, -285, 0);
            Slider.AddComponent<RawImageElement>();

            Nob = ModelElement.CreateNew("Nob");
            Nob.data.sizeDelta = new Vector2(30, 30);
            Nob.SetParent(Slider);
            Nob.data.localPosition = new Vector3(200, 0, 0);
            img = Nob.AddComponent<ImageElement>();
            img.color = img.data.color = new Color(1, 1, 1, 1f);
            img.textureName = icons;
            img.spriteName = Aim;
            UIPalette palette = new UIPalette();
            palette.Initial(mod);

            return palette;
        }
    }
}
