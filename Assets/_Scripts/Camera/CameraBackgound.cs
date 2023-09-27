using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class CameraBackgound : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer SpriteRenderer;

        public void SetSprite(Sprite sprite)
        {
            SpriteRenderer.sprite = sprite;
        }

        public void UpdateScale(Camera targetCamera)
        {
            float screenHeight = targetCamera.orthographicSize * 2f;
            float screenWidth = screenHeight * targetCamera.aspect;
            transform.localScale = new Vector3(screenWidth, screenHeight, 1f);
        }
    }
}
