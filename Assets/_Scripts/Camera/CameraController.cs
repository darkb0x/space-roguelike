using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

namespace Game
{
    using Player;
    using Save;
    using Input;

    public class CameraController : MonoBehaviour, IService, IEntryComponent<PlayerController>
    {
        [SerializeField] private Volume _MapVolume;
        [SerializeField] private Volume _MainVolume;

        [Header("Camera View")]
        [SerializeField] private float maxCamViewScale = 15;
        [SerializeField] private float minCamViewScale = 5;
        [Space]
        [SerializeField] private float scrollSensivity = 1.2f;
        [SerializeField] private float scaleSpeed = 2.2f;

        public Volume MapVolume => _MapVolume;
        public Volume MainVolume => _MainVolume;

        private Camera cam;
        private Transform target;
        private Transform myTransform;
        private float currentZoom;

        private float _scrollDelta;

        private UISaveData currentUISettingsData => SaveManager.UISaveData;

        public void Initialize(PlayerController player)
        {
            myTransform = GetComponent<Transform>();
            cam = GetComponent<Camera>();

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
