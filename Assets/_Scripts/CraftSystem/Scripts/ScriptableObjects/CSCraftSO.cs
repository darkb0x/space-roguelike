using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Game.CraftSystem.Editor.ScriptableObjects
{
    using Player.Inventory;

    [CreateAssetMenu(fileName = "Craft", menuName = "Game/new Craft")]
    public class CSCraftSO : ScriptableObject
    {
        [field: SerializeField] public string CraftName { get; set; }
        [field: SerializeField] public GameObject ObjectPrefab { get; set; }
        [field: SerializeField] public Sprite IconSprite { get; set; }
        [field: SerializeField] public int CraftCost { get; set; }
        [field: SerializeField] public List<ItemData> ObjectCraft { get; set; }
        [field: SerializeField] public string AssetPath { get; set; }

        public void OnEnable()
        {
            if(string.IsNullOrEmpty(AssetPath))
            {
                UpdateAssetPath();
            }
        }

        [NaughtyAttributes.Button]
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
