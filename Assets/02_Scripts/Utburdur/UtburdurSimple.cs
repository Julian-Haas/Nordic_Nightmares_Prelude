using UnityEngine;

public class UtburdurSimple : MonoBehaviour
{
    public float FlyingSpeed = 3.0f, Distance = 0.0f, DissolveDistance = 50.0f;
    public bool IsFlying = false;
    private Vector3 _direction;
    private Animator _animator;
    private SoundManager _soundManager;
    public float _idleTimer;
    GameObject _player;


    void Start()
    {
        DissolveDistance = 50.0f;
        _animator = GetComponentInChildren<Animator>();
        _animator.SetBool("IsFlying", false);
        _soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        _soundManager.PlaySound3D("event:/SFX/UtburdurIdle", transform.position);
    }

void Update()
    {
        _idleTimer += Time.deltaTime;
        if (!IsFlying && _idleTimer >= 120.0f)
        {
            _soundManager.PlaySound3D("event:/SFX/UtburdurIdle", transform.position);
            _idleTimer = 0.0f;
        }
        else if (IsFlying)
        {
            FlyAway();
            if (Distance > DissolveDistance)
            {
                //Debug.Log("Distance: " + Distance);
                //Debug.Log("DissolveDistance: " + DissolveDistance);
                Destroy(gameObject);
            }
        }
    }

    void FlyAway()
    {
        transform.position += _direction * FlyingSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(_direction), Time.deltaTime * FlyingSpeed);
        Distance = Vector3.Distance(_player.transform.position, transform.position);
        _animator.SetBool("IsFlying", true);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            _player = other.gameObject;
            _direction = -(_player.transform.position - transform.position).normalized;
            _direction.y = 0.5f;
            IsFlying = true;
            _soundManager.PlaySound3D("event:/SFX/UtburdurScreech", transform.position);
        }
    }
}