using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Game.CraftSystem.Research
{
    using Visual;
    using Visual.Node;
    using Game.Inventory;
    using Save;
    using global::CraftSystem.ScriptableObjects;
    using System.Linq;
    using Notifications;
    using UI;

    public class ResearchManager : MonoBehaviour, IService, IEntryComponent<Inventory, UIWindowService>
    {
        public const WindowID CRAFT_RESEARCH_WINDOW_ID = WindowID.CraftResearch;

        [SerializeField] private bool ShowVisual = true;
        [SerializeField, ShowIf("ShowVisual")] private ResearchVisual Visual;
        [Space]
        public List<ResearchTree> Trees = new List<ResearchTree>();

        private SessionSaveData currentSessionData => SaveManager.SessionSaveData;

        private Inventory _inventory;
        private UIWindowService _uiWindowService;

        private List<CraftSO> crafts;

        public void Initialize(Inventory inventory, UIWindowService windowService)
        {
            _inventory = inventory;
            _uiWindowService = windowService;

            crafts = currentSessionData.GetCraftList();

            if(ShowVisual)
            {
                var visual = _uiWindowService.RegisterWindow<ResearchVisual>(CRAFT_RESEARCH_WINDOW_ID);
                visual.Initalize(Trees, this);
                visual.LoadSaveData(crafts.ConvertAll(result => result as CSTreeCraftSO));
            }
        }

        public void Research(ResearchTreeCraft craft, CraftTreeNodeVisual nodeVisual)
        {
            var currentCraft = craft.GetCurrentCraft();
            int craftCost = currentCraft.CraftCost;

            if(_inventory.TakeMoney(craftCost))
            {
                if(!crafts.Contains(currentCraft)) crafts.Add(currentCraft);

                nodeVisual.SetState(VisualNodeState.Purchased);

                currentSessionData.InjectCrafts(crafts);

                NotificationService.NewNotification(
                    currentCraft.CraftIcon,
                    "Researched!",
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

            if (!crafts.Any(x => craft.crafts.Contains(x))) 
            {
                crafts.Add(currentCraft);
                currentSessionData.InjectCrafts(crafts);
            }
        }
        public void Research(CraftSO craft)
        {
            if (!crafts.Contains(craft))
            {
                crafts.Add(craft);
                currentSessionData.InjectCrafts(crafts);
            }
        }
        public void Upgrade(ResearchTreeCraft craft, CraftTreeNodeVisual nodeVisual)
        {
            var nextCraft = craft.GetNextCraft();
            int craftCost = nextCraft.CraftCost;

            if (craft.Upgradable() && _inventory.TakeMoney(craftCost))
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
            }
            else
            {
                // to do
            }
        }
    }
}