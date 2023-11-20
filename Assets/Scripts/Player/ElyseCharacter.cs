using Photon.Pun;
using Pulsar.Debug;
using UnityEngine;
using Pulsar.Utils;
using UnityEngine.Animations.Rigging;

public class ElyseCharacter : Character
{
    [SerializeField] private EViewMode _viewMode = EViewMode.FPS;

    [Header("Character References")]
    [SerializeField] private GameObject _fpsView;
    [SerializeField] private GameObject _tpsView;

    [Header("Animation Properties")]
    [SerializeField] private Animator _tpsAnimator;
    [SerializeField] private Rig _rig;
    
    private PlayerInputManager _playerInputManager;
    private PlayerHealth _playerHealth;
    private WeaponAnimation _weaponAnimation;
    private ElyseController _elyseController;
    private HUD _hud;
    private bool _isPaused;

    public bool IsDead { get; set; }
    public PlayerInputManager PlayerInputManager => _playerInputManager;
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
        
        _playerInputManager = _inputManager as PlayerInputManager;
        _hud = HUD.Instance;
        
        // Find all components in object
        _weaponAnimation = GetComponentInChildren<WeaponAnimation>();
        _playerHealth = GetComponent<PlayerHealth>();

        _playerHealth.OnPlayerDied += OnPlayerDied;
        
        // Cast to elyse controller
        _elyseController = (ElyseController) PlayerController;
        DebugUtils.CheckForNull<ElyseController>(_elyseController);
        
        if (_photonView.IsMine)
        {
            Utils.SetLayerRecursively(gameObject, LayerMask.NameToLayer("Local Player"));
            SetViewMode(EViewMode.FPS);
        }
        else
        {
            _fpsView.SetActive(false);
            Utils.SetLayerRecursively(gameObject, LayerMask.NameToLayer("Remote Player"));
        }

        _hud.OnGameResume += UnpauseGame;
    }

    private void Start()
    {
        _hud.SetDamageScreenAlpha(0);
        ShowMouseCursor(false);
        EnableView(_photonView.IsMine);
        _rig.weight = 1;
    }

    private void Update()
    {
        if (_playerInputManager.GetPauseInputDown())
        {
            PauseGame();
        }
    }

    private void LateUpdate()
    {
        UpdateCharacterAnimation();
    }

    private void UpdateCharacterAnimation()
    {
        if (IsDead) return;
        
        Vector3 velocity = _playerMovement.CharacterVelocity;
        float verticalValue = _playerInputManager.GetMoveInput().y != 0
            ? Vector3.Dot(velocity.normalized, transform.forward)
            : 0;
        float horizontalValue = _playerInputManager.GetMoveInput().x != 0
            ? Vector3.Dot(velocity.normalized, transform.right)
            : 0;

        _tpsAnimator.SetFloat(Vertical, verticalValue, 0.1f, Time.deltaTime);
        _tpsAnimator.SetFloat(Horizontal, horizontalValue, 0.1f, Time.deltaTime);
        _tpsAnimator.SetBool(Grounded, _playerMovement.IsGrounded);
        _tpsAnimator.SetBool(Crouch, _playerInputManager.GetCrouchInputHeld());
    }

    public void SetViewMode(EViewMode viewMode, bool setCamera = true)
    {
        if(setCamera) _playerView.SetCameraViewMode(viewMode);

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

    private void OnPlayerDied(DamageCauserInfo damageCauserInfo)
    {
        if (!_photonView.IsMine) return;
        _rig.weight = 0;
        EnableMovement(false, false);
        SetViewMode(EViewMode.TPS);
        _tpsAnimator.SetBool(Dead, true);
        _elyseController.Die();   
    }

    private void UnpauseGame()
    {
        EnableMovement(true, false);
        ShowMouseCursor(false);
    }

    private void PauseGame()
    {
        _hud.DisplayPauseMenu();
        EnableMovement(false, false);
        ShowMouseCursor(true);
    }
    
    private void OnDestroy()
    {
        if (_playerHealth != null)
        {
            _playerHealth.OnPlayerDied -= OnPlayerDied;
        }
    }
}