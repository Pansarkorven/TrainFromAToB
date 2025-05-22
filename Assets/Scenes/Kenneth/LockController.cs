using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LockController : MonoBehaviour
{
    public float rotationAngle = 36f; // 360 / 10 digits
    public float rotationSpeed = 200f;
    public float interactionDistance = 3f;
    public LayerMask wheelLayer;

    public Transform[] wheels;
    public float[] wheelOffsets;
    public int[] correctCombination = new int[] { 3, 1, 4, 1, 1, 1 };

    private Camera mainCam;
    private bool isRotating = false;
    private Transform targetWheel;

    void Start()
    {
        mainCam = Camera.main;
        if (mainCam == null)
        {
            Debug.LogError("Main Camera not found. Tag your camera as MainCamera.");
        }

        if (wheelOffsets.Length != wheels.Length)
        {
            Debug.LogWarning("wheelOffsets length does not match wheels length. Defaulting to 0 offsets.");
            wheelOffsets = new float[wheels.Length];
        }

        // Initialize each wheel to start with digit 1 facing the player
        for (int i = 0; i < wheels.Length; i++)
        {
            float offset = wheelOffsets[i];
            float initialAngle = (1 * rotationAngle + offset) % 360f;

            Vector3 localEuler = wheels[i].localEulerAngles;
            localEuler.y = initialAngle;
            wheels[i].localEulerAngles = localEuler;
        }
    }

    void Update()
    {
        if (isRotating) return;

        Ray ray = new Ray(mainCam.transform.position, mainCam.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance, wheelLayer))
        {
            targetWheel = hit.transform;

            if (Input.GetKeyDown(KeyCode.E))
            {
                StartCoroutine(RotateWheel(targetWheel, rotationAngle));
            }
        }
    }

    IEnumerator RotateWheel(Transform wheel, float angle)
    {
        isRotating = true;

        float rotated = 0f;

        while (rotated < angle)
        {
            float step = rotationSpeed * Time.deltaTime;
            if (rotated + step > angle) step = angle - rotated;

            wheel.Rotate(Vector3.up, step, Space.Self);
            rotated += step;

            yield return null;
        }

        // Snap to exact angle
        int index = System.Array.IndexOf(wheels, wheel);
        float currentY = wheel.localEulerAngles.y;
        float offset = wheelOffsets[index];

        // Remove offset before snapping
        float rawAngle = (currentY - offset + 360f) % 360f;
        int currentDigit = Mathf.RoundToInt(rawAngle / rotationAngle) % 10;

        // Apply new snapped rotation
        float snappedY = (currentDigit * rotationAngle + offset) % 360f;
        Vector3 finalEuler = wheel.localEulerAngles;
        finalEuler.y = snappedY;
        wheel.localEulerAngles = finalEuler;

        Debug.Log($"Wheel [{wheel.name}] snapped to digit: {currentDigit}");

        isRotating = false;

        CheckCombination();
    }

    void CheckCombination()
    {
        for (int i = 0; i < wheels.Length; i++)
        {
            int digit = GetDigitFromRotation(wheels[i], wheelOffsets[i]);
            if (digit != correctCombination[i])
                return;
        }

        Debug.Log("✅ Lock Unlocked!");
        // Unlock logic here
    }

    /// <summary>
    /// Calculates which digit is facing front using wheel rotation and offset.
    /// </summary>
    int GetDigitFromRotation(Transform wheel, float offset)
    {
        float y = wheel.localEulerAngles.y;
        float rawAngle = (y - offset + 360f) % 360f;

        int digit = Mathf.RoundToInt(rawAngle / rotationAngle) % 10;

        return digit;
    }
}
