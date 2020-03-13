using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraControl : MonoBehaviour
{
    public float rotationSpeed = 30.0f;
    public float cameraMoveFactor;
    public RawImage colorInterface;
    private GameObject selectedObject;
    private GameObject focusedObject;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(2))
        {
            transform.Rotate(Vector3.up, Time.deltaTime * rotationSpeed * Input.GetAxis("Mouse X"));
        }
        else if (Input.GetMouseButton(1))
        {
            Vector3 move = new Vector3(Input.GetAxis("Mouse X") * -cameraMoveFactor, 0,
                Input.GetAxis("Mouse Y") * -cameraMoveFactor);
            transform.Translate(move, Space.Self);
        }

        //SelectObject

        

        //return;
    }
}