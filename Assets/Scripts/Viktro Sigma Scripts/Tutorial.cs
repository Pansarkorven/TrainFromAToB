using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [Tooltip("Tags that will trigger object changes.")]
    public string[] triggerTags;

    [Tooltip("Objects to disable when triggered.")]
    public GameObject[] objectsToDisable;

    [Tooltip("Objects to enable when triggered.")]
    public GameObject[] objectsToEnable;

    private void OnTriggerEnter(Collider other)
    {
        foreach (string tag in triggerTags)
        {
            if (other.CompareTag(tag))
            {
                foreach (GameObject obj in objectsToDisable)
                {
                    if (obj != null)
                        obj.SetActive(false);
                }

                foreach (GameObject obj in objectsToEnable)
                {
                    if (obj != null)
                        obj.SetActive(true);
                }

                Debug.Log($"Triggered by tag: {tag}. Disabled {objectsToDisable.Length} and enabled {objectsToEnable.Length} object(s).");
                break; // Stop after first match
            }
        }
    }
}