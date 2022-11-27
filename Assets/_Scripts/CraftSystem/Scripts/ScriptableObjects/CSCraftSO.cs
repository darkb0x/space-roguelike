using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.CraftSystem.Editor.ScriptableObjects
{
    using Data;

    public class CSCraftSO : ScriptableObject
    {
        [field: SerializeField] public string CraftName { get; set; }
        [field: SerializeField] public List<CSCraftChoiceData> Choices { get; set; }
        [field: SerializeField] public GameObject GameObjectPrefab { get; set; }
        [field: SerializeField] public Sprite IconSprite { get; set; }
        [field: SerializeField] public int CraftCost { get; set; }
        [field: SerializeField] public List<ItemCraft> ObjectCraft { get; set; }
        [field: SerializeField] public bool IsStartingNode { get; set; }

        public void Initialize(string craftName, List<CSCraftChoiceData> choices, GameObject gameObjectPrefab, Sprite iconSprite, int craftCost, List<ItemCraft> objectCraft, bool isStartingDialogue)
        {
            CraftName = craftName;
            Choices = choices;
            GameObjectPrefab = gameObjectPrefab;
            IconSprite = iconSprite;
            CraftCost = craftCost;
            ObjectCraft = objectCraft;
            IsStartingNode = isStartingDialogue;
        }
    }
}
