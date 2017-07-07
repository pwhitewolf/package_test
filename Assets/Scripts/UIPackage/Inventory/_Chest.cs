using UnityEngine;
using System.Collections;

/// <summary>
/// 箱子类，继承自存储物品基类
/// </summary>
public class _Chest : _Inventory
{
    //单例模式
    private static _Chest _instance;
    public static _Chest Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.Find("ChestPanel").GetComponent<_Chest>();
            }
            return _instance;
        }
    }

}
