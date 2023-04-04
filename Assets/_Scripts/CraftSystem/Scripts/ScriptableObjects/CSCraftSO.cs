using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.CraftSystem.Editor.ScriptableObjects
{
    using Data;
    using Player.Inventory;

    public class CSCraftSO : ScriptableObject
    {
        [field: SerializeField] public string CraftName { get; set; }
        [field: SerializeField] public List<CSCraftChoiceData> Choices { get; set; }
        [field: SerializeField] public GameObject ObjectPrefab { get; set; }
        [field: SerializeField] public Sprite IconSprite { get; set; }
        [field: SerializeField] public int CraftCost { get; set; }
        [field: SerializeField] public List<ItemData> ObjectCraft { get; set; }
        [field: SerializeField] public bool IsStartingNode { get; set; }
        [field: SerializeField] public Vector2 Position { get; set; }
        [field: SerializeField] public string AssetPath { get; set; }

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

/*
namespace Game.CraftSystem.Editor.ScriptableObjects.Serialized
{
    using Player.Inventory;
    using Data;

    public class SerializedCraft
    {
        public string CraftName;
        public List<CSCraftChoiceData> Choices;
        public GameObject ObjectPrefab;
        public Sprite IconSprite;
        public int CraftCost;
        public List<ItemData> ObjectCraf;
        public bool IsStartingNode;
        public Vector2 Position;
        public string AssetPath;
    }
}
*/
