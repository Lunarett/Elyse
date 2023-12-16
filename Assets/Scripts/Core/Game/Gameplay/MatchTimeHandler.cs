using System;
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

    public EMatchState CurrentMatchState => _currentMatchState;
    public float ElapsedMatchTime => _elapsedMatchTime;

    public event Action OnMatchStarted;
    public event Action OnMatchEnded;
    public event Action OnGameEnded;

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
        _elapsedMatchTime = 0;
        _beginCountdown = true;
    }

    public void StartMatch()
    {
        _elapsedMatchTime = 0;
        _currentMatchState = EMatchState.InProgress;
        OnMatchStarted?.Invoke();
    }

    public void EndMatch()
    {
        _elapsedMatchTime = 0;
        _currentMatchState = EMatchState.PostMatch;
        OnMatchEnded?.Invoke();
    }

    public void PauseMatch(bool isPaused)
    {
        _isPaused = isPaused;
    }
}