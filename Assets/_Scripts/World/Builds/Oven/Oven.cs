using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Oven
{
    using Player;
    using Player.Inventory;
    using Manager;

    [RequireComponent(typeof(PlayerInteractObject))]
    [RequireComponent(typeof(Enemy.EnemyTarget))]
    public class Oven : MonoBehaviour, IDamagable
    {
        OvenManager manager;

        [Header("UI")]
        [SerializeField] private GameObject progressBarObject;
        [Space]
        [SerializeField] private Image itemImage;
        [SerializeField] private Image progressRender;
        [SerializeField] private Gradient progressColor;

        [Header("Variables")]
        [SerializeField] private float remeltingTime = 2f;

        OvenCraftList.craft currentItem;
        float currentTime;
        bool canTakeItem = false;

        private void Start()
        {
            manager = FindObjectOfType<OvenManager>();

            GetComponent<Enemy.EnemyTarget>().Initialize(this);

            DisableProgressBar();
        }

        private void Update()
        {
            if (currentItem == null)
                return;

            if(currentTime <= 0)
            {
                canTakeItem = true;
            }
            else
            {
                currentTime -= Time.deltaTime;

                UpdateProgressBar();
            }
        }

        public void StartRemelting(OvenCraftList.craft craft)
        {
            currentItem = craft;
            currentTime = remeltingTime;

            EnableProgressBar();
        }

        public void OpenOvenMenu()
        {
            if(currentItem == null)
            {
                manager.OpenPanel(this);
            }
            else
            {
                if(canTakeItem)
                {
                    PlayerInventory.instance.GiveItem(currentItem.finalItem.item, currentItem.finalItem.amount);
                    currentItem = null;
                    canTakeItem = false;

                    DisableProgressBar();
                }
            }
        }

        private void UpdateProgressBar()
        {
            float progress = Mathf.Abs((currentTime / remeltingTime) - 1);

            progressRender.fillAmount = progress;
            progressRender.color = progressColor.Evaluate(progress);
        }
        private void EnableProgressBar()
        {
            itemImage.sprite = currentItem.finalItem.item._icon;
            progressRender.fillAmount = 0;
            progressRender.color = progressColor.Evaluate(0);

            progressBarObject.SetActive(true);
        }
        private void DisableProgressBar()
        {
            progressBarObject.SetActive(false);
        }

        public void Damage(float dmg)
        {
            Debug.Log(gameObject.name + " damaged!");
        }

        public void Die()
        {
            Destroy(gameObject);
        }
    }
}
