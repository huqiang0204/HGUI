using System.Collections;
using UnityEngine;
using huqiang.Core.HGUI;
using huqiang.Data;

public class EditorUIData
{
    public static void LoadUI(FakeStruct fake, UIElement parent)
    {
        string type = fake.GetData<string>(0);
        switch(type)
        {
            case "UIElement":
                LoadUIElement(fake, parent);
                break;
            case "HImage":
                LoadUIElement(fake, parent);
                break;
        }
    }
    public static void LoadUIElement(FakeStruct fake, UIElement parent)
    {
        UIElement ui = new UIElement();
        ui.parent = parent;
        ui.name = fake.GetData<string>(1);
        
    }
    public static void SaveUI(DataBuffer db, UIElement ui)
    {
        
    }

}
