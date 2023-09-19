using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Player
{
    using UI.HUD;

    public class HealthHUD : HUDElement
    {
        [SerializeField] private Sprite FullHeartSprite;
        [SerializeField] private Sprite DamagedHeartSprite;
        [SerializeField] private List<Image> HeartsImages = new List<Image>();

        public override HUDElementID ID => HUDElementID.Health;

        private PlayerController _player;

        public override void Initialize()
        {
            _player = ServiceLocator.GetService<PlayerController>();

            foreach (var img in HeartsImages)
            {
                img.sprite = FullHeartSprite;
            }

            base.Initialize();
        }

        protected override void SubscribeToEvents()
        {
            base.SubscribeToEvents();
            _player.Health.OnHealthChanged += UpdateHealthVisual;
        }

        protected override void UnsubscribeFromEvents()
        {
            base.UnsubscribeFromEvents();
            _player.Health.OnHealthChanged -= UpdateHealthVisual;
        }

        private void UpdateHealthVisual(int value)
        {
            for (int i = 0; i < HeartsImages.Count; i++)
            {
                if (i < value)
                {
                    HeartsImages[i].sprite = FullHeartSprite;
                }
                else
                {
                    HeartsImages[i].sprite = DamagedHeartSprite;
                }
            }
        }

    }
}
