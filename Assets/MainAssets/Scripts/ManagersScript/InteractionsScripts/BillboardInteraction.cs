using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BillboardInteraction : MonoBehaviour
{
    [SerializeField] private BillboardType billBoardType;

    [Header("Lock Rotation")]
    [SerializeField] private bool lockX;
    [SerializeField] private bool lockY;
    [SerializeField] private bool lockZ;

    [Header("Sprites")]
    [SerializeField] private Sprite mouseAndKeyboardSprite;
    [SerializeField] private Sprite gamepadSprite;
    [SerializeField] private Sprite mobileSprite;

    private SpriteRenderer spriteRenderer;
    private Vector3 originalRotation;
    private bool isUsingGamepad;
    private bool isUsingMouseAndKeyboard;
    private bool isUsingMobile;

    public enum BillboardType { LookAtCamera, CameraForward };

    private void Awake()
    {
        originalRotation = transform.rotation.eulerAngles;
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (PlatformChecker.isMobile)
        {
            isUsingMobile = true;
            UpdateSprite(mobileSprite);
        }
        else
        {
            isUsingMobile = false;
            isUsingMouseAndKeyboard = true;
            UpdateSprite(mouseAndKeyboardSprite);
        }
    }

    private void Update()
    {
        if (PlatformChecker.isMobile)
        {
            return; // No need to check input on mobile, it's always using mobile sprite.
        }

        if (Gamepad.current != null && Gamepad.current.wasUpdatedThisFrame)
        {
            if (!isUsingGamepad)
            {
                isUsingGamepad = true;
                isUsingMouseAndKeyboard = false;
                UpdateSprite(gamepadSprite);
            }
        }
        else if ((Keyboard.current != null && Keyboard.current.wasUpdatedThisFrame) ||
                 (Mouse.current != null && Mouse.current.wasUpdatedThisFrame))
        {
            if (!isUsingMouseAndKeyboard)
            {
                isUsingMouseAndKeyboard = true;
                isUsingGamepad = false;
                UpdateSprite(mouseAndKeyboardSprite);
            }
        }
    }

    private void LateUpdate()
    {
        switch (billBoardType)
        {
            case BillboardType.LookAtCamera:
                transform.LookAt(Camera.main.transform.position, Vector3.up);
                break;

            case BillboardType.CameraForward:
                transform.forward = Camera.main.transform.forward;
                break;

            default:
                break;
        }

        Vector3 rotation = transform.rotation.eulerAngles;
        if (lockX) { rotation.x = originalRotation.x; }
        if (lockY) { rotation.y = originalRotation.y; }
        if (lockZ) { rotation.z = originalRotation.z; }
        transform.rotation = Quaternion.Euler(rotation);
    }

    private void UpdateSprite(Sprite newSprite)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = newSprite;
        }
    }
}
