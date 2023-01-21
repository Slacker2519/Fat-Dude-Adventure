using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    public Animator Anim;

    [Header("Components")]
    Rigidbody2D _rb;

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

    [Header("Walls Interaction Variables")]
    [SerializeField] float _wallRaycastLength;
    [SerializeField] float _wallSlideGravity;
    [SerializeField] float _wallSlideDrag;
    [SerializeField] float _wallJumpForce;
    [SerializeField] float _wallJumpAngle;
    [SerializeField] float _heightToWallSlide;
    [SerializeField] float _wallsRaycastYOffSet;
    bool _wallOnRight;
    bool _wallOnLeft;
    bool _nextToWall;
    bool _groundedTransition;
    bool _canWallSlide;

    [Header("Ground Collision Variables")]
    [SerializeField] float _groundRaycastLength;
    [SerializeField] float _groundRaycastOffset;
    bool _grounded;

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
    public bool CanWallSlide { get { return _canWallSlide; } }
    public float WallSlideGravity { get { return _wallSlideGravity; } }
    public float WallSlideDrag { get { return _wallSlideDrag; } }
    public bool WallOnLeft { get { return _wallOnLeft; } }
    public bool WallOnRight { get { return _wallOnRight; } }
    public bool GroundedTransition { get { return _groundedTransition; } }
    public bool NextToWall { get { return _nextToWall; } }
    public float WallJumpForce { get { return _wallJumpForce; } }
    public float WallJumpAngle { get { return _wallJumpAngle; } }

    // Start is called before the first frame update
    void Start()
    {
        Anim = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
        _states = new PlayerStateFactory(this);
        _currentState = _states.Grounded();
        _airHangTimeCounter = _airHangTime;
    }

    // Update is called once per frame
    void Update()
    {
        CheckCollisions(_groundRaycastOffset, _groundRaycastLength, _groundLayer, _wallsRaycastYOffSet, _wallRaycastLength, _wallLayer, _heightToWallSlide);
        CheckFlipPlayer();

        IsJumpPress = Input.GetButtonDown("Jump");
        _horizontalDirection = GetInput().x;

        CheckToUpdateState();

        _currentState.UpdateStates();
    }

    private void CheckToUpdateState()
    {
        _nextToWall = _wallOnRight || _wallOnLeft;
        _canJump = IsJumpPress && (_airHangTimeCounter > 0f || _grounded);
        _canAirJump = IsJumpPress && !_grounded && _airJumpValue > 0f && _airHangTimeCounter <= 0f;
        _canWallSlide = !_groundedTransition && _nextToWall;
    }

    private void CheckFlipPlayer()
    {
        if (_horizontalDirection < 0f && facingRight && !_canWallSlide)
        {
            FlipPlayer();
        }
        else if (_horizontalDirection > 0f && !facingRight && !_canWallSlide)
        {
            FlipPlayer();
        }
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

    void CheckCollisions(float groundRaycastOffset, float groundRaycastLength, LayerMask groundLayer, float wallsRaycastYOffSet, float wallRaycastLength, LayerMask wallLayer, float heightToWallSlide)
    {
        _grounded = Physics2D.Raycast(new Vector2(transform.position.x - groundRaycastOffset, transform.position.y), Vector2.down, groundRaycastLength, groundLayer) ||
                    Physics2D.Raycast(new Vector2(transform.position.x + groundRaycastOffset, transform.position.y), Vector2.down, groundRaycastLength, groundLayer) ||
                    Physics2D.Raycast(transform.position, Vector2.down, groundRaycastLength, groundLayer);

        _wallOnLeft = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + wallsRaycastYOffSet), Vector2.left, wallRaycastLength, wallLayer);
        _wallOnRight = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + wallsRaycastYOffSet), Vector2.right, wallRaycastLength, wallLayer);

        _groundedTransition = Physics2D.Raycast(transform.position, Vector2.down, heightToWallSlide, groundLayer);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        // ground raycast
        Gizmos.DrawLine(new Vector2(transform.position.x + _groundRaycastOffset, transform.position.y), 
        new Vector2(transform.position.x + _groundRaycastOffset, transform.position.y) + Vector2.down * _groundRaycastLength);

        Gizmos.DrawLine(new Vector2(transform.position.x - _groundRaycastOffset, transform.position.y), 
        new Vector2(transform.position.x - _groundRaycastOffset, transform.position.y) + Vector2.down * _groundRaycastLength);

        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * _groundRaycastLength);

        // walls raycast
        Gizmos.DrawLine(new Vector2(transform.position.x, transform.position.y + _wallsRaycastYOffSet), 
        new Vector2(transform.position.x, transform.position.y + _wallsRaycastYOffSet) + Vector2.left * _wallRaycastLength);
        
        Gizmos.DrawLine(new Vector2(transform.position.x, transform.position.y + _wallsRaycastYOffSet), 
        new Vector2(transform.position.x, transform.position.y + _wallsRaycastYOffSet) + Vector2.right * _wallRaycastLength);

        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * _heightToWallSlide);
    }
}
