using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game
{
    using Player;

    public class CameraController : MonoBehaviour
    {
        [Header("מבחמנ ךאלונû")]
        [SerializeField] private float maxCamViewScale = 15;
        [SerializeField] private float minCamViewScale = 5;
        [Space]
        [SerializeField] private float scrollSpeed = 1.2f;
        [SerializeField] private float scaleSpeed = 2.2f;

        private Transform target;
        private Transform myTransform;
        private Camera cam;
        private float currentZoom;

        private void Start()
        {
            myTransform = GetComponent<Transform>();
            cam = GetComponent<Camera>();

            target = FindObjectOfType<PlayerController>().transform;
            currentZoom = cam.orthographicSize;
        }

        private void Update()
        {
            if (!UIPanelManager.Instance.SomethinkIsOpened())
            {
                currentZoom = Mathf.Clamp(currentZoom + -GameInput.Instance.GetMouseScrollDeltaY() * scrollSpeed, minCamViewScale, maxCamViewScale);
                cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, currentZoom, scaleSpeed * Time.deltaTime);
            }

            myTransform.position = target.position - new Vector3(0, 0, Mathf.Abs(myTransform.position.z)); 
        }
    }
}
