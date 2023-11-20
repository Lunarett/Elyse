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

    public void Die()
    {
        if (DebugUtils.CheckForNull<Pawn>(_pawn)) return;
        _elysePlayerState.AddDeath();
        StartCoroutine(RespawnCoroutine());
    }

    private IEnumerator RespawnCoroutine()
    {
        yield return new WaitForSeconds(_respawnTimeSeconds);
        DestroyPawn();
        CreatePawn();
    }

    public void ResetPlayerState()
    {
        _elysePlayerState.ResetStats();
    }
}
