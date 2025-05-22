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
    public float[] wheelOffsets; // Offset for each wheel to align '0' correctly
    public int[] correctCombination = new int[] { 3, 1, 4, 1, 1, 1 };

    private Dictionary<Transform, int> rotationSteps = new Dictionary<Transform, int>();
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
            wheelOffsets = new float[wheels.Length]; // Fill with 0s
        }

        foreach (var wheel in wheels)
        {
            rotationSteps[wheel] = 0;
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
                rotationSteps[targetWheel] = (rotationSteps[targetWheel] + 1) % 10;

                StartCoroutine(RotateWheel(targetWheel, rotationAngle));

                Debug.Log($"Rotated {targetWheel.name}. Step = {rotationSteps[targetWheel]}");

                CheckCombination();
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

        // Snap to precise final angle
        int wheelIndex = System.Array.IndexOf(wheels, wheel);
        int stepCount = rotationSteps[wheel];
        float offset = wheelOffsets[wheelIndex]; // per-wheel offset

        float snappedY = (stepCount * rotationAngle + offset) % 360f;

        Vector3 localEuler = wheel.localEulerAngles;
        localEuler.y = snappedY;
        wheel.localEulerAngles = localEuler;

        isRotating = false;
    }

    void CheckCombination()
    {
        for (int i = 0; i < wheels.Length; i++)
        {
            int frontDigit = GetDigitFacingPlayer(wheels[i]);

            if (frontDigit != correctCombination[i])
                return;
        }

        Debug.Log("✅ Lock Unlocked!");
        // Unlock logic here
    }

    /// <summary>
    /// Determines the digit currently facing the player based on Y rotation.
    /// </summary>
    int GetDigitFacingPlayer(Transform wheel)
    {
        Vector3 toCamera = (mainCam.transform.position - wheel.position).normalized;
        Vector3 localToCamera = wheel.InverseTransformDirection(toCamera); // in wheel's local space

        float angle = Mathf.Atan2(localToCamera.x, localToCamera.z) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360f;

        int digit = Mathf.RoundToInt(angle / rotationAngle) % 10;

        return digit;
    }
}
