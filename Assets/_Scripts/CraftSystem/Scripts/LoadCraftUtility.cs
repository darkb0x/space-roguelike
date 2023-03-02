using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Game.CraftSystem
{
    using CraftSystem.Editor.ScriptableObjects;
    using SaveData;

    public interface ICraftListObserver
    {
        public void Initialize(List<CSCraftSO> crafts);
        public void GetNewCraft(LoadCraftUtility craftUtility, CSCraftSO newCraft);
    }

    public class LoadCraftUtility : MonoBehaviour
    {
        public static LoadCraftUtility Instance;
        private List<ICraftListObserver> observers = new List<ICraftListObserver>();

        public List<CSCraftSO> allUnlockedCrafts = new List<CSCraftSO>();

        private GameData.SessionData currentSessionData => GameData.Instance.CurrentSessionData;

        private void Awake()
        {
            Instance = this;
        }
        private void Start()
        {
            allUnlockedCrafts = GetCraftByPathList(currentSessionData.UnlockedCraftPaths);
            InitializeObservers();
        }

        public void AddUnlockedCraft(CSCraftSO craft)
        {
            if (!currentSessionData.UnlockedCraftPaths.Contains(craft.AssetPath))
            {
                currentSessionData.UnlockedCraftPaths.Add(craft.AssetPath);
            }
            if (!allUnlockedCrafts.Contains(craft))
            {
                allUnlockedCrafts.Add(craft);
                UpdateObservers(craft);
            }
        }
        [Button]
        public void ClearUnlockedCrafts()
        {
            currentSessionData.UnlockedCraftPaths.Clear();
            allUnlockedCrafts.Clear();
        }

        #region Utilities
        public CSCraftSO GetCraft(string path)
        {
            return Resources.Load<CSCraftSO>(path);
        }
        private List<CSCraftSO> GetCraftByPathList(List<string> pathList)
        {
            List<CSCraftSO> items = new List<CSCraftSO>();
            foreach (var itemPath in pathList)
            {
                CSCraftSO craft = GetCraft(itemPath);
                if(craft != null)
                {
                    items.Add(craft);
                }
            }
            return items;
        }
        #endregion

        #region Observer
        public void AddObserver(ICraftListObserver o)
        {
            observers.Add(o);
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
        public void InitializeObservers()
        {
            foreach (ICraftListObserver observer in observers)
            {
                observer.Initialize(allUnlockedCrafts);
            }
        }
        #endregion
    }
}
