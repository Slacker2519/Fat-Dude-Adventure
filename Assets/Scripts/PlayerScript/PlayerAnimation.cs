using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    PlayerStateMachine _player;
    Animator _anim;
    int _currentState;
    float _lockTill;

    static readonly int PlayerIdle = Animator.StringToHash("Idle");
    static readonly int PlayerRun = Animator.StringToHash("Run");
    static readonly int PlayerJump = Animator.StringToHash("Jump");
    static readonly int PlayerFall = Animator.StringToHash("JumpFall");
    static readonly int playerAirJump = Animator.StringToHash("WallJump");
    static readonly int playerStopping = Animator.StringToHash("IdleTransition");
    static readonly int playerWallSlide = Animator.StringToHash("WallSlide");

    // Start is called before the first frame update
    void Start()
    {
        _anim = GetComponent<Animator>();
        _player = GetComponent<PlayerStateMachine>();
    }

    // Update is called once per frame
    void Update()
    {
        DumbSolutionToFixAnimationBug();

        var state = GetState();

        if (state == _currentState) return;
        _anim.CrossFade(state, 0, 0);
        _currentState = state;
    }

    int GetState()
    {
        if (Time.time < _lockTill) return _currentState;
        
        if (_player.HorizontalDirection != 0f && _player.Grounded) return PlayerRun;
        if (_player.CanJump) return PlayerJump;
        if (!_player.Grounded || _player.Rb.velocity.y < -0.05f) return PlayerFall;
        else return PlayerIdle;

        int LockState(int s, float t)
        {
            _lockTill = t + Time.time;
            return s;
        }
    }

    void DumbSolutionToFixAnimationBug()
    {
        if (_player.HorizontalDirection == 0f && _player.Rb.velocity.x != 0f && _player.Grounded)
        {
            _anim.CrossFade(playerStopping, 0f);
        }
        else if (_player.HorizontalDirection == 0f && Mathf.Abs(_player.Rb.velocity.x) <= 0.5f && _player.Grounded)
        {
            _anim.CrossFade(PlayerIdle, 0f);
        }

        if (_player.CanWallSlide)
        {
            _anim.CrossFade(playerWallSlide, 0f);   
        }
        else if (!_player.Grounded && !_player.NextToWall)
        {
            _anim.CrossFade(PlayerFall, 0f);
        }
        else if (_player.CanAirJump)
        {
            _anim.CrossFade(playerAirJump, 0f);
        }
    }
}
