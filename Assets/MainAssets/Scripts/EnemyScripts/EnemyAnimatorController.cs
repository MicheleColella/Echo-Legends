// File: EnemyAnimatorController.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAnimatorController : MonoBehaviour
{
    public Animator animator;
    public EnemyMovement enemyMovement;
    public EnemyAttack enemyAttack;

    private NavMeshAgent agent;
    private Transform player;

    private float moveX;
    private float moveY;

    private float smoothTime = 0.1f; // Tempo per la transizione dei valori
    private float shootSmoothTime = 0.2f; // Tempo per la transizione del layer di sparo

    private float currentVelocityX;
    private float currentVelocityY;
    private float currentShootWeight;
    private float targetShootWeight;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = enemyMovement.player;
        currentShootWeight = 0f;
        targetShootWeight = 0f;
    }

    private void Update()
    {
        if (enemyMovement.currentState == EnemyState.Dying)
        {
            //Debug.Log("EnemyAnimatorController: Enemy is in Dying state");
            return;
        }

        UpdateMovementAnimation();
        UpdateShootLayerWeight();
    }

    private void UpdateMovementAnimation()
    {
        Vector3 moveDirection = agent.velocity;
        Vector3 lookDirection = player.position - transform.position;
        lookDirection.y = 0;
        lookDirection.Normalize();

        float targetMoveX = Vector3.Dot(moveDirection, transform.right);
        float targetMoveY = Vector3.Dot(moveDirection, transform.forward);

        moveX = Mathf.SmoothDamp(moveX, targetMoveX, ref currentVelocityX, smoothTime);
        moveY = Mathf.SmoothDamp(moveY, targetMoveY, ref currentVelocityY, smoothTime);

        animator.SetFloat("MoveX", moveX);
        animator.SetFloat("MoveY", moveY);

        bool isMoving = moveDirection.sqrMagnitude > 0.01f;
        animator.SetBool("isMoving", isMoving);

        float targetAngle = Mathf.Atan2(lookDirection.x, lookDirection.z) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, targetAngle, 0);
    }

    private void UpdateShootLayerWeight()
    {
        bool isShooting = enemyAttack.IsBursting();
        targetShootWeight = isShooting ? 1f : 0f;

        currentShootWeight = Mathf.MoveTowards(currentShootWeight, targetShootWeight, Time.deltaTime / shootSmoothTime);
        animator.SetLayerWeight(animator.GetLayerIndex("ShootingLayer"), currentShootWeight);
    }
}
