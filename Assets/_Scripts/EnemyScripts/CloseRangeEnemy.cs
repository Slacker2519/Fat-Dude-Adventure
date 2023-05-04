using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseRangeEnemy : EnemyBase
{
    [Header("Components")]
    [SerializeField] Rigidbody2D _rb2D;
    public Animator Anim;

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
    [SerializeField] float _attackRange;
    int _attackVelocity = 0;
    public bool Attacking;

    [Header("TakeDamageVariables")]
    public float KnockBackAngle;
    public long KnockBackForce;
    public float RecoveringTime;
    bool _takingDamage;
    public bool IsDead = false;

    public override void Start()
    {
        base.Start();
        _rb2D = GetComponent<Rigidbody2D>();
        CurrentHealth = Health;
    }

    public override void Update()
    {
        base.Update();
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
        //if (InRangeAttack())
        //{
        //    if (transform.position.x - InRangeAttack().transform.position.x < 0 && !facingRight)
        //    {
        //        GraphicFlip();
        //    }

        //    else if (transform.position.x - InRangeAttack().transform.position.x > 0 && facingRight)
        //    {
        //        GraphicFlip();
        //    }
        //}

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
        if (CurrentHealth == 0)
        {
            _characterVelocity = 0;
        }
        else if (Attacking == false && CurrentHealth > 0)
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

        if (_takingDamage) return;
        Anim.SetFloat("EnemySpeed", _characterVelocity);
        Anim.SetBool("EnemyAttack", Attacking);
    }

    protected override void Attack()
    {
        base.Attack();
    }

    protected override void TakeDamage()
    {
        base.TakeDamage();
    }

    public override IEnumerator Die()
    {
        if (CurrentHealth <= 0)
        {
            
            yield return new WaitForSeconds(2);
            Destroy(gameObject);
        }
    }

    public void KnockBack(float knockBackAngle, long knockBackForce)
    {
        float xAngle = Mathf.Cos(knockBackAngle * Mathf.Deg2Rad);
        float yAngle = Mathf.Sin(knockBackAngle * Mathf.Deg2Rad);
        _rb2D.velocity = Vector2.zero;
        _rb2D.AddForce(new Vector2(xAngle, yAngle).normalized * knockBackForce * 100f);
    }

    //protected override Collider2D InRangeAttack()
    //{
    //    Collider2D attackingTarget = Physics2D.OverlapCircle(new Vector2(transform.position.x, transform.position.y + _targetDetectRaycastYOffset), _targetAttackCircleCastRadius, _targetLayer);
    //    return attackingTarget;
    //}

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

    public IEnumerator RecoverTime()
    {
        _takingDamage = true;
        yield return new WaitForSeconds(RecoveringTime);
        _takingDamage = false;
    }

    private void AttackTarget(Collider2D collision)
    {
        Attacking = true;

        if (transform.position.x - collision.transform.position.x < 0 && !facingRight)
        {
            GraphicFlip();
        }
        else if (transform.position.x - collision.transform.position.x > 0 && facingRight)
        {
            GraphicFlip();
        }
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
        //Gizmos.DrawWireSphere(new Vector2(transform.position.x, transform.position.y + _targetDetectRaycastYOffset), _targetAttackCircleCastRadius);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (Vector2.Distance(collision.transform.position, transform.position) > _attackRange) return;

            AttackTarget(collision);
        }
    }
}
