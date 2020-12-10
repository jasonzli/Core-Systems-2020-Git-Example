using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerMove : MonoBehaviour
{
    // demo comment
    
    public int speed = 4;
    public int jumpForce = 800;

    private Rigidbody2D _rigidbody;
    public LayerMask groundLayer;
    public Transform feetPoint;
    public bool grounded;
    private bool isPaused = false;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }


    void FixedUpdate()
    {
        float xSpeed = Input.GetAxisRaw("Horizontal") * speed;
        _rigidbody.velocity = new Vector2(xSpeed, _rigidbody.velocity.y);
    }

    void Update()
    {
        if (isPaused)
        {
            return;
        }
        grounded = Physics2D.OverlapCircle(feetPoint.position, .5f, groundLayer);
        if (Input.GetButtonDown("Jump") && grounded)
        {
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, 0);
            _rigidbody.AddForce(new Vector2(0, jumpForce));
        }
    }

}