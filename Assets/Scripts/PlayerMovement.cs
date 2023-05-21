using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public float speed = 200;
    Vector2 move;
    Rigidbody2D rb;
    public enum DifferentMovements
    {
        AddForce, Velocity, MovePosition
    }

    public DifferentMovements typeOfMovement;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (typeOfMovement == DifferentMovements.AddForce)
        {
            speed = 1000;
            rb.AddForce(move *  speed * Time.deltaTime);
        }
        else if (typeOfMovement == DifferentMovements.Velocity)
        {
            speed = 1000;
            rb.velocity = new Vector2(move.x * speed * Time.deltaTime, rb.velocity.y); // Þetta er það sama og rb.velocity = move * speed * Time.deltaTime;
        }
        else if (typeOfMovement == DifferentMovements.MovePosition)
        {
            speed = 10;
            rb.MovePosition(rb.position + (move * speed * Time.deltaTime));
        }
    }

    // Update is called once per frame
    void Update()
    {
        move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }
}
