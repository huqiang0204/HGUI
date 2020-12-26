using System;
using System.Collections.Generic;
using UnityEditor;

public class HGUIEditor:Attribute
{
    public Type Target;
    public HGUIEditor(Type target)
    {
        Target = target;
    }
}
