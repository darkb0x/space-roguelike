using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Game.Console
{
    using Commands;
    using UnityEngine.EventSystems;

    public class CommandPromptVisual : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        private enum VisualStyleEnum
        {
            Selected,
            Deselected
        }

        [SerializeField] private TextMeshProUGUI CommandText;
        [SerializeField] private TextMeshProUGUI CommandDescription;
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

            CommandText.text = command.Help;
            CommandDescription.text = command.Description;
        }

        public void Select()
        {
            SetStyle(VisualStyleEnum.Selected);
        }
        public void Deselect()
        {
            SetStyle(VisualStyleEnum.Deselected);
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