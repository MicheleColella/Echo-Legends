using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleSync : MonoBehaviour
{
    public static int posID = Shader.PropertyToID("_Position");
    public static int sizeID = Shader.PropertyToID("_Size");

    public Material wallMaterial;
    public Camera Camera;
    public LayerMask Mask;

    private float targetSize = 0f;
    private float currentSize = 0f;
    public float transitionSpeed = 2f;
    public Vector3 offset = Vector3.zero;

    private Vector3 targetPosition;
    private Vector3 currentPosition;

    void Start()
    {
        targetPosition = Camera.WorldToViewportPoint(transform.position) + offset;
        currentPosition = targetPosition;
    }

    void LateUpdate()
    {
        var dir = Camera.transform.position - transform.position;
        var ray = new Ray(transform.position, dir.normalized);

        if (Physics.Raycast(ray, 3000, Mask))
        {
            targetSize = 0.8f;
        }
        else
        {
            targetSize = 0f;
        }

        currentSize = Mathf.Lerp(currentSize, targetSize, Time.deltaTime * transitionSpeed);
        wallMaterial.SetFloat(sizeID, currentSize);

        targetPosition = Camera.WorldToViewportPoint(transform.position) + offset;
        currentPosition = Vector3.Lerp(currentPosition, targetPosition, Time.deltaTime * transitionSpeed);

        wallMaterial.SetVector(posID, currentPosition);
    }
}
