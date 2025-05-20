using UnityEngine;

public class DestroyOnTouch : MonoBehaviour
{
    public string targetTag = "Enemy"; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            Destroy(gameObject);
            Debug.Log("Removed after enter train");
        }
    }
}