using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerFall : MonoBehaviour
{
    PlayerController2_0 _playerController;

    void Start()
    {
        _playerController = GetComponent<PlayerController2_0>();
    }

    public void FallingPlayer(float fallMultiplier ,float maxGravity)
    {
        _playerController.Rb.gravityScale += fallMultiplier * Time.deltaTime;
        if (_playerController.Rb.gravityScale > maxGravity)
        {
            _playerController.Rb.gravityScale = maxGravity;
        }
    }
}
