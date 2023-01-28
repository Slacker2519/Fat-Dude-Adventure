using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAfterImages : MonoBehaviour
{
    [SerializeField] float _destroyTime;

    private void OnEnable()
    {
        StartCoroutine(SelfDestroy());
    }

    IEnumerator SelfDestroy()
    {
        yield return new WaitForSeconds(_destroyTime);
        Destroy(gameObject);
    }
}
