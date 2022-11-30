using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.World
{
    using Player.Inventory;

    public class Ore : MonoBehaviour
    {
        public InventoryItem item;
        public int maxAmount;
        int amount;
        public bool canGiveOre = true;
        [Space]
        [SerializeField] private SpriteRenderer rockRender;
        [SerializeField] private SpriteRenderer oreRender;
        [Space]
        [SerializeField] private Sprite[] rockSprites;

        private void Start()
        {
            name = $"Ore ({item.name})";

            amount = maxAmount;

            GenerateRandomSprites();
        }

        public void Give(int value)
        {
            amount = Mathf.Clamp(amount -= value, 0, maxAmount);

            if (amount <= 0)
            {
                canGiveOre = false;
                oreRender.color = new Color(1, 1, 1, 0);
            }
        }

        [NaughtyAttributes.Button]
        public void GenerateRandomSprites()
        {
            rockRender.sprite = rockSprites[Random.Range(0, rockSprites.Length-1)];
            oreRender.sprite = item.oreSprites[Random.Range(0, item.oreSprites.Count-1)];
        }
    }
}
