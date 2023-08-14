using UnityEngine;

namespace Game.CraftSystem.Research.Visual
{
    public class CraftTreeConnectionVisual : MonoBehaviour
    {
        [SerializeField] private RectTransform MainRectTransform;
        [SerializeField] private RectTransform VisualRectTransform;

        public void SetPosition(Vector2 startPosition, Vector2 position, float canvasScale)
        {   
            // Set position
            MainRectTransform.position = startPosition;

            // Set rotation
            MainRectTransform.rotation = Quaternion.Euler(0, 0, GetAngleFromVector(position - startPosition));

            // Set width
            float distance = Vector2.Distance(position, startPosition) / canvasScale;
            VisualRectTransform.sizeDelta = new Vector2(distance, 100);
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