using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using AYellowpaper.SerializedCollections;

namespace Game.Notifications
{
    public class NotificationVisual : MonoBehaviour
    {
        [System.Serializable]
        private struct StyleData
        {
            public Sprite IconBaseSprite;
            public Sprite FadeSprite;
        }

        [SerializeField] private Image ItemIconImage;
        [SerializeField] private Image IconBaseImage;
        [Space]
        [SerializeField] private Image TextFieldFadeImage;
        [SerializeField] private TMP_Text NotificationText;

        [Space]
        [SerializeField] private Animator Anim;
        [SerializeField, NaughtyAttributes.AnimatorParam("Anim")] private string Anim_disappearTrigger = "Disappear";
        [SerializeField] private Material OutlineMaterialIcon;
        [SerializeField] private Material OutlineMaterialTextHighlighted;
        [SerializeField] private Material OutlineMaterialTextDefault;

        [Space]
        [SerializedDictionary("Style enum", "Data"), SerializeField] private SerializedDictionary<NotificationStyle, StyleData> Styles = new SerializedDictionary<NotificationStyle, StyleData>(); 

        public void Initialize(Sprite icon, string title, bool isHighlighting, float destroyTime, NotificationStyle style)
        {
            ItemIconImage.sprite = icon;
            NotificationText.text = title;

            SetStyle(style);

            if (isHighlighting)
            {
                ItemIconImage.material = OutlineMaterialIcon;
                NotificationText.fontMaterial = OutlineMaterialTextHighlighted;
            }
            else
            {
                ItemIconImage.material = null;
                NotificationText.fontMaterial = OutlineMaterialTextDefault;
            }

            Invoke("StartHidingNotification", destroyTime);
        }

        private void SetStyle(NotificationStyle target)
        {
            StyleData currentStyle = Styles[target];

            IconBaseImage.sprite = currentStyle.IconBaseSprite;
            TextFieldFadeImage.sprite = currentStyle.FadeSprite;
        }

        private void StartHidingNotification()
        {
            Anim.SetTrigger(Anim_disappearTrigger);
        }

        private void Anim_DestroyObj()
        {
            Destroy(gameObject);
        }
    }
}
