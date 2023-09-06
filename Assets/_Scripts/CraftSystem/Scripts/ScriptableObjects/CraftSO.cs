using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CraftSystem.ScriptableObjects
{
    using Enumerations;
    using Game.Inventory;

    [CreateAssetMenu(fileName = "Craft", menuName = "Game/new Craft")]
    public class CraftSO : ScriptableObject
    {
        [field: SerializeField, ReadOnly] public string AssetPath { get; protected set; }
        [field: SerializeField] public string CraftName { get; protected set; }
        [field: SerializeField] public string CraftDescription { get; protected set; }
        [field: SerializeField] public CSCraftType CraftType { get; protected set; }
        [field: SerializeField] public Sprite CraftIcon { get; protected set; }
        [field: SerializeField] public GameObject CraftPrefab { get; protected set; }
        [field: SerializeField] public List<ItemData> ItemsInCraft { get; protected set; }

        #if UNITY_EDITOR
        private void OnEnable()
        {
            if(string.IsNullOrEmpty(AssetPath))
            {
                UpdateAssetPath();
            }
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
        #endif
    }
}
