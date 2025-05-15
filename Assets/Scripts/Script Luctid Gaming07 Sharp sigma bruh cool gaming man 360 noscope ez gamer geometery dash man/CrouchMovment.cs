using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrouchMovement : MonoBehaviour
{
    public Vector3 normalScale = new Vector3(0.5f, 0.5f, 0.5f);
    public Vector3 crouchScale = new Vector3(1f, 1f, 1f);

    private bool isCrouched = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            isCrouched = !isCrouched; 
            transform.localScale = isCrouched ? crouchScale : normalScale;
        }
    }
}
