using UnityEngine;

public class PressureButton : MonoBehaviour
{
    public string targetTag = "PickupItem";
    public bool useTrigger = true; // Toggle between Trigger or Collision
    public GameObject buttonVisual;
    public Vector3 pressedOffset = new Vector3(0, -0.05f, 0);
    public float moveSpeed = 3f;

    private int pressCount = 0;
    private Vector3 originalPosition;

    void Start()
    {
        if (buttonVisual == null) buttonVisual = this.gameObject;
        originalPosition = buttonVisual.transform.localPosition;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!useTrigger) return;
        if (other.CompareTag(targetTag))
        {
            pressCount++;
            if (pressCount == 1)
                Press();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!useTrigger) return;
        if (other.CompareTag(targetTag))
        {
            pressCount--;
            if (pressCount == 0)
                Release();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (useTrigger) return;
        if (collision.gameObject.CompareTag(targetTag))
        {
            pressCount++;
            if (pressCount == 1)
                Press();
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (useTrigger) return;
        if (collision.gameObject.CompareTag(targetTag))
        {
            pressCount--;
            if (pressCount == 0)
                Release();
        }
    }

    void Press()
    {
        StopAllCoroutines();
        StartCoroutine(MoveButton(originalPosition + pressedOffset));
        Debug.Log("Button Pressed!");
        // Add your logic here (e.g. open a door)
    }

    void Release()
    {
        StopAllCoroutines();
        StartCoroutine(MoveButton(originalPosition));
        Debug.Log("Button Released!");
        // Add your logic here (e.g. close a door)
    }

    System.Collections.IEnumerator MoveButton(Vector3 targetPos)
    {
        while (Vector3.Distance(buttonVisual.transform.localPosition, targetPos) > 0.001f)
        {
            buttonVisual.transform.localPosition = Vector3.Lerp(buttonVisual.transform.localPosition, targetPos, Time.deltaTime * moveSpeed);
            yield return null;
        }
        buttonVisual.transform.localPosition = targetPos;
    }
}
