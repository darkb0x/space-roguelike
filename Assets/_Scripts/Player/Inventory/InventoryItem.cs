using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player.Inventory
{
    [CreateAssetMenu(fileName = "Inventory item", menuName = "Item/new Inventory item")]
    public class InventoryItem : ScriptableObject
    {
        [SerializeField, NaughtyAttributes.ShowAssetPreview] private Sprite icon;
        [SerializeField, Min(1)] private int cost = 1;
        [Space]
        public List<Sprite> oreSprites = new List<Sprite>();

        public Sprite _icon => icon;
        public int _cost => cost;

    }
}