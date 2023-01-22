using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallSlide : MonoBehaviour
{
    PlayerMovementManager _playerController;

    // Start is called before the first frame update
    void Start()
    {
        _playerController = GetComponent<PlayerMovementManager>();
    }

    public void WallSlide(float wallSlideGravity, float wallSLideDrag)
    {
        _playerController.Rb.gravityScale = wallSlideGravity;
        _playerController.Rb.drag = wallSLideDrag;
    }
}
