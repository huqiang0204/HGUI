using huqiang.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ui帮助类,只存在Editor模式
/// </summary>
[DisallowMultipleComponent]
public class UICompositeHelp : MonoBehaviour
{
    /// <summary>
    /// 将数据存储到FakeStrcut中
    /// </summary>
    /// <param name="data">数据缓存</param>
    /// <returns>FakeStruct数据</returns>
    public virtual object ToBufferData(DataBuffer data)
    {
        return null;
    }
    /// <summary>
    /// 刷新
    /// </summary>
    public virtual void Refresh()
    {

    }
    /// <summary>
    /// 重新设置尺寸
    /// </summary>
    public virtual void ReSize()
    {
    }
}
