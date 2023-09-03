using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControls : MonoBehaviour
{


    private float _cameraX;
    private float _cameraY;

    [SerializeField] private Transform CamBase;
    [SerializeField] private Transform CamLeft;
    [SerializeField] private Transform CamRight;
    [SerializeField] private Transform CamUp;
    [SerializeField] private Transform CamDown;

    // Update is called once per frame
    void Update()
    {
        GetInput();
        MoveCamera();
    }

    private void GetInput()
    {
        _cameraX = Input.GetAxis("CameraX");
        _cameraY = Input.GetAxis("CameraY");
    }

    private void MoveCamera()
    {
        //Ideally we can physically move the camera as well as rotate it, to get the ability to turn out the window, or around in the cab, or over the hood. That's what the cam transforms are for. 
        //Had trouble figuring out the math of how we actually average those positions together so rip
    }

}
