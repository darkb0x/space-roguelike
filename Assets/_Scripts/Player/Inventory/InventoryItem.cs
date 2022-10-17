using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Inventory item", menuName = "Item/new Inventory item")]
public class InventoryItem : ScriptableObject
{
    [SerializeField, NaughtyAttributes.ShowAssetPreview] private Sprite icon;
    [SerializeField, Min(1)] private int cost = 1;

    public Sprite _icon => icon;
    public int _cost => cost;
}
