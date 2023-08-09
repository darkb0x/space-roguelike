using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace Game.CraftSystem.Visual.Node.Components
{
    public class NodeFieldVisual : MonoBehaviour
    {
        [SerializeField] private Image FillImage;
        [SerializeField] private TextMeshProUGUI TitleText;

        private System.Action OnActivate;

        public void Initialize(string title, System.Action onActivate)
        {
            OnActivate = onActivate;
            UpdateTitleText(title);
        }

        public void UpdateTitleText(string value)
        {
            TitleText.text = value;
        }
        public void UpdateFill(float progress)
        {
            if (FillImage == null)
                return;
            if (progress < 0 | progress > 1)
                throw new System.NotImplementedException(nameof(progress));

            FillImage.fillAmount = progress;
        }

        public void Activate()
        {
            OnActivate?.Invoke();
        }

        public void SetActive(bool enabled)
        {
            gameObject.SetActive(enabled);
        }
    }
}