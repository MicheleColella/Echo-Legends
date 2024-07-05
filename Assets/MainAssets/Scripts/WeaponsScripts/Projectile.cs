using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Vector2 damageRange; // Range del danno del proiettile
    public float lifeTime = 2f;
    private float totalDamage; // Danno totale del proiettile
    private ProjectileObjectPool pool; // Riferimento alla pool

    private Rigidbody rb; // Riferimento al componente Rigidbody
    public LayerMask collisionLayers; // LayerMask dei layer con cui il proiettile pu? collidere

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        // Disattiva il proiettile dopo lifeTime secondi se non ha colpito nulla
        Invoke(nameof(ReturnToPool), lifeTime);
        //Debug.Log("Projectile enabled at position: " + transform.position);
    }

    void OnDisable()
    {
        CancelInvoke();
        rb.velocity = Vector3.zero;
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision with object: " + collision.gameObject.name + " on layer: " + LayerMask.LayerToName(collision.gameObject.layer));
        if (((1 << collision.gameObject.layer) & collisionLayers) != 0)
        {
            //Debug.Log("Return to pool due to collision with valid layer");
            ReturnToPool();
        }
    }

    public void Initialize(Vector2 weaponDamageRange, ProjectileObjectPool pool)
    {
        this.pool = pool;
        float weaponDamage = Random.Range(weaponDamageRange.x, weaponDamageRange.y);
        float projectileDamage = Random.Range(damageRange.x, damageRange.y);
        totalDamage = weaponDamage + projectileDamage;
        //Debug.Log("Projectile initialized with total damage: " + totalDamage);
    }

    void ReturnToPool()
    {
        Debug.Log("Returning projectile to pool");
        pool.ReturnObject(gameObject);
    }
}
