using Photon.Pun;
using UnityEngine;
using Pulsar.Utils;

[RequireComponent(typeof(PlayerMovement), typeof(PlayerView))]
public class PlayerCharacter : MonoBehaviour
{
    [SerializeField] private EViewMode _viewMode = EViewMode.FPS;

    [Header("Character References")]
    [SerializeField] private GameObject _fpsView;
    [SerializeField] private GameObject _tpsView;

    [Header("Animation Properties")] [SerializeField]
    private Animator _tpsAnimator;

    protected PhotonView _photonView;
    
    private PlayerMovement _playerMovement;
    private PlayerView _playerView;
    private PlayerInputManager _inputManager;

    public PlayerMovement PlayerMovement => _playerMovement;
    public PlayerView PlayerView => _playerView;
    public PlayerInputManager InputManager => _inputManager;
    public EViewMode ViewMode => _viewMode;

    private static readonly int Vertical = Animator.StringToHash("Vertical");
    private static readonly int Horizontal = Animator.StringToHash("Horizontal");
    private static readonly int Crouch = Animator.StringToHash("Crouch");
    private static readonly int Grounded = Animator.StringToHash("Grounded");

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
        if (_photonView == null)
        {
            Debug.LogError("PhotonView is null", this);
            return;
        }
        _playerMovement = GetComponent<PlayerMovement>();
        _playerView = GetComponent<PlayerView>();
        _inputManager = GetComponent<PlayerInputManager>();
        
        _fpsView.SetActive(_photonView.IsMine);
        //_playerView.PlayerCamera.SetLayerRendering("CharacterMesh", !_photonView.IsMine);
        
        if (_photonView.IsMine)
        {
            Utils.SetLayerRecursively(_tpsView, LayerMask.NameToLayer("CharacterMesh"));
        }
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        SetControl(true);
    }

    private void LateUpdate()
    {
        // Update Character Animation
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
    }

    public void SetControl(bool isEnabled)
    {
        _playerMovement.EnableMovement = isEnabled ? _photonView.IsMine : false;
        _playerView.EnableView = isEnabled ? _photonView.IsMine : false;
    }

    public void SetViewMode(EViewMode viewMode)
    {
        _playerView.SetCameraViewMode(viewMode);

        switch (viewMode)
        {
            case EViewMode.FPS:
                _fpsView.SetActive(true);
                break;
            case EViewMode.TPS:
                _fpsView.SetActive(false);
                break;
            default:
                return;
        }
    }
}