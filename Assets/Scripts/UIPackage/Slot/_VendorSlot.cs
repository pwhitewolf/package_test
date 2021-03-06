﻿using UnityEngine;
using System.Collections;

/// <summary>
/// 商店面板的物品槽类，继承自物品槽基类
/// </summary>
public class _VendorSlot : _Slot {

    //重写父类Slot的鼠标点击方法，但是什么都不用做，因为不需要交互，商贩只管卖物品
    public override void OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
    {
        if (eventData.button == UnityEngine.EventSystems.PointerEventData.InputButton.Right)//在商贩那里点击右键直接购买物品
        {
            if (transform.childCount > 0 && _InventroyManager.Instance.IsPickedItem == false)//首先商贩得有物品,其次鼠标上没有物品
            {
                _Item currentItem = transform.GetChild(0).GetComponent<_ItemUI>().Item;//取得当前点击的要买的物品
                transform.parent.SendMessage("BuyItem", currentItem);
            }
        }
        else if (eventData.button == UnityEngine.EventSystems.PointerEventData.InputButton.Left && _InventroyManager.Instance.IsPickedItem == true)//在背包鼠标左键拖动售卖物品
        {
            //_Item currentItem = GameObject.Find("PickedItem").GetComponent<_ItemUI>().Item;
            //transform.parent.SendMessage("SellItem",currentItem);
            transform.parent.SendMessage("SellItem");
        }
    }
}
