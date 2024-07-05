using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Vector2 damageRange; // Range del danno del proiettile
    public float lifeTime = 2f;
    private float totalDamage; // Danno totale del proiettile
    private ProjectileObjectPool pool; // Riferimento alla pool

    private Rigidbody rb; // Riferimento al componente Rigidbody
    public LayerMask collisionLayers; // LayerMask dei layer con cui il proiettile pu? collidere
    private Collider projectileCollider; // Aggiungi un riferimento al Collider

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        projectileCollider = GetComponent<Collider>();
    }

    void OnEnable()
    {
        // Disattiva temporaneamente il Collider per evitare collisioni immediate
        projectileCollider.enabled = false;
        Invoke(nameof(EnableCollider), 0.1f); // Abilita il Collider dopo 0.1 secondi
        Invoke(nameof(ReturnToPool), lifeTime);
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
            ReturnToPool();
        }
    }

    public void Initialize(Vector2 weaponDamageRange, ProjectileObjectPool pool)
    {
        this.pool = pool;
        float weaponDamage = Random.Range(weaponDamageRange.x, weaponDamageRange.y);
        float projectileDamage = Random.Range(damageRange.x, damageRange.y);
        totalDamage = weaponDamage + projectileDamage;
    }

    void ReturnToPool()
    {
        pool.ReturnObject(gameObject);
    }
}
