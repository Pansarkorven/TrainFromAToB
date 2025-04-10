using UnityEngine;

public class Rotate : MonoBehaviour
{
    public float rotationDuration = 0.5f; // How long the rotation takes
    private int currentNumber = 0;
    private bool isRotating = false;

    private void OnMouseDown()
    {
        if (!isRotating)
        {
            RotateToNextNumber();
        }
    }

    void RotateToNextNumber()
    {
        currentNumber = (currentNumber + 1) % 10;
        float targetAngle = currentNumber * -36f; // Negative for clockwise rotation
        StartCoroutine(RotateWheel(targetAngle));
    }

    System.Collections.IEnumerator RotateWheel(float targetAngle)
    {
        isRotating = true;
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = Quaternion.Euler(0, 0, targetAngle);
        float elapsed = 0f;

        while (elapsed < rotationDuration)
        {
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, elapsed / rotationDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.rotation = endRotation;
        isRotating = false;
    }
}
