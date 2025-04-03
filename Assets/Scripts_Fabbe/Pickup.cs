using UnityEngine;
using TMPro;

public class PickupSystem : MonoBehaviour
{
    public float pickupRange = 3f;          // How far the player can pick up objects
    public Transform holdPosition;          // Where the item is held
    public TextMeshProUGUI interactText;    // "E" interaction prompt
    public float moveSmoothness = 10f;      // How smoothly item follows hold position
    public float throwForce = 10f;          // Force applied when throwing item

    private Camera playerCamera;
    private Rigidbody currentItemRb;
    private GameObject currentItem;

    void Start()
    {
        if (interactText == null)
            Debug.LogError("interactText is not assigned in the Inspector.");
        if (holdPosition == null)
            Debug.LogError("holdPosition is not assigned in the Inspector.");

        GameObject cameraObject = GameObject.Find("Camera");
        if (cameraObject != null)
            playerCamera = cameraObject.GetComponent<Camera>();
        else
            Debug.LogError("No GameObject named 'Camera' found!");

        if (interactText != null)
            interactText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (interactText == null || holdPosition == null || playerCamera == null) return;

        if (currentItem == null) // Not holding an item
        {
            Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, pickupRange))
            {
                if (hit.collider.CompareTag("PickupItem"))
                {
                    interactText.text = "E";
                    interactText.gameObject.SetActive(true);

                    if (Input.GetKeyDown(KeyCode.E))
                        PickupItem(hit.collider.gameObject);
                }
                else
                    interactText.gameObject.SetActive(false);
            }
            else
                interactText.gameObject.SetActive(false);
        }
        else // Holding an item
        {
            if (Input.GetKeyDown(KeyCode.E))
                DropItem();

            if (Input.GetMouseButtonDown(1)) // Right-click to throw
                ThrowItem();
        }
    }

    void FixedUpdate()
    {
        if (currentItem != null)
        {
            Vector3 moveDirection = (holdPosition.position - currentItem.transform.position);
            currentItemRb.linearVelocity = moveDirection * moveSmoothness;
        }
    }

    void PickupItem(GameObject item)
    {
        currentItem = item;
        currentItemRb = currentItem.GetComponent<Rigidbody>();

        if (currentItemRb != null)
        {
            currentItemRb.useGravity = true;
            currentItemRb.linearDamping = 5f;  // Add drag to prevent instant snapping
        }
    }

    void DropItem()
    {
        if (currentItem != null)
        {
            currentItemRb.linearDamping = 0f; // Reset drag
            currentItemRb.linearVelocity = Vector3.zero; // Stop movement
            currentItem = null;
            currentItemRb = null;
        }
    }

    void ThrowItem()
    {
        if (currentItem != null)
        {
            currentItemRb.linearDamping = 0f;
            currentItemRb.linearVelocity = playerCamera.transform.forward * throwForce;
            currentItem = null;
            currentItemRb = null;
        }
    }
}
