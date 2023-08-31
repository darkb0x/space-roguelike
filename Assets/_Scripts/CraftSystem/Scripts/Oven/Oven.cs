using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

namespace Game.CraftSystem.Oven
{
    using Player;
    using Player.Inventory;
    using Manager;

    [RequireComponent(typeof(PlayerInteractObject))]
    public class Oven : MonoBehaviour
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

        [Header("Animation")]
        [SerializeField] private Animator Anim;
        [AnimatorParam("Anim"), SerializeField] private string Anim_StartFireTrigger;
        [AnimatorParam("Anim"), SerializeField] private string Anim_FireBool;

        [Header("Particles")]
        [SerializeField] private ParticleSystem SmokeParticle;
        [ReadOnly, SerializeField] private float StartRateOverTime = 5f;
        [SerializeField] private float SmokeEnabledSpeed = 0.2f;

        OvenConfig.craft currentItem;
        float currentTime;
        bool canTakeItem = false;

        private PlayerInventory PlayerInventory;

        private void Start()
        {
            PlayerInventory = ServiceLocator.GetService<PlayerInventory>();

            manager = FindObjectOfType<OvenManager>();

            SmokeParticle.gameObject.SetActive(false);
            StartRateOverTime = SmokeParticle.emission.rateOverTime.constantMax;

            DisableProgressBar();
        }

        private void Update()
        {
            float targetEmmision = 0;
            ParticleSystem.EmissionModule emmision = SmokeParticle.emission;
            if(currentItem != null)
            {
                targetEmmision = StartRateOverTime;
                if(canTakeItem)
                {
                    targetEmmision = 0;
                }
            }
            emmision.rateOverTime = Mathf.Lerp(emmision.rateOverTime.constantMax, targetEmmision, SmokeEnabledSpeed * Time.deltaTime);

            if (currentItem == null)
                return;

            if(currentTime <= 0)
            {
                Anim.SetBool(Anim_FireBool, false);

                canTakeItem = true;
            }
            else
            {
                currentTime -= Time.deltaTime;
                UpdateProgressBar();
            }
        }

        public void StartRemelting(OvenConfig.craft craft)
        {
            Anim.SetTrigger(Anim_StartFireTrigger);
            Anim.SetBool(Anim_FireBool, true);

            SmokeParticle.gameObject.SetActive(true);

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
                    PlayerInventory.AddItem(currentItem.finalItem.item, currentItem.finalItem.amount);
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
            itemImage.sprite = currentItem.finalItem.item.LowSizeIcon;
            progressRender.fillAmount = 0;
            progressRender.color = progressColor.Evaluate(0);

            progressBarObject.SetActive(true);
        }
        private void DisableProgressBar()
        {
            progressBarObject.SetActive(false);
        }
    }
}
