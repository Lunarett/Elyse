
using System;
using Pulsar.Debug;
using UnityEngine;
using Pulsar.Utils;
using UnityEngine.Animations.Rigging;

public enum EViewMode
{
    FPS,
    TPS
}

public class ElyseCharacter : Character
{
    [SerializeField] private EViewMode _viewMode = EViewMode.FPS;

    [Header("TPS View Mode Properties")]
    [SerializeField] private float _tpsArmLength = 3.0f;
    [SerializeField] private Vector3 _tpsOffset;

    [Header("Character References")]
    [SerializeField] private GameObject _fpsView;
    [SerializeField] private GameObject _tpsView;
    [SerializeField] private Camera _weaponCamera;

    [Header("Animation Properties")]
    [SerializeField] private Animator _tpsAnimator;
    [SerializeField] private Rig _rig;
    
    private PlayerInputManager _playerInputManager;
    private Player _player;
    private WeaponAnimation _weaponAnimation;
    private ElyseController _elyseController;
    private HUD _hud;
    private bool _isPaused;
    private bool _isViewFPS;
    private bool _isDead;

    public bool IsDead { get; set; }
    public PlayerInputManager PlayerInputManager => _playerInputManager;
    public EViewMode ViewMode => _viewMode;
    public WeaponAnimation WeaponAnim => _weaponAnimation;
    public ElyseController ElyseElyseController => _elyseController;

    public event Action<EViewMode> OnViewChanged;

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
        _player = GetComponent<Player>();

        _player.OnPlayerDied += OnPlayerDied;
        
        // Set to Local player
        Utils.SetLayerRecursively(gameObject, LayerMask.NameToLayer("Local Player"));
        SetViewMode(_viewMode);

        _hud.OnGameResume += UnpauseGame;

        _isViewFPS = _viewMode == EViewMode.FPS;
        _rig.weight = 1;
    }

    private void Start()
    {
        if (!DebugUtils.CheckForNull<PawnCameraController>(_pawnCameraController, "ElyseCharacter: CameraController is null!"))
        {
            _weaponCamera.transform.position = _pawnCameraController.SpringArm.transform.position;
        }
        
        if (!DebugUtils.CheckForNull<Controller>(Owner, "ElyseCharacter: Owner is null!"))
        {
            Debug.Log("Attempting to cast Owner to ElyseController...");
            _elyseController = Owner as ElyseController;
            DebugUtils.CheckForNull<ElyseController>(_elyseController, "ElyseCharacter: Failed to cast controller as ElyseController");
        }
        
        _hud.SetDamageScreenAlpha(0);
        ShowMouseCursor(false);
    }

    private void Update()
    {
        if (_playerInputManager.GetPauseInputDown())
        {
            Debug.Log("Pause input fired!");
            _isPaused = !_isPaused;
            if (_isPaused)
            {
                Debug.Log("Game Paused!");
                PauseGame();
            }
            else
            {
                Debug.Log("Game Unpaused!");
                UnpauseGame();
            }
        }

        if (_playerInputManager.GetViewInputDown())
        {
            Debug.Log("View input fired!");
            _isViewFPS = !_isViewFPS;
            EViewMode targetViewMode = _isViewFPS ? EViewMode.FPS : EViewMode.TPS;
            SetViewMode(targetViewMode);
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

    public void SetViewMode(EViewMode viewMode)
    {
        if (DebugUtils.CheckForNull<PawnCameraController>(_pawnCameraController,
                "ElyseCharacter: PlayerCameraController is null")) return;
        
        
        switch (viewMode)
        {
            case EViewMode.FPS:
                
                _fpsView.SetActive(true);
                _pawnCameraController.SpringArm.SetArmLength(0);
                _pawnCameraController.SpringArm.SetCameraOffset(Vector3.zero);
                OnViewChanged?.Invoke(EViewMode.FPS);
                Utils.SetLayerRecursively(_tpsView, LayerMask.NameToLayer("Ignore Render"));
                break;
            case EViewMode.TPS:
                _fpsView.SetActive(false);
                _pawnCameraController.SpringArm.SetArmLength(_tpsArmLength);
                _pawnCameraController.SpringArm.SetCameraOffset(_tpsOffset);
                OnViewChanged?.Invoke(EViewMode.TPS);
                Utils.SetLayerRecursively(_tpsView, LayerMask.NameToLayer("Local Player"));
                break;
            default:
                return;
        }
    }

    private void OnPlayerDied()
    {
        _isDead = true;
        _rig.weight = 0;
        EnableInput(false, false);
        SetViewMode(EViewMode.TPS);
        _tpsAnimator.SetBool(Dead, true);
        _elyseController.Die();
    }

    private void UnpauseGame()
    {
        _isPaused = false;
        _hud.DisplayPauseMenu(false);

        if (!_isDead)
        {
            EnableInput(true, false);    
        }
        
        ShowMouseCursor(false);
    }

    private void PauseGame()
    {
        _isPaused = true;
        _hud.DisplayPauseMenu();
        EnableInput(false, false);
        ShowMouseCursor(true);
    }
    
    private void OnDestroy()
    {
        if (_player != null)
        {
            _player.OnPlayerDied -= OnPlayerDied;
        }
    }
}