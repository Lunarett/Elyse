using System;
using Pulsar.Debug;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IInteractable
{
    void Interact(Controller controller);
}

public class InteractionManager : MonoBehaviour
{
    [Header("Interaction References")]
    [SerializeField] private Camera _mainCamera;
    
    [Header("Interaction Properties")]
    [SerializeField] private LayerMask _interactableLayerMask;
    [SerializeField] private float _interactionDistance = 5f;
    [SerializeField] private float _sphereCastRadius = 0.5f;

    private ElyseCharacter _character;
    private PlayerInputManager _input;
    private Controller _owner;

    private void Start()
    {
        _owner = _character.Owner;
    }

    private void Update()
    {
        if (!_input.GetInteractInputDown()) return;
        if (DebugUtils.CheckForNull<Controller>(_owner, "InteractionManager: Owner was not found!")) return;
        IInteractable interactedObject = DetectInteraction();
        if (interactedObject != null)
        {
            interactedObject.Interact(_owner);
        }
    }

    private IInteractable DetectInteraction()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        if (Physics.SphereCast(ray, _sphereCastRadius, out hit, _interactionDistance, _interactableLayerMask))
        {
            return hit.collider.GetComponent<IInteractable>();
        }

        return null;
    }
}
