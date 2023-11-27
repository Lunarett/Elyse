using Pulsar.Debug;
using UnityEngine;

public class DeathMatchMode : GameModeBase
{
    private HUD _hud;
    private MatchTimeHandler _matchTimeHandler;

    protected override void Awake()
    {
        base.Awake();
        _matchTimeHandler = GetComponent<MatchTimeHandler>();
        DebugUtils.CheckForNull<MatchTimeHandler>(_matchTimeHandler, "DeathMatchMode: MatchTimeHander is missing!");

        _matchTimeHandler.OnMatchStarted += StartMatch;
        _matchTimeHandler.OnMatchEnded += EndMatch;
        _matchTimeHandler.OnGameEnded += ResetGame;
    }

    protected override void Start()
    {
        base.Start();
        _hud = HUD.Instance;
        Invoke(nameof(LateStart), 0.1f);
    }

    private void LateStart()
    {
        EnableAllPlayerControl(false);
        _matchTimeHandler.StartPreMatchCountdown();
        _hud.SetMatchText("Match will begin in...");
    }

    private void LateUpdate()
    {
        if (_matchTimeHandler.CurrentMatchState is not (EMatchState.PreMatch or EMatchState.InProgress)) return;
        if (DebugUtils.CheckForNull<HUD>(_hud, "HUD is returning null while trying to update time")) return;
        _hud.SetTimerText(_matchTimeHandler.ElapsedMatchTime);
    }

    private void StartMatch()
    {
        EnableAllPlayerControl(true);
        _hud.SetMatchText("Match has begun!");
        Invoke(nameof(TurnOffMatchText), 3.0f);
    }
    
    private void EndMatch()
    {
        EnableAllPlayerControl(false);
        _hud.SetTimerText(0, false);
        _hud.SetMatchText("Match is over!");
        Invoke(nameof(DisplayScoreBoard), 3.0f);
    }

    private void TurnOffMatchText()
    {
        _hud.SetMatchText("");
    }

    private void DisplayScoreBoard()
    {
        _hud.DisplayScoreBoard();
    }
    
    private void ResetGame()
    {
        _hud.ResetHUD();
    }

    public void LeaveMatch()
    {
    }
}