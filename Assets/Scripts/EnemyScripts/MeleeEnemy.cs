using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class MeleeEnemy : EnemyBase
{
    #region Variables
    Rigidbody2D _rb2d;

    [Header("WallCheckVariables")]
    [SerializeField] LayerMask _wallsLayer;
    [SerializeField] float _wallsRaycastLength;
    [SerializeField] float _wallsRaycastOffset;
    bool _wallOnLeft;
    bool _wallOnRight;

    [Header("LedgeCheckVariables")]
    [SerializeField] LayerMask _groundLayer;
    [SerializeField] float _ledgesRaycastLength;
    [SerializeField] float _ledgesRaycastOffset;
    bool _noLeftLedge;
    bool _noRightLedge;

    [Header("PatrolVariables")]
    [SerializeField] float _walkVelocity;
    int _enemyHorizontalDirection = -1;

    [Header("DetectPlayer")]
    [SerializeField] LayerMask _targetLayer;
    [SerializeField] float _targetDetectRaycastLength;
    [SerializeField] float _targetDetectRaycastOffset;
    bool _playerOnRight;
    bool _playerOnLeft;
    bool _attacking => _playerOnLeft || _playerOnRight;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        _rb2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_attacking) 
            Attack();
        else
            Patrolling();
    }

    void FixedUpdate()
    {
        WallsDetection(_wallsRaycastOffset, _wallsRaycastLength, _wallsLayer);
        LedgesDetection(_ledgesRaycastLength, _ledgesRaycastOffset, _groundLayer);
        TargetDectection();
    }

    void Patrolling()
    {
        CaculateWalkDirection();
        GraphicFlip();
        _rb2d.velocity = new Vector2(_enemyHorizontalDirection * _walkVelocity, _rb2d.velocity.y);
    }

    void CaculateWalkDirection()
    {
        if (_wallOnLeft || !_noLeftLedge)
        {
            _enemyHorizontalDirection = -1;
        }
        else if (_wallOnRight || !_noRightLedge)
        {
            _enemyHorizontalDirection = 1;
        }
    }

    protected override void GraphicFlip()
    {
        if (_enemyHorizontalDirection == 1)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        if (_enemyHorizontalDirection == -1)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    protected override void Attack()
    {
        if (_playerOnLeft)
        {
            _rb2d.velocity = new Vector2(0f, _rb2d.velocity.y);
            transform.localScale = new Vector3(-1, 1, 1);
            // attack animation
        }

        if (_playerOnRight)
        {
            _rb2d.velocity = new Vector2(0f, _rb2d.velocity.y);
            transform.localScale = new Vector3(1, 1, 1);
            // attack animation
        }
    }

    protected override void TakeDamage()
    {
        
    }

    protected override void Die()
    {
        base.Die();
    }

    void WallsDetection(float wallsRaycastOffset, float wallsRaycastLength, LayerMask wallsLayer)
    {
        _wallOnLeft = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + wallsRaycastOffset), Vector2.right, wallsRaycastLength, wallsLayer);
        _wallOnRight = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + wallsRaycastOffset), Vector2.left, wallsRaycastLength, wallsLayer);
    }

    void LedgesDetection(float ledgesRaycastLength, float ledgesRaycastOffset, LayerMask groundLayer)
    {
        _noLeftLedge = Physics2D.Raycast(new Vector2(transform.position.x + ledgesRaycastOffset, transform.position.y), Vector2.down, ledgesRaycastLength, groundLayer);
        _noRightLedge = Physics2D.Raycast(new Vector2(transform.position.x - ledgesRaycastOffset, transform.position.y), Vector2.down, ledgesRaycastLength, groundLayer);
    }

    protected override void TargetDectection()
    {
        _playerOnLeft = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + _targetDetectRaycastOffset), Vector2.left, _targetDetectRaycastLength, _targetLayer);
        _playerOnRight = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + _targetDetectRaycastOffset), Vector2.right, _targetDetectRaycastLength, _targetLayer);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawLine(new Vector2(transform.position.x, transform.position.y + _wallsRaycastOffset), new Vector2(transform.position.x, transform.position.y + _wallsRaycastOffset) + Vector2.right * _wallsRaycastLength);
        Gizmos.DrawLine(new Vector2(transform.position.x, transform.position.y + _wallsRaycastOffset), new Vector2(transform.position.x, transform.position.y + _wallsRaycastOffset) + Vector2.left * _wallsRaycastLength);

        Gizmos.DrawLine(new Vector2(transform.position.x + _ledgesRaycastOffset, transform.position.y), new Vector2(transform.position.x + _ledgesRaycastOffset, transform.position.y) + Vector2.down * _ledgesRaycastLength);
        Gizmos.DrawLine(new Vector2(transform.position.x - _ledgesRaycastOffset, transform.position.y), new Vector2(transform.position.x - _ledgesRaycastOffset, transform.position.y) + Vector2.down * _ledgesRaycastLength);

        Gizmos.DrawLine(new Vector2(transform.position.x, transform.position.y + _targetDetectRaycastOffset), new Vector2(transform.position.x, transform.position.y + _targetDetectRaycastOffset) + Vector2.left * _targetDetectRaycastLength);
        Gizmos.DrawLine(new Vector2(transform.position.x, transform.position.y + _targetDetectRaycastOffset), new Vector2(transform.position.x, transform.position.y + _targetDetectRaycastOffset) + Vector2.right * _targetDetectRaycastLength);
    }
}
