using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    static readonly int Idle = Animator.StringToHash("Idle");
    static readonly int Run = Animator.StringToHash("Run");
    static readonly int IdleTransition = Animator.StringToHash("IdleTransition");
    static readonly int Jump = Animator.StringToHash("Jump");
    static readonly int Fall = Animator.StringToHash("JumpFall");
    static readonly int WallSlide = Animator.StringToHash("WallSlide");
    static readonly int WallJump = Animator.StringToHash("WallJump");
    static readonly int Dash = Animator.StringToHash("Dash");
    
    Animator _anim;
    PlayerController2_0 _playerController;
    Rigidbody2D _rb;
    [SerializeField] float _wallJumpAnimDuration;
    [SerializeField] float _dashAnimDuration;
    float _lockedTill;
    int _currentState;
    bool _onAir;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _playerController = GetComponent<PlayerController2_0>();
        _anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        _onAir = _rb.velocity.y != 0 && !_playerController.Grounded ? true : false;

        int state = GetState();
        if (state == _currentState) return;
        _anim.CrossFade(state, 0, 0);
        _currentState = state;
    }

    int GetState()
    {
        if (Time.time < _lockedTill) return _currentState;

        if (_playerController.IsDashing) return LockState(Dash, _dashAnimDuration);
        if (_playerController.WallJump) return LockState(WallJump, _wallJumpAnimDuration);
        if (_playerController.WallSliding) return WallSlide;
        if (_playerController.Jumping) return Jump;
        if (_playerController.Falling) return Fall;
        if (_playerController.HorizontalDirection == 0f && _playerController.Rb.velocity.x != 0 && _playerController.Grounded) return IdleTransition;
        if (_playerController.Rb.velocity.y == 0f && _playerController.Grounded) return _playerController.HorizontalDirection == 0f ? Idle : Run;
        return 0;

        int LockState(int s, float t)
        {
            _lockedTill = Time.time + t;
            return s;
        }
    }
}
