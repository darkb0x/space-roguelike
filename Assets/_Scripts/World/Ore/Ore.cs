using System.Collections;
using UnityEngine;

namespace Game.World.Generation.Ore
{
    using Inventory;
    using Drill;
    using Drone;

    public class Ore : MonoBehaviour
    {
        public Drill currentDrill { get; set; }
        public DroneAI currentDrone { get; set; }

        public InventoryItem Item;
        public int MaxAmount;
        public int Amount;

        [Header("Visual")]
        [SerializeField] private SpriteRenderer RockRenderer;
        [SerializeField] private SpriteRenderer OreRenderer;
        [Space]
        [SerializeField] private Transform Visual;
        [SerializeField] private AnimationCurve RockHurtAnimation;
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
            StartCoroutine(PlayHurtAnimation());

            int oreAmount = 0;

            if (Amount >= value)
                oreAmount = value;
            else
                oreAmount = Amount;

            Amount = Mathf.Clamp(Amount -= value, 0, MaxAmount);

            if (Amount <= 0)
            {
                OreRenderer.gameObject.SetActive(false);
            }

            return oreAmount;
        }

        private IEnumerator PlayHurtAnimation()
        {
            float currentTime = 0f;
            float animationSpeed = 5f;

            while(currentTime < 1)
            {
                currentTime = Mathf.Clamp(currentTime += (Time.deltaTime * animationSpeed), 0, 1);

                float scaleValue = RockHurtAnimation.Evaluate(currentTime);
                Vector3 scale = new Vector3(scaleValue, scaleValue, scaleValue);

                Visual.localScale = scale;

                yield return null;
            }
        }
    }
}
