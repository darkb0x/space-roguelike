using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.World.Generation.Ore
{
    using Player.Inventory;
    using Drill;

    public class Ore : MonoBehaviour
    {
        [HideInInspector] public Drill currentDrill;

        public InventoryItem Item;
        public int MaxAmount;
        public int Amount;
        public bool CanGiveOre = true;
        [Space]
        [SerializeField] private SpriteRenderer RockRenderer;
        [SerializeField] private SpriteRenderer OreRenderer;
        [Space]
        [SerializeField] private Sprite[] RockSprites;

        public bool isInitialized { get; private set; }

        private void Start()
        {
            name = $"Ore ({Item.name})";

            Amount = MaxAmount;
        }
        public void Initialize(InventoryItem item)
        {
            if (!item.IsOre)
            {
                Debug.LogError($"{item.ItemName} isn't ore!");
                return;
            }

            Item = item;

            RockRenderer.sprite = RockSprites[Random.Range(0, RockSprites.Length)];
            OreRenderer.sprite = Item.OreSprites[Random.Range(0, Item.OreSprites.Count)];

            isInitialized = true;
        }

        public int Take(int value)
        {
            Amount = Mathf.Clamp(Amount -= value, 0, MaxAmount);

            if (Amount <= 0)
            {
                CanGiveOre = false;
                OreRenderer.gameObject.SetActive(false);
            }

            return Amount;
        }
    }
}
