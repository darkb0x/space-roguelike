using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.World.Generation.Ore
{
    using Player.Inventory;
    using Drill;
    using Drone;

    public class Ore : MonoBehaviour
    {
        public Drill currentDrill { get; set; }
        public DroneAI currentDrone { get; set; }

        public bool CanGiveOre
        {
            get
            {
                bool used = false;

                if (currentDrill)
                    used = true;
                if (Amount <= 0)
                    used = true;
                if (currentDrone)
                    used = true;

                m_CanGiveOre = used;
                return m_CanGiveOre;
            }
            private set
            {
                m_CanGiveOre = value;
            }
        }

        public InventoryItem Item;
        public int MaxAmount;
        public int Amount;
        [SerializeField, NaughtyAttributes.ReadOnly] private bool m_CanGiveOre;
        [Space]
        [SerializeField] private SpriteRenderer RockRenderer;
        [SerializeField] private SpriteRenderer OreRenderer;
        [Space]
        [SerializeField] private Sprite[] RockSprites;

        public bool isInitialized { get; private set; }

        private void Start()
        {

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
            name = $"Ore ({Item.name})";

            RockRenderer.sprite = RockSprites[Random.Range(0, RockSprites.Length)];
            OreRenderer.sprite = Item.OreSprites[Random.Range(0, Item.OreSprites.Count)];

            isInitialized = true;
        }

        public int Take(int value)
        {
            Amount = Mathf.Clamp(Amount -= value, 0, MaxAmount);

            if (Amount <= 0)
            {
                OreRenderer.gameObject.SetActive(false);
            }

            return Amount;
        }
    }
}
