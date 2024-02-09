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
    private WeaponAnimation _weaponAnimation;
    private WeaponManager _weaponManager;
    private WeaponBase _currentActiveWeapon;
    private PlayerHealth _playerHealth;
    private ElyseController _elyseController;
    private HUD _hud;
    private bool _isPaused;
    private bool _isViewFPS;
    private bool _isDead;

    public bool IsDead { get; set; }
    public PlayerInputManager PlayerInputManager => _playerInputManager;
    public EViewMode ViewMode => _viewMode;
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
        _playerHealth = GetComponent<PlayerHealth>();
        _weaponManager = GetComponent<WeaponManager>();

        // Subscribe to events
        _weaponManager.OnWeaponAdded += OnWeaponAdded;
        _weaponManager.OnWeaponRemoved += OnWeaponRemoved;
        _weaponManager.OnWeaponSwitched += OnWeaponSwitched;
        _playerHealth.OnPlayerDied += OnPlayerHealthDied;
        _hud.OnGameResume += UnpauseGame;
        
        // Set to Local player
        Utils.SetLayerRecursively(gameObject, LayerMask.NameToLayer("Local Player"));
        SetViewMode(_viewMode);

        _isViewFPS = _viewMode == EViewMode.FPS;
        _rig.weight = 1;
    }

    protected override void Start()
    {
        base.Start();
        _weaponManager.AmmoManager.OnAmmoChanged += UpdateAllWeaponAmmoInHUD;
        
        if (!DebugUtils.CheckForNull<PawnCameraController>(_pawnCameraController, "ElyseCharacter: CameraController is null!"))
        {
            _weaponCamera.transform.position = _pawnCameraController.SpringArm.transform.position;
        }
        
        if (!DebugUtils.CheckForNull<Controller>(Owner, "ElyseCharacter: Owner is null!"))
        {
            _elyseController = Owner as ElyseController;
            DebugUtils.CheckForNull<ElyseController>(_elyseController, "ElyseCharacter: Failed to cast controller as ElyseController");
        }
        
        _hud.SetScreenEffectAlpha(0);
        ShowMouseCursor(false);
    }

    private void Update()
    {
        // Pause Game
        if (_playerInputManager.GetPauseInputDown())
        {
            Debug.Log("Pause input fired!");
            _isPaused = !_isPaused;
            if (_isPaused)
            {
                PauseGame();
            }
            else
            {
                UnpauseGame();
            }
        }

        // Toggle View Mode
        if (_playerInputManager.GetViewInputDown())
        {
            Debug.Log("View input fired!");
            _isViewFPS = !_isViewFPS;
            EViewMode targetViewMode = _isViewFPS ? EViewMode.FPS : EViewMode.TPS;
            SetViewMode(targetViewMode);
        }
        
        //Fire Weapon
        if (_weaponManager.ActiveWeapon != null)
        {
            switch (_weaponManager.ActiveWeapon.FireMode)
            {
                case FireMode.Single:
                case FireMode.Burst:
                    if (_playerInputManager.GetFireInputDown()) _weaponManager.ActiveWeapon.StartFire();
                    break;
                case FireMode.Automatic:
                    if (_playerInputManager.GetFireInputHeld()) _weaponManager.ActiveWeapon.StartFire();
                    break;
                default:
                    Debug.LogError("ElyseCharacter: Fire mode is not implemented");
                    break;
            }
        }
        
        // Reload Weapon
        if (_playerInputManager.GetReloadInputDown())
        {
            _weaponManager.ActiveWeapon.StartReload();
        }
        
        // Switch Weapon
        int switchDirection = _playerInputManager.GetSwitchWeaponInput();
        if (switchDirection != 0)
        {
            _weaponManager.SwitchWeapon(switchDirection);
        }
    }

    private void LateUpdate()
    {
        UpdateCharacterAnimation();
    }

    private void UpdateCharacterAnimation()
    {
        if (IsDead) return;
        
        // Update Third Person Mesh
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
        
        
        // Bob First Person
        _weaponAnimation.SetIsGrounded(_playerMovement.IsGrounded);
    }

    public void SetViewMode(EViewMode viewMode)
    {
        if (DebugUtils.CheckForNull<PawnCameraController>(_pawnCameraController,
                "ElyseCharacter: PlayerCameraController is null")) return;
        
        if (_weaponManager == null) { Debug.LogError("ElyseCharacter: WeaponManager is null");}
        else _weaponManager.SetViewMode(viewMode);
        
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

    private void OnPlayerHealthDied()
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
    
    private void OnWeaponAdded(WeaponBase weapon)
    {
        _hud.Inventory.AddElement(weapon.Name, weapon.Icon, weapon.CurrentAmmo, _weaponManager.AmmoManager.CheckAmmo(weapon.AmmoType));
        _hud.Inventory.SetActive(weapon.Name, false);
    }

    private void OnWeaponRemoved(string weaponID)
    {
        _hud.Inventory.RemoveElement(weaponID);
    }

    private void OnWeaponSwitched(WeaponBase weapon)
    {
        _weaponAnimation.PlaySwitchAnimation(_weaponManager.SwitchCooldown);
        
        if (_currentActiveWeapon != null)
        {
            _hud.Inventory.SetActive(_currentActiveWeapon.Name, false);
            
            // Unsubscribe from the old weapon's events
            _currentActiveWeapon.OnWeaponFire -= OnWeaponFired;
            _currentActiveWeapon.OnWeaponReloadStarted -= OnWeaponReloadStarted;
            _currentActiveWeapon.OnWeaponReloadEnded -= OnWeaponReloadEnded;
        }

        // Subscribe to the new weapon's events
        weapon.OnWeaponFire += OnWeaponFired;
        weapon.OnWeaponReloadStarted += OnWeaponReloadStarted;
        weapon.OnWeaponReloadEnded += OnWeaponReloadEnded;

        _currentActiveWeapon = weapon;

        _hud.Inventory.SetActive(weapon.Name, true);
        UpdateHUDAmmo(_currentActiveWeapon.CurrentAmmo, _weaponManager.AmmoManager.CheckAmmo(_currentActiveWeapon.AmmoType));
    }

    private void OnWeaponFired()
    {
        _weaponAnimation.FireRecoil(_weaponManager.ActiveWeapon.RecoilForce);
        UpdateHUDAmmo(_currentActiveWeapon.CurrentAmmo, _weaponManager.AmmoManager.CheckAmmo(_currentActiveWeapon.AmmoType));
    }

    private void OnWeaponReloadStarted()
    {
        StartCoroutine(_weaponAnimation.ReloadAnimation(_weaponManager.ActiveWeapon.ReloadDelay));
    }

    private void OnWeaponReloadEnded()
    {
        UpdateHUDAmmo(_currentActiveWeapon.CurrentAmmo, _weaponManager.AmmoManager.CheckAmmo(_currentActiveWeapon.AmmoType));
    }
    
    private void UpdateHUDAmmo(int currentAmmo, int maxAmmo)
    {
        _hud.Inventory.UpdateElementAmmo(_currentActiveWeapon.Name, currentAmmo, maxAmmo);
    }
    
    private void UpdateAllWeaponAmmoInHUD(AmmoType ammoType, int newAmmoCount)
    {
        foreach (var weaponId in _weaponManager.WeaponOrder)
        {
            if (!_weaponManager.Weapons.TryGetValue(weaponId, out WeaponBase weapon)) continue;
            // Only update the ammo display for weapons that use the type of ammo that was changed
            if (weapon.AmmoType != ammoType) continue;
            int currentAmmo = weapon.CurrentAmmo;
            // No need to use newAmmoCount directly here, but it's available if needed
            int maxAmmo = _weaponManager.AmmoManager.CheckAmmo(weapon.AmmoType);
            _hud.Inventory.UpdateElementAmmo(weapon.Name, currentAmmo, maxAmmo);
        }
    }

    
    private void OnDestroy()
    {
        if (_weaponManager != null)
        {
            _weaponManager.OnWeaponAdded -= OnWeaponAdded;
            _weaponManager.OnWeaponRemoved -= OnWeaponRemoved;
            _weaponManager.OnWeaponSwitched -= OnWeaponSwitched;
            _weaponManager.AmmoManager.OnAmmoChanged -= UpdateAllWeaponAmmoInHUD;
        }

        if (_currentActiveWeapon != null)
        {
            _currentActiveWeapon.OnWeaponFire -= OnWeaponFired;
            _currentActiveWeapon.OnWeaponReloadStarted -= OnWeaponReloadStarted;
            _currentActiveWeapon.OnWeaponReloadEnded -= OnWeaponReloadEnded;
        }
        
        if (_playerHealth != null)
        {
            _playerHealth.OnPlayerDied -= OnPlayerHealthDied;
        }
    }
}