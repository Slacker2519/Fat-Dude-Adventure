using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Vector2 movementDirection;
    Vector2 velocity;
    float magnitude;
    float _horizontalInput;
    float _playerSpeed;
    float _currentSpeed;

    [SerializeField] float Accelerate = 15f;
    [SerializeField] float MaxSpeed = 15f;
    [SerializeField] float GravityScale = -9.81f;
    [SerializeField] float MinSpeed = 7f;
    [SerializeField] LayerMask WhatIsGround;
    [SerializeField] Transform GroundCheck;

    bool _grounded;

    void Awake()
    {   
        transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, 0);
    }

    // Update is called once per frame
    void Update()
    {
        _grounded = Physics2D.Raycast(transform.position, Vector2.down);

        _playerSpeed = _currentSpeed;

        MovePlayer();
        
        //PlayerGravity();

        //Debug.Log("Speed: " + _currentSpeed);
        //Debug.DrawRay(transform.position, Vector2.down * 1f, Color.red);
        //Debug.Log("grounded: " + _grounded);
    }

    void FixedUpdate()
    {
        FlipPlayer();
        //PlayerGravity();
    }

    void MovePlayer()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        var verticalInput = Input.GetAxisRaw("Vertical");
        movementDirection = new Vector2(_horizontalInput, verticalInput);

        SpeedController();

        magnitude = Mathf.Clamp01(movementDirection.magnitude) * _playerSpeed;
        movementDirection.Normalize();

        transform.Translate(movementDirection * magnitude * Time.deltaTime);
    }

    void SpeedController()
    {
        if (_horizontalInput != 0)
        {
            _currentSpeed += Accelerate * Time.deltaTime;
        }
        if (_currentSpeed > MaxSpeed)
        {
            _currentSpeed = MaxSpeed;
        }
        if (_horizontalInput == 0)
        {
            _currentSpeed = MinSpeed;
            _currentSpeed -= Accelerate * Time.deltaTime;
        }
    }

    void FlipPlayer()
    {
        if (_horizontalInput < 0)
        {
            transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
        }
    }

    void PlayerGravity()
    {
        velocity.y += GravityScale * Time.deltaTime;
        transform.Translate(velocity * Time.deltaTime);

        if (_grounded && velocity.y < 0)
        {
            velocity.y = -.5f;
        }
    }
}
