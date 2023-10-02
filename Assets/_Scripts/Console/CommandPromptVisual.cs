using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Game.Console
{
    using Commands;
    using UnityEngine.EventSystems;

    public class CommandPromptVisual : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        private readonly Color _helpTextColor = new Color(1, 1, 1, 1);
        private readonly Color _descriptionTextColor = new Color(1, 1, 1, .5f); 

        private enum VisualStyleEnum
        {
            Selected,
            Deselected
        }

        [SerializeField] private TextMeshProUGUI Title;
        [Space]
        [SerializeField] private Image BGImage;
        [SerializeField] private Color SelectedColor;
        [SerializeField] private Color DeselectedColor;

        public CommandBase Command => _command;

        private ConsoleWindow _consoleWindow;
        private CommandBase _command;

        public void Initialize(CommandBase command, ConsoleWindow consoleWindow)
        {
            _command = command;
            _consoleWindow = consoleWindow;

            Title.text = $"<color=#{ColorUtility.ToHtmlStringRGBA(_helpTextColor)}>{command.Help}</color> <color=#{ColorUtility.ToHtmlStringRGBA(_descriptionTextColor)}>{command.Description}</color>";
        }

        public void Select()
        {
            SetStyle(VisualStyleEnum.Selected);
        }
        public void Deselect()
        {
            SetStyle(VisualStyleEnum.Deselected);
        }

        public void ResetFields()
        {
            _command = null;

            Title.text = string.Empty;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            SetStyle(VisualStyleEnum.Selected);
            _consoleWindow.SelectCommandPrompt(this);
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            SetStyle(VisualStyleEnum.Deselected);
            _consoleWindow.DeselectCommandPrompt(this);
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            _consoleWindow.ConfirmSelectedCommandPrompt(this);
        }

        private void SetStyle(VisualStyleEnum visualStyle)
        {
            switch (visualStyle)
            {
                case VisualStyleEnum.Selected:
                    {
                        BGImage.color = SelectedColor;
                    }
                    break;
                case VisualStyleEnum.Deselected:
                    {
                        BGImage.color = DeselectedColor;
                    }
                    break;
            }
        }
    }
}