using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectTarget : MonoBehaviour
{
    [SerializeField] CloseRangeEnemy _closeRangeEnemy;

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _closeRangeEnemy.Attacking = false;
        }
    }
}
