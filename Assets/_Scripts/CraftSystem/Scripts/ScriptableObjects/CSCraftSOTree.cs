using System.Collections.Generic;
using UnityEngine;

namespace Game.CraftSystem.Editor.ScriptableObjects
{
    using Data;
    using Player.Inventory;

    public class CSCraftSOTree : CSCraftSO
    {
        [field: SerializeField] public List<CSCraftChoiceData> Choices { get; set; }
        [field: SerializeField] public bool IsStartingNode { get; set; }
        [field: SerializeField] public Vector2 Position { get; set; }

        public void Initialize(string craftName, List<CSCraftChoiceData> choices, GameObject obj, Sprite iconSprite, int craftCost, List<ItemData> objectCraft, bool isStartingDialogue, Vector2 pos, string path)
        {
            CraftName = craftName;
            Choices = choices;
            ObjectPrefab = obj;
            IconSprite = iconSprite;
            CraftCost = craftCost;
            ObjectCraft = objectCraft;
            IsStartingNode = isStartingDialogue;
            Position = pos;
            AssetPath = path;
        }
    }
}
