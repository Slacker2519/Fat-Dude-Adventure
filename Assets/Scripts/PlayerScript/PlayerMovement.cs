using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
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
    bool changingDirection => (_rb.velocity.x > 0f && _horizontalDirection < 0f) || (_rb.velocity.x < 0f && _horizontalDirection > 0f);

    [Header("Jump Variables")]
    [SerializeField] float _jumpForce;
    [SerializeField] float _airLinearDrag = 2.5f;
    bool _canJump;
    [SerializeField] float _fallMultiplier = 8f;
    [SerializeField] int _extraJumps;
    [SerializeField] float _maxGravity;
    [SerializeField] float _jumpGravity;
    [SerializeField] float _hangTime = .5f;
    float _hangTimeCounter;
    int _extraJumpsValue;
    public bool IsJumpPress { get; private set; }

    [Header("Ground Collision Variables")]
    [SerializeField] float _groundRaycastLength;
    [SerializeField] float _groundRaycastOffset;
    bool _onGround;

    [Header("Wall Interaction Variables")]
    [SerializeField] float _wallRaycastLength;
    [SerializeField] float _wallSlideGravity;
    [SerializeField] float _wallJumpForce;
    [SerializeField] float _wallJumpAngle;
    [SerializeField] float _wallDrag;
    [SerializeField] float _heightToWallSlide;
    bool _wallOnRight;
    bool _wallOnLeft;
    bool _canNotWallSlide;
    float xAngle, yAngle;

    [Header("Ledge Grabing Variables")]
    [SerializeField] float _ledgeRaycastYOffset;
    [SerializeField] float _ledgeRaycastXOffset;
    bool _touchingLedgeOnLeft;
    bool _touchingLedgeOnRight;
    
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        _horizontalDirection = GetInput().x;

        if (_onGround)
        {
            _hangTimeCounter = _hangTime;
            _rb.gravityScale = _jumpGravity;
            _extraJumpsValue = _extraJumps;
            ApllyGroundLinearDrag();
        }
        else
        {
            ApplyAirLinearDrag();
            FallMultiplier();
        }

        if (!Input.GetButtonDown("Jump") && !_onGround)
        {
            _hangTimeCounter -= Time.deltaTime;
        }

        if (_canJump && Input.GetButtonDown("Jump")) 
        {
            Jump();
        }

        if (Input.GetButtonDown("Jump"))
        {
            AirJump();
        }

        if (_hangTimeCounter > 0f || _onGround)
        {
            _canJump = true;
        }
        else
        {
            _canJump = false;
        }

        if (!_canNotWallSlide)
        {
            WallInteraction();
        }
    }

    void FixedUpdate()
    {
        CheckCollisions();
        MoveCharacter();
    }

    Vector2 GetInput()
    {
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    void MoveCharacter()
    {
        _rb.AddForce(new Vector2(_horizontalDirection, 0f) * _movementAcceleration);

        if (Mathf.Abs(_rb.velocity.x) > _maxMoveSpeed)
        {
            _rb.velocity = new Vector2(Mathf.Sign(_rb.velocity.x) * _maxMoveSpeed, _rb.velocity.y);
        }
    }

    void ApllyGroundLinearDrag()
    {
        if (Mathf.Abs(_horizontalDirection) < 0.4f || !changingDirection)
        {
            _rb.drag = _groundLinearDrag;
        }
        else
        {
            _rb.drag = _airLinearDrag;
        }
    }

    void ApplyAirLinearDrag()
    {
        _rb.drag = _airLinearDrag;
    }

    void Jump()
    {
        ApplyAirLinearDrag();
        _rb.velocity = new Vector2(_rb.velocity.x, 0f);
        _rb.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
    }

    void AirJump()
    {
        if (!_canJump && _extraJumpsValue > 0f)
        {
            _extraJumpsValue--;
            ApplyAirLinearDrag();
            _rb.velocity = new Vector2(_rb.velocity.x, 0f);
            _rb.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
        }
    }

    void FallMultiplier()
    {
        if (_rb.velocity.y < 0f) 
        { 
            _rb.gravityScale += _fallMultiplier * Time.deltaTime;
        }
        else
        {
            _hangTimeCounter = 0f;
            _rb.gravityScale = _jumpGravity;
        }

        if (_rb.gravityScale > _maxGravity)
        {
            _rb.gravityScale = _maxGravity;
        }
    }

    void WallInteraction()
    {
        if (_wallOnLeft || _wallOnRight)
        {
            _rb.drag = 30f;
            _rb.gravityScale = _wallSlideGravity;
            _extraJumpsValue = _extraJumps;
        }

        xAngle = Mathf.Cos(_wallJumpAngle * Mathf.Deg2Rad);
        yAngle = Mathf.Sin(_wallJumpAngle * Mathf.Deg2Rad);

        if (_wallOnLeft && Input.GetButtonDown("Jump"))
        {
            _rb.drag = _wallDrag;
            _rb.velocity = Vector2.zero;
            _rb.AddForce(new Vector2(xAngle, yAngle).normalized * _wallJumpForce);
        }
        else if (_wallOnRight && Input.GetButtonDown("Jump"))
        {
            _rb.drag = _wallDrag;
            _rb.velocity = Vector2.zero;
            _rb.AddForce(new Vector2(xAngle * -1f, yAngle).normalized * _wallJumpForce);
        }
    }

    void GrabLedge()
    {
        if (_touchingLedgeOnLeft || _touchingLedgeOnRight)
        {

        }
    }

    void CheckCollisions()
    {
        _onGround = Physics2D.Raycast(new Vector2(transform.position.x - _groundRaycastOffset, transform.position.y), Vector2.down, _groundRaycastLength, _groundLayer) ||
                    Physics2D.Raycast(new Vector2(transform.position.x + _groundRaycastOffset, transform.position.y), Vector2.down, _groundRaycastLength, _groundLayer) ||
                    Physics2D.Raycast(transform.position, Vector2.down, _groundRaycastLength, _groundLayer);

        _wallOnLeft = Physics2D.Raycast(transform.position, Vector2.left, _wallRaycastLength, _wallLayer);
        _wallOnRight = Physics2D.Raycast(transform.position, Vector2.right, _wallRaycastLength, _wallLayer);

        _canNotWallSlide = Physics2D.Raycast(transform.position, Vector2.down, _heightToWallSlide, _groundLayer);

        _touchingLedgeOnLeft = Physics2D.Raycast(new Vector2(transform.position.x + _ledgeRaycastXOffset, transform.position.y + _ledgeRaycastYOffset)
                              ,Vector2.left, _wallRaycastLength, _groundLayer);
        _touchingLedgeOnRight = Physics2D.Raycast(new Vector2(transform.position.x - _ledgeRaycastXOffset, transform.position.y + _ledgeRaycastYOffset)
                              , Vector2.right, _wallRaycastLength, _groundLayer);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        // ground raycast
        Gizmos.DrawLine(new Vector2(transform.position.x + _groundRaycastOffset, transform.position.y), new Vector2(transform.position.x + _groundRaycastOffset, transform.position.y) + Vector2.down * _groundRaycastLength);
        Gizmos.DrawLine(new Vector2(transform.position.x - _groundRaycastOffset, transform.position.y), new Vector2(transform.position.x - _groundRaycastOffset, transform.position.y) + Vector2.down * _groundRaycastLength);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * _groundRaycastLength);

        // wall raycast
        Gizmos.DrawLine(transform.position, transform.position + Vector3.left * _wallRaycastLength);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * _wallRaycastLength);

        // ground distance raycast
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * _heightToWallSlide);

        // ledge raycast
        Gizmos.DrawLine(new Vector2(transform.position.x + _ledgeRaycastXOffset, transform.position.y + _ledgeRaycastYOffset)
                      , new Vector2(transform.position.x + _ledgeRaycastXOffset, transform.position.y + _ledgeRaycastYOffset) + Vector2.left * _wallRaycastLength);
        Gizmos.DrawLine(new Vector2(transform.position.x + _ledgeRaycastXOffset, transform.position.y + _ledgeRaycastYOffset)
                      , new Vector2(transform.position.x + _ledgeRaycastXOffset, transform.position.y + _ledgeRaycastYOffset) + Vector2.right * _wallRaycastLength);
    }
}
