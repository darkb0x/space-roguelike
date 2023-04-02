using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Game.Player.Inventory.Visual.Notifications
{
    public class InventoryNotificationsVisualItem : MonoBehaviour
    {
        [SerializeField] private Image ItemIconImage;
        [SerializeField] private Transform ItemIsNew_Visual;
        [Space]
        [SerializeField] private TextMeshProUGUI ItemNameText;
        [SerializeField] private TextMeshProUGUI AddedAmountText;
        [SerializeField] private Color AmountPositiveColor;
        [SerializeField] private Color AmountNegativeColor;
        [Space]
        [SerializeField] private CanvasGroup CanvasGroup;
        [Space]
        [SerializeField] private Transform VisualTransform;

        private bool itemIsNew = false;

        public void Initialize(InventoryItem item, int amount, bool isNew, float destroyTime, bool isTake)
        {
            gameObject.name = "Item " + item.name;

            ItemIconImage.sprite = item.Icon;
            ItemNameText.text = item.ItemName;
            if(!isTake)
            {
                AddedAmountText.text = "+" + amount;
                AddedAmountText.color = AmountPositiveColor;
            }
            else
            {
                AddedAmountText.text = "-" + amount;
                AddedAmountText.color = AmountNegativeColor;
            }

            itemIsNew = isNew;
            ItemIsNew_Visual.gameObject.SetActive(isNew);

            StartCoroutine(ShowNotification());

            Invoke("StartHidingNotification", destroyTime);
        }

        private void Update()
        {
            if (itemIsNew)
            {
                float rotationSpeed = -30f;
                ItemIsNew_Visual.Rotate(new Vector3(0, 0, rotationSpeed) * Time.deltaTime);
            }
        }

        private void StartHidingNotification()
        {
            StartCoroutine(HideNotification());
        }
        private IEnumerator HideNotification()
        {
            float speed = 3f;
            while(CanvasGroup.alpha > 0)
            {
                CanvasGroup.alpha = Mathf.MoveTowards(CanvasGroup.alpha, 0, speed * Time.deltaTime);
                yield return null;
            }
            Destroy(gameObject);
        }

        private IEnumerator ShowNotification()
        {
            float timeForMove = 2f;
            float showSpeed = 2f / timeForMove;
            float moveSpeed = 5f / timeForMove;
            float time = Time.time + timeForMove;

            CanvasGroup.alpha = 0;
            VisualTransform.localPosition = new Vector3(0, -130f);

            while(Time.time < time)
            {
                VisualTransform.localPosition = Vector3.Lerp(VisualTransform.localPosition, Vector3.zero, moveSpeed * Time.deltaTime);
                CanvasGroup.alpha = Mathf.MoveTowards(CanvasGroup.alpha, 1, showSpeed * Time.deltaTime);

                yield return null;
            }
            CanvasGroup.alpha = 1;
        }
    }
}
