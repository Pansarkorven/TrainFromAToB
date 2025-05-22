using UnityEngine;

public class Box : MonoBehaviour
{
    public Transform holdPoint; 
    public Canvas pickupCanvas; 
    public float pickupRange = 3f;
    public string cameraTag = "MainCamera"; 

    private bool isPickedUp = false;
    private Rigidbody rb;
    private Transform cameraTransform;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (pickupCanvas != null)
            pickupCanvas.enabled = false;

        GameObject camObj = GameObject.FindGameObjectWithTag(cameraTag);
        if (camObj != null)
        {
            cameraTransform = camObj.transform;
        }
    }

    void Update()
    {
        if (cameraTransform == null) return;

        float distance = Vector3.Distance(transform.position, cameraTransform.position);

        if (!isPickedUp)
        {
            if (distance <= pickupRange)
            {
                if (pickupCanvas != null)
                    pickupCanvas.enabled = true;

                if (Input.GetKeyDown(KeyCode.E))
                {
                    PickUp();
                }
            }
            else
            {
                if (pickupCanvas != null)
                    pickupCanvas.enabled = false;
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Drop();
            }
        }
    }

    void PickUp()
    {
        isPickedUp = true;
        rb.isKinematic = true;
        transform.SetParent(holdPoint);
        transform.localPosition = Vector3.zero;

        if (pickupCanvas != null)
            pickupCanvas.enabled = false;
    }

    void Drop()
    {
        isPickedUp = false;
        rb.isKinematic = false;
        transform.SetParent(null);
    }
}
