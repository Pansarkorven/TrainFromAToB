using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;
using TMPro;
using System.Xml;


public class InspectCode : MonoBehaviour
{
    public GameObject offset;
    private PlayerInput _playerInput;
    GameObject targetObject;
    public bool isExamining = false;
    public Canvas _canva;
    public GameObject tableObject;
    private Vector3 lastMousePosition;
    private Transform examinedObject;
    public TMP_Text tmpText;


    private Dictionary<Transform, Vector3> originalPositions = new Dictionary<Transform, Vector3>();
    private Dictionary<Transform, Quaternion> originalRotations = new Dictionary<Transform, Quaternion>();

    private CinemachineBrain cinemachineBrain;

    void Start()
    {
        _canva.enabled = false;
        targetObject = GameObject.Find("Player");
        _playerInput = targetObject.GetComponent<PlayerInput>();
        cinemachineBrain = GameObject.Find("Camera").GetComponent<CinemachineBrain>();
    }

    void Update()
    {
        {
            if (CheckUserClose())
            {
                Debug.Log("User is close to the table!");
            }
        }
        Camera mainCamera = cinemachineBrain.OutputCamera;
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        bool isLookingAtInspectable = false;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.CompareTag("Object"))  
            {
                isLookingAtInspectable = true;
                _canva.enabled = true;  

                if (Input.GetKeyDown(KeyCode.E))
                {
                    ToggleExamination();
                    if (isExamining)
                    {
                        examinedObject = hit.transform;
                        originalPositions[examinedObject] = examinedObject.position;
                        originalRotations[examinedObject] = examinedObject.rotation;
                    }
                }
            }
        }

        if (!isLookingAtInspectable && !isExamining)
        {
            _canva.enabled = false;
        }

        if (isExamining)
        {
            Examine();
            StartExamination();
        }
        else
        {
            NonExamine();
            StopExamination();
        }
    }


    public void ToggleExamination()
    {
        isExamining = !isExamining;
        tmpText.gameObject.SetActive(!isExamining); 

        if (!isExamining && examinedObject != null)
        {
            if (originalPositions.ContainsKey(examinedObject))
            {
                examinedObject.position = originalPositions[examinedObject];
            }
            if (originalRotations.ContainsKey(examinedObject))
            {
                examinedObject.rotation = originalRotations[examinedObject];
            }

            examinedObject = null; 
        }
    }



    void StartExamination()
    {
        lastMousePosition = Input.mousePosition;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _playerInput.enabled = false;
    }

    void StopExamination()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _playerInput.enabled = true;
    }

    void Examine()
    {
        if (examinedObject != null)
        {
            examinedObject.position = Vector3.Lerp(examinedObject.position, offset.transform.position, 0.1f);
            Vector3 deltaMouse = Input.mousePosition - lastMousePosition;
            float rotationSpeed = 1.0f;
            examinedObject.Rotate(deltaMouse.x * rotationSpeed * Vector3.up, Space.World);
            examinedObject.Rotate(deltaMouse.y * rotationSpeed * Vector3.left, Space.World);
            lastMousePosition = Input.mousePosition;
        }
    }

    void NonExamine()
    {
        if (examinedObject != null)
        {
            if (originalPositions.ContainsKey(examinedObject))
            {
                examinedObject.position = Vector3.Lerp(examinedObject.position, originalPositions[examinedObject], 0.2f);
            }
            if (originalRotations.ContainsKey(examinedObject))
            {
                examinedObject.rotation = Quaternion.Slerp(examinedObject.rotation, originalRotations[examinedObject], 0.2f);
            }
        }
    }

    bool CheckUserClose()
    {
        float distance = Vector3.Distance(targetObject.transform.position, tableObject.transform.position);
        return (distance < 2);
    }
}
