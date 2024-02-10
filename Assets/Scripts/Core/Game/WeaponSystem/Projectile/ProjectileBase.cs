using System.Collections;
using FMODUnity;
using UnityEngine;

public class ProjectileBase : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private EventReference _impactSoundEvent;
    
    public float Speed = 100f;
    public float DownForce = 0.0f;
    public float MaxDistance = 100f;
    public LayerMask HitLayers;
    public DamageCauseInfo DamageInfo;
    public ParticleSystem ImpactEffectPrefab;
    
    private Vector3 _velocity;
    private Vector3 _startPosition;

    protected virtual void Start()
    {
        _startPosition = transform.position;
        _velocity = transform.forward * Speed;
    }

    protected virtual void Update()
    {
        UpdateMovement();
    }

    private void UpdateMovement()
    {
        float distanceToMove = _velocity.magnitude * Time.deltaTime;
        _velocity += Vector3.down * (DownForce * Time.deltaTime);
        Ray ray = new Ray(transform.position, _velocity.normalized);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, distanceToMove, HitLayers))
        {
            HandleImpact(hit.point, hit.normal, hit.collider);
        }
        else
        {
            transform.position += _velocity * Time.deltaTime;
            if (Vector3.Distance(_startPosition, transform.position) >= MaxDistance)
            {
                Destroy(gameObject);
            }
        }
    }

    protected virtual void HandleImpact(Vector3 position, Vector3 normal, Collider collider)
    {
        SpawnImpactEffect(position, normal);
        PlayImpactSound(position);
    }

    protected void SpawnImpactEffect(Vector3 position, Vector3 normal)
    {
        if (ImpactEffectPrefab == null) return;
        Instantiate(ImpactEffectPrefab, position, Quaternion.LookRotation(normal));
    }
    
    protected void PlayImpactSound(Vector3 position)
    {
        RuntimeManager.PlayOneShot(_impactSoundEvent, position);
    }
}
