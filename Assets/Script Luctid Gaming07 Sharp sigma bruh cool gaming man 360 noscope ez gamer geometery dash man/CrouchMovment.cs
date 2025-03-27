using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrouchMovement : MonoBehaviour
{
    private bool isCrouched = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            isCrouched = !isCrouched; // Toggle the crouch state
            transform.localScale = isCrouched ? new Vector3(1f, 1f, 1f) : new Vector3(0.5f, 0.5f, 0.5f);
        }
    }
}
