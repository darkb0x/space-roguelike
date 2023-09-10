using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class SpacefieldVisual : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer SpriteRender;
        [SerializeField] private Material m_Material;

        private Material material;

        private void Start()
        {
            material = Instantiate(m_Material);
            SpriteRender.material = material;
        }

        public void UpdateScale(Camera targetCamera)
        {
            material.SetFloat("_CameraSize", targetCamera.orthographicSize);

            float screenHeight = targetCamera.orthographicSize * 2f;
            float screenWidth = screenHeight * targetCamera.aspect;
            transform.localScale = new Vector3(screenWidth, screenHeight, 1f);
        }
    }
}
