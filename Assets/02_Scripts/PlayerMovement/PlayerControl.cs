using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerControl : MonoBehaviour, PlayerInput.IPlayerMoveActions
{
    [SerializeField] CapsuleCollider _collider;
    [SerializeField] private Rigidbody _rigidBody;
    [SerializeField] private float _speed = 5;
    [SerializeField] private float _turnSpeed = 7.5f;
    [SerializeField] private float _SneakingSpeedModifier = 0.5f;
    [SerializeField] private float NoiseRange = 5.0f;
    [SerializeField] private bool _isSneaking = false, _isInteracting = false, _timeSinceRespawnIsUp = true;
    [SerializeField] private s_PlayerCollider _playerCollider;
    [SerializeField] private Naddagil _naddagil;
    private PlayerInput _playerInput;
    private Vector3 _input, _direction;
    private float _maxSpeed, _respawnInputBlockingTimer = 0.0f;
    [SerializeField] Animator _animator, _animatorHidden;

    private Vector3 lastPosition;
    private float lastTime;
    [SerializeField] float CurrentSpeed;
    [SerializeField] bool Moving = false;

    private s_SoundManager _soundManager;
    private bool _canPlayFootstep = true;

    private void Start()
    {
        _soundManager = GameObject.Find("SoundManager").GetComponent<s_SoundManager>();
        _playerCollider = GetComponent< s_PlayerCollider>();
        if (_playerInput == null)
        {
            _playerInput = new PlayerInput();
            _playerInput.PlayerMove.Enable();
            _playerInput.PlayerMove.SetCallbacks(this);
        }
        _maxSpeed = _speed;
    }

    private void Update()
    {
        if (_timeSinceRespawnIsUp)
        {
            GatherInput();
        }
        _respawnInputBlockingTimer += Time.deltaTime;
        if(_respawnInputBlockingTimer >= 0.5f)
        {
            _timeSinceRespawnIsUp = true;
        }

        CurrentSpeed = ActualSpeed();
        Moving = IsMoving();
    }

    private void FixedUpdate()
    {
        Move();
        Look();
        PlayMovingOrIdleAnimation();
    }

    private void GatherInput()
    {
        _input = _direction.normalized;
    }

    public bool GetSneakingStatus() { return _isSneaking; }
    public float GetNoiseRange() { return NoiseRange; }

    public bool IsMoving()
    {
        if(CurrentSpeed >= 0.1f && !_playerCollider._inSafeZone)
        {
            return true;
        }
        return false;
    }

    private float ActualSpeed()
    {
        Vector3 displacement = transform.position - lastPosition;
        float deltaTime = Time.time - lastTime;
        Vector3 velocity = displacement / deltaTime;
        lastPosition = transform.position;
        lastTime = Time.time;

        return velocity.magnitude;
    }

    private void Look()
    {
        if (_input == Vector3.zero) return;

        Quaternion targetRotation = Quaternion.LookRotation(ToIso(_input), Vector3.up);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _turnSpeed * 100 * Time.deltaTime);

    }

    private void Move()
    {
        _rigidBody.MovePosition(transform.position + (transform.forward * _input.magnitude) * _speed * Time.deltaTime);
    }


    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 readValue = context.ReadValue<Vector2>();
        _direction = new Vector3(readValue.x, 0, readValue.y);
        if(_direction.magnitude >= 0.0f)
        {
            _isInteracting = false;
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _playerCollider.interact(true);
            _isInteracting = true;
            //PlayMovingOrIdleAnimation();
        }
        //if (context.canceled)
        //{
        //    _playercollider.interact(false);
        //    _isinteracting = false;
        //}
    }

    public void OnSneaking(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _speed *= _SneakingSpeedModifier;
            _isSneaking = true;
            _naddagil.NaddiEye.PlayerIsSneaking = true; 
        }
        if (context.canceled)
        {
            _speed /= _SneakingSpeedModifier;
            _isSneaking = false;
            _naddagil.NaddiEye.PlayerIsSneaking = false; 
        }
    }

    public void OnThrowObject(InputAction.CallbackContext context)
    {
        //if (context.started)
        //{
        //_inventory.TryToThrowStone();
        //}
        //throw new System.NotImplementedException();
    }

    public void OnJump(InputAction.CallbackContext context)
    {

    }

    public void OnDropObject(InputAction.CallbackContext context)
    {

    }

    public void PlayMovingOrIdleAnimation()
    {        Animator active;
        if (_playerCollider._inShadow)
        {
            active = _animatorHidden;
            _animatorHidden.gameObject.SetActive(true);
            _animator.gameObject.SetActive(false);
        }
        else
        {
            _animatorHidden.gameObject.SetActive(false);
            _animator.gameObject.SetActive(true);
            active = _animator;
        }

        float _tmp = (0.17f / _maxSpeed) * _speed * _input.magnitude;
        active.ResetTrigger("IsInteracting");
        active.ResetTrigger("IsMoving");
        active.ResetTrigger("IsIdling");


        if (_isInteracting)
        {
            AnimatorStateInfo animStateInfo = active.GetCurrentAnimatorStateInfo(0);
            float NTime = animStateInfo.normalizedTime;

            if (NTime > 1.0f) _isInteracting = false;
        }

        if ( !_isInteracting && _isSneaking && _tmp > 0.0f)
        {
            active.SetTrigger("IsMoving");
            active.SetFloat("Velocity", 0.0f);
        }
        else if (!_isInteracting && _isSneaking && _tmp == 0.0f)
        {
            active.SetTrigger("IsIdling");
            active.SetFloat("Velocity", 0.0f);
        }
        else if (!_isInteracting && !_isSneaking && _tmp > 0.0f)
        {
            active.SetTrigger("IsMoving");
            active.SetFloat("Velocity", 0.18f);

            if (_canPlayFootstep)
            {
                _soundManager.PlaySound3D("event:/SFX/Footstep", _rigidBody.transform.position);
                _canPlayFootstep = false;
                StartCoroutine(FootstepCooldown());
            }
        }
        else if (!_isInteracting && !_isSneaking && _tmp == 0.0f)
        {
            active.SetTrigger("IsIdling");
            active.SetFloat("Velocity", 0.18f);
        }


    }

    IEnumerator FootstepCooldown()
    {
        yield return new WaitForSeconds(0.32f);
        _canPlayFootstep = true;
    }

    public void PlayInteractAnimation()
    {
        _isInteracting = true;
        _animator.ResetTrigger("IsMoving");
        _animator.ResetTrigger("IsIdling");
        _animator.SetTrigger("IsInteracting");
    }

    public void PlayRespawnAnimation()
    {
        Debug.Log("Player respawn animation go!");
        _timeSinceRespawnIsUp = false;
        _respawnInputBlockingTimer = 0.0f;
        _animator.ResetTrigger("IsInteracting");
        _animator.ResetTrigger("IsMoving");
        _animator.ResetTrigger("IsIdling");
        _animator.SetTrigger("IsCharacterLose");
        _animatorHidden.ResetTrigger("IsInteracting");
        _animatorHidden.ResetTrigger("IsMoving");
        _animatorHidden.ResetTrigger("IsIdling");
        _animatorHidden.SetTrigger("IsCharacterLose");
    }

    private Vector3 ToIso(Vector3 input)
    {
        Matrix4x4 _isoMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, -135, 0));
        return _isoMatrix.MultiplyPoint3x4(input);
    }

    void PlayerInput.IPlayerMoveActions.OnJuliansTestFunktion(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            
            _playerCollider.JuliansTestFunktion();
        }
        //if (context.canceled)
        //{
        //    _speed /= _SneakingSpeedModifier;
        //    _isSneaking = false;
        //}

        //throw new System.NotImplementedException();
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.color = Color.cyan;
        Handles.DrawWireDisc(transform.position, Vector3.up, NoiseRange, 3.0f);
    }
#endif
}