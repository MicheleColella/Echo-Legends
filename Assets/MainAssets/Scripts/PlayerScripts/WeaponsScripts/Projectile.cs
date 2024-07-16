using UnityEngine;
using MidniteOilSoftware.ObjectPoolManager;

public class Projectile : MonoBehaviour, IDespawnedPoolObject, IRetrievedPoolObject
{
    public Vector2 damageRange; // Range del danno del proiettile
    public float lifeTime = 2f;
    private float totalDamage; // Danno totale del proiettile
    private Transform playerTransform; // Riferimento al player

    private Rigidbody rb; // Riferimento al componente Rigidbody
    public LayerMask collisionLayers; // LayerMask dei layer con cui il proiettile può collidere
    private Collider projectileCollider; // Riferimento al Collider

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        projectileCollider = GetComponent<Collider>();
    }

    void OnEnable()
    {
        projectileCollider.enabled = false;
        Invoke(nameof(EnableCollider), 0.1f);
        Invoke(nameof(DespawnProjectile), lifeTime);
    }

    void EnableCollider()
    {
        projectileCollider.enabled = true;
    }

    void OnDisable()
    {
        CancelInvoke();
        rb.velocity = Vector3.zero;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!projectileCollider.enabled)
        {
            return;
        }

        if (((1 << collision.gameObject.layer) & collisionLayers) != 0)
        {
            EnemyHealth enemyHealth = collision.gameObject.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage((int)totalDamage);
            }

            DespawnProjectile();
        }
    }

    public void Initialize(Vector2 weaponDamageRange, Transform playerTransform)
    {
        this.playerTransform = playerTransform;
        float weaponDamage = Random.Range(weaponDamageRange.x, weaponDamageRange.y);
        float projectileDamage = Random.Range(damageRange.x, damageRange.y);
        totalDamage = weaponDamage + projectileDamage;
    }

    void DespawnProjectile()
    {
        if (Vector3.Distance(transform.position, playerTransform.position) > 4f)
        {
            ObjectPoolManager.DespawnGameObject(gameObject);
        }
        else
        {
            Invoke(nameof(DespawnProjectile), 0.5f);
        }
    }

    public void ReturnedToPool()
    {
        if (rb)
        {
            rb.velocity = rb.angularVelocity = Vector3.zero;
        }
    }

    public void RetrievedFromPool(GameObject prefab)
    {
        // Reset the projectile state if necessary
    }
}
