using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Notifications
{
    using SaveData;

    public class NotificationsEnabledButton : MonoBehaviour
    {
        [SerializeField] private Button m_Button;
        [Space]
        [SerializeField] private Sprite EnabledButtonNormalSprite;
        [SerializeField] private SpriteState EnabledButtonSpriteState;
        [Space]
        [SerializeField] private Sprite DisabledButtonNormalSprite;
        [SerializeField] private SpriteState DisabledButtonSpriteState;

        private void Start()
        {
            UpdateVisual(SaveDataManager.Instance.CurrentUISettingsData.EnableNotifications);
        }

        public void SetEnabled()
        {
            UISettingsData data = SaveDataManager.Instance.CurrentUISettingsData;
            data.EnableNotifications = !data.EnableNotifications;
            data.Save();

            UpdateVisual(data.EnableNotifications);
        }

        private void UpdateVisual(bool enabled)
        {
            if (enabled)
            {
                m_Button.image.sprite = EnabledButtonNormalSprite;
                m_Button.spriteState = EnabledButtonSpriteState;
            }
            else
            {
                m_Button.image.sprite = DisabledButtonNormalSprite;
                m_Button.spriteState = DisabledButtonSpriteState;
            }
        } 
    }
}
