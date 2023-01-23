using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementManager : MonoBehaviour
{
    Rigidbody2D _rb;
    PlayerRun _playerRun;
    PlayerFall _playerFall;
    PlayerJump _playerJump;
    PlayerAirJump _playerAirJump;
    PlayerWallSlide _playerWallSlide;
    PlayerWallJump _playerWallJump;

    [Header("MovePlayerValues")]
    [SerializeField] float _playerAcceleration;
    [SerializeField] float _maxMoveSpeed;
    float _horizontalDirection;
    //bool changingDirection => (_rb.velocity.x > 0f && _playerRun.HorizontalDirection < 0f) || (_rb.velocity.x < 0f && _playerRun.HorizontalDirection > 0f);

    [Header("PlayerJumpValues")]
    [SerializeField] float _jumpForce;
    [SerializeField] float _jumpGravity;
    bool _isJumpPress => Input.GetKeyDown(KeyCode.Space);
    bool _canJump;
    bool _jumping => _rb.velocity.y >=0 && !_grounded;

    [Header("PlayerFallValues")]
    [SerializeField] float _fallMultiPlier;
    [SerializeField] float _maxGravity;

    [Header("PlayerAirJumpValue")]
    [SerializeField] int _airJumpNumber;
    int _airJumpValue;

    [Header("PlayerWallSlideValues")]
    [SerializeField] float _wallSlideDrag;
    [SerializeField] float _wallSlideGravity;
    bool _wallSliding => (_wallOnLeft || _wallOnRight) && !_grounded;

    [Header("PlayerWallJump")]
    [SerializeField] float _wallJumpAngle;
    [SerializeField] long _wallJumpForce;
    bool _wallJump => (_wallOnLeft || _wallOnRight) && !_grounded && _isJumpPress;

    [Header("GroundCheckValues")]
    [SerializeField] LayerMask _groundLayer;
    [SerializeField] float _groundRaycastOffset;
    [SerializeField] float _groundRaycastLength;
    bool _grounded;

    [Header("WallCheckValues")]
    [SerializeField] LayerMask _wallLayer;
    [SerializeField] float _wallRaycastLength;
    [SerializeField] float _wallRaycastOffset;
    bool _wallOnRight;
    bool _wallOnLeft;

    [Header("OnGroundValues")]
    [SerializeField] float _groundedGravity;
    [SerializeField] float _groundedDrag;

    [Header("OnAirValues")]
    [SerializeField] float _airDrag;

    [Header("HangTimeOnAir")]
    [SerializeField] float _airHangTime;
    float _airHangTimeCounter;

    public Rigidbody2D Rb { get { return _rb; } }
    public float GroundedGravity { get { return _groundedGravity; } }
    public float HorizontalDirection { get { return _horizontalDirection; } }
    public bool Grounded { get { return _grounded; } }
    public bool WallSliding { get { return _wallSliding; } }
    public bool WallJump { get { return _wallJump; } }

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _playerRun = GetComponent<PlayerRun>();
        _playerFall = GetComponent<PlayerFall>();
        _playerJump = GetComponent<PlayerJump>();
        _playerAirJump = GetComponent<PlayerAirJump>();
        _playerWallSlide = GetComponent<PlayerWallSlide>();
        _playerWallJump = GetComponent <PlayerWallJump>();
        _airHangTimeCounter = _airHangTime;
        _airJumpValue = _airJumpNumber;
    }

    // Update is called once per frame
    void Update()
    {
        PhysicsOnGround(_groundedGravity, _groundedDrag);
        PhysicsOnAir(_airDrag);
        PhysicsOnWall();
        FlipPlayer();
        CoyoteTime();
        JumpCondition();
    }

    void FixedUpdate()
    {
        GroundCheck(_groundRaycastOffset, _groundRaycastLength, _groundLayer);
        WallCheck(_wallRaycastLength, _wallRaycastOffset, _wallLayer);
        _playerRun.MovePlayer(_playerAcceleration, _maxMoveSpeed, ref _horizontalDirection);
    }

    private void JumpCondition()
    {
        _canJump = _airHangTimeCounter > 0f || _grounded || _wallOnLeft || _wallOnRight ? true : false;

        if (_jumping) _rb.gravityScale = _jumpGravity;
        if (_canJump && _isJumpPress) _playerJump.JumpPlayer(_jumpForce);
        if (_rb.velocity.y <= 0f) _playerFall.FallingPlayer(_fallMultiPlier, _maxGravity);
        if (!_canJump && !_grounded && _isJumpPress && _airJumpValue > 0f) _playerAirJump.AirJumping(ref _airJumpValue, _jumpForce);
        if ((_wallOnLeft || _wallOnRight) && !_grounded) _playerWallSlide.WallSlide(_wallSlideGravity, _wallSlideDrag);
        if (_wallOnLeft && !_grounded && _isJumpPress) _playerWallJump.JumpToTheRight(_wallJumpAngle, _wallJumpForce);
        if (_wallOnRight && !_grounded && _isJumpPress) _playerWallJump.JumpToTheLeft(_wallJumpAngle, _wallJumpForce);
    }

    void CoyoteTime()
    {
        if (_rb.velocity.y >= 0f)
        {
            _airHangTimeCounter = 0f;
        }
        if (_grounded)
        {
            _airHangTimeCounter = _airHangTime;
        }
        if (!_grounded && !_isJumpPress)
        {
            _airHangTimeCounter -= Time.deltaTime;
        }
    }

    void PhysicsOnGround(float groundedGravity, float groundedDrag)
    {
        if (_grounded) 
        {
            _canJump = true;
            _airJumpValue = _airJumpNumber;
            _rb.gravityScale = groundedGravity;
            _rb.drag = groundedDrag;
        }
    }

    void PhysicsOnAir(float airDrag)
    {
        if (!_grounded)
        {
            _canJump = false;
            _rb.drag = airDrag;
        }
    }

    void PhysicsOnWall()
    {
        if (_wallSliding)
        {
            _canJump = true;
            _airJumpValue = _airJumpNumber;
        }
    }

    void FlipPlayer()
    {
        if (_horizontalDirection < 0f)
        {
            transform.localScale = new Vector3(-1, 1f, 1f);
        }
        else if (_horizontalDirection > 0f)
        {
            transform.localScale = new Vector3(1, 1f, 1f);
        }
    }

    void GroundCheck(float groundRaycastOffset, float groundRaycastLength, LayerMask groundLayer)
    {
        _grounded = Physics2D.Raycast(new Vector2(transform.position.x - groundRaycastOffset, transform.position.y), Vector2.down, groundRaycastLength, groundLayer) ||
                    Physics2D.Raycast(new Vector2(transform.position.x + groundRaycastOffset, transform.position.y), Vector2.down, groundRaycastLength, groundLayer);
    }

    void WallCheck(float wallRaycastLength, float wallRaycastOffset, LayerMask wallLayer)
    {
        _wallOnLeft = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + wallRaycastOffset), Vector2.left, wallRaycastLength, wallLayer);
        _wallOnRight = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + wallRaycastOffset), Vector2.right, wallRaycastLength, wallLayer);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        // ground raycast
        Gizmos.DrawLine(new Vector2(transform.position.x + _groundRaycastOffset, transform.position.y),
        new Vector2(transform.position.x + _groundRaycastOffset, transform.position.y) + Vector2.down * _groundRaycastLength);

        Gizmos.DrawLine(new Vector2(transform.position.x - _groundRaycastOffset, transform.position.y),
        new Vector2(transform.position.x - _groundRaycastOffset, transform.position.y) + Vector2.down * _groundRaycastLength);

        // wall raycast
        Gizmos.DrawLine(new Vector2(transform.position.x, transform.position.y + _wallRaycastOffset), new Vector2(transform.position.x, transform.position.y + _wallRaycastOffset) + Vector2.left * _wallRaycastLength);
        Gizmos.DrawLine(new Vector2(transform.position.x, transform.position.y + _wallRaycastOffset), new Vector2(transform.position.x, transform.position.y + _wallRaycastOffset) + Vector2.right * _wallRaycastLength);
    }
}
