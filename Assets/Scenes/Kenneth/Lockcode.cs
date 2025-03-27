using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CodeLock : MonoBehaviour
{
    public string correctCode = "123456"; // Set your desired unlock code
    private string inputCode = "";
    public Text displayText; // Assign a UI Text to show input
    public GameObject lockObject; // Assign the object to unlock

    public void PressNumber(string num)
    {
        if (inputCode.Length < correctCode.Length)
        {
            inputCode += num;
            displayText.text = inputCode;
        }
    }

    public void CheckCode()
    {
        if (inputCode == correctCode)
        {
            Unlock();
        }
        else
        {
            StartCoroutine(WrongCode());
        }
    }

    IEnumerator WrongCode()
    {
        displayText.text = "Wrong Code!";
        yield return new WaitForSeconds(1);
        inputCode = "";
        displayText.text = "";
    }

    void Unlock()
    {
        displayText.text = "Unlocked!";
        if (lockObject != null)
        {
            lockObject.SetActive(false); // Hide or disable the lock
        }
    }

    public void ClearCode()
    {
        inputCode = "";
        displayText.text = "";
    }
}
