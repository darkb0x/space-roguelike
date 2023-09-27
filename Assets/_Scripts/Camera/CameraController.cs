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
        private const float Z_POSITION = -10f;

        [SerializeField] private Volume _MapVolume;
        [SerializeField] private Volume _MainVolume;

        [Header("Camera View")]
        [SerializeField] private float maxCamViewScale = 15;
        [SerializeField] private float minCamViewScale = 5;
        [Space]
        [SerializeField] private float scrollSensivity = 1.2f;
        [SerializeField] private float scaleSpeed = 2.2f;

        [Header("Camera Bacgkound")]
        [SerializeField] private bool ShowBackgound = true;
        [SerializeField, NaughtyAttributes.ShowIf("ShowBackgound")] private CameraBackgound CameraBackgound;
        [SerializeField, NaughtyAttributes.ShowIf("ShowBackgound")] private Sprite BackgroundSprite;

        public Volume MapVolume => _MapVolume;
        public Volume MainVolume => _MainVolume;

        private Camera _cam;
        private Transform _target;
        private bool _lockCamera;
        private float _currentZoom;
        private float _scrollDelta;

        private UISaveData currentUISettingsData => SaveManager.UISaveData;

        public void Initialize(PlayerController player)
        {
            _cam = GetComponent<Camera>();

            _target = player.transform;
            _currentZoom = currentUISettingsData.CameraZoom;
            _cam.orthographicSize = _currentZoom;

            _lockCamera = false;

            CameraBackgound.SetSprite(BackgroundSprite);
            CameraBackgound.gameObject.SetActive(ShowBackgound);
        }

        private void Update()
        {
            _scrollDelta = InputManager.Instance.GetMouseScrollDeltaY();

            CameraBackgound.UpdateScale(_cam);
        }
        private void LateUpdate()
        {
            if (_lockCamera)
            {
                Vector3 pos = new Vector3(transform.position.x, transform.position.y, Z_POSITION);
                transform.position = pos;
                return;
            }

            if (_scrollDelta != 0)
            {
                if(!IsPointerOverUIObject())
                {
                    _currentZoom = Mathf.Clamp(_currentZoom + _scrollDelta * scrollSensivity, minCamViewScale, maxCamViewScale);
                    currentUISettingsData.CameraZoom = _currentZoom;
                    currentUISettingsData.Save();
                }
            }

            _cam.orthographicSize = Mathf.Lerp(_cam.orthographicSize, _currentZoom, scaleSpeed * Time.deltaTime);

            transform.position = new Vector3(_target.position.x, _target.position.y, Z_POSITION);
        }

        private bool IsPointerOverUIObject()
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = InputManager.Instance.GetMousePosition();
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0;
        }

        public void Lock(bool enabled)
        {
            _lockCamera = enabled;
        }
    }
}
