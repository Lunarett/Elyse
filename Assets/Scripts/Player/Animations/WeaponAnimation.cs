using UnityEngine;
using UnityEngine.InputSystem;
using Pulsar.Player;

public class WeaponAnimation : MonoBehaviour
{
    [Header("Bobbing")]
    [SerializeField] private float bobSpeed = 1f; 
    [SerializeField] private float speedCurve;
    [SerializeField] private Vector3 travelLimit = Vector3.one * 0.025f;
    [SerializeField] private Vector3 bobLimit = Vector3.one * 0.01f;
    [SerializeField] private float bobExaggeration;
    [SerializeField] private float smoothIdleReturn = 5f;

    [Header("Bob Rotation")]
    [SerializeField] private Vector3 multiplier;

    [Header("Recoil Animation")]
    [SerializeField] private float recoilAmount = 0.1f; // How far the gun moves backward.
    [SerializeField] private float recoilSpeed = 1.0f; // Speed at which the gun recoils.
    [SerializeField] private float returnSpeed = 2.0f; // Speed at which the gun returns.
    [SerializeField] private float recoilRotationStrength = 5.0f; // Adjust as needed for x rotation.

    private Vector3 bobPosition;
    private Vector3 bobEulerRotation;
    private Vector3 idlePosition;

    private float smoothRot = 12f;

    private bool isRecoiling = false;
    private float currentRecoilZ = 0.0f; // To track the current recoil position.

    public PlayerCharacter playerCharacter;
    
    private enum RecoilState
    {
        Idle,
        Recoiling,
        Returning
    }

    private RecoilState recoilState = RecoilState.Idle;

    private void Update()
    {
        if (playerCharacter.PlayerMovement.IsMoving())
        {
            BobOffset();
        }
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, idlePosition, Time.deltaTime * smoothIdleReturn);
        }

        HandleRecoil();
        
        // New Input System for detecting left mouse button
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            FireRecoil();
        }
    }

    private void BobOffset()
    {
        speedCurve += Time.deltaTime * bobSpeed * (playerCharacter.PlayerMovement.IsGrounded ? 1f : 0f) + 0.01f;

        bobPosition.x = Mathf.Sin(speedCurve) * bobLimit.x;
        bobPosition.y = Mathf.Sin(speedCurve) * bobLimit.y;
        bobPosition.z = Mathf.Sin(speedCurve) * bobLimit.z;

        transform.localPosition = Vector3.Lerp(transform.localPosition, bobPosition, Time.deltaTime * smoothRot);
    }

    private void BobRotation()
    {
        bobEulerRotation.x = Mathf.Sin(speedCurve) * multiplier.x;
        bobEulerRotation.y = Mathf.Sin(speedCurve) * multiplier.y;
        bobEulerRotation.z = Mathf.Sin(speedCurve) * multiplier.z;

        transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(bobEulerRotation), Time.deltaTime * smoothRot);
    }

    private void HandleRecoil()
    {
        switch (recoilState)
        {
            case RecoilState.Recoiling:
                if (currentRecoilZ > -recoilAmount)
                {
                    float moveAmount = Time.deltaTime * recoilSpeed;
                    transform.localPosition -= new Vector3(0, 0, moveAmount);
                    currentRecoilZ -= moveAmount;
                }
                else
                {
                    recoilState = RecoilState.Returning;
                }
                break;

            case RecoilState.Returning:
                if (currentRecoilZ < 0)
                {
                    float moveAmount = Time.deltaTime * returnSpeed;
                    transform.localPosition += new Vector3(0, 0, moveAmount);
                    currentRecoilZ += moveAmount;
                }
                else
                {
                    recoilState = RecoilState.Idle;
                    transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);
                    currentRecoilZ = 0;
                }
                break;
        }

        // Reset rotation and apply new rotation
        transform.localRotation = Quaternion.identity;
        float rotationX = Mathf.Clamp(-recoilRotationStrength * (currentRecoilZ / recoilAmount), -90f, 90f);
        transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
    }

    public void FireRecoil()
    {
        recoilState = RecoilState.Recoiling;
    }
}
