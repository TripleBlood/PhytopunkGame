using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


public class SelectionController : MonoBehaviour
{
    public bool selected = false;
    private ObjectControl _objectControl;
    private GameObject _pivotPoint;
    private CameraControl _control;

    public RawImage portrait;
    private Color _color;
    public GameObject selectIndicator;

    // Start is called before the first frame update
    void Start()
    {
        _pivotPoint = GameObject.Find("PivotPoint");
        _control = _pivotPoint.GetComponent<CameraControl>();

        _objectControl = GetComponent<ObjectControl>();
    }

    // Update is called once per frame
    void Update()
    {
        // if (Input.GetMouseButtonDown(0))
        // {
        //
        //     RaycastHit hitInfo = new RaycastHit();
        //     bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
        //     if (hit)
        //     {
        //         if (hitInfo.transform.gameObject == gameObject)
        //         {
        //             _objectControl.Focused = true;
        //             portrait.color = _color;
        //             selectIndicator.gameObject.SetActive(true);
        //         }
        //         else if (_objectControl.Focused == true)
        //         {
        //             _objectControl.Focused = false;
        //             selectIndicator.gameObject.SetActive(false);
        //         }
        //     }
        //     else
        //     {
        //     }
        // }
    }
}