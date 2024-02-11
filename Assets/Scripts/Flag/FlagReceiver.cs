using UnityEngine;

public class FlagReceiver : MonoBehaviour
{
    [SerializeField] private Transform _flagAttachmentPoint;

    private Flag _attachedFlag = null;
    private PlayerHealth _health;

    private void Awake()
    {
        _health = GetComponent<PlayerHealth>();
        if (_health == null) return;

        _health.OnPlayerDied += DropFlag;
    }

    public void AttachFlag(Flag flag)
    {
        Transform flagTransform = flag.gameObject.transform;
        flagTransform.SetParent(_flagAttachmentPoint, false);
        flagTransform.localPosition = Vector3.zero;
        _attachedFlag = flagTransform.GetComponent<Flag>();
    }

    public void DropFlag()
    {
        if (!_attachedFlag) return;
        _attachedFlag.transform.SetParent(null);
        _attachedFlag.gameObject.transform.rotation = Quaternion.identity;
        _attachedFlag.DropFlag();
        _attachedFlag = null;
    }

    public bool HasFlag()
    {
        return _attachedFlag != null;
    }

    public void ReturnCapturedFlagToBase()
    {
        if (_attachedFlag == null) return;
        _attachedFlag.ReturnToBase();
    }
}