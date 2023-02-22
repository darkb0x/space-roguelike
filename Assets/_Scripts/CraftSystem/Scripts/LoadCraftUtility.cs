using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System.IO;

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
        private string saveDataPath;
        private string dataName = "unlockedCraftsList";

        public static LoadCraftUtility Instance;
        private List<ICraftListObserver> observers = new List<ICraftListObserver>();

        [SerializeField] private List<string> unlockedCraftPaths = new List<string>();
        public List<CSCraftSO> allUnlockedCrafts = new List<CSCraftSO>();

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
                craftPaths = unlockedCraftPaths
            };

            string json = JsonUtility.ToJson(data, true);

            File.WriteAllText(saveDataPath, json);
        }
        [Button]
        private void Load()
        {
            if(!File.Exists(saveDataPath))
            {
                return;
            }

            string json = File.ReadAllText(saveDataPath);

            SaveData data = JsonUtility.FromJson<SaveData>(json);
            unlockedCraftPaths = data.craftPaths;
            allUnlockedCrafts = GetCraftByPathList(ref unlockedCraftPaths);
        }
        #endregion

        #region Utilities
        public CSCraftSO GetCraft(string path)
        {
            return Resources.Load<CSCraftSO>(path);
        }
        private List<CSCraftSO> GetCraftByPathList(ref List<string> pathList)
        {
            List<CSCraftSO> items = new List<CSCraftSO>();
            List<string> elementToRemove = new List<string>();
            foreach (var itemPath in pathList)
            {
                CSCraftSO craft = GetCraft(itemPath);
                if(craft != null)
                {
                    items.Add(craft);
                }
                else
                {
                    elementToRemove.Add(itemPath);
                }
            }
            foreach (var itemToRemove in elementToRemove)
            {
                pathList.Remove(itemToRemove);
            }
            return items;
        }
        #endregion

        #region MonoBehaviour
        private void Awake()
        {
            Instance = this;

#if UNITY_EDITOR
            saveDataPath = Path.Combine(Application.dataPath, dataName+".txt");
#else
            saveDataPath = Path.Combine(Application.dataPath, dataName);
#endif
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
    }
}
