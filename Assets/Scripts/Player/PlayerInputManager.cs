using UnityEngine;
using UnityEngine.InputSystem;

namespace Plusar.Player
{
    public class PlayerInputManager : InputManager
    {
        private bool m_FireInputWasHeld;

        private void LateUpdate()
        {
            m_FireInputWasHeld = GetFireInputHeld();
        }

        public Vector3 GetMoveInputVector3()
        {
            if (!CanProcessInput()) return Vector3.zero;
            Vector2 inputAxis = GetInputActionValue<Vector2>("Movement");
            return new Vector3(inputAxis.x, 0, inputAxis.y);
        }

        public Vector2 GetMoveInput(bool forceAllowInput = false)
        {
            if (forceAllowInput)
            {
                return GetInputActionValue<Vector2>("Movement");
            }

            return CanProcessInput() ? GetInputActionValue<Vector2>("Movement") : Vector2.zero;

        }

        public Vector2 GetMouseInput()
        {
            return CanProcessInput() ? GetInputActionValue<Vector2>("Look") : Vector2.zero;
        }

        public bool GetJumpInputDown()
        {
            return CheckInputActionPhase("Jump", InputActionPhase.Started);
        }

        public bool GetJumpInputHeld()
        {
            return CheckInputActionPhase("Jump", InputActionPhase.Performed);
        }

        public bool GetJumpInputReleased()
        {
            return CheckInputActionPhase("Jump", InputActionPhase.Canceled);
        }

        public bool GetFireInputDown()
        {
            return CanProcessInput() && (GetFireInputHeld() && !m_FireInputWasHeld);
        }

        public bool GetFireInputReleased()
        {
            return CanProcessInput() && !GetFireInputHeld() && m_FireInputWasHeld;
        }

        public bool GetFireInputHeld()
        {
            return CheckInputActionPhase("Fire", InputActionPhase.Performed);
        }

        public bool GetAimInputHeld()
        {
            return CheckInputActionPhase("Aim", InputActionPhase.Performed);
        }

        public bool GetSprintInputHeld()
        {
            return CheckInputActionPhase("Sprint", InputActionPhase.Performed);
        }

        public bool GetCrouchInputDown()
        {
            return CheckInputActionPhase("Crouch", InputActionPhase.Started);
        }

        public bool GetCrouchInputReleased()
        {
            return CheckInputActionPhase("Crouch", InputActionPhase.Canceled);
        }

        public bool GetReloadButtonDown()
        {
            return CheckInputActionPhase("Reload", InputActionPhase.Started);
        }

        public bool GetViewButtonDown()
        {
            return CheckInputActionPhase("View", InputActionPhase.Started);
        }

        public int GetSwitchWeaponInput()
        {
            if (!CanProcessInput()) return 0;
            float scrollVal = GetInputActionValue<float>("Switch");

            return scrollVal switch
            {
                > 0 => -1,
                < 0 => 1,
                _ => 0
            };
        }

        public int GetSelectWeaponInput()
        {
            if (!CanProcessInput()) return 0;

            for (int i = 1; i <= 4; i++)
            {
                if (CheckInputActionPhase($"Slot{i}", InputActionPhase.Performed)) return i;
            }

            return 0;
        }
    }
}
