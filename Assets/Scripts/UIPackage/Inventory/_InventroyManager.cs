using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using LitJson;

/// <summary>
/// 存放物品总管理类
/// </summary>
public class _InventroyManager : MonoBehaviour {

    //单例模式
    private static _InventroyManager _instance;
    public static _InventroyManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.Find("InventroyManager").GetComponent<_InventroyManager>();
            }
            return _instance;
        }
    }
    private List<_Item> itemList;//存储json解析出来的物品列表

    private _ToolTip toolTip;//获取ToolTip脚本，方便对其管理
    private bool isToolTipShow = false;//提示框是否在显示状态
    private Canvas canvas;//Canva物体
    private Vector2 toolTipOffset = new Vector2(15, -10);//设置提示框跟随时与鼠标的偏移

    private _ItemUI pickedItem;//鼠标选中的物品的脚本组件，用于制作拖动功能 
    public _ItemUI PickedItem { get { return pickedItem; } }
    private bool isPickedItem = false;//鼠标是否选中该物品
    public bool IsPickedItem { get { return isPickedItem; } }

    void Awake()
    {
        ParseItemJson();
    }

    void Start()
    {
        toolTip = GameObject.FindObjectOfType<_ToolTip>();//根据类型获取
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        pickedItem = GameObject.Find("PickedItem").GetComponent<_ItemUI>();
        pickedItem.Hide();//开始为隐藏状态
    }

    void Update()
    {
        if (isToolTipShow == true && isPickedItem == false) //控制提示框跟随鼠标移动
        {
            Vector2 postionToolTip;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, null, out postionToolTip);
            toolTip.SetLocalPosition(postionToolTip + toolTipOffset);//设置提示框的位置，二维坐标会自动转化为三维坐标但Z坐标为0
        }
        else if (isPickedItem == true) //控制盛放物品的容器UI跟随鼠标移动
        {
            Vector2 postionPickeItem;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, null, out postionPickeItem);
            pickedItem.SetLocalPosition(postionPickeItem);//设置容器的位置，二维坐标会自动转化为三维坐标但Z坐标为0
        }
        //物品丢弃功能：
        if (isPickedItem == true && Input.GetMouseButtonDown(0) && UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(-1) == false)//UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(-1) == false 表示判断鼠标是否正在在UI上
        {
            isPickedItem = false;
            pickedItem.Hide();
        }
    }

    /// <summary>
    /// 解析Json文件
    /// </summary>
    public void ParseItemJson()
    {
        itemList = new List<_Item>();
        //文本在unity里面是TextAsset类型
        TextAsset itemText = Resources.Load<TextAsset>("_ItemJS");//加载json文本
        
        string itemJson = itemText.text;//得到json文本里面的内容
        //Debug.Log(itemJson);
        JsonData data = JsonMapper.ToObject(itemJson);

        for (int i = 0; i < data.Count; i++)
        {
            int id = (int)data[i]["id"];
            string name = (string)data[i]["name"];
            _Item.ItemType type = (_Item.ItemType)System.Enum.Parse(typeof(_Item.ItemType), (string)data[i]["type"]);
            _Item.ItemQuality quality = (_Item.ItemQuality)System.Enum.Parse(typeof(_Item.ItemQuality), (string)data[i]["quality"]);
            string description = (string)data[i]["description"];
            int capacity = (int)data[i]["capacity"];
            int buyPrice = (int)data[i]["buyPrice"];
            int sellPrice = (int)data[i]["sellPrice"];
            string sprite = (string)data[i]["sprite"];
            _Item item = null;
            switch (type)
            {
                case _Item.ItemType.Consumable:
                    int hp = (int)data[i]["hp"];
                    int mp = (int)data[i]["mp"];
                    item = new _Consumable(id, name, type, quality, description, capacity, buyPrice, sellPrice, sprite, hp, mp);
                    break;
                case _Item.ItemType.Equipment:
                    int strength = (int)data[i]["strength"];
                    int intellect = (int)data[i]["intellect"];
                    int agility = (int)data[i]["agility"];
                    int stamina = (int)data[i]["stamina"];
                    _Equipment.EquipmentType equipType = (_Equipment.EquipmentType)System.Enum.Parse(typeof(_Equipment.EquipmentType), (string)data[i]["equipType"]);
                    item = new _Equipment(id, name, type, quality, description, capacity, buyPrice, sellPrice, sprite, strength, intellect, agility, stamina, equipType);
                    break;
                case _Item.ItemType.Weapon:
                    int damage = (int)data[i]["damage"];
                    _Weapon.WeaponType weaponType = (_Weapon.WeaponType)System.Enum.Parse(typeof(_Weapon.WeaponType), (string)data[i]["weaponType"]);
                    item = new _Weapon(id, name, type, quality, description, capacity, buyPrice, sellPrice, sprite, damage, weaponType);
                    break;
                case _Item.ItemType.Material:
                    item = new _Material(id, name, type, quality, description, capacity, buyPrice, sellPrice, sprite);
                    break;
            }

            itemList.Add(item);

        }
        //Debug.Log(item);

    }

    //得到根据 id 得到Item
    public _Item GetItemById(int id)
    {
        foreach (_Item item in itemList)
        {
            if (item.ID == id)
            {
                return item;
            }
        }
        return null;
    }

    //显示提示框方法
    public void ShowToolTip(string content)
    {
        if (this.isPickedItem == true) return;//如果物品槽中的物品被捡起了，那就不需要再显示提示框了
        toolTip.Show(content);
        isToolTipShow = true;
    }
    //隐藏提示框方法
    public void HideToolTip()
    {
        toolTip.Hide();
        isToolTipShow = false;
    }

    //获取（拾取）物品槽里的指定数量的（amount）物品UI
    public void PickUpItem(_Item item, int amount)
    {
        PickedItem.SetItem(item, amount);
        this.isPickedItem = true;
        PickedItem.Show();//获取到物品之后把跟随鼠标的容器（用来盛放捡起的物品的容器）显示出来
        this.toolTip.Hide();//同时隐藏物品信息提示框

        //控制盛放物品的容器UI跟随鼠标移动
        Vector2 postionPickeItem;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, null, out postionPickeItem);
        pickedItem.SetLocalPosition(postionPickeItem);//设置容器的位置，二维坐标会自动转化为三维坐标但Z坐标为0

    }

    //从鼠标上减少（移除）指定数量的物品
    public void ReduceAmountItem(int amount = 1)
    {
        this.pickedItem.RemoveItemAmount(amount);
        if (pickedItem.Amount <= 0)
        {
            isPickedItem = false;
            PickedItem.Hide();//如果鼠标上没有物品了那就隐藏了
        }
    }

    //点击保存按钮，保存当前物品信息
    public void SaveInventory()
    {
        _Knapscak.Instance.SaveInventory();
        _Chest.Instance.SaveInventory();
        _CharacterPanel.Instance.SaveInventory();
        _Forge.Instance.SaveInventory();
        PlayerPrefs.SetInt("CoinAmount", GameObject.FindGameObjectWithTag("Player").GetComponent<_Player>().CoinAmount);//保存玩家金币
    }

    //点击加载按钮，加载当前物品
    public void LoadInventory()
    {
        _Knapscak.Instance.LoadInventory();
        _Chest.Instance.LoadInventory();
        _CharacterPanel.Instance.LoadInventory();
        _Forge.Instance.LoadInventory();
        //加载玩家金币
        if (PlayerPrefs.HasKey("CoinAmount") == true)
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<_Player>().CoinAmount = PlayerPrefs.GetInt("CoinAmount");
        }
    }
}
