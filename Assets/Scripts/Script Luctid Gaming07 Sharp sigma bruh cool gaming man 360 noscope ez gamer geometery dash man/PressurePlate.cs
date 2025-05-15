using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    public Animator doorAnimator; // Assign the Animator from your door in Inspector
    private bool hasOpened = false; // Prevents triggering multiple times

    private void OnCollisionEnter(Collision collision)
    {
        if (!hasOpened && collision.gameObject.CompareTag("Box"))
        {
            Debug.Log("Box on pad");
            if (doorAnimator != null)
            {
                doorAnimator.SetTrigger("Open");
                Debug.Log("Door opens (animation)");
                hasOpened = true; 
            }
        }
    }
}
