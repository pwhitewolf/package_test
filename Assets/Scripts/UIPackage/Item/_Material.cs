using UnityEngine;
using System.Collections;

/// <summary>
/// 材料类
/// </summary>
public class _Material : _Item {

    public _Material(int id, string name, ItemType type, ItemQuality quality, string description, int capaticy, int buyPrice, int sellPrice, string sprite) : base(id, name, type, quality, description, capaticy, buyPrice, sellPrice, sprite)
    {

    }
}
