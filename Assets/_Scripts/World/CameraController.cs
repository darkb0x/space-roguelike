using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game
{
    using Player;
    using SaveData;

    public class CameraController : MonoBehaviour
    {
        [Header("Camera View")]
        [SerializeField] private float maxCamViewScale = 15;
        [SerializeField] private float minCamViewScale = 5;
        [Space]
        [SerializeField] private float scrollSensivity = 1.2f;
        [SerializeField] private float scaleSpeed = 2.2f;

        [Header("Sacefield Visual")]
        [SerializeField] private SpacefieldVisual SpacefieldVisual;

        private Camera cam;
        private PlayerController player;
        private Transform target;
        private Transform myTransform;
        private float currentZoom;

        private UISettingsData currentUISettingsData => SaveDataManager.Instance.CurrentUISettingsData;

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
            float scrollDelta = -GameInput.Instance.GetMouseScrollDeltaY();
            if (scrollDelta != 0)
            {
                if(!IsPointerOverUIObject())
                {
                    currentZoom = Mathf.Clamp(currentZoom + scrollDelta * scrollSensivity, minCamViewScale, maxCamViewScale);
                }
            }

            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, currentZoom, scaleSpeed * Time.deltaTime);
            currentUISettingsData.CameraZoom = currentZoom;

            myTransform.position = target.position - new Vector3(0, 0, Mathf.Abs(myTransform.position.z));

            if (SpacefieldVisual != null)
                SpacefieldVisual.UpdateScale(cam);
        }

        private bool IsPointerOverUIObject()
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = GameInput.Instance.GetMousePosition();
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0;
        }
    }
}
