using Photon.Pun;
using UnityEngine;
using System;

namespace Plusar.Player
{
    [RequireComponent(typeof(PhotonView), typeof(PlayerMovement), typeof(PlayerView))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private GameObject _fpsView;
        [SerializeField] private GameObject _tpsView;
        [SerializeField] private EViewMode _viewMode = EViewMode.FPS;

        private PlayerMovement _playerMovement;
        private PlayerView _playerView;
        private PlayerInputManager _inputManager;

        public PlayerMovement PlayerMovement => _playerMovement;
        public PlayerView PlayerView => _playerView;
        public PlayerInputManager InputManager => _inputManager;

        private PlayerManager _pm;
        private PhotonView _pv;

        private void Awake()
        {
            _playerMovement = GetComponent<PlayerMovement>();
            _playerView = GetComponent<PlayerView>();
            _inputManager = GetComponent<PlayerInputManager>();

            _pv = GetComponent<PhotonView>();
            _pm = PhotonView.Find((int)_pv.InstantiationData[0]).GetComponent<PlayerManager>();

            _fpsView.SetActive(_pv.IsMine);
            _tpsView.SetActive(!_pv.IsMine);
        }

        private void Start()
        {
            ShowMouseCursor(false);
            SetControl(true);
        }

        private void Update()
        {
            if (_inputManager.GetViewButtonDown())
            {
                // Toggle View Modes
                _viewMode = (_viewMode == EViewMode.FPS) ? EViewMode.TPS : EViewMode.FPS;
                SetViewMode(_viewMode);
            }
        }

        public void ShowMouseCursor(bool isVisible)
        {
            Cursor.lockState = isVisible ? CursorLockMode.None : CursorLockMode.Locked;
        }

        public void SetControl(bool isEnabled)
        {
            _playerMovement.EnableMovement = isEnabled ? _pv.IsMine : false;
            _playerView.EnableView = isEnabled ? _pv.IsMine : false;
        }

        public void SetViewMode(EViewMode viewMode)
        {
            _playerView.SetCameraViewMode(viewMode);

            switch (viewMode)
            {
                case EViewMode.FPS:
                    _fpsView.SetActive(true);
                    _tpsView.SetActive(false);
                    break;
                case EViewMode.TPS:
                    _fpsView.SetActive(false);
                    _tpsView.SetActive(true);
                    break;
                default:
                    return;
            }
        }
    }
}
