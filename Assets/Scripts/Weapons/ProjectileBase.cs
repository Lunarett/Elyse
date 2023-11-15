using UnityEngine;
using Photon.Pun;

public abstract class ProjectileBase : MonoBehaviourPun
{
    public float Damage = 10.0f;
    public float Speed = 100f;
    public float DownForce = 0.0f;
    public float MaxDistance = 100f;
    public LayerMask HitLayers;
    public ParticleSystem ImpactEffectPrefab;
    public DamageCauserInfo DamageInfo;
    
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
            if (photonView.IsMine)
            {
                Debug.Log("Base Impact");
                HandleImpact(hit.point, hit.normal, hit.collider);
            }
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

    protected abstract void HandleImpact(Vector3 position, Vector3 normal, Collider collider);

    protected void SpawnImpactEffect(Vector3 position, Vector3 normal)
    {
        if (ImpactEffectPrefab != null)
        {
            Instantiate(ImpactEffectPrefab, position, Quaternion.LookRotation(normal));
        }
    }
}