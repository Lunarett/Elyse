using System.IO;
using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
using Photon.Pun.Demo.SlotRacer;

namespace Plusar.Player
{
    public class WeaponManager : MonoBehaviourPunCallbacks
    {
        [Header("References")]
        [SerializeField] private Transform _fpsWeaponSpawnLocation;
        [SerializeField] private Transform _tpsWeaponSpawnLocation;

        [Header("Weapon Inventory")]
        [SerializeField] private List<WeaponBase> _startingWeapons;

        private List<WeaponBase> _currentWeapons;
        private int _activeWeaponIndex = 0;
        private PlayerInputManager _inputManager;
        private PlayerController _playerController;

        private void Awake()
        {
            _playerController = GetComponent<PlayerController>();
        }

        private void Start()
        {
            _inputManager = GetComponent<PlayerInputManager>();
            _currentWeapons = new List<WeaponBase>();

            foreach (var weapon in _startingWeapons)
            {
                AddWeapon(weapon);
            }

            if (_currentWeapons.Count > 0)
            {
                SetActiveWeapon(0);
            }
        }

        private void Update()
        {
            if (!photonView.IsMine) return;

            int switchInput = _inputManager.GetSwitchWeaponInput();
            if (switchInput != 0)
            {
                SetActiveWeapon((_activeWeaponIndex + switchInput + _currentWeapons.Count) % _currentWeapons.Count);
            }
        }

        public void AddWeapon(WeaponBase weapon)
        {
            if (!photonView.IsMine) return;

            Transform weaponSpawnLocation = _fpsWeaponSpawnLocation; // Always set it to the FPS view for the local player
            WeaponBase newWeapon = PhotonNetwork.Instantiate(
                Path.Combine("PhotonPrefabs", "Weapons", weapon.gameObject.name),
                weaponSpawnLocation.position,
                weaponSpawnLocation.rotation).GetComponent<WeaponBase>();

            // Get the PhotonView of the newWeapon
            PhotonView newWeaponPV = newWeapon.GetComponent<PhotonView>();

            // Change ownership if necessary
            if (newWeaponPV && !newWeaponPV.IsMine)
            {
                newWeaponPV.TransferOwnership(PhotonNetwork.LocalPlayer);
            }

            // Always set the parent to FPS view for the local player
            newWeapon.transform.SetParent(weaponSpawnLocation);

            // Send an RPC to set the parent to TPS view for remote players
            photonView.RPC("RPC_SetWeaponParent", RpcTarget.Others, newWeaponPV.ViewID, _tpsWeaponSpawnLocation.gameObject.name);

            // Set the parent to weaponSpawnLocation
            newWeapon.transform.SetParent(weaponSpawnLocation);

            // Set Layers
            SetLayerRecursively(newWeapon.gameObject, LayerMask.NameToLayer(photonView.IsMine ? "FP_View" : "TP_View"));

            newWeapon.PlayerController = _playerController;

            // Apply Offset
            newWeapon.transform.localPosition = Vector3.zero;
            newWeapon.transform.localPosition += newWeapon.Offset;
            newWeapon.transform.localRotation = Quaternion.identity;

            newWeapon.gameObject.SetActive(false);
            _currentWeapons.Add(newWeapon);

            photonView.RPC("RPC_AddWeapon", RpcTarget.Others, weapon.name);
        }

        // RPC to set the parent of the new weapon
        [PunRPC]
        void RPC_SetWeaponParent(int viewID, string parentName)
        {
            PhotonView targetView = PhotonView.Find(viewID);
            if (targetView)
            {
                GameObject parentObject = GameObject.Find(parentName);
                if (parentObject)
                {
                    targetView.transform.SetParent(parentObject.transform);
                }
            }
        }

        public void RemoveWeapon(WeaponBase weapon)
        {
            if (!photonView.IsMine || !_currentWeapons.Contains(weapon)) return;

            _currentWeapons.Remove(weapon);
            Destroy(weapon.gameObject);

            photonView.RPC("RPC_RemoveWeapon", RpcTarget.Others, weapon.name);
        }

        private void SetActiveWeapon(int index)
        {
            if (_currentWeapons == null || _currentWeapons.Count == 0) return;

            // Deactivate all weapons first
            foreach (var weapon in _currentWeapons)
            {
                weapon.gameObject.SetActive(false);
            }

            // Activate the selected weapon
            _activeWeaponIndex = index;
            _currentWeapons[_activeWeaponIndex].gameObject.SetActive(true);

            // If it's the local player, update the other clients as well
            if (photonView.IsMine)
            {
                photonView.RPC("RPC_SetActiveWeapon", RpcTarget.Others, _activeWeaponIndex);
            }
        }

        [PunRPC]
        private void RPC_AddWeapon(string weaponName)
        {
            WeaponBase weaponPrefab = _startingWeapons.Find(w => w.name == weaponName);
            if (weaponPrefab != null)
            {
                AddWeapon(weaponPrefab);
            }
            else
            {
                Debug.LogError("Weapon not found: " + weaponName);
            }
        }

        [PunRPC]
        private void RPC_RemoveWeapon(string weaponName)
        {
            WeaponBase weaponToRemove = _currentWeapons.Find(w => w.name == weaponName);
            if (weaponToRemove != null)
            {
                RemoveWeapon(weaponToRemove);
            }
        }

        [PunRPC]
        private void RPC_SetActiveWeapon(int index)
        {
            SetActiveWeapon(index);
        }

        public void SetLayerRecursively(GameObject obj, int newLayer)
        {
            if (obj == null)
            {
                return;
            }

            obj.layer = newLayer;

            foreach (Transform child in obj.transform)
            {
                if (child == null)
                {
                    continue;
                }
                SetLayerRecursively(child.gameObject, newLayer);
            }
        }
    }
}