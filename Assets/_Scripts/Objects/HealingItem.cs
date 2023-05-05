using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingItem : MonoBehaviour
{
    [SerializeField] float _distance;
    [SerializeField] float _duration;

    // Start is called before the first frame update
    void Start()
    {
        transform.DOLocalMoveY(_distance, _duration).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
