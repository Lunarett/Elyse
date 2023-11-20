using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

public class Teleporter : MonoBehaviour
{
    public Vector3 teleportLocation; // Set this in the inspector to the desired location

    private Keyboard keyboard;
    private PhotonView pv;

    void Start()
    {
        pv = GetComponent<PhotonView>();
        keyboard = Keyboard.current;
    }

    void Update()
    {
        if (!pv.IsMine) return;
        if (keyboard != null)
        {
            bool isCtrlPressed = keyboard.ctrlKey.isPressed;
            bool isTPressed = keyboard.tKey.wasPressedThisFrame;

            if (isCtrlPressed && isTPressed)
            {
                transform.position = teleportLocation; // Teleport the player
            }
        }
    }
}