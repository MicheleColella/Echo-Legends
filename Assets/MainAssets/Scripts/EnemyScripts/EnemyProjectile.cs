using UnityEngine;
using MidniteOilSoftware.ObjectPoolManager;

public class EnemyProjectile : MonoBehaviour, IDespawnedPoolObject, IRetrievedPoolObject
{
    public float speed = 10f;
    public Vector2 damageRange = new Vector2(5f, 15f); // Intervallo di danno (minimo e massimo)
    public float despawnTime = 5f;

    private Rigidbody rb;
    private Collider projectileCollider;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        projectileCollider = GetComponent<Collider>();
    }

    void OnEnable()
    {
        rb.velocity = transform.forward * speed;
        projectileCollider.enabled = false;
        Invoke(nameof(EnableCollider), 0.1f);
        Invoke(nameof(Despawn), despawnTime);
    }

    void OnDisable()
    {
        CancelInvoke();
        rb.velocity = Vector3.zero;
    }

    void EnableCollider()
    {
        projectileCollider.enabled = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!projectileCollider.enabled)
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            HealthSystem playerHealth = other.GetComponent<HealthSystem>();
            if (playerHealth != null)
            {
                float damage = Random.Range(damageRange.x, damageRange.y); // Calcola un danno casuale
                Debug.Log($"Projectile hit the player for {damage} damage!");
                playerHealth.TakeDamage(damage);
            }
            else
            {
                Debug.LogWarning("PlayerHealth component not found on the player object.");
            }
            Despawn();
        }
        else
        {
            Debug.Log("Projectile hit something else.");
            Despawn();
        }
    }

    void Despawn()
    {
        ObjectPoolManager.DespawnGameObject(gameObject);
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
        transform.position = prefab.transform.position;
        transform.rotation = prefab.transform.rotation;
        rb.velocity = transform.forward * speed;
    }
}
