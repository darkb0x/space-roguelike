using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game
{
    using Player;
    using Save;
    using Input;

    public class CameraController : MonoBehaviour
    {
        [Header("Camera View")]
        [SerializeField] private float maxCamViewScale = 15;
        [SerializeField] private float minCamViewScale = 5;
        [Space]
        [SerializeField] private float scrollSensivity = 1.2f;
        [SerializeField] private float scaleSpeed = 2.2f;

        private Camera cam;
        private PlayerController player;
        private Transform target;
        private Transform myTransform;
        private float currentZoom;

        private float _scrollDelta;

        private UISaveData currentUISettingsData => SaveManager.UISaveData;

        private void Start()
        {
            myTransform = GetComponent<Transform>();
            cam = GetComponent<Camera>();
            player = FindObjectOfType<PlayerController>();

            target = player.transform;
            currentZoom = currentUISettingsData.CameraZoom;
            cam.orthographicSize = currentZoom;
        }

        private void Update()
        {
            _scrollDelta = InputManager.Instance.GetMouseScrollDeltaY();
        }
        private void LateUpdate()
        {
            if (_scrollDelta != 0)
            {
                if(!IsPointerOverUIObject())
                {
                    currentZoom = Mathf.Clamp(currentZoom + _scrollDelta * scrollSensivity, minCamViewScale, maxCamViewScale);
                    currentUISettingsData.CameraZoom = currentZoom;
                    currentUISettingsData.Save();
                }
            }

            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, currentZoom, scaleSpeed * Time.deltaTime);

            myTransform.position = target.position - new Vector3(0, 0, Mathf.Abs(myTransform.position.z));
        }

        private bool IsPointerOverUIObject()
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = InputManager.Instance.GetMousePosition();
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0;
        }
    }
}
