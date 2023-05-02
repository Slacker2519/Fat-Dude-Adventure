using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerWallSlide : MonoBehaviour
{
    PlayerController2_0 _playerController;

    // Start is called before the first frame update
    void Start()
    {
        _playerController = GetComponent<PlayerController2_0>();
    }

    public void WallSlide(float wallSlideVelocity)
    {
        _playerController.Rb.velocity = new Vector2(_playerController.Rb.velocity.x, Mathf.Clamp(_playerController.Rb.velocity.y, -wallSlideVelocity, float.MaxValue));
    }
}
