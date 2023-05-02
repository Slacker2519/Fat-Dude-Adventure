using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseRangeEnemy : EnemyBase
{
    [Header("Components")]
    [SerializeField] Rigidbody2D _rb2D;
    [SerializeField] Animator _animator;

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
    [SerializeField] float _patrolVelocity;
    float _characterVelocity;
    int _enemyHorizontalDirection = -1;

    [Header("AttackPlayer")]
    [SerializeField] LayerMask _targetLayer;
    [SerializeField] float _targetAttackCircleCastRadius;
    [SerializeField] float _targetDetectRaycastYOffset;
    int _attackVelocity = 0;

    [Header("TakeDamageVariables")]
    [SerializeField] float _knockBackAngle;
    [SerializeField] long _knockBackForce;
    [SerializeField] float _recoveringTime;
    bool _takingDamage;

    public override void Start()
    {
        base.Start();
        _rb2D = GetComponent<Rigidbody2D>();
    }

    public override void Update()
    {
        base.Update();
        InRangeAttack();
        CalculateFLipCharacter();
    }

    void FixedUpdate()
    {
        WallsDetection(_wallsRaycastOffset, _wallsRaycastLength, _wallsLayer);
        LedgesDetection(_ledgesRaycastLength, _ledgesRaycastLength, _groundLayer);

        if (_takingDamage) return;
        CharacterState();
    }

    private void CalculateFLipCharacter()
    {
        if (InRangeAttack())
        {
            if (transform.position.x - InRangeAttack().transform.position.x < 0 && !facingRight)
            {
                GraphicFlip();
            }

            else if (transform.position.x - InRangeAttack().transform.position.x > 0 && facingRight)
            {
                GraphicFlip();
            }
        }

        if (_rb2D.velocity.x < 0 && facingRight)
        {
            GraphicFlip();
        }
        else if (_rb2D.velocity.x > 0 && !facingRight)
        {
            GraphicFlip();
        }
    }

    void CharacterState()
    {
        if (!InRangeAttack())
        {
            _characterVelocity = _patrolVelocity;

            if (_wallOnRight || !_noLeftLedge)
            {
                _enemyHorizontalDirection = -1;
            }
            else if (_wallOnLeft || !_noRightLedge)
            {
                _enemyHorizontalDirection = 1;
            }
        }
        else
        {
            _characterVelocity = _attackVelocity;
        }

        _rb2D.velocity = new Vector2(_characterVelocity * _enemyHorizontalDirection, _rb2D.velocity.y);
        _animator.SetFloat("EnemySpeed", _characterVelocity);
        _animator.SetBool("EnemyAttack", InRangeAttack());
    }

    protected override void Attack()
    {
        base.Attack();
    }

    protected override void Die()
    {
        base.Die();
    }

    protected override void TakeDamage()
    {
        base.TakeDamage();
    }

    public void KnockBack(float knockBackAngle, long knockBackForce)
    {
        float xAngle = Mathf.Cos(knockBackAngle * Mathf.Deg2Rad);
        float yAngle = Mathf.Sin(knockBackAngle * Mathf.Deg2Rad);
        _rb2D.velocity = Vector2.zero;
        _rb2D.AddForce(new Vector2(xAngle, yAngle).normalized * knockBackForce * 100f);
    }

    public void JumpToTheLeft(float wallJumpAngle, long wallJumpForce)
    {
        float xAngle = Mathf.Cos(wallJumpAngle * Mathf.Deg2Rad);
        float yAngle = Mathf.Sin(wallJumpAngle * Mathf.Deg2Rad);
        _rb2D.velocity = Vector2.zero;
        _rb2D.AddForce(new Vector2(xAngle * -1f, yAngle).normalized * wallJumpForce * 100f);
    }

    protected override Collider2D InRangeAttack()
    {
        Collider2D attackingTarget = Physics2D.OverlapCircle(new Vector2(transform.position.x, transform.position.y + _targetDetectRaycastYOffset), _targetAttackCircleCastRadius, _targetLayer);
        return attackingTarget;
    }

    void WallsDetection(float wallsRaycastOffset, float wallsRaycastLength, LayerMask wallsLayer)
    {
        _wallOnRight = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + wallsRaycastOffset), Vector2.right, wallsRaycastLength, wallsLayer);
        _wallOnLeft = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + wallsRaycastOffset), Vector2.left, wallsRaycastLength, wallsLayer);
    }

    void LedgesDetection(float ledgesRaycastLength, float ledgesRaycastOffset, LayerMask groundLayer)
    {
        _noLeftLedge = Physics2D.Raycast(new Vector2(transform.position.x + ledgesRaycastOffset, transform.position.y), Vector2.down, ledgesRaycastLength, groundLayer);
        _noRightLedge = Physics2D.Raycast(new Vector2(transform.position.x - ledgesRaycastOffset, transform.position.y), Vector2.down, ledgesRaycastLength, groundLayer);
    }

    IEnumerator RecoverTime()
    {
        _takingDamage = true;
        yield return new WaitForSeconds(_recoveringTime);
        _takingDamage = false;
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

        // player detector
        Gizmos.DrawWireSphere(new Vector2(transform.position.x, transform.position.y + _targetDetectRaycastYOffset), _targetAttackCircleCastRadius);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!InRangeAttack()) return;

        if (collision.tag.Equals("PlayerWeapon"))
        {
            StartCoroutine(RecoverTime());

            if (transform.position.x - InRangeAttack().transform.position.x < 0)
            {
                KnockBack(90 + _knockBackAngle, _knockBackForce);
            }
            else if (transform.position.x - InRangeAttack().transform.position.x > 0)
            {
                KnockBack(_knockBackAngle, _knockBackForce);
            }
        }
    }
}
