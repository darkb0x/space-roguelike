using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEditor;

namespace Game.MainMenu.Mission.Planet
{
    using Player.Inventory;

    [CreateAssetMenu(fileName = "Planet map", menuName = "Game/Mission/new Planet")]
    public class PlanetSO : ScriptableObject
    {
        [System.Serializable]
        public struct ItemGenerationData
        {
            public InventoryItem Item;
            [MaxValue(100)] public int PercentInWorld;
        }

        public string MissionName;
        public Sprite MissionIcon;
        [Space]
        public Sprite PlanetSprite;
        [Scene] public int SceneId;

        [Header("Items")]
        [OnValueChanged("UpdateItemsPercents"), ReorderableList] public List<ItemGenerationData> DefaultItems = new List<ItemGenerationData>();
        [ReadOnly, SerializeField] private string DefaultItemsAmount;
        [OnValueChanged("UpdateItemsPercents"), ReorderableList] public List<ItemGenerationData> UniqueItems = new List<ItemGenerationData>();
        [ReadOnly, SerializeField] private string UniqueItemsAmount;

        [Header("Asset")]
        [ReadOnly] public string AssetPath;

        public List<ItemGenerationData> OresInPlanet
        {
            get
            {
                List<ItemGenerationData> ores = new List<ItemGenerationData>();

                foreach (var defaultItem in DefaultItems)
                {
                    if(defaultItem.Item.IsOre)
                        ores.Add(defaultItem);
                }
                foreach (var uniqueItem in UniqueItems)
                {
                    if(uniqueItem.Item.IsOre)
                        ores.Add(uniqueItem);
                }

                return ores;
            }
        }

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
                AssetPath = AssetPath.Substring(7 + 10); // Assets/Resources/___
                AssetPath = AssetPath.Substring(0, AssetPath.Length - 6); // ___.asset
            }
            catch (System.Exception)
            {
                return;
            }
        }
        private void UpdateItemsPercents()
        {
            int percents = SumPercent(OresInPlanet);
            DefaultItemsAmount = percents.ToString();
            UniqueItemsAmount = percents.ToString();
        }

        private int SumPercent(List<ItemGenerationData> data)
        {
            int sum = 0;
            foreach (var item in data)
            {
                sum += item.PercentInWorld;
            }
            return sum;
        }
    }
}
