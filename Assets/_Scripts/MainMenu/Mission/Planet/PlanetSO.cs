using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEditor;

namespace Game.MainMenu.MissionChoose.Planet
{
    using Game.Inventory;

    [CreateAssetMenu(fileName = "Planet map", menuName = "Game/Mission/new Planet")]
    public class PlanetSO : ScriptableObject
    {
        [System.Serializable]
        public class ItemGenerationData
        {
            public InventoryItem Item;
            [Range(0, 100)] public int PercentInWorld;
        }

        public string MissionName;
        public Sprite MissionIcon;
        [Space]
        public Sprite PlanetSprite;
        [Scene] public int SceneId;

        [Header("Items")]
        [OnValueChanged("UpdateItemsPercents"), ReorderableList] public List<ItemGenerationData> DefaultItems = new List<ItemGenerationData>();
        [OnValueChanged("UpdateItemsPercents"), ReorderableList] public List<ItemGenerationData> UniqueItems = new List<ItemGenerationData>();
        [ReadOnly, SerializeField] private string ItemsAmount;

        [Header("Asset")]
        [ReadOnly] public string AssetPath;

        public List<ItemGenerationData> OresInPlanet
        {
            get
            {
                List<ItemGenerationData> ores = new List<ItemGenerationData>();

                foreach (var defaultItem in DefaultItems)
                {
                    if (defaultItem.Item == null)
                        continue;

                    if(defaultItem.Item.IsOre)
                        ores.Add(defaultItem);
                }
                foreach (var uniqueItem in UniqueItems)
                {
                    if (uniqueItem.Item == null)
                        continue;

                    if(uniqueItem.Item.IsOre)
                        ores.Add(uniqueItem);
                }

                return ores;
            }
        }

        #if UNITY_EDITOR
        private void OnEnable()
        {
            UpdateAssetPath();
        }

        [Button]
        private void UpdateAssetPath()
        {
            try
            {
                AssetPath = AssetDatabase.GetAssetPath(this);
                AssetPath = AssetPath.Substring(7 + 10); // remove: Assets/Resources/___
                AssetPath = AssetPath.Substring(0, AssetPath.Length - 6); // remove: ___.asset
            }
            catch (System.Exception)
            {
                return;
            }
        }
        [Button]
        private void FixItemsAmount()
        {
            if (SumPercent() <= 100)
                return;

            do
            {
                foreach (var _item in DefaultItems)
                {
                    ItemGenerationData item = _item;
                    item.PercentInWorld--;

                    if(SumPercent() <= 100)
                    {
                        UpdateItemsPercents();
                        return;
                    }
                }
            }
            while (SumPercent() > 100);

            UpdateItemsPercents();
        }

        private void UpdateItemsPercents()
        {
            ItemsAmount = SumPercent().ToString();
        }

        private int SumPercent()
        {
            int sum = 0;
            foreach (var item in DefaultItems)
            {
                sum += item.PercentInWorld;
            }
            foreach (var item in UniqueItems)
            {
                sum += item.PercentInWorld;
            }
            return sum;
        }
    #endif
    }
}
