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
    [SerializeField] private List<WeaponBase> weaponPrefabs;

    private PhotonView _photonView;
    private PlayerInputManager _input;
    private WeaponBase _activeWeapon;
    private ElyseCharacter _character;
    private int _currentWeaponIndex = 0;

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
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

        if (_photonView.IsMine)
        {
            AddWeapon(_currentWeaponIndex);
        }
    }

    private void Update()
    {
        if (!_photonView.IsMine || _activeWeapon == null) return;
        
        // Call Weapon Shooting
        switch (_activeWeapon.FireModeType)
        {
            case FireMode.Auto:
                if (_input.GetFireInputHeld())
                {
                    _activeWeapon.StartFire();
                }
                break;
            case FireMode.Single:
            case FireMode.Burst:
                if (_input.GetFireInputDown())
                {
                    _activeWeapon.StartFire();
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
    Transform targetPosition = _photonView.IsMine ? _fpsSpawnLocation : _tpsSpawnLocation;
    string layerName = _photonView.IsMine ? "FP_Weapon" : "TP_Weapon";

    if (PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InRoom)
    {
        GameObject weaponObject = PhotonNetwork.Instantiate(
            Path.Combine("PhotonPrefabs", "Weapons", weaponPrefabs[weaponIndex].gameObject.name),
            targetPosition.position,
            targetPosition.rotation,
            0
        );
        
        WeaponBase weapon = weaponObject.GetComponent<WeaponBase>();
        weaponObject.transform.SetParent(targetPosition);
        weaponObject.transform.localPosition = weapon.WeaponOffset;
        weaponObject.transform.localRotation = Quaternion.identity;
        weaponObject.transform.localScale = Vector3.one;

        Utils.SetLayerRecursively(weaponObject, LayerMask.NameToLayer(layerName));

        _activeWeapon = weapon;
        _photonView.RPC(nameof(ParentWeapon), RpcTarget.AllBuffered, weaponObject.GetPhotonView().ViewID, _photonView.ViewID);
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
        
        WeaponBase weaponRef = weaponPV.GetComponent<WeaponBase>();
        if (weaponRef != null)
        {
            weaponPV.transform.localPosition = weaponRef.WeaponOffset;
            weaponPV.transform.localRotation = Quaternion.identity;
            
            ElyseCharacter charRef = playerPV.GetComponent<ElyseCharacter>();
            if (charRef != null)
            {
                weaponRef.CharacterReference = charRef;
            }
            else
            {
                Debug.LogError("ElyseCharacter component not found");
            }
        }
        else
        {
            Debug.LogError("Weapon component not found");
        }
    }
}



    public WeaponBase GetActiveWeapon()
    {
        return _activeWeapon;
    }
}
