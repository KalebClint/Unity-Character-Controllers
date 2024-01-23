using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mouseLook : MonoBehaviour
{

    /// <summary>
    ///  Attach this script to the camera. Set prefered sensitivy, thats it really.
    /// </summary>


    public float mouseSensetivity = 200f;

    public Transform playerBody;

    private float xRotation = 0f;

    void Start()
    {
        //Locks and hides the cursor.
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensetivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensetivity * Time.deltaTime;

        xRotation -= mouseY;
        // Limits rotation of camera up and down.
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
