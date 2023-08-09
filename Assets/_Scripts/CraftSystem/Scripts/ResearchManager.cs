using System.Collections.Generic;
using UnityEngine;

namespace Game.CraftSystem
{
    using Visual;
    using Visual.Node;
    using Player.Inventory;
    using SaveData;
    using global::CraftSystem.ScriptableObjects;

    public class ResearchManager : MonoBehaviour, ISingleton
    {
        [SerializeField] private ResearchVisual Visual;
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

            Visual.Initalize(Trees, this);
        }

        public void Research(ResearchTreeCraft craft, CraftTreeNodeVisual nodeVisual)
        {
            var nextCraft = craft.GetCurrentCraft();
            int craftCost = nextCraft.CraftCost;

            if(PlayerInventory.money >= craftCost)
            {
                PlayerInventory.money -= craftCost;

                crafts.Add(nextCraft);

                nodeVisual.SetState(VisualNodeState.Purchased);

                currentSessionData.InjectCrafts(crafts);
                currentSessionData.Save();
            }
            else
            {
                // to do
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
                    crafts.Add(nextCraft);

                    if(!craft.Upgradable())
                    {
                        nodeVisual.SetState(VisualNodeState.FullyUpgraded);
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
    }
}