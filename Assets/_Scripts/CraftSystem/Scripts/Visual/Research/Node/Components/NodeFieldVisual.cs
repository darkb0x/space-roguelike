using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace Game.CraftSystem.Research.Visual.Node.Components
{   
    // Note:
    // This code would be better if i wasn't lazy.
    public class NodeFieldVisual : MonoBehaviour
    {
        [SerializeField] private Image m_FillImage;
        [SerializeField] private Image m_MainImage;

        public Image FillImage
        {
            get
            {
                if (m_FillImage == null)
                    return m_MainImage;

                return m_FillImage;
            } 
        }
        public Image MainImage
        {
            get
            {
                return m_MainImage;
            }
        }
        [SerializeField] private TextMeshProUGUI TitleText;

        private System.Action OnActivate;

        public void Initialize(string title, System.Action onActivate)
        {
            OnActivate = onActivate;
            UpdateTitleText(title);
        }

        public void UpdateTitleText(string value)
        {
            if (TitleText == null)
                return;

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