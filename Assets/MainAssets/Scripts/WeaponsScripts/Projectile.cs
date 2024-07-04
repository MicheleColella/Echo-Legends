using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Vector2 damageRange; // Range del danno del proiettile
    public float lifeTime = 2f;
    private float totalDamage; // Danno totale del proiettile
    private ProjectileObjectPool pool; // Riferimento alla pool

    void Start()
    {
        // Disattiva il proiettile dopo lifeTime secondi se non ha colpito nulla
        Invoke(nameof(ReturnToPool), lifeTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Gestisci il danno all'impatto

        /*
        var health = collision.gameObject.GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(totalDamage);
        }
        */
        ReturnToPool();
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
