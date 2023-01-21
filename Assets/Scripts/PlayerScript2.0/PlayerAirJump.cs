using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAirJump : MonoBehaviour
{
    PlayerMovementManager _playerController;

    void Start()
    {
        _playerController = GetComponent<PlayerMovementManager>();
    }

    void AirJumping()
    {

    }
}
