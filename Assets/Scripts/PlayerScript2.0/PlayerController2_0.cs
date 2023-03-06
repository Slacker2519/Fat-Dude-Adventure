using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController2_0 : MonoBehaviour
{
    #region Variables
    Rigidbody2D _rb;
    PlayerRun _playerRun;
    PlayerFall _playerFall;
    PlayerJump _playerJump;
    PlayerAirJump _playerAirJump;
    PlayerWallSlide _playerWallSlide;
    PlayerWallJump _playerWallJump;
    PlayerDash _playerDash;

    [Header("PlayerTakeDamage")]
    [SerializeField] int _playerHealth;
    [SerializeField] float _timeToResetPlayerAfterTakeDamage;
    [SerializeField] long _knockBackForce;
    [SerializeField] float _knockBackAngle;
    int _playerCurrentHealth;
    bool _playerTakeDamage = false;

    [Header("MovePlayerValues")]
    [SerializeField] float _playerAcceleration = 75f;
    float _playerCurrentAcceleration;
    [SerializeField] float _maxMoveSpeed;
    float _horizontalDirection;

    [Header("PlayerJumpValues")]
    [SerializeField] float _jumpForce;
    [SerializeField] float _jumpGravity;
    bool _isJumpPress => Input.GetKeyDown(KeyCode.Space);
    bool _canJump;
    bool _jumping;

    [Header("PlayerFallValues")]
    [SerializeField] float _fallMultiPlier;
    [SerializeField] float _maxGravity;
    bool _falling;

    [Header("PlayerAirJumpValue")]
    [SerializeField] int _airJumpNumber;
    int _airJumpValue;
    bool _airJumping;

    [Header("PlayerWallSlideValues")]
    [SerializeField] float _wallSlideVelocity;
    bool _wallSliding;

    [Header("PlayerWallJump")]
    [SerializeField] float _wallJumpAngle;
    [SerializeField] long _wallJumpForce;
    bool _wallJump => (_wallOnLeft || _wallOnRight) && !_grounded && _isJumpPress;

    [Header("PlayerDash")]
    [SerializeField] GameObject _playerAfterImagePrefabs;
    [SerializeField] float _distanceBetweenAfterImages;
    [SerializeField] float _dashVelocity;
    [SerializeField] float _dashingDuration;
    [SerializeField] float _dashCoolDown;
    bool _canDash = true;
    bool _isDashing = false;

    [Header("PlayerAttack")]
    [SerializeField] float _attackDamage;
    [SerializeField] float _playerAttackDuration;
    bool _playerAttacking;

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

    [Header("EnemyDetectionValue")]
    [SerializeField] LayerMask _enemyLayer;
    [SerializeField] float _enemyDetectorRadius;
    [SerializeField] float _enemyCirclecastOffset;
    RaycastHit2D _enemyDetector;

    [Header("OnGroundValues")]
    [SerializeField] float _groundedGravity;
    [SerializeField] float _groundedDrag;

    [Header("OnAirValues")]
    [SerializeField] float _airDrag;

    [Header("HangTimeOnAir")]
    [SerializeField] float _airHangTime;
    float _airHangTimeCounter;
    #endregion

    #region Getters & Setters
    public Rigidbody2D Rb { get { return _rb; } }
    public float GroundedGravity { get { return _groundedGravity; } }
    public float HorizontalDirection { get { return _horizontalDirection; } }
    public bool Grounded { get { return _grounded; } }
    public bool WallSliding { get { return _wallSliding; } }
    public bool WallJump { get { return _wallJump; } }
    public bool IsDashing { get { return _isDashing; } }
    public bool Falling { get { return _falling; } }
    public bool Jumping { get { return _jumping; } }
    public bool AirJumping { get { return _airJumping; } }
    public bool PlayerTakeDamage { get { return _playerTakeDamage; } }
    #endregion

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
        _playerDash = GetComponent<PlayerDash>();
        _playerCurrentHealth = _playerHealth;
        _playerCurrentAcceleration = _playerAcceleration;
        _airHangTimeCounter = _airHangTime;
        _airJumpValue = _airJumpNumber;
    }

    // Update is called once per frame
    void Update()
    {
        
        FlipPlayer();
        CoyoteTime();

        if (_isDashing) return;
        MovementLogic();
    }

    void FixedUpdate()
    {
        PhysicsOnGround(_groundedGravity, _groundedDrag);
        PhysicsOnAir(_airDrag);
        PhysicsOnWall();
        GroundCheck(_groundRaycastOffset, _groundRaycastLength, _groundLayer);
        WallCheck(_wallRaycastLength, _wallRaycastOffset, _wallLayer);
        EnemyDetection(_enemyDetectorRadius, _enemyCirclecastOffset, _enemyLayer);

        if (_playerTakeDamage) return;
        if (_playerAttacking) return;
        if (_isDashing) return;
        _playerRun.MovePlayer(_playerCurrentAcceleration, _maxMoveSpeed, ref _horizontalDirection);
    }

    void MovementLogic()
    {
        _canJump = _airHangTimeCounter > 0f || _grounded || _wallOnLeft || _wallOnRight ? true : false;
        _falling = _rb.velocity.y < 0f && !_wallSliding && !_grounded ? true : false;
        _jumping = _rb.velocity.y >= 0 && !_grounded ? true : false;
        _airJumping = _rb.velocity.y >= 0 && !_canJump && !_grounded && _airJumpValue == 0f ? true : false;
        if (_jumping) _rb.gravityScale = _jumpGravity;

        if (Input.GetKeyDown(KeyCode.Mouse1) && _canDash) StartCoroutine(DashCoolDown());
        if (_canJump && _isJumpPress) _playerJump.JumpPlayer(_jumpForce);
        if (_falling) _playerFall.FallingPlayer(_fallMultiPlier, _maxGravity);
        if (!_canJump && !_grounded && _isJumpPress && _airJumpValue > 0f) _playerAirJump.AirJumping(ref _airJumpValue, _jumpForce);
        if (_wallOnLeft && !_grounded && _isJumpPress) _playerWallJump.JumpToTheRight(_wallJumpAngle, _wallJumpForce);
        if (_wallOnRight && !_grounded && _isJumpPress) _playerWallJump.JumpToTheLeft(_wallJumpAngle, _wallJumpForce);
        WallSlideLogic();

        if (Input.GetMouseButtonDown(0))
        {
            _playerAttacking = true;
            Invoke("Attacking", _playerAttackDuration);
        }
    }

    void Attacking()
    {
        _playerAttacking = false;
    }

    void WallSlideLogic()
    {
        _horizontalDirection = Input.GetAxisRaw("Horizontal");
        _wallSliding = (_wallOnLeft || _wallOnRight) && !_grounded && _horizontalDirection != 0f ? true : false;
        if (_wallSliding)
        {
            _playerWallSlide.WallSlide(_wallSlideVelocity);
        }
    }

    IEnumerator DashCoolDown()
    {
        _canDash = false;
        _isDashing = true;
        float _originalGravity = _rb.gravityScale;
        _playerDash.Dash(_dashVelocity);
        Coroutine c = StartCoroutine(_playerDash.SpawnAfterImage(_playerAfterImagePrefabs, _distanceBetweenAfterImages));
        yield return new WaitForSeconds(_dashingDuration);
        StopCoroutine(c);
        _rb.velocity = Vector2.zero;
        _rb.gravityScale = _originalGravity;
        _isDashing = false;
        yield return new WaitForSeconds(_dashCoolDown);
        _canDash = true;
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
            _airJumping = false;
            _canJump = true;
            _airJumpValue = _airJumpNumber;
            _rb.gravityScale = groundedGravity;
            _rb.drag = groundedDrag;
        }
    }

    void PhysicsOnAir(float airDrag)
    {
        if (!_grounded && !_wallSliding)
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

        if (!_wallSliding) _playerCurrentAcceleration = _playerAcceleration;
    }

    void FlipPlayer()
    {
        if (_horizontalDirection < 0f)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else if (_horizontalDirection > 0f)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }

        if (_wallJump && _wallOnRight) transform.localScale = new Vector3(-1f, 1f, 1f);
        if (_wallJump && _wallOnLeft) transform.localScale = new Vector3(1f, 1f, 1f);
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

    void EnemyDetection(float enemyDetectorRadius, float enemyCirclecastOffset, LayerMask enemyLayer)
    {
        _enemyDetector = Physics2D.CircleCast(new Vector2(transform.position.x, transform.position.y + enemyCirclecastOffset), enemyDetectorRadius, Vector2.right, enemyLayer);
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

        // enemy circlecast
        Gizmos.DrawWireSphere(new Vector2(transform.position.x, transform.position.y + _enemyCirclecastOffset), _enemyDetectorRadius);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Enemy"))
        {
            if (transform.position.x - _enemyDetector.point.x < 0f) _playerWallJump.JumpToTheLeft(_knockBackAngle, _knockBackForce);
            if (transform.position.x - _enemyDetector.point.x > 0f) _playerWallJump.JumpToTheRight(_knockBackAngle, _knockBackForce);
            _playerTakeDamage = true;
            _playerCurrentHealth--;
            Invoke("ResetPlayerAfterTakeDamage", _timeToResetPlayerAfterTakeDamage);
        }
    }

    void ResetPlayerAfterTakeDamage()
    {
        _playerTakeDamage = false;
    }
}
