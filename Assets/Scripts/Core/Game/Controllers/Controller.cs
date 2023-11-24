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
        if (DebugUtils.CheckForNull<PhotonView>(_photonView, "Controller: PhotonView is missing!")) return;
    }

    protected virtual void Start()
    {
        if (_photonView.IsMine)
        {
            CreatePawn();
        }
    }

    public void CreatePawn()
    {
        if (!_photonView.IsMine) return;
        Transform spawnpoint = PlayerSpawner.Instance.GetSpawnpoint(_photonView.Owner);
        DebugUtils.CheckForNull<Transform>(spawnpoint, "Controller: Failed to get Spawnpoint!");

        if (DebugUtils.CheckForNull<Pawn>(_pawnPrefab, "Controller: Pawn prefab is missing!")) return;
        _pawnObject = PhotonNetwork.Instantiate(
            Path.Combine("PhotonPrefabs", "Pawns", _pawnPrefab.gameObject.name),
            spawnpoint.position,
            spawnpoint.rotation,
            0,
            new object[] { _photonView.ViewID }
        );

        if (DebugUtils.CheckForNull<GameObject>(_pawnObject, "Controller: Instantiation of PawnPrefab has failed!")) return;

        _pawn = _pawnObject.GetComponent<Pawn>();
        if (DebugUtils.CheckForNull<Pawn>(_pawn, "Controller: Pawn is missing form PawnPrefab")) return;
        _pawn.Initialize(_photonView.Owner);

        // Notify remote clients
        _photonView.RPC(nameof(RPC_SetRemotePawn), RpcTarget.OthersBuffered, _pawnObject.GetPhotonView().ViewID);
    }

    [PunRPC]
    protected void RPC_SetRemotePawn(int pawnViewID)
    {
        _pawnObject = PhotonView.Find(pawnViewID).gameObject;
        if (DebugUtils.CheckForNull<GameObject>(_pawnObject,
                "Controller RPC_SetRemote: Failed to find PawnObject by ViewID")) return;
        _pawn = _pawnObject.GetComponent<Pawn>();
        if (DebugUtils.CheckForNull<GameObject>(_pawnObject,
                "Controller RPC_SetRemote: Pawn isn't found on the object")) return;
    }

    public void DestroyPawn()
    {
        if (!_photonView.IsMine) return;
        PhotonNetwork.Destroy(_pawnObject);
        _photonView.RPC(nameof(RPC_DestroyPawn), RpcTarget.OthersBuffered);
    }

    [PunRPC]
    protected void RPC_DestroyPawn()
    {
        _pawnObject = null;
        _pawn = null;
    }

    public static Controller Find(Player player)
    {
        return FindObjectsOfType<Controller>().SingleOrDefault(x => x._photonView.Owner == player);
    }
}
