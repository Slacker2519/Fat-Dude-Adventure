using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallJump : MonoBehaviour
{
    PlayerController2_0 _playerController;

    // Start is called before the first frame update
    void Start()
    {
        _playerController = GetComponent<PlayerController2_0>();
    }

    public void JumpToTheRight(float wallJumpAngle, long wallJumpForce)
    {
        float xAngle = Mathf.Cos(wallJumpAngle * Mathf.Deg2Rad);
        float yAngle = Mathf.Sin(wallJumpAngle * Mathf.Deg2Rad);
        _playerController.Rb.velocity = Vector2.zero;
        _playerController.Rb.AddForce(new Vector2(xAngle, yAngle).normalized * wallJumpForce * 100f);
    }

    public void JumpToTheLeft(float wallJumpAngle, long wallJumpForce)
    {
        float xAngle = Mathf.Cos(wallJumpAngle * Mathf.Deg2Rad);
        float yAngle = Mathf.Sin(wallJumpAngle * Mathf.Deg2Rad);
        _playerController.Rb.velocity = Vector2.zero;
        _playerController.Rb.AddForce(new Vector2(xAngle * -1f, yAngle).normalized * wallJumpForce * 100f);
    }
}
