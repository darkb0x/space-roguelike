using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Lobby
{
    using Save;
    using Game.Inventory;

    public class ResourceAutomat : MonoBehaviour
    {
        [Header("Parameters")]
        [SerializeField] private int MaxInteractionsAmount = 4;
        [Space]
        [SerializeField] private int ItemAmount = 3;
        [SerializeField] private InventoryItem[] NecessaryItems = new InventoryItem[2];
        [SerializeField] private List<InventoryItem> UnnecessaryItems = new List<InventoryItem>();

        [Header("Visual")]
        [SerializeField] private Animator Anim;
        [SerializeField, NaughtyAttributes.AnimatorParam("Anim")] private string Anim_interactTrigger = "Interact";
        [SerializeField] private SpriteRenderer CurrentItemSprite;
        [SerializeField] private SpriteRenderer FinalItemSprite;
        [SerializeField] private SpriteRenderer FinalItemOutline;
        [Space]
        [SerializeField] private TMPro.TMP_Text AvaiableInteractionsText;

        private IInventory _inventory;
        private SessionSaveData currentSessionData => SaveManager.SessionSaveData;
        private InventoryItem finalItem;
        private int avaiableInteractionCount
        {
            get
            {
                return MaxInteractionsAmount - currentSessionData.ResourceAutomatCurrentInteract;
            }
        }

        private void Start()
        {
            _inventory = ServiceLocator.GetService<IInventory>();

            AvaiableInteractionsText.text = avaiableInteractionCount.ToString();

            if(avaiableInteractionCount > 0)
            {
                finalItem = UnnecessaryItems[Random.Range(0, UnnecessaryItems.Count)];
                FinalItemSprite.sprite = finalItem.LowSizeIcon;
                FinalItemOutline.sprite = finalItem.LowSizeIcon;
            }
            else
            {
                FinalItemSprite.gameObject.SetActive(false);
                FinalItemOutline.color = new Color(1, 1, 1, 0);
            }
        }

        public void Interact()
        {
            if (avaiableInteractionCount <= 0)
                return;

            int currentInteract = currentSessionData.ResourceAutomatCurrentInteract;
            InventoryItem selectedItem;
            if (currentInteract < 2)
            {
                selectedItem = NecessaryItems[currentInteract];
            }
            else if(avaiableInteractionCount == 1)
            {
                if(finalItem != null)
                    selectedItem = finalItem;
                else
                    selectedItem = UnnecessaryItems[Random.Range(0, UnnecessaryItems.Count)];
            }
            else
            {
                selectedItem = UnnecessaryItems[Random.Range(0, UnnecessaryItems.Count)];
            }

            CurrentItemSprite.sprite = selectedItem.LowSizeIcon;
            Anim.SetTrigger(Anim_interactTrigger);

            _inventory.AddItem(new ItemData(selectedItem, ItemAmount));

            currentSessionData.ResourceAutomatCurrentInteract++;

            AvaiableInteractionsText.text = avaiableInteractionCount.ToString();
        }
    }
}
