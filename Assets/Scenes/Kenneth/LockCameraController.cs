using UnityEngine;

public class LockCameraController : MonoBehaviour
{
    public Camera mainCamera; // Assign your main gameplay camera
    public Camera lockCamera; // Assign a separate lock camera
    public GameObject lockUIPanel; // Assign UI Panel for lock screen

    private bool isZoomed = false;

    void Start()
    {
        lockCamera.enabled = false; // Start with lock camera off
        lockUIPanel.SetActive(false); // Hide UI at start
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 2f))
            {
                if (hit.collider.CompareTag("Lock")) // Add "Lock" tag to the lock object
                {
                    EnterLockView();
                }
            }
        }

        if (isZoomed && Input.GetKeyDown(KeyCode.Escape))
        {
            ExitLockView();
        }
    }

    void EnterLockView()
    {
        mainCamera.enabled = false;
        lockCamera.enabled = true;
        lockUIPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isZoomed = true;
    }

    void ExitLockView()
    {
        lockCamera.enabled = false;
        mainCamera.enabled = true;
        lockUIPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isZoomed = false;
    }
}