using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    public Animator doorAnimator; 
    private bool hasOpened = false; 

    private void OnCollisionEnter(Collision collision)
    {
        if (!hasOpened && collision.gameObject.CompareTag("PickupItem"))
        {
            Debug.Log("Box on pad");
            if (doorAnimator != null)
            {
                doorAnimator.SetTrigger("Open");
                hasOpened = true; 
            }
        }
    }
}
