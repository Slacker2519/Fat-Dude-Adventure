using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRun : MonoBehaviour
{
    PlayerController2_0 _playerController;

    private void Start()
    {
        _playerController = GetComponent<PlayerController2_0>();
    }

    public void MovePlayer(float playerAcceleration, float maxMoveSpeed, ref float horizontalDirection)
    {
        horizontalDirection = Input.GetAxisRaw("Horizontal");

        _playerController.Rb.AddForce(new Vector2(horizontalDirection, 0f) * playerAcceleration);
        if (Mathf.Abs(_playerController.Rb.velocity.x) > maxMoveSpeed)
        {
            _playerController.Rb.velocity = new Vector2(Mathf.Sign(_playerController.Rb.velocity.x) * maxMoveSpeed, _playerController.Rb.velocity.y);
        }
    }
}
