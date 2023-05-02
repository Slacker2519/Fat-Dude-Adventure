using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class MeleeEnemy : EnemyBase
{
    #region Variables
    Rigidbody2D _rb2d;
    Animator _animator;

    [Header("Health")]
    [SerializeField] int _enemyHp;
    [SerializeField] float _resetTakingDamageTime;
    [SerializeField] float _knockBackForce;
    int _enemyCurrentHp;
    bool _takingDamage = false;

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
    float _walkVelocity;
    [SerializeField] float _patrolVelocity;
    int _enemyHorizontalDirection = 1;

    [Header("DetectPlayer")]
    [SerializeField] LayerMask _targetLayer;

    [Header("AttackPlayer")]
    [SerializeField] float _targetAttackCircleCastRadius;
    [SerializeField] float _targetDetectRaycastYOffset;
    float _targetDetectRaycastXOffset;
    [SerializeField] float newOffet;
    float _attackWalkingSpeed = 0;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _rb2d = GetComponent<Rigidbody2D>();
        _enemyCurrentHp = _enemyHp;
        _targetDetectRaycastXOffset = newOffet;
    }

    // Update is called once per frame
    void Update()
    {
        if (_takingDamage) return;

        EnemyStateControl();
        AnimationController();
    }

    void AnimationController()
    {
        _animator.SetFloat("EnemySpeed", Mathf.Abs(_rb2d.velocity.x));
        _animator.SetBool("EnemyAttack", InRangeAttack());
    }

    void FixedUpdate()
    {
        WallsDetection(_wallsRaycastOffset, _wallsRaycastLength, _wallsLayer);
        LedgesDetection(_ledgesRaycastLength, _ledgesRaycastOffset, _groundLayer);
        //TargetDectection();
        InRangeAttack();
    }

    void EnemyStateControl()
    {
        if (InRangeAttack())
        {
            Attack();
        }
        else if (!InRangeAttack())
        {
            PatrolDirection();
        }

        _rb2d.velocity = new Vector2(_enemyHorizontalDirection * _walkVelocity, _rb2d.velocity.y);
    }

    void PatrolDirection()
    {
        _walkVelocity = _patrolVelocity; 

        if (_wallOnLeft || !_noLeftLedge)
        {
            _enemyHorizontalDirection = -1;
            _targetDetectRaycastXOffset = -newOffet;
        }
        else if (_wallOnRight || !_noRightLedge)
        {
            _enemyHorizontalDirection = 1;
            _targetDetectRaycastXOffset = newOffet;
        }

        //GraphicFlip();
    }

    //protected override void GraphicFlip()
    //{
    //    if (_enemyHorizontalDirection == 1)
    //    {
    //        transform.localScale = new Vector3(1, 1, 1);
    //    }
    //    if (_enemyHorizontalDirection == -1)
    //    {
    //        transform.localScale = new Vector3(-1, 1, 1);
    //    }
    //}

    protected override void Attack()
    {
        
    }

    protected override void TakeDamage()
    {
        _takingDamage = true;
        _enemyCurrentHp--;
        Invoke("ResetTakeDamage", _resetTakingDamageTime);
    }

    void ResetTakeDamage()
    {
        _takingDamage = false;
    }

    protected override void Die()
    {
        base.Die();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerWeapon"))
        {
            TakeDamage();
            if (transform.position.x - collision.transform.position.x < 0)
            {
                _rb2d.velocity = new Vector2(-1 * _knockBackForce, 0);
            }
            else if (transform.position.x - collision.transform.position.x > 0)
            {
                _rb2d.velocity = new Vector2(1 * _knockBackForce, 0);
            }
        }
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

    protected override Collider2D InRangeAttack()
    {
        Collider2D attackingTarget = Physics2D.OverlapCircle(new Vector2(transform.position.x + _targetDetectRaycastXOffset, transform.position.y + _targetDetectRaycastYOffset), _targetAttackCircleCastRadius, _targetLayer);
        return attackingTarget;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        // wall detector
        Gizmos.DrawLine(new Vector2(transform.position.x, transform.position.y + _wallsRaycastOffset), new Vector2(transform.position.x, transform.position.y + _wallsRaycastOffset) + Vector2.right * _wallsRaycastLength);
        Gizmos.DrawLine(new Vector2(transform.position.x, transform.position.y + _wallsRaycastOffset), new Vector2(transform.position.x, transform.position.y + _wallsRaycastOffset) + Vector2.left * _wallsRaycastLength);

        // ledge detector
        Gizmos.DrawLine(new Vector2(transform.position.x + _ledgesRaycastOffset, transform.position.y), new Vector2(transform.position.x + _ledgesRaycastOffset, transform.position.y) + Vector2.down * _ledgesRaycastLength);
        Gizmos.DrawLine(new Vector2(transform.position.x - _ledgesRaycastOffset, transform.position.y), new Vector2(transform.position.x - _ledgesRaycastOffset, transform.position.y) + Vector2.down * _ledgesRaycastLength);

        //Gizmos.DrawWireSphere(new Vector2(transform.position.x, transform.position.y + _targetDetectRaycastOffset), _targetDetectCircleCastRadius);

        Gizmos.DrawWireSphere(new Vector2(transform.position.x + _targetDetectRaycastXOffset, transform.position.y + _targetDetectRaycastYOffset), _targetAttackCircleCastRadius);
    }
}
