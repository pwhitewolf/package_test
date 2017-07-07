using UnityEngine;
using System.Collections;

/// <summary>
/// 背包类，继承自存储物品基类
/// </summary>
public class _Knapscak : _Inventory
{
    //单例模式 
    private static _Knapscak _instance;
    public static _Knapscak Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.Find("KnapscakPanel").GetComponent<_Knapscak>();
            }
            return _instance;
        }
    }
}
