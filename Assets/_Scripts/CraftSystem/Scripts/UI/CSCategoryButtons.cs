using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Game.CraftSystem
{
    public interface ICategoryButtonsChecker
    {
        public void OpenedPanel(RectTransform panelTransform);
    }
    public class CSCategoryButtons : MonoBehaviour
    {
        [System.Serializable]
        public struct Panel
        {
            public RectTransform panelTransform;
            public Sprite icon;
        }

        public List<ICategoryButtonsChecker> observers = new List<ICategoryButtonsChecker>();

        [SerializeField] private List<Panel> panels = new List<Panel>();
        [Space]
        [SerializeField] private GameObject buttonPrefab;
        [Space]
        [SerializeField] private bool resetPosition = false;
        [NaughtyAttributes.ShowIf("resetPosition"), SerializeField] private RectTransform contentTransform;

        public void Initialize(List<TechTree> trees, ICategoryButtonsChecker o = null)
        {
            if(o != null)
            {
                observers.Add(o);
            }

            foreach (TechTree tree in trees)
            {
                panels.Add(new Panel {
                    panelTransform = tree.techTreeRenderTransform.GetComponent<RectTransform>(),
                    icon = tree.categoryIcon
                    });

                Button btn = Instantiate(buttonPrefab, transform).GetComponent<Button>();
                UnityAction<int> btnAction = new UnityAction<int>(Open);
                btn.onClick.AddListener(() => btnAction(trees.IndexOf(tree)));

                btn.GetComponent<Image>().sprite = tree.categoryIcon;
            }

            Open(0);
        }
        public void Open(int idx)
        {
            Panel panel = panels[idx];
            foreach (Panel item in panels)
            {
                item.panelTransform.gameObject.SetActive(false);
            }

            if(resetPosition)
            {
                contentTransform.localPosition = Vector3.zero;
            }

            panel.panelTransform.gameObject.SetActive(true);

            // Observer
            foreach (var observer in observers)
            {
                observer.OpenedPanel(panel.panelTransform);
            }
        }
    }
}
