using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// 角色面板类，继承自存储物品基类
/// </summary>
public class _CharacterPanel : _Inventory
{
    //单例模式
    private static _CharacterPanel _instance;
    public static _CharacterPanel Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.Find("CharacterPanel").GetComponent<_CharacterPanel>();
            }
            return _instance;
        }
    }

    private Text characterPropertyText;//对角色属性面板中Text组件的引用
    private _Player player;//对角色脚本的引用

    public override void Start()
    {
        base.Start();
        characterPropertyText = transform.Find("CharacterPropertyPanel/Text").GetComponent<Text>();
        player = GameObject.FindWithTag("Player").GetComponent<_Player>();
        UpdatePropertyText();//初始化显示角色属性值
    }

    //更新角色属性显示
    private void UpdatePropertyText()
    {
        int strength = 0, intellect = 0, agility = 0, stamina = 0, damage = 0;
        foreach (_EquipmentSlot slot in slotArray)//遍历角色面板中的装备物品槽
        {
            if (slot.transform.childCount > 0)//找到有物品的物品槽，获取里面装备的属性值
            {
                _Item item = slot.transform.GetChild(0).GetComponent<_ItemUI>().Item;
                if (item is _Equipment)//如果物品是装备，那就加角色对应的属性
                {
                    _Equipment e = (_Equipment)item;
                    strength += e.Strength;
                    intellect += e.Intellect;
                    agility += e.Agility;
                    stamina += e.Stamina;
                }
                else if (item is _Weapon)///如果物品是武器，那就加角色的伤害（damage）属性
                {
                    _Weapon w = (_Weapon)item;
                    damage += w.Damage;
                }
            }
        }
        strength += player.BasicStrength;
        intellect += player.BasicIntellect;
        agility += player.BasicAgility;
        stamina += player.BasicStamina;
        damage += player.BasicDamage;
        string text = string.Format("力量：{0}\n智力：{1}\n敏捷：{2}\n体力：{3}\n攻击力：{4}\n", strength, intellect, agility, stamina, damage);
        characterPropertyText.text = text;
    }

    //直接穿戴功能（不需拖拽）
    public void PutOn(_Item item)
    {
        _Item exitItem = null;//临时保存需要交换的物品
        foreach (_Slot slot in slotArray)//遍历角色面板中的物品槽，查找合适的的物品槽
        {
            _EquipmentSlot equipmentSlot = (_EquipmentSlot)slot;
            if (equipmentSlot.IsRightItem(item)) //判断物品是否合适放置在该物品槽里
            {
                if (equipmentSlot.transform.childCount > 0)//判断角色面板中的物品槽合适的位置是否已经有了装备
                {
                    _ItemUI currentItemUI = equipmentSlot.transform.GetChild(0).GetComponent<_ItemUI>();
                    exitItem = currentItemUI.Item;
                    currentItemUI.SetItem(item, 1);
                }
                else
                {
                    equipmentSlot.StoreItem(item);
                }
                break;
            }
        }
        if (exitItem != null)
        {
            _Knapscak.Instance.StoreItem(exitItem);//把角色面板上是物品替换到背包里面
        }
        UpdatePropertyText();//更新显示角色属性值
    }

    //脱掉装备功能（不需拖拽）
    public void PutOff(_Item item)
    {
        _Knapscak.Instance.StoreItem(item);//把角色面板上是物品替换到背包里面
        UpdatePropertyText();//更新显示角色属性值
    }

}
