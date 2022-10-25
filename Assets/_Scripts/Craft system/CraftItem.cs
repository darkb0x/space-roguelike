using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "Craft item", menuName = "Craft/new craft")]
public class CraftItem : ScriptableObject
{
    [System.Serializable]
    public struct Item
    {
        [Expandable, AllowNesting] public InventoryItem item;
        public int amount;
    }

    public Sprite itemIcon;
    public string itemName;
    [Space]
    public Item[] itemsToLearn;
    public Item[] itemsToCraft;
    [Space]
    public GameObject itemPrefab;
}
