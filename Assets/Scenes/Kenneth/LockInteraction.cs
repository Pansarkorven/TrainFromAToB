using UnityEngine;

public class LockInteraction : MonoBehaviour
{
    public Transform player;
    public float interactionDistance = 3f;

    public Camera mainCamera;
    public Transform lockViewPoint;
    public Transform mainCameraOriginalPosition;

    private bool isInteracting = false;

    void Start()
    {
        // Automatically find tagged objects if fields aren't assigned
        if (lockViewPoint == null)
            lockViewPoint = GameObject.FindWithTag("LockViewPoint")?.transform;

        if (mainCameraOriginalPosition == null)
            mainCameraOriginalPosition = GameObject.FindWithTag("MainCameraOriginal")?.transform;

        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    void Update()
    {
        float distance = Vector3.Distance(player.position, transform.position);

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!isInteracting && distance <= interactionDistance)
            {
                EnterLockView();
            }
            else if (isInteracting)
            {
                ExitLockView();
            }
        }
    }

    void EnterLockView()
    {
        isInteracting = true;
        mainCamera.transform.position = lockViewPoint.position;
        mainCamera.transform.rotation = lockViewPoint.rotation;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void ExitLockView()
    {
        isInteracting = false;
        mainCamera.transform.position = mainCameraOriginalPosition.position;
        mainCamera.transform.rotation = mainCameraOriginalPosition.rotation;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public bool IsInteracting()
    {
        return isInteracting;
    }
}
