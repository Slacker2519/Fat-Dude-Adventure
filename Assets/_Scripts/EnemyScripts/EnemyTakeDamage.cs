using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTakeDamage : MonoBehaviour
{
    [SerializeField] CloseRangeEnemy _closeRangeEnemy;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("PlayerWeapon") && collision.gameObject.tag.Equals("Trap"))
        {
            _closeRangeEnemy.CurrentHealth--;
            _closeRangeEnemy.
            StartCoroutine(_closeRangeEnemy.RecoverTime());
            _closeRangeEnemy.Anim.SetFloat("EnemySpeed", 0);
            _closeRangeEnemy.Anim.SetBool("EnemyAttack", _closeRangeEnemy.Attacking);

            if (transform.position.x - collision.transform.parent.transform.position.x < 0)
            {
                _closeRangeEnemy.KnockBack(90 + _closeRangeEnemy.KnockBackAngle, _closeRangeEnemy.KnockBackForce);
            }
            else if (transform.position.x - collision.transform.parent.transform.position.x > 0)
            {
                _closeRangeEnemy.KnockBack(_closeRangeEnemy.KnockBackAngle, _closeRangeEnemy.KnockBackForce);
            }
            StartCoroutine(_closeRangeEnemy.Die());
        }
    }
}
