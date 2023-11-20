using UnityEngine;
using Photon.Pun;

public class ProjectileBase : MonoBehaviourPun
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
                HandleImpact(hit.point, hit.normal, hit.collider);
                photonView.RPC(nameof(RPC_ConfirmImpact), RpcTarget.Others, hit.point, hit.normal, hit.collider.gameObject.name);
            }
        }
        else
        {
            transform.position += _velocity * Time.deltaTime;
            if (Vector3.Distance(_startPosition, transform.position) >= MaxDistance)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }

    protected void HandleImpact(Vector3 position, Vector3 normal, Collider collider)
    {
        // Apply impact logic here (e.g., visual effects)
        SpawnImpactEffect(position, normal);

        // Apply damage if the collider has a BodyDamageMultiplier component
        BodyDamageMultiplier bodyDamageMultiplier = collider.GetComponent<BodyDamageMultiplier>();
        if (bodyDamageMultiplier != null)
        {
            bodyDamageMultiplier.TakeDamage(Damage, DamageInfo);
        }

        // Destroy the projectile
        PhotonNetwork.Destroy(gameObject);
    }


    protected void SpawnImpactEffect(Vector3 position, Vector3 normal)
    {
        if (ImpactEffectPrefab != null)
        {
            Instantiate(ImpactEffectPrefab, position, Quaternion.LookRotation(normal));
        }
    }

    [PunRPC]
    private void RPC_ConfirmImpact(Vector3 position, Vector3 normal, string colliderName)
    {
        // Find the collider by name and confirm the impact
        var collider = GameObject.Find(colliderName).GetComponent<Collider>();
        if (collider != null)
        {
            HandleImpact(position, normal, collider);
        }
    }
}
