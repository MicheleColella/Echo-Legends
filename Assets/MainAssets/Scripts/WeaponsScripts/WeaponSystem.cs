// WeaponSystem.cs
using UnityEngine;

public class WeaponSystem : MonoBehaviour
{
    public WeaponData[] weapons; // Array di ScriptableObject delle armi
    public Transform firePoint; // Punto di fuoco
    private int currentWeaponIndex = 0; // Indice dell'arma attualmente selezionata
    private float nextFireTime = 0f; // Tempo di attesa per il prossimo sparo

    public LayerMask collisionLayers; // LayerMask dei layer con cui i proiettili possono collidere

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SelectWeapon(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SelectWeapon(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SelectWeapon(2);

        if (Input.GetKey(KeyCode.Space))
        {
            FireWeapon();
        }
    }

    void SelectWeapon(int index)
    {
        if (index >= 0 && index < weapons.Length)
        {
            currentWeaponIndex = index;
            Debug.Log("Selected Weapon: " + weapons[currentWeaponIndex].weaponName);
        }
    }

    void FireWeapon()
    {
        if (Time.time >= nextFireTime)
        {
            WeaponData currentWeapon = weapons[currentWeaponIndex];
            nextFireTime = Time.time + 1f / currentWeapon.fireRate;

            for (int i = 0; i < currentWeapon.projectilesPerShot; i++)
            {
                float angle = CalculateSpreadAngle(i, currentWeapon.projectilesPerShot, currentWeapon.spreadAngle);
                Quaternion rotation = Quaternion.Euler(new Vector3(0, angle, 0));
                Vector3 direction = rotation * firePoint.forward;

                if (float.IsNaN(direction.x) || float.IsNaN(direction.y) || float.IsNaN(direction.z))
                {
                    Debug.LogError("Direzione del proiettile non valida");
                    continue;
                }

                GameObject projectileObject = Instantiate(currentWeapon.projectilePrefab, firePoint.position, Quaternion.LookRotation(direction));
                Rigidbody rb = projectileObject.GetComponent<Rigidbody>();
                if (rb == null)
                {
                    Debug.LogError("Rigidbody non trovato sul proiettile");
                    continue;
                }

                rb.velocity = direction * currentWeapon.projectileSpeed;

                Projectile projectile = projectileObject.GetComponent<Projectile>();
                if (projectile == null)
                {
                    Debug.LogError("Componente Projectile non trovato sul proiettile");
                    continue;
                }

                projectile.Initialize(currentWeapon.damageRange, transform);
                projectile.collisionLayers = collisionLayers; // Assegna i layer con cui può collidere
            }
        }
    }

    float CalculateSpreadAngle(int index, int totalProjectiles, float spreadAngle)
    {
        if (totalProjectiles == 1)
        {
            return 0f;
        }

        float step = spreadAngle / (totalProjectiles - 1);
        return -spreadAngle / 2 + step * index;
    }
}
