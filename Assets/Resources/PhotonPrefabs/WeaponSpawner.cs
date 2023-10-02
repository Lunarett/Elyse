using System;
using UnityEngine;
using Photon.Pun;

public class WeaponSpawner : MonoBehaviour
{
    [SerializeField] private GameObject weaponPrefab;

    [SerializeField] private Transform fpsPosition;

    [SerializeField] private Transform tpsPosition;

    private PhotonView pv;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
    }

    void Start()
    {
        if (weaponPrefab == null || fpsPosition == null || tpsPosition == null)
        {
            Debug.LogError("WeaponSpawner: One or more serialized fields are null!");
            return;
        }

        AddWeapon();
    }

    private void AddWeapon()
    {
        // Determine the correct position based on the owner of the PhotonView
        Transform targetPosition = pv.IsMine ? fpsPosition : tpsPosition;
        string layerName = pv.IsMine ? "FP_View" : "TP_View";

        if (PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InRoom)
        {
            // Instantiate the weapon locally
            GameObject weaponObject = Instantiate(weaponPrefab, targetPosition.position, targetPosition.rotation);
            weaponObject.transform.SetParent(targetPosition);
            weaponObject.transform.localPosition = Vector3.zero;
            weaponObject.transform.localRotation = Quaternion.identity;
            weaponObject.transform.localScale = Vector3.one;

            // Set the layer recursively using your utility method
            Pulsar.Utils.Utils.SetLayerRecursively(weaponObject, LayerMask.NameToLayer(layerName));
        }
        else
        {
            Debug.LogError("Not connected to network or not in room.");
        }
    }
}