using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Game.CraftSystem
{
    using CraftSystem.Editor.ScriptableObjects;

    public interface ICraftListObserver
    {
        public void Initialize(List<CSCraftSO> crafts);
        public void GetNewCraft(LoadCraftUtility craftUtility, CSCraftSO newCraft);
    }

    public class LoadCraftUtility : MonoBehaviour
    {
        public static LoadCraftUtility loadCraftUtility;
        private const string unlockedCraftsKey = "unlockedCraftsList";
        private List<ICraftListObserver> observers = new List<ICraftListObserver>();

        [SerializeField] private List<string> unlockedCraftPaths = new List<string>();
        public List<CSCraftSO> allUnlockedCrafts = new List<CSCraftSO>();

        public CSCraftSO GetCraft(string path)
        {
            return Resources.Load<CSCraftSO>(path);
        }

        public void AddUnlockedCraft(CSCraftSO craft)
        {
            if (!unlockedCraftPaths.Contains(craft.AssetPath))
            {
                unlockedCraftPaths.Add(craft.AssetPath);
            }
            if (!allUnlockedCrafts.Contains(craft))
            {
                allUnlockedCrafts.Add(craft);
                UpdateObservers(craft);
            }

            Save();
        }
        [Button]
        public void ClearUnlockedCrafts()
        {
            unlockedCraftPaths.Clear();
            allUnlockedCrafts.Clear();

            Save();
            //ResetDataInObservers();

            PlayerPrefs.SetString("unlockedCraftsList", "");
        }

        #region Save/Load
        [Button]
        private void Save()
        {
            SaveData data = new SaveData()
            {
                craftPaths = unlockedCraftPaths,
                crafts = allUnlockedCrafts
            };
            string json = JsonUtility.ToJson(data);
            PlayerPrefs.SetString("unlockedCraftsList", json);
        }
        [Button]
        private void Load()
        {
            if (string.IsNullOrEmpty(PlayerPrefs.GetString("unlockedCraftsList")))
                return;

            SaveData data = JsonUtility.FromJson<SaveData>(PlayerPrefs.GetString("unlockedCraftsList"));
            unlockedCraftPaths = data.craftPaths;
            allUnlockedCrafts = data.crafts;
        }
        #endregion

        #region MonoBehaviour
        private void Awake()
        {
            loadCraftUtility = this;

            Load();
        }
        #endregion

        #region Observer
        public void AddObserver(ICraftListObserver o)
        {
            observers.Add(o);
            o.Initialize(allUnlockedCrafts);
        }
        public void RemoveObserver(ICraftListObserver o)
        {
            observers.Remove(o);
        }
        public void UpdateObservers(CSCraftSO craft)
        {
            foreach (ICraftListObserver observer in observers)
            {
                observer.GetNewCraft(this, craft);
            }
        }
        public void ResetDataInObservers()
        {
            foreach (ICraftListObserver observer in observers)
            {
                observer.Initialize(new List<CSCraftSO>());
            }
        }
        #endregion
    }

    [System.Serializable]
    class SaveData
    {
        public List<string> craftPaths;
        public List<CSCraftSO> crafts;
    }
}
