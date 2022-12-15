using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.CraftSystem
{
    using CraftSystem.Editor.ScriptableObjects;

    public class LoadCraftUtility : MonoBehaviour
    {
        public static LoadCraftUtility loadCraftUtility;

        public List<string> unlockedCrafts = new List<string>();

        public CSCraftSO GetCraft(string path)
        {
            return Resources.Load<CSCraftSO>(path);
        }

        public void AddUnlockedCraft(CSCraftSO craft)
        {
            unlockedCrafts.Add(craft.Path);
            Debug.Log("Da");
        }
        public void RemoveUnlockedCraft(CSCraftSO craft)
        {
            if(unlockedCrafts.Contains(craft.Path))
            {
                unlockedCrafts.Remove(craft.Path);
            }
        }
        public void ClearUnlockedCrafts()
        {
            unlockedCrafts.Clear();
        }

        #region MonoBehaviour
        private void Awake() => loadCraftUtility = this;
        #endregion
    }
}
