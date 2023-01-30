using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundSlam : MonoBehaviour
{
    PlayerMovementManager _playerController;

    // Start is called before the first frame update
    void Start()
    {
        _playerController = GetComponent<PlayerMovementManager>();
    }

    public void SlamGround(float groundSlamGravity)
    {
        _playerController.Rb.gravityScale = groundSlamGravity;
    }
}
