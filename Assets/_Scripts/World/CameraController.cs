using UnityEngine;

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

        private Camera cam;
        private PlayerController player;
        private Transform target;
        private Transform myTransform;
        private float currentZoom;

        private SessionData currentSessionData => GameData.Instance.CurrentSessionData;

        private void Start()
        {
            myTransform = GetComponent<Transform>();
            cam = GetComponent<Camera>();
            player = FindObjectOfType<PlayerController>();

            target = player.transform;
            currentZoom = currentSessionData.CameraZoom;
            cam.orthographicSize = currentZoom;
        }

        private void Update()
        {
            if (!UIPanelManager.Instance.SomethinkIsOpened())
            {
                currentZoom = Mathf.Clamp(currentZoom + -GameInput.Instance.GetMouseScrollDeltaY() * scrollSensivity, minCamViewScale, maxCamViewScale);
                cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, currentZoom, scaleSpeed * Time.deltaTime);

                currentSessionData.CameraZoom = currentZoom;
            }

            myTransform.position = target.position - new Vector3(0, 0, Mathf.Abs(myTransform.position.z)); 
        }
    }
}
