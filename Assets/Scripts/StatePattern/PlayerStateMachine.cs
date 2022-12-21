using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    [Header("Components")]
    Rigidbody2D _rb;
    Animator _animator;

    [Header("Layer Masks")]
    [SerializeField] LayerMask _groundLayer;
    [SerializeField] LayerMask _wallLayer;

    [Header("Movement Variables")]
    [SerializeField] float _movementAcceleration;
    [SerializeField] float _maxMoveSpeed;
    [SerializeField] float _groundLinearDrag;
    float _horizontalDirection;
    bool _changingDirection => (_rb.velocity.x > 0f && _horizontalDirection < 0f) || (_rb.velocity.x < 0f && _horizontalDirection > 0f);

    [Header("Jump Variables")]
    [SerializeField] float _jumpForce;
    [SerializeField] float _airLinearDrag = 2.5f;
    [SerializeField] float _fallMultiplier = 8f;
    [SerializeField] float _maxGravity;
    [SerializeField] float _jumpGravity;
    [SerializeField] float _airHangTime = .5f;
    bool _canJump;
    float _airHangTimeCounter;

    [Header("Air Jump Variables")]
    [SerializeField] int _airJump;
    int _airJumpValue;
    bool _canAirJump;

    [Header("Ground Collision Variables")]
    [SerializeField] float _groundRaycastLength;
    [SerializeField] float _groundRaycastOffset;
    bool _grounded;

    //[Header("Wall Interaction Variables")]
    //[SerializeField] float _wallRaycastLength;
    //[SerializeField] float _wallSlideGravity;
    //[SerializeField] float _wallJumpForce;
    //[SerializeField] float _wallJumpAngle;
    //[SerializeField] float _wallDrag;
    //[SerializeField] float _heightToWallSlide;
    //bool _wallOnRight;
    //bool _wallOnLeft;
    //bool _canNotWallSlide;
    //float xAngle, yAngle;

    //[Header("Ledge Hang Variables")]
    //[SerializeField] float _ledgeRaycastYOffset;
    //[SerializeField] float _ledgeRaycastXOffset;
    //bool _detectedLedgeOnLeft;
    //bool _detectedLedgeOnRight;

    // State Variables
    PlayerBaseState _currentState;
    PlayerStateFactory _states;

    // Flip Player
    bool facingRight = true;

    //Getters and Setters
    public Rigidbody2D Rb { get { return _rb; } }
    public float MovementAcceleration { get { return _movementAcceleration; } }
    public float MaxMoveSpeed { get { return _maxMoveSpeed; } }
    public float GroundLinearDrag { get { return _groundLinearDrag; } }
    public float HorizontalDirection { get { return _horizontalDirection; } }
    public bool ChangingDirection { get { return _changingDirection; } }
    public float JumpForce { get { return _jumpForce; } }
    public float AirLinearDrag { get { return _airLinearDrag; } }
    public float FallMultiplier { get { return _fallMultiplier; } }
    public int AirJump { get { return _airJump; } }
    public float MaxGravity { get { return _maxGravity; } }
    public float JumpGravity { get { return _jumpGravity; } }
    public float AirHangTime { get { return _airHangTime; } }
    public bool IsJumpPress { get; private set; }
    public bool CanJump { get { return _canJump; } }
    public float AirHangTimeCounter { get { return _airHangTimeCounter; } set { _airHangTimeCounter = value; } }
    public int AirJumpValue { get { return _airJumpValue; } set { _airJumpValue = value; } }
    public bool CanAirJump { get { return _canAirJump; } }
    public bool Grounded { get { return _grounded; } }
    public PlayerBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }

    // Start is called before the first frame update
    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _states = new PlayerStateFactory(this);
        _currentState = _states.Grounded();
        _airHangTimeCounter = _airHangTime;
        //_currentState.EnterState();
    }

    // Update is called once per frame
    void Update()
    {
        CheckCollisions();
        if (_horizontalDirection < 0f && facingRight)
        {
            FlipPlayer();
        }
        else if (_horizontalDirection > 0f && !facingRight)
        {
            FlipPlayer();
        }

        IsJumpPress = Input.GetButtonDown("Jump");
        _canJump = IsJumpPress && (_airHangTimeCounter > 0f || _grounded);
        _canAirJump = IsJumpPress && !_grounded && _airJumpValue > 0f && _airHangTimeCounter <= 0f;
        _currentState.UpdateStates();
        _horizontalDirection = GetInput().x;
    }

    Vector2 GetInput()
    {
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    public void ApplyAirLinearDrag()
    {
        _rb.drag = _airLinearDrag;
    }

    void FlipPlayer()
    {
        facingRight = !facingRight;
        transform.Rotate(0f, 180f, 0f);
    }

    void CheckCollisions()
    {
        _grounded = Physics2D.Raycast(new Vector2(transform.position.x - _groundRaycastOffset, transform.position.y), Vector2.down, _groundRaycastLength, _groundLayer) ||
                    Physics2D.Raycast(new Vector2(transform.position.x + _groundRaycastOffset, transform.position.y), Vector2.down, _groundRaycastLength, _groundLayer) ||
                    Physics2D.Raycast(transform.position, Vector2.down, _groundRaycastLength, _groundLayer);

        //_wallOnLeft = Physics2D.Raycast(transform.position, Vector2.left, _wallRaycastLength, _wallLayer);
        //_wallOnRight = Physics2D.Raycast(transform.position, Vector2.right, _wallRaycastLength, _wallLayer);

        //_canNotWallSlide = Physics2D.Raycast(transform.position, Vector2.down, _heightToWallSlide, _groundLayer);

        //_detectedLedgeOnLeft = Physics2D.Raycast(new Vector2(transform.position.x + _ledgeRaycastXOffset, transform.position.y + _ledgeRaycastYOffset)
        //                      , Vector2.left, _wallRaycastLength, _groundLayer);
        //_detectedLedgeOnRight = Physics2D.Raycast(new Vector2(transform.position.x - _ledgeRaycastXOffset, transform.position.y + _ledgeRaycastYOffset)
        //                      , Vector2.right, _wallRaycastLength, _groundLayer);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        // ground raycast
        Gizmos.DrawLine(new Vector2(transform.position.x + _groundRaycastOffset, transform.position.y), new Vector2(transform.position.x + _groundRaycastOffset, transform.position.y) + Vector2.down * _groundRaycastLength);
        Gizmos.DrawLine(new Vector2(transform.position.x - _groundRaycastOffset, transform.position.y), new Vector2(transform.position.x - _groundRaycastOffset, transform.position.y) + Vector2.down * _groundRaycastLength);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * _groundRaycastLength);

        //// wall raycast
        //Gizmos.DrawLine(transform.position, transform.position + Vector3.left * _wallRaycastLength);
        //Gizmos.DrawLine(transform.position, transform.position + Vector3.right * _wallRaycastLength);

        //// ground distance raycast
        //Gizmos.DrawLine(transform.position, transform.position + Vector3.down * _heightToWallSlide);

        // ledge raycast
        //Gizmos.DrawLine(new Vector2(transform.position.x + _ledgeRaycastXOffset, transform.position.y + _ledgeRaycastYOffset)
        //              , new Vector2(transform.position.x + _ledgeRaycastXOffset, transform.position.y + _ledgeRaycastYOffset) + Vector2.left * _wallRaycastLength);
        //Gizmos.DrawLine(new Vector2(transform.position.x + _ledgeRaycastXOffset, transform.position.y + _ledgeRaycastYOffset)
        //              , new Vector2(transform.position.x + _ledgeRaycastXOffset, transform.position.y + _ledgeRaycastYOffset) + Vector2.right * _wallRaycastLength);
    }
}
