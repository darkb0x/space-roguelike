using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player.Inventory
{
    [CreateAssetMenu(fileName = "Inventory item", menuName = "Game/new Inventory item")]
    public class InventoryItem : ScriptableObject
    {
        [field: SerializeField, NaughtyAttributes.ShowAssetPreview] public Sprite Icon { get; private set; }
        [field: SerializeField, Min(1)] public int Cost { get; private set; }
        [Space]
        [SerializeField] private bool IsOre = false;
        [NaughtyAttributes.ShowIf("IsOre")] public List<Sprite> OreSprites = new List<Sprite>();
    }
}