using UnityEngine;
using VInspector;
using MidniteOilSoftware.ObjectPoolManager;

public class EnemyAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float fireRate = 5f;
    public float burstDuration = 3f;
    public float burstCooldown = 5f;
    public float projectileSpeed = 10f;
    public Transform player;
    public LayerMask obstacleLayer;

    private bool isBursting = false;
    private float nextBurstTime = 0f;
    private float burstEndTime = 0f;
    private float nextFireTime = 0f;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

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
            GameObject projectile = ObjectPoolManager.SpawnGameObject(projectilePrefab, firePoint.position, firePoint.rotation);
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