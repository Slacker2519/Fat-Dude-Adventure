using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAirJump : MonoBehaviour
{
    PlayerController2_0 _playerController;

    void Start()
    {
        _playerController = GetComponent<PlayerController2_0>();
    }

    public void AirJumping(ref int airJumpValue, float jumpForce)
    {
        airJumpValue--;
        _playerController.Rb.velocity = new Vector2(_playerController.Rb.velocity.x, 0f);
        _playerController.Rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }
}
