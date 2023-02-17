using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.World
{
    using Player.Inventory;
    using Drill;

    public class Ore : MonoBehaviour
    {
        [HideInInspector] public Drill currentDrill;

        public InventoryItem item;
        public int maxAmount;
        public int amount;
        public bool canGiveOre = true;
        [Space]
        [SerializeField] private SpriteRenderer oreRender;
        [Space]
        [SerializeField] private Material standartMaterial;
        [SerializeField] private Material selectMaterial;
        [Space]
        [SerializeField] private Sprite[] rockSprites;

        Sprite currentRockSprite;
        Sprite currentOreSprite;

        private void Start()
        {
            name = $"Ore ({item.name})";

            amount = maxAmount;

            GenerateRandomSprites();
        }

        public int Take(int value)
        {
            amount = Mathf.Clamp(amount -= value, 0, maxAmount);

            if (amount <= 0)
            {
                canGiveOre = false;
                oreRender.sprite = currentRockSprite;
            }

            return amount;
        }

        public void Select()
        {
            oreRender.material = selectMaterial;
        }
        public void DisSelect()
        {
            oreRender.material = standartMaterial;
        }

        [NaughtyAttributes.Button]
        public void GenerateRandomSprites()
        {
            currentRockSprite = rockSprites[Random.Range(0, rockSprites.Length)];
            currentOreSprite = item.OreSprites[Random.Range(0, item.OreSprites.Count)];

            oreRender.sprite = CombineSprites.MergeSprites(new Sprite[2] { currentRockSprite, currentOreSprite }, new Vector2Int(18, 18), name);
        }
    }
}
