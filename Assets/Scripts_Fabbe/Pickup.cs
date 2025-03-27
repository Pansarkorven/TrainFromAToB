using UnityEngine;
using TMPro; // Required for TextMeshPro

public class PickupSystem : MonoBehaviour
{
    public float pickupRange = 3f; // How far the player can reach to pick up objects
    public Transform holdPosition; // The position where the item will be held in front of the player
    public TextMeshProUGUI interactText; // TextMeshProUGUI element that displays "E"

    private GameObject currentItem; // Currently picked-up item
    private GameObject itemInSight; // The item the player is looking at

    void Update()
    {
        // Cast a ray from the center of the camera forward
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        // Check if the ray hits an object within the pickup range
        if (Physics.Raycast(ray, out hit, pickupRange))
        {
            if (hit.collider.CompareTag("PickupItem"))
            {
                itemInSight = hit.collider.gameObject;

                // Show "E" prompt when looking at a pickup item
                interactText.text = "E";
                interactText.gameObject.SetActive(true);

                // Check if the player presses the "E" key to pick up the object
                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (currentItem == null) // Pick up the object
                    {
                        PickupItem(hit.collider.gameObject);
                    }
                    else // If already holding an item, drop it
                    {
                        DropItem();
                    }
                }
            }
            else
            {
                // Hide the text when not looking at a pickup item
                interactText.gameObject.SetActive(false);
            }
        }
        else
        {
            interactText.gameObject.SetActive(false);
        }

        // If holding an item, keep it at the hold position
        if (currentItem != null)
        {
            currentItem.transform.position = holdPosition.position;
            currentItem.transform.rotation = holdPosition.rotation;
        }
    }

    void PickupItem(GameObject item)
    {
        currentItem = item;
        Rigidbody itemRb = currentItem.GetComponent<Rigidbody>();

        if (itemRb != null)
        {
            itemRb.isKinematic = true; // Disable physics while holding the item
        }

        currentItem.transform.SetParent(holdPosition); // Make the item a child of the hold position
    }

    void DropItem()
    {
        if (currentItem != null)
        {
            Rigidbody itemRb = currentItem.GetComponent<Rigidbody>();

            if (itemRb != null)
            {
                itemRb.isKinematic = false; // Re-enable physics when dropping the item
            }

            currentItem.transform.SetParent(null); // Remove the item from the hold position
            currentItem = null;
        }
    }
}
