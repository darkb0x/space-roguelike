using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("movement")]
    [SerializeField] private float speed;

    Vector2 move_input;

    Rigidbody2D rigidbody;
    PlayerInventory inventory;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        inventory = FindObjectOfType<PlayerInventory>();
    }

    private void Update()
    {
        move_input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }

    private void FixedUpdate()
    {
        rigidbody.MovePosition(rigidbody.position + (move_input * speed) * Time.fixedDeltaTime);
    }
}
