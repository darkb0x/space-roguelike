using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("movement")]
    [SerializeField] private float speed;

    [Header("picked turret")]
    [SerializeField, NaughtyAttributes.ReadOnly] TurretAI selectedTurret;
    [SerializeField] private Transform pickedTurretPosition;

    Vector2 move_input;

    new Rigidbody2D rigidbody;
    new Transform transform;
    PlayerInventory inventory;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        inventory = FindObjectOfType<PlayerInventory>();
        transform = GetComponent<Transform>();
    }

    private void Update()
    {
        move_input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        if(Input.GetKeyDown(KeyCode.E))
        {
            if (selectedDrone)
            {
                if (!selectedDrone.isPicked) selectedDrone.Init();
            }
        }
    }

    private void FixedUpdate()
    {
        rigidbody.MovePosition(rigidbody.position + (move_input * speed) * Time.fixedDeltaTime);
    }

    DroneAI selectedDrone;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<DroneAI>(out DroneAI drone))
        {
            if(!drone.isPicked) selectedDrone = drone;
        }
        if (collision.TryGetComponent<TurretAI>(out TurretAI turret))
        {
            if (!selectedTurret) selectedTurret = turret;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        selectedDrone = null;
        selectedTurret = null;
    }
}
