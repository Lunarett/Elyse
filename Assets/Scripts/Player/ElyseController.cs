using System.Collections;
using UnityEngine;
using Photon.Pun;
using Pulsar.Utils;

public class ElyseController : PlayerController
{
    [SerializeField] private float _respawnTimeSeconds = 3.0f;
    
    private ElysePlayerState _elysePlayerState;

    public ElysePlayerState ElysePlayerState => _elysePlayerState;
    
    protected override void Awake()
    {
        base.Awake();
        _elysePlayerState = gameObject.AddComponent<ElysePlayerState>();
        Utils.CheckForNull<ElysePlayerState>(_elysePlayerState);
    }

    public void Die()
    {
        if (Utils.CheckForNull<Pawn>(_pawn)) return;
        _elysePlayerState.AddDeath();
        StartCoroutine(RespawnCoroutine());
    }

    private IEnumerator RespawnCoroutine()
    {
        yield return new WaitForSeconds(_respawnTimeSeconds);
        DestroyPawn();
        CreatePawn();
    }
}
