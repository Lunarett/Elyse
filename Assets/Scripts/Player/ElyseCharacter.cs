using Photon.Realtime;
using UnityEngine;
using Pulsar.Utils;

public class ElyseCharacter : Character
{
    [SerializeField] private EViewMode _viewMode = EViewMode.FPS;

    [Header("Character References")]
    [SerializeField] private GameObject _fpsView;
    [SerializeField] private GameObject _tpsView;

    [Header("Animation Properties")] [SerializeField]
    private Animator _tpsAnimator;
    
    private PlayerInputManager _inputManager;
    private PlayerHealth _playerHealth;
    private WeaponAnimation _weaponAnimation;
    private ElyseController _elyseController;

    public bool IsDead { get; set; }
    public PlayerInputManager InputManager => _inputManager;
    public EViewMode ViewMode => _viewMode;
    public WeaponAnimation WeaponAnim => _weaponAnimation;
    public ElyseController ElyseElyseController => _elyseController;

    private static readonly int Vertical = Animator.StringToHash("Vertical");
    private static readonly int Horizontal = Animator.StringToHash("Horizontal");
    private static readonly int Crouch = Animator.StringToHash("Crouch");
    private static readonly int Grounded = Animator.StringToHash("Grounded");
    private static readonly int Dead = Animator.StringToHash("Dead");

    protected override void Awake()
    {
        base.Awake();
        
        // Find all components in object
        _weaponAnimation = GetComponentInChildren<WeaponAnimation>();
        _inputManager = GetComponent<PlayerInputManager>();
        _playerHealth = GetComponent<PlayerHealth>();

        _playerHealth.OnPlayerDied += OnPlayerDied;
        
        // Cast to elyse controller
        _elyseController = (ElyseController) PlayerController;
        Utils.CheckForNull<ElyseController>(_elyseController);
        
        if (_photonView.IsMine)
        {
            Utils.SetLayerRecursively(gameObject, LayerMask.NameToLayer("Local Player"));
            SetViewMode(EViewMode.FPS);
        }
        else
        {
            Utils.SetLayerRecursively(gameObject, LayerMask.NameToLayer("Remote Player"));
        }
    }

    private void Start()
    {
        ShowMouseCursor(false);
        EnableMovement(_photonView.IsMine);
        EnableView(_photonView.IsMine);
    }

    private void LateUpdate()
    {
        UpdateCharacterAnimation();
    }

    private void UpdateCharacterAnimation()
    {
        if (IsDead) return; // Don't update if we are dead
        
        Vector3 velocity = _playerMovement.CharacterVelocity;
        float verticalValue = _inputManager.GetMoveInput(true).y != 0
            ? Vector3.Dot(velocity.normalized, transform.forward)
            : 0;
        float horizontalValue = _inputManager.GetMoveInput(true).x != 0
            ? Vector3.Dot(velocity.normalized, transform.right)
            : 0;

        _tpsAnimator.SetFloat(Vertical, verticalValue, 0.1f, Time.deltaTime);
        _tpsAnimator.SetFloat(Horizontal, horizontalValue, 0.1f, Time.deltaTime);
        _tpsAnimator.SetBool(Grounded, _playerMovement.IsGrounded);
        _tpsAnimator.SetBool(Crouch, _inputManager.GetCrouchInputHeld());
    }

    public void SetViewMode(EViewMode viewMode)
    {
        _playerView.SetCameraViewMode(viewMode);

        switch (viewMode)
        {
            case EViewMode.FPS:
                _fpsView.SetActive(true);
                Utils.SetLayerRecursively(_tpsView, LayerMask.NameToLayer("Ignore Render"));
                break;
            case EViewMode.TPS:
                _fpsView.SetActive(false);
                Utils.SetLayerRecursively(_tpsView, LayerMask.NameToLayer("Local Player"));
                break;
            default:
                return;
        }
    }

    private void OnPlayerDied(Player causerPlayer)
    {
        if (!_photonView.IsMine) return;
        EnableMovement(false);
        SetViewMode(EViewMode.TPS);
        _tpsAnimator.SetBool(Dead, true);
        _elyseController.Die();
    }
}