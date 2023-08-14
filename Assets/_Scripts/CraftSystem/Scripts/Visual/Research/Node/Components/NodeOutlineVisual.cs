using UnityEngine.UI;
using UnityEngine;

namespace Game.CraftSystem.Research.Visual.Node.Components
{
    public class NodeOutlineVisual : MonoBehaviour
    {
        [SerializeField] private Image FillImage;
        [SerializeField] private Image MainImage;

        public void UpdateFill(float progress)
        {
            FillImage.fillAmount = progress;
        }

        public void SetColor(Color main, Color fill)
        {
            MainImage.color = main;
            FillImage.color = fill;
        }
    }
}