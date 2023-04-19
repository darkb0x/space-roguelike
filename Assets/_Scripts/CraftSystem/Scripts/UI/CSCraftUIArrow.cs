using UnityEngine;

namespace Game.CraftSystem
{
    public class CSCraftUIArrow : MonoBehaviour
    {
        private RectTransform rectTransform;

        public void SetPosition(Vector2 startPosition, Vector2 position, float size)
        {
            // Get RectTransform
            rectTransform = GetComponent<RectTransform>();

            // Set position
            rectTransform.position = startPosition;

            // Set rotation
            rectTransform.rotation = Quaternion.Euler(0,0, GetAngleFromVector(position - startPosition));

            // Set width
            float distance = Vector2.Distance(position, startPosition) / size;
            rectTransform.sizeDelta = new Vector2(distance, 100);
        }

        public float GetAngleFromVector(Vector2 direction)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            if (angle < 0f)
            {
                angle += 360f;
            }
            return angle;
        }
    }
}
