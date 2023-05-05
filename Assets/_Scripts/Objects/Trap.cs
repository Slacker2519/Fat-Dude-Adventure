using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum Type
{
    MovingTrap,
    NonMovingTrap
}

public class Trap : MonoBehaviour
{
    [SerializeField] float _duration;
    [SerializeField] float _distance;
    [SerializeField] Type TrapType;

    // Start is called before the first frame update
    void Start()
    {
        if (TrapType.Equals(Type.NonMovingTrap))
        {
            transform.DORotate(new Vector3(0, 0, 360), _duration * .5f, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);
        }
        else
        {
            transform.DORotate(new Vector3(0, 0, 360), _duration * .5f, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);
            transform.DOLocalMoveY(_distance, _duration).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
        }
    }
}
