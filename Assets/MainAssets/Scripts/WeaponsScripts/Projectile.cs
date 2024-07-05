// Projectile.cs
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Vector2 damageRange; // Range del danno del proiettile
    public float lifeTime = 2f;
    private float totalDamage; // Danno totale del proiettile
    private Transform playerTransform; // Riferimento al player

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
        Invoke(nameof(DestroyProjectile), lifeTime);
        //Debug.Log("Projectile enabled at position: " + transform.position);
    }

    void EnableCollider()
    {
        projectileCollider.enabled = true;
    }

    void OnDisable()
    {
        CancelInvoke();
        rb.velocity = Vector3.zero;
        //Debug.Log("Projectile disabled at position: " + transform.position);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!projectileCollider.enabled)
        {
            return;
        }

        //Debug.Log("Collision with object: " + collision.gameObject.name + " on layer: " + LayerMask.LayerToName(collision.gameObject.layer));
        if (((1 << collision.gameObject.layer) & collisionLayers) != 0)
        {
            DestroyProjectile();
        }
    }

    public void Initialize(Vector2 weaponDamageRange, Transform playerTransform)
    {
        this.playerTransform = playerTransform;
        float weaponDamage = Random.Range(weaponDamageRange.x, weaponDamageRange.y);
        float projectileDamage = Random.Range(damageRange.x, damageRange.y);
        totalDamage = weaponDamage + projectileDamage;
        //Debug.Log("Projectile initialized with total damage: " + totalDamage);
    }

    void DestroyProjectile()
    {
        // Verifica la distanza dal player prima di disattivare il proiettile
        if (Vector3.Distance(transform.position, playerTransform.position) > 4f)
        {
            //Debug.Log("Destroying projectile");
            Destroy(gameObject);
        }
        else
        {
            // Riattiva l'invocazione di DestroyProjectile finch? non si allontana dal player
            Invoke(nameof(DestroyProjectile), 0.5f);
        }
    }
}
