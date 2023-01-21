using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerFall : MonoBehaviour
{
    PlayerMovementManager _playerController;

    void Start()
    {
        _playerController = GetComponent<PlayerMovementManager>();
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
