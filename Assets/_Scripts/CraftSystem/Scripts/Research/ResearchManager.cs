using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Game.CraftSystem.Research
{
    using Visual;
    using Visual.Node;
    using Game.Inventory;
    using SaveData;
    using global::CraftSystem.ScriptableObjects;
    using System.Linq;
    using Notifications;

    public class ResearchManager : MonoBehaviour, IService, IEntryComponent<IInventory>
    {
        [SerializeField] private bool ShowVisual = true;
        [SerializeField, ShowIf("ShowVisual")] private ResearchVisual Visual;
        [Space]
        public List<ResearchTree> Trees = new List<ResearchTree>();

        private SessionData currentSessionData => SaveDataManager.Instance.CurrentSessionData;
        private IInventory PlayerInventory;
        private List<CraftSO> crafts;

        public void Initialize(IInventory playerInventory)
        {
            PlayerInventory = playerInventory;

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

            if(PlayerInventory.TakeMoney(craftCost))
            {
                if(!crafts.Contains(currentCraft)) crafts.Add(currentCraft);

                nodeVisual.SetState(VisualNodeState.Purchased);

                currentSessionData.InjectCrafts(crafts);
                currentSessionData.Save();

                NotificationManager.NewNotification(
                    currentCraft.CraftIcon,
                    "Researched!",
                    true,
                    Color.white,
                    NotificationStyle.NewCraft
                    );
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

            if (craft.Upgradable() && PlayerInventory.TakeMoney(craftCost))
            {
                craft.Upgrade();

                crafts.Remove(craft.GetPreviousCraft());
                if (!crafts.Contains(nextCraft)) crafts.Add(nextCraft);

                if (!craft.Upgradable())
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