using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    PlayerController2_0 _playerController;

    // Start is called before the first frame update
    void Start()
    {
        _playerController = GetComponent<PlayerController2_0>();
    }

    public void JumpPlayer(float jumpForce)
    {
        _playerController.Rb.velocity = new Vector2(_playerController.Rb.velocity.x, 0f);
        _playerController.Rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }
}
