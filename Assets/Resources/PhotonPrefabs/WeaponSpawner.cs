using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using Pulsar.Utils;

public class WeaponSpawner : MonoBehaviourPunCallbacks
{
    [Header("Spawn Location References")]
    [SerializeField] private Transform _fpsSpawnLocation;
    [SerializeField] private Transform _tpsSpawnLocation;
    [Space]
    [SerializeField] private List<Weapon> weaponPrefabs;

    private PhotonView _pv;
    private PlayerInputManager _input;
    private Weapon _activeWeapon;
    private ElyseCharacter _character;
    private int _currentWeaponIndex = 0;

    private void Awake()
    {
        _pv = GetComponent<PhotonView>();
        _input = GetComponent<PlayerInputManager>();
        _character = GetComponent<ElyseCharacter>();
    }

    void Start()
    {
        if (_fpsSpawnLocation == null || _tpsSpawnLocation == null)
        {
            Debug.LogError("WeaponSpawner: Spawn Location references are missing!");
            return;
        }

        if (weaponPrefabs.Count == 0)
        {
            Debug.LogWarning("WeaponSpawner: Your weapon prefabs list is empty!");
            return;
        }

        if (_pv.IsMine)
        {
            AddWeapon(_currentWeaponIndex);
        }
    }

    private void Update()
    {
        if (!_pv.IsMine || _activeWeapon == null) return;
        
        // Call Weapon Shooting
        switch (_activeWeapon.FireModeType)
        {
            case FireMode.Auto:
                if (_input.GetFireInputHeld())
                {
                    _activeWeapon.Fire();
                }
                break;
            case FireMode.Single:
            case FireMode.Burst:
                if (_input.GetFireInputDown())
                {
                    _activeWeapon.Fire();
                }
                break;
        }

        int switchInput = _input.GetSwitchWeaponInput();
        if (switchInput != 0)
        {
            SwitchWeapon(switchInput);
        }
    }

    public void SwitchWeapon(int direction)
    {
        _currentWeaponIndex = (_currentWeaponIndex + direction + weaponPrefabs.Count) % weaponPrefabs.Count;
        PhotonNetwork.Destroy(_activeWeapon.gameObject);
        AddWeapon(_currentWeaponIndex);
    }

public void AddWeapon(int weaponIndex)
{
    Transform targetPosition = _pv.IsMine ? _fpsSpawnLocation : _tpsSpawnLocation;
    string layerName = _pv.IsMine ? "FP_Weapon" : "TP_Weapon";

    if (PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InRoom)
    {
        GameObject weaponObject = PhotonNetwork.Instantiate(
            Path.Combine(weaponPrefabs[weaponIndex].gameObject.name),
            targetPosition.position,
            targetPosition.rotation,
            0
        );
        
        Weapon weapon = weaponObject.GetComponent<Weapon>();
        weaponObject.transform.SetParent(targetPosition);
        weaponObject.transform.localPosition = weapon.WeaponOffset;
        weaponObject.transform.localRotation = Quaternion.identity;
        weaponObject.transform.localScale = Vector3.one;

        Utils.SetLayerRecursively(weaponObject, LayerMask.NameToLayer(layerName));

        _activeWeapon = weapon;
        _pv.RPC(nameof(ParentWeapon), RpcTarget.AllBuffered, weaponObject.GetPhotonView().ViewID, _pv.ViewID);
    }
    else
    {
        Debug.LogError("Not connected to network or not in room.");
    }
}

[PunRPC]
private void ParentWeapon(int weaponViewID, int playerViewID)
{
    PhotonView weaponPV = PhotonView.Find(weaponViewID);
    PhotonView playerPV = PhotonView.Find(playerViewID);

    if (weaponPV != null && playerPV != null)
    {
        Transform targetPosition = playerPV.IsMine ? _fpsSpawnLocation : _tpsSpawnLocation;
        weaponPV.transform.SetParent(targetPosition, false);
        weaponPV.transform.localPosition = Vector3.zero;
        weaponPV.transform.localRotation = Quaternion.identity;
        
        ElyseCharacter charRef = playerPV.GetComponent<ElyseCharacter>();
        Weapon weaponRef = weaponPV.GetComponent<Weapon>();
        if (charRef != null && weaponRef != null)
        {
            weaponRef.CharacterReference = charRef;  // Set the CharacterReference field on all clients
        }
        else
        {
            Debug.LogError("ElyseCharacter or Weapon component not found");
        }
    }
}


    public Weapon GetActiveWeapon()
    {
        return _activeWeapon;
    }
}
