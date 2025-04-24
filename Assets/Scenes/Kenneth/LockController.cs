using UnityEngine;

public class LockController : MonoBehaviour
{
    public float rotationAngle = 90f;
    public float rotationSpeed = 200f;
    public float interactionDistance = 3f;
    public LayerMask wheelLayer; // Set this to a layer just for your wheels

    private Transform targetWheel;
    private Quaternion targetRotation;
    private bool isRotating = false;
    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
        if (mainCam == null)
            Debug.LogError("Main Camera not found. Tag your camera as MainCamera.");
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
                targetRotation = targetWheel.rotation * Quaternion.Euler(0, rotationAngle, 0);
                StartCoroutine(RotateWheel(targetWheel));
            }
        }
    }

    System.Collections.IEnumerator RotateWheel(Transform wheel)
    {
        isRotating = true;

        while (Quaternion.Angle(wheel.rotation, targetRotation) > 0.1f)
        {
            wheel.rotation = Quaternion.RotateTowards(wheel.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            yield return null;
        }

        isRotating = false;
    }
}
