using UnityEngine;
using Photon.Pun;
using Pulsar.Utils;

public class WeaponManager : MonoBehaviour
{
    [Header("Position References")] 
    [SerializeField] private Transform _fpsSpawnPosition;
    [SerializeField] private Transform _tpsSpawnPosition;

    [SerializeField] private GameObject weaponPrefab;

    private PhotonView pv;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
    }

    void Start()
    {
        AddWeapon();
    }

    private void AddWeapon()
    {
        // Determine the correct position based on the owner of the PhotonView
        Transform targetPosition = pv.IsMine ? _fpsSpawnPosition : _tpsSpawnPosition;
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
            Utils.SetLayerRecursively(weaponObject, LayerMask.NameToLayer(layerName));
        }
        else
        {
            Debug.LogError("Not connected to network or not in room.");
        }
    }
}