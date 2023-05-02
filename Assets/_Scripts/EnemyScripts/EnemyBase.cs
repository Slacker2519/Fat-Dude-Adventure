using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    protected bool facingRight = false;
    public float Health;
    public float CurrentHealth;

    // Start is called before the first frame update
    public virtual void Start()
    {
        
    }

    // Update is called once per frame
    public virtual void Update()
    {
        
    }

    public void GraphicFlip() 
    {
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);
    }

    protected virtual Collider2D InRangeAttack()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, 0);
        return hit;
    }

    protected virtual void Attack() { }

    protected virtual void TakeDamage() 
    { 
        
    }

    protected virtual void Die() { }
}
