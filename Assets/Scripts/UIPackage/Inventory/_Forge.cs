using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;

/// <summary>
/// 锻造类，继承自存放物品基类
/// </summary>
public class _Forge : _Inventory
{
    //单例模式
    private static _Forge _instance;
    public static _Forge Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.Find("ForgePanel").GetComponent<_Forge>();
            }
            return _instance;
        }
    }

    private List<_Formula> formulaList = null;//用来存放解析出来的材料

    public override void Start()
    {
        base.Start();
        ParseFormulaJSON();
        Hide();
    }

    //解析武器锻造配方Json文件
    public void ParseFormulaJSON()
    {
        formulaList = new List<_Formula>();
        TextAsset formulaText = Resources.Load<TextAsset>("Formulas");
        string formulaJson = formulaText.text;
        JsonData datao = JsonMapper.ToObject(formulaJson);
        for (int i = 0; i < datao.Count; i++)
        {
            int item1ID = (int)datao[i]["Item1ID"];
            int item1Amount = (int)datao[i]["Item1Amount"];
            int item2ID = (int)datao[i]["Item2ID"];
            int item2Amount = (int)datao[i]["Item2Amount"];
            int resID = (int)datao[i]["ResID"];
            _Formula formula = new _Formula(item1ID, item1Amount, item2ID, item2Amount, resID);
            formulaList.Add(formula);
        }
        //Debug.Log(formulaList[1].ResID);
    }

    /// <summary>
    /// 锻造物品功能：重点
    /// </summary>
    public void ForgeItem()
    {
        //得到当前锻造面板里面有哪些材料
        List<int> haveMaterialIDList = new List<int>();//存储当前锻造面板里面拥有的材料的ID
        foreach (_Slot slot in slotArray)
        {
            if (slot.transform.childCount > 0)
            {
                _ItemUI currentItemUI = slot.transform.GetChild(0).GetComponent<_ItemUI>();
                for (int i = 0; i < currentItemUI.Amount; i++)
                {
                    haveMaterialIDList.Add(currentItemUI.Item.ID);//物品槽里有多少个物品，就存储多少个ID
                }
            }
        }
        //Debug.Log(haveMaterialIDList[0].ToString());
        //判断满足哪一个锻造配方的需求
        _Formula matchedFormula = null;
        foreach (_Formula formula in formulaList)
        {
            bool isMatch = formula.Match(haveMaterialIDList);
            //Debug.Log(isMatch);
            if (isMatch)
            {
                matchedFormula = formula;
                break;
            }
        }
        // Debug.Log(matchedFormula.ResID);
        if (matchedFormula != null)
        {

            _Knapscak.Instance.StoreItem(matchedFormula.ResID);//把锻造出来的物品放入背包
            //减掉消耗的材料
            foreach (int id in matchedFormula.NeedIDList)
            {
                foreach (_Slot slot in slotArray)
                {
                    if (slot.transform.childCount > 0)
                    {
                        _ItemUI itemUI = slot.transform.GetChild(0).GetComponent<_ItemUI>();
                        if (itemUI.Item.ID == id && itemUI.Amount > 0)
                        {
                            itemUI.RemoveItemAmount();
                            if (itemUI.Amount <= 0)
                            {
                                DestroyImmediate(itemUI.gameObject);
                            }
                            break;
                        }
                    }
                }
            }
        }
    }

}
