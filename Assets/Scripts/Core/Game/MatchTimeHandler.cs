using System;
using Photon.Pun;
using Pulsar.Debug;
using UnityEngine;

public enum EMatchState
{
    PreMatch,
    InProgress,
    PostMatch
}

public class MatchTimeHandler : MonoBehaviour
{
    [Header("Match Time Settings")]
    [SerializeField] private float _preMatchCountdownTime = 3.0f;
    [SerializeField] private float _matchDuration = 120.0f;
    [SerializeField] private float _postMatchDuration = 8.0f;

    private EMatchState _currentMatchState = EMatchState.PreMatch;
    private float _elapsedMatchTime;
    private float _elapsedPostMatchTime;
    private bool _isPaused;
    private bool _beginCountdown;

    private PhotonView _photonView;

    public EMatchState CurrentMatchState => _currentMatchState;
    public float ElapsedMatchTime => _elapsedMatchTime;

    public event Action OnMatchStarted;
    public event Action OnMatchEnded;
    public event Action OnGameEnded;

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
        DebugUtils.CheckForNull<PhotonView>(_photonView, "MatchTimeHandler: PhotonView is missing!");
    }

    private void Update()
    {
        if (!_beginCountdown) return;
        if (_isPaused) return;

        switch (_currentMatchState)
        {
            case EMatchState.PreMatch:
                _elapsedMatchTime += Time.deltaTime;
                if (_elapsedMatchTime >= _preMatchCountdownTime) StartMatch();
                break;
            case EMatchState.InProgress:
                _elapsedMatchTime += Time.deltaTime;
                if (_elapsedMatchTime >= _matchDuration) EndMatch();
                break;
            case EMatchState.PostMatch:
                _elapsedPostMatchTime += Time.deltaTime;
                if (_elapsedPostMatchTime >= _postMatchDuration) OnGameEnded?.Invoke();
                break;
        }
    }

    public void StartPreMatchCountdown()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        _elapsedMatchTime = 0;
        _beginCountdown = true;
    }

    public void StartMatch()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        _photonView.RPC(nameof(RPC_StartMatch), RpcTarget.All);
    }

    [PunRPC]
    private void RPC_StartMatch()
    {
        _elapsedMatchTime = 0;
        _currentMatchState = EMatchState.InProgress;
        OnMatchStarted?.Invoke();
    }

    public void EndMatch()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        _photonView.RPC(nameof(RPC_EndMatch), RpcTarget.All);
    }

    [PunRPC]
    private void RPC_EndMatch()
    {
        _elapsedMatchTime = 0;
        _currentMatchState = EMatchState.PostMatch;
        OnMatchEnded?.Invoke();
    }

    public void PauseMatch(bool isPaused)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        _photonView.RPC(nameof(RPC_PauseMatch), RpcTarget.All, isPaused);
    }

    [PunRPC]
    private void RPC_PauseMatch(bool isPaused)
    {
        _isPaused = isPaused;
    }
}