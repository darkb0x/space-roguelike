using System.Collections;
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
        [SerializeField] private string m_ItemName;
        [field: SerializeField, Min(1)] public int Cost { get; private set; }
        [field: Space]
        [field: SerializeField] public bool IsOre { get; set; }
        [ShowIf("IsOre")] public List<Sprite> OreSprites = new List<Sprite>();

        [field: Space]
        [field: SerializeField, ReadOnly] public string AssetPath { get; private set; }

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
    }
}