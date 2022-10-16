using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("—корость с какой камера будет приследость игрока(target)")]
    [SerializeField, Range(1, 10)] private float followSpeed = 3;

    Transform target;
    Vector3 _targetInVector3;
    new Transform transform;

    private void Start()
    {
        transform = GetComponent<Transform>();

        target = FindObjectOfType<PlayerController>().transform;
    }

    private void FixedUpdate()
    {
        _targetInVector3 = new Vector3(target.position.x, target.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, _targetInVector3, followSpeed * Time.fixedDeltaTime);
    }
}
