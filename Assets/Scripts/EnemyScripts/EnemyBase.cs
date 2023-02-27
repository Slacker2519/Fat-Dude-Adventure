using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected virtual void GraphicFlip() { }

    protected virtual void TargetDectection() { }

    protected virtual void Attack() { }

    protected virtual void TakeDamage() { }

    protected virtual void Die() { }
}
