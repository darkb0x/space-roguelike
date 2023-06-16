using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NaughtyAttributes;

namespace Game.Player.Inventory
{
    [CreateAssetMenu(fileName = "Inventory item", menuName = "Game/new Inventory item")]
    public class InventoryItem : ScriptableObject
    {
        [field: SerializeField, ShowAssetPreview] public Sprite Icon { get; private set; }
        [field: SerializeField, ShowAssetPreview] public Sprite LowSizeIcon { get; private set; }
        [SerializeField, OnValueChanged("UpdateAssetName")] private string m_ItemName;
        [field: SerializeField] public Color ItemTextColor { get; private set; }
        [field: SerializeField, Min(1)] public int Cost { get; private set; }
        [field: Space]
        [field: SerializeField] public bool CanTakeInMission { get; private set; }
        [field: Space]
        [field: SerializeField] public bool IsOre { get; set; }
        [ShowIf("IsOre")] public List<Sprite> OreSprites = new List<Sprite>();

        [field: Space]
        [field: SerializeField, ReadOnly] public string AssetPath { get; private set; }
        [field: SerializeField, ReadOnly] private string AssetName;

        public string ItemName
        {
            get
            {
                if (string.IsNullOrEmpty(m_ItemName))
                    return name;
                else
                    return m_ItemName;
            }
        }

        #if UNITY_EDITOR
        private void OnEnable()
        {
            UpdateAssetPath();
            UpdateAssetName();
        }

        [Button]
        public void UpdateAssetPath()
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
        public void UpdateAssetName()
        {
            if(ItemName == name)
            {
                AssetName = ItemName + " (default)";
            }
            else
            {
                AssetName = ItemName + " (custom)";
            }
        }

        [Button]
        public void CalculateAverageColor()
        {
            Dictionary<Color, int> colorCounts = new Dictionary<Color, int>();

            Color[] pixels = Icon.texture.GetPixels();

            for (int i = 0; i < pixels.Length; i++)
            {
                Color color = pixels[i];
                if (color == Color.black)
                    continue;
                else if (color == new Color(0, 0, 0, 0))
                    continue;

                if (colorCounts.ContainsKey(color))
                {
                    colorCounts[color]++;
                }
                else
                {
                    colorCounts.Add(color, 1);
                }
            }

            Color mostUsedColor = Color.white;
            int maxCount = 0;

            foreach (var pair in colorCounts)
            {
                if (pair.Value > maxCount)
                {
                    mostUsedColor = pair.Key;
                    maxCount = pair.Value;
                }
            }

            ItemTextColor = mostUsedColor;
        }
        #endif
    }
}