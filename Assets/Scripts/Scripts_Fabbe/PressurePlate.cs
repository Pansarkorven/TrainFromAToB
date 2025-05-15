using UnityEngine;

public class PressurePlate : MonoBehaviour
{

    public GameObject door;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Box"))
        {
            Debug.Log("Box on pad");
            Debug.Log("Door is Open");
            OpenDoor();

        }
    }


    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Box"))
        {
            Debug.Log("Box left pad");
            Debug.Log("Door is Closed");
            CloseDoor();



        }

    }


    private void OpenDoor()
    {
        if (door != null)
        {
            door.SetActive(false);  
            Debug.Log("Door opens");
        }
    }

    private void CloseDoor()
    {
        if (door != null)
        {
            door.SetActive(true);   
            Debug.Log("Door is locked");
        }
    }
}



