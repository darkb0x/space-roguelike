using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Player;

public class CameraController : MonoBehaviour
{
    [Header("Скорость с какой камера будет приследость игрока(target)")]
    [SerializeField, Range(1, 10)] private float followSpeed = 3;
    [Header("обзор камеры")]
    [SerializeField] private float maxCamViewScale = 15;
    [SerializeField] private float minCamViewScale = 5;
    [Space]
    [SerializeField] private float scrollSpeed = 1.2f;
    [SerializeField] private float scaleSpeed = 2.2f;

    Transform target;
    Vector3 _targetInVector3;
    new Transform transform;
    Camera cam;
    float currentZoom;

    private void Start()
    {
        transform = GetComponent<Transform>();
        cam = GetComponent<Camera>();

        target = FindObjectOfType<PlayerController>().transform;
        currentZoom = cam.orthographicSize;
    }

    private void Update()
    {
        currentZoom = Mathf.Clamp(currentZoom + -Input.mouseScrollDelta.y * scrollSpeed, minCamViewScale, maxCamViewScale);
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, currentZoom, scaleSpeed * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        //_targetInVector3 = new Vector3(target.position.x, target.position.y, transform.position.z);
        //transform.position = Vector3.Lerp(transform.position, _targetInVector3, followSpeed * Time.fixedDeltaTime);
    }
}
