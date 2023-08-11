using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Game.CraftSystem
{
    using Visual;
    using Visual.Node;
    using Player.Inventory;
    using SaveData;
    using global::CraftSystem.ScriptableObjects;
    using System.Linq;

    public class ResearchManager : MonoBehaviour, ISingleton
    {
        [SerializeField] private bool ShowVisual = true;
        [SerializeField, ShowIf("ShowVisual")] private ResearchVisual Visual;
        [Space]
        [SerializeField] private List<ResearchTree> Trees = new List<ResearchTree>();

        private SessionData currentSessionData => SaveDataManager.Instance.CurrentSessionData;
        private PlayerInventory PlayerInventory;
        private List<CraftSO> crafts;

        private void Awake()
        {
            Singleton.Add(this);
        }
        private void Start()
        {
            PlayerInventory = Singleton.Get<PlayerInventory>();

            crafts = currentSessionData.GetCraftList();

            if(ShowVisual && Visual != null)
            {
                Visual.Initalize(Trees, this);
                Visual.LoadSaveData(crafts.ConvertAll(result => result as CSTreeCraftSO));
            }
        }

        public void Research(ResearchTreeCraft craft, CraftTreeNodeVisual nodeVisual)
        {
            var currentCraft = craft.GetCurrentCraft();
            int craftCost = currentCraft.CraftCost;

            if(PlayerInventory.money >= craftCost)
            {
                PlayerInventory.money -= craftCost;

                if(!crafts.Contains(currentCraft)) crafts.Add(currentCraft);

                nodeVisual.SetState(VisualNodeState.Purchased);

                currentSessionData.InjectCrafts(crafts);
                currentSessionData.Save();
            }
            else
            {
                // to do
            }
        }
        public void Research(ResearchTreeCraft craft)
        {
            var currentCraft = craft.GetCurrentCraft();

            if (!crafts.Any(item => craft.crafts.Contains(item)))
            {
                crafts.Add(currentCraft);
                currentSessionData.InjectCrafts(crafts);
                currentSessionData.Save();
            }
        }
        public void Upgrade(ResearchTreeCraft craft, CraftTreeNodeVisual nodeVisual)
        {
            var nextCraft = craft.GetNextCraft();
            int craftCost = nextCraft.CraftCost;

            if (PlayerInventory.money >= craftCost)
            {
                if(craft.Upgradable())
                {
                    PlayerInventory.money -= craftCost; 

                    craft.Upgrade();

                    crafts.Remove(craft.GetPreviousCraft());
                    if(!crafts.Contains(nextCraft)) crafts.Add(nextCraft);

                    if(!craft.Upgradable())
                    {
                        nodeVisual.SetState(VisualNodeState.FullyUpgraded);
                    }
                    else
                    {
                        nodeVisual.UpdateState();
                    }

                    currentSessionData.InjectCrafts(crafts);
                    currentSessionData.Save();
                }
            }
            else
            {
                // to do
            }
        }

        public void Open()
        {
            Visual.Open();
        }
    }
}