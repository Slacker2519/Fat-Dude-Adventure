using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    // Movement Input
    PlayerStateMachine _playerStateMachine;
    float _horizontalDirection;
    bool _changingDirection => (_playerStateMachine.Rb.velocity.x > 0f && _horizontalDirection < 0f) || (_playerStateMachine.Rb.velocity.x < 0f && _horizontalDirection > 0f);

    // Getters Setters
    public float HorizontalDirection { get { return _horizontalDirection; } }
    public bool ChangingDirection { get { return _changingDirection; } }
    public bool IsJumpPress { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        IsJumpPress = Input.GetButtonDown("Jump");
        _horizontalDirection = GetInput().x;
    }

    Vector2 GetInput()
    {
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }
}
