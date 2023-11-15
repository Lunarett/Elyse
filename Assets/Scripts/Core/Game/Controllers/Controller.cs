using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using System.IO;
using Pulsar.Debug;

[RequireComponent(typeof(PhotonView))]
public class Controller : MonoBehaviour
{
    [SerializeField] private Pawn _pawnPrefab;

    protected PhotonView _photonView;
    protected GameObject _pawnObject;
    protected Pawn _pawn;

    public Pawn Pawn => _pawn;

    protected virtual void Awake()
    {
        _photonView = GetComponent<PhotonView>();
    }

    protected virtual void Start()
    {
        if (_photonView.IsMine)
        {
            CreatePawn();
        }
    }

    protected void CreatePawn()
    {
        if (!_photonView.IsMine) return;
        Transform spawnpoint = PlayerSpawner.Instance.GetSpawnpoint(_photonView.Owner);
        DebugUtils.CheckForNull(spawnpoint);

        if (DebugUtils.CheckForNull(_pawnPrefab)) return;
        _pawnObject = PhotonNetwork.Instantiate(
            Path.Combine("PhotonPrefabs", "Pawns", _pawnPrefab.gameObject.name),
            spawnpoint.position,
            spawnpoint.rotation,
            0,
            new object[] { _photonView.ViewID }
        );

        if (_pawnObject == null)
        {
            Debug.LogError($"Controller: Failed to Instantiate {_pawnPrefab.gameObject.name} Pawn from 'Resources/PhotonPrefabs/Pawns' directory!");
            return;
        }

        _pawn = _pawnObject.GetComponent<Pawn>();
        _pawn.Initialize(_photonView.Owner);

        // Notify remote clients
        _photonView.RPC("SetRemotePawn", RpcTarget.OthersBuffered, _pawnObject.GetPhotonView().ViewID);
    }

    [PunRPC]
    protected void SetRemotePawn(int pawnViewID)
    {
        _pawnObject = PhotonView.Find(pawnViewID).gameObject;
        _pawn = _pawnObject.GetComponent<Pawn>();
    }

    public void DestroyPawn()
    {
        // If this is the owner of the pawn, destroy it and notify others
        if (_photonView.IsMine)
        {
            PhotonNetwork.Destroy(_pawnObject);
            _photonView.RPC("OnPawnDestroyed", RpcTarget.OthersBuffered);
        }
    }

    [PunRPC]
    protected void OnPawnDestroyed()
    {
        _pawnObject = null;
        _pawn = null;
    }

    public static Controller Find(Player player)
    {
        return FindObjectsOfType<Controller>().SingleOrDefault(x => x._photonView.Owner == player);
    }
}
