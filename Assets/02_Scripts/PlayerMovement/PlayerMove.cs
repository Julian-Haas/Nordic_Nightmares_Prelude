using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour, PlayerInput.IPlayerMoveActions
{

    private PlayerInput _playerInput;

    private Vector3 _direction;

    [SerializeField] private float _MaximumMoveSpeed = 5;
    [SerializeField] private float _moveAcceleration = 1200;
    [SerializeField] private float _isometricCorrectionAngle;
    [SerializeField] private float _SneakingSpeedModifier = 0.5f;
    [SerializeField] public bool _isSneaking = false;
    [SerializeField] private float _jumpForce;
    private Inventory _inventory;
    private Rigidbody _body;
    private bool _isGround;
    //Vector3 _positionToDropItem;
    private GameObject _player;
    private s_PlayerCollider _playerCollider;
    [SerializeField]
    private Naddagil _naddagil; 

    void Start() {
        //_positionToDropItem = GameObject.Find("Player").transform.Find("DropPosition").transform.position;
        _player = GameObject.Find("PlayerAnimated");
        _playerCollider = _player.GetComponent<s_PlayerCollider>();
        if(_playerInput == null) {
            _playerInput = new PlayerInput();
            _playerInput.PlayerMove.Enable();
            _playerInput.PlayerMove.SetCallbacks(this);
        }
        _body = GetComponent<Rigidbody>();
        _isGround = true;
        _inventory = GameObject.Find("Inventory").GetComponentInChildren<Inventory>();
    }

    void Update() {
        Quaternion moveRot = Quaternion.Euler(new Vector3(0,_isometricCorrectionAngle,0));
        Vector3 movement = moveRot * _direction;
        //transform.position = transform.position + movement.normalized * _moveSpeed * Time.deltaTime;
        _body.AddForce(movement.normalized * _moveAcceleration * Time.deltaTime);
        _body.velocity = _body.velocity.normalized * Mathf.Clamp(_body.velocity.magnitude,0,_MaximumMoveSpeed);
    }
    public bool IsGround {
        get => _isGround; set => _isGround = value;
    }

    public void OnDropObject(UnityEngine.InputSystem.InputAction.CallbackContext context) {
        //if (context.started)
        //{
        //    _inventory.TryToDropItem(_player.transform.Find("DropPosition").transform.position);
        //}
        //throw new System.NotImplementedException();
    }

    public void OnInteract(UnityEngine.InputSystem.InputAction.CallbackContext context) {
        if(context.started) {
            InteractableManager.Instance.Interact();
        }
        //if (context.canceled)
        //{
        //    _player_Test.interact(false);
        //}
        //throw new System.NotImplementedException();
    }

    public void OnJump(UnityEngine.InputSystem.InputAction.CallbackContext context) {
    }

    public void OnMove(UnityEngine.InputSystem.InputAction.CallbackContext context) {
        Vector2 readValue = context.ReadValue<Vector2>();
        _direction = new Vector3(readValue.x,0,readValue.y);
    }

    public void OnSneaking(UnityEngine.InputSystem.InputAction.CallbackContext context) {
        if(context.started) {
            _isSneaking = true;
            _MaximumMoveSpeed *= _SneakingSpeedModifier;
            _naddagil.NaddiEye.PlayerIsSneaking = true; 
        }
        if(context.canceled) {
            _isSneaking = false;
            _MaximumMoveSpeed /= _SneakingSpeedModifier;
            _naddagil.NaddiEye.PlayerIsSneaking = false;
        }
    }

    public void OnThrowObject(UnityEngine.InputSystem.InputAction.CallbackContext context) {
        //if (context.started)
        //{
        //_inventory.TryToThrowStone();
        //}
        //throw new System.NotImplementedException();
    }

    void PlayerInput.IPlayerMoveActions.OnJuliansTestFunktion(UnityEngine.InputSystem.InputAction.CallbackContext context) {
        //_playerCollider.
        throw new System.NotImplementedException();
    }

    public void JuliansTestFunktion() {
        Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }
}