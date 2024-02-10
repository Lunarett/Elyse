using UnityEngine;

public class DestroyAfterSeconds : MonoBehaviour
{
    [SerializeField] private float _seconds = 1.0f;

    private void Start()
    {
        Invoke(nameof(SelfDestruct), _seconds);
    }

    private void SelfDestruct()
    {
        Destroy(gameObject);
    }
}
