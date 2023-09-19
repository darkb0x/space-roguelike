using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

namespace Game.CraftSystem.Oven
{
    using Player;
    using Game.Inventory;

    [RequireComponent(typeof(PlayerInteractObject))]
    public class Oven : MonoBehaviour
    {
        OvenManager _manager;

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

        private OvenConfig.craft _currentItem;
        private float _currentTime;
        private bool _canTakeItem = false;

        private Inventory _inventory;

        private void Start()
        {
            _inventory = ServiceLocator.GetService<Inventory>();
            _manager = ServiceLocator.GetService<OvenManager>();

            _currentTime = remeltingTime;
            _currentItem = null;
            SmokeParticle.gameObject.SetActive(false);
            StartRateOverTime = SmokeParticle.emission.rateOverTime.constantMax;

            DisableProgressBar();
        }

        private void Update()
        {
            float targetEmmision = 0;
            ParticleSystem.EmissionModule emmision = SmokeParticle.emission;
            if (_currentItem != null)
            {
                targetEmmision = StartRateOverTime;
                if (_canTakeItem)
                {
                    targetEmmision = 0;
                }
                
                HandleRemelting();
            }
            emmision.rateOverTime = Mathf.Lerp(emmision.rateOverTime.constantMax, targetEmmision, SmokeEnabledSpeed * Time.deltaTime);
        }

        private void HandleRemelting()
        {
            if (_currentTime <= 0)
            {
                Anim.SetBool(Anim_FireBool, false);

                _canTakeItem = true;
            }
            else
            {
                _currentTime -= Time.deltaTime;
                UpdateProgressBar();

                _canTakeItem = false;
            }
        }

        public void StartRemelting(OvenConfig.craft craft)
        {
            Anim.SetTrigger(Anim_StartFireTrigger);
            Anim.SetBool(Anim_FireBool, true);

            SmokeParticle.gameObject.SetActive(true);

            _currentItem = craft;
            _currentTime = remeltingTime;

            EnableProgressBar();
        }

        public void OpenOvenMenu()
        {
            if(_currentItem == null)
            {
                _manager.Open(this);
            }
            else
            {
                if(_canTakeItem)
                {
                    _inventory.AddItem(_currentItem.finalItem);
                    _currentItem = null;
                    _canTakeItem = false;

                    DisableProgressBar();
                }
            }
        }

        private void UpdateProgressBar()
        {
            float progress = Mathf.Abs((_currentTime / remeltingTime) - 1);

            progressRender.fillAmount = progress;
            progressRender.color = progressColor.Evaluate(progress);
        }
        private void EnableProgressBar()
        {
            itemImage.sprite = _currentItem.finalItem.Item.LowSizeIcon;
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
