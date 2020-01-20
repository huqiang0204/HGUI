using huqiang.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICompositeHelp : MonoBehaviour
{
    [HideInInspector]
    public bool ForbidChild = false;
    // Start is called before the first frame update
    public virtual object ToBufferData(DataBuffer data)
    {
        return null;
    }
    public virtual void Refresh()
    {

    }
    public virtual void ReSize()
    {
    }
}
