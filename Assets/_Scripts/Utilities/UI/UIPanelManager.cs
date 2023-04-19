using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game
{
    public interface IUIPanelManagerObserver
    {
        public void PanelStateIsChanged(GameObject panel);
    }

    public class UIPanelManager : MonoBehaviour
    {
        public static UIPanelManager Instance;

        List<IUIPanelManagerObserver> observers = new List<IUIPanelManagerObserver>();
        [NaughtyAttributes.ReadOnly] public GameObject currentOpenedPanel;

        [SerializeField, Tooltip("Canvas/Player")] private GameObject playerUI;
        [Space]
        [SerializeField] private Volume blurVolume;

        [System.Serializable]
        public struct Panels
        {
            public string name;
            public GameObject panel_obj;
        }
        [Header("OpenClose panels")]
        public List<Panels> panels = new List<Panels>();

        private bool canOpen = true;

        private void Awake() => Instance = this;

        private void EnablePanel(GameObject panel, bool enabled)
        {
            playerUI.SetActive(!enabled);

            for (int i = 0; i < panels.Count; i++)
            {
                panels[i].panel_obj.SetActive(false);
            }

            foreach (var item in panels)
            {
                if (item.panel_obj == panel)
                {
                    panel.SetActive(enabled);
                    currentOpenedPanel = enabled ? panel : null;
                    Notify();

                    if (blurVolume != null)
                        blurVolume.weight = enabled ? 1 : 0;
                    else
                        Debug.LogWarning(gameObject.name + " Blur Volume is null!");

                    return;
                }
            }

            if(panel != null)
            {
                panel.SetActive(enabled);
            }

            if (blurVolume != null)
                blurVolume.weight = enabled ? 1 : 0;
            else
                Debug.LogWarning(gameObject.name + " Blur Volume is null!");
        }

        public void ClosePanel(GameObject panel)
        {
            EnablePanel(panel, false);

            GameInput.Instance.SetPlayerActionMap();

            Notify();
            StartCoroutine(SetCooldown());
        }

        public bool OpenPanel(GameObject panel)
        {
            if (!canOpen)
                return false;

            currentOpenedPanel = panel;
            EnablePanel(panel, true);

            GameInput.Instance.SetUIActionMap();

            Notify();

            return true;
        }

        public void CloseAllPanel(bool showPlayerInterface = true)
        {
            Time.timeScale = 1;
            blurVolume.weight = 0;
            for (int i = 0; i < panels.Count; i++)
            {
                panels[i].panel_obj.SetActive(false);
            }
            playerUI.SetActive(showPlayerInterface);
            GameInput.Instance.SetPlayerActionMap();
        }

        public bool SomethinkIsOpened()
        {
            foreach (var item in panels)
            {
                if (item.panel_obj.activeSelf)
                {
                    return true;
                }
            }
            return false;
        }

        private IEnumerator SetCooldown()
        {
            float time = Time.time;
            float cooldown = 0.15f;
            canOpen = false;
            while(Time.time < time+cooldown)
            {
                yield return null;
            }
            canOpen = true;
        }

        #region interface logic
        public void Attach(IUIPanelManagerObserver o)
        {
            observers.Add(o);
        }
        public void Detach(IUIPanelManagerObserver o)
        {
            observers.Remove(o);
        }

        public void Notify()
        {
            foreach (var item in observers)
            {
                item.PanelStateIsChanged(currentOpenedPanel);
            }
        }
        #endregion
    }
}
