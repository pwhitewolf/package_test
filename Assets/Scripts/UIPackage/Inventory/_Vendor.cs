using UnityEngine;
using System.Collections;

/// <summary>
/// 商店类，继承自存储物品基类
/// </summary>
public class _Vendor : _Inventory
{
    //单例模式
    private static _Vendor _instance;
    public static _Vendor Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.Find("VendorPanel").GetComponent<_Vendor>();
            }
            return _instance;
        }
    }

    public int[] itemIdArray;//一个物品ID的数组，用于给商贩初始化
    private _Player player;//对主角Player脚本的引用，用于购买物品功能

    public override void Start()
    {
        base.Start();
        InitShop();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<_Player>();
        Hide();
    }

    //初始化商贩
    private void InitShop()
    {
        foreach (int itemId in itemIdArray)
        {
            StoreItem(itemId);
        }
    }

    //主角购买物品
    public void BuyItem(_Item item)
    {
        bool isSusscess = player.ConsumeCoin(item.BuyPrice);//主角消耗金币购买物品
        if (isSusscess)
        {
            _Knapscak.Instance.StoreItem(item);
        }
    }

    //主角售卖物品
    public void SellItem()
    {
        int sellAmount = 1;//销售数量
        if (Input.GetKey(KeyCode.LeftControl))//按住坐标Ctrl键物品一个一个的售卖，否则全部售卖
        {
            sellAmount = 1;
        }
        else
        {
            sellAmount = _InventroyManager.Instance.PickedItem.Amount;
            //_Vendor.Instance.StoreItem(item);
        }
        int coinAmount = _InventroyManager.Instance.PickedItem.Item.SellPrice * sellAmount;//售卖所获得的金币总数
        player.EarnCoin(coinAmount);//主角赚取到售卖物品的金币
        _InventroyManager.Instance.ReduceAmountItem(sellAmount);//鼠标上的物品减少或者销毁
    }

}
