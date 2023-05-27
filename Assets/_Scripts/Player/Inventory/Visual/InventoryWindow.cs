using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player.Inventory.Visual
{
    public class InventoryWindow : MonoBehaviour
    {
        [SerializeField] private GameObject WindowVisual;

        public bool Active
        {
            get
            {
                return WindowVisual.activeSelf;
            }
            set
            {
                WindowVisual.SetActive(value);
            }
        }

        private void Start()
        {
            Active = false;
        }

        public void Open()
        {
            Active = !Active;
        }
    }
}
