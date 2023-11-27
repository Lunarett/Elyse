using System.Collections;
using UnityEngine;
using Pulsar.Debug;

public class ElyseController : PlayerController
{
    [SerializeField] private float _respawnTimeSeconds = 3.0f;
    
    private ElysePlayerState _elysePlayerState;

    public ElysePlayerState ElysePlayerState => _elysePlayerState;
    
    protected override void Awake()
    {
        base.Awake();
        _elysePlayerState = gameObject.AddComponent<ElysePlayerState>();
        DebugUtils.CheckForNull<ElysePlayerState>(_elysePlayerState);
    }

    protected override void Start()
    {
        base.Start();
    }

    public override void CreatePawn(Pawn pawn)
    {
        base.CreatePawn(pawn);
        _controlledPawn.Owner = this;
    }

    public void Die()
    {
        RespawnPawn(_respawnTimeSeconds);
    }
}
