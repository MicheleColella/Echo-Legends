using UnityEngine;
using VInspector;

public class EnemyAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float fireRate = 5f; // Proiettili al secondo durante il burst
    public float burstDuration = 3f; // Durata del burst in secondi
    public float burstCooldown = 5f; // Tempo di cooldown tra i burst in secondi
    public float projectileSpeed = 10f; // Velocità del proiettile
    public Transform player;
    public LayerMask obstacleLayer;

    private bool isBursting = false;
    private float nextBurstTime = 0f;
    private float burstEndTime = 0f;
    private float nextFireTime = 0f;

    void Update()
    {
        if (isBursting)
        {
            if (Time.time >= burstEndTime)
            {
                StopBurst();
            }
            else
            {
                Fire();
            }
        }
    }

    [Button]
    public void StartBurst()
    {
        if (CanSeePlayer())
        {
            isBursting = true;
            burstEndTime = Time.time + burstDuration;
        }
    }

    public void StopBurst()
    {
        isBursting = false;
        nextBurstTime = Time.time + burstCooldown;
    }

    public bool IsBursting()
    {
        return isBursting;
    }

    void Fire()
    {
        if (Time.time >= nextFireTime)
        {
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = firePoint.forward * projectileSpeed;
            }
            nextFireTime = Time.time + 1f / fireRate;
        }
    }

    bool CanSeePlayer()
    {
        RaycastHit hit;
        if (Physics.Linecast(transform.position, player.position, out hit, obstacleLayer))
        {
            if (hit.transform != player)
            {
                return false;
            }
        }
        return true;
    }
}
