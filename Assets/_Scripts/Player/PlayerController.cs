using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("movement")]
    [SerializeField] private float speed;

    Vector2 move_input;

    new Rigidbody2D rigidbody;
    PlayerInventory inventory;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        inventory = FindObjectOfType<PlayerInventory>();
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
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        selectedDrone = null;
    }
}
