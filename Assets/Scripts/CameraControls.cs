using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CameraTransforms
{
    public Transform lookTransform;
    public float weight;
}

public class CameraControls : MonoBehaviour
{

    [SerializeField] private float sensitivity = 100f;
    [SerializeField, Range(-90, 0)] private float verticalRangeNeg = -40f;
    [SerializeField, Range(0, 90)] private float verticalRangePos = 40f;
    [SerializeField, Range(-180, 0)] private float horizontalRangeNeg = -80f;
    [SerializeField, Range(0, 180)] private float horizontalRangePos = 80f;

    private float _cameraXInput;
    private float _cameraYInput;

    // Moves camera up and down by rotating on X axis.
    private float _localXRotation = 0f;
    private float _localYRotation = 0f;

    [SerializeField] private Transform CamBase;
    [SerializeField] private Transform CamLeft;
    [SerializeField] private Transform CamRight;
    [SerializeField] private Transform CamUp;
    [SerializeField] private Transform CamDown;
    
    [SerializeField] private CameraTransforms[] cameraTransforms;

    private Vector3 _initOffset;

    private void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
        _initOffset = transform.localPosition;
    }

    void Update()
    {
        GetInput();
        RotateCamera();
        MoveCamera();
    }

    private void GetInput()
    {
        _cameraXInput = Input.GetAxis("Mouse X") + Input.GetAxis("CameraX");
        _cameraYInput = Input.GetAxis("Mouse Y") + Input.GetAxis("CameraY");
    }

    private void RotateCamera()
    {
        //Ideally we can physically move the camera as well as rotate it, to get the ability to turn out the window, or around in the cab, or over the hood. That's what the cam transforms are for. 
        //Had trouble figuring out the math of how we actually average those positions together so rip

        float adjustedXInput = _cameraXInput * sensitivity * Time.deltaTime;
        float adjustedYInput = _cameraYInput * sensitivity * Time.deltaTime;
        
        //Debug.Log($"adjustedXInput = {adjustedXInput}, adjustedYInput = {adjustedYInput}");
        
        // Move camera vertically.
        _localXRotation += adjustedYInput;
        _localXRotation = Mathf.Clamp(_localXRotation, verticalRangeNeg, verticalRangePos);
        _localYRotation += adjustedXInput;
        _localYRotation = Mathf.Clamp(_localYRotation, horizontalRangeNeg, horizontalRangePos);
        
        transform.localRotation = Quaternion.Euler(_localXRotation, _localYRotation, 0);

        // Rotate player horizontally.
        //transform.Rotate(transform.up * adjustedXInput); // needs clamping like xrot
    }
    
    private void MoveCamera()
    {
        // Set camera to average position of each transform, weighted by how close the player forward is to each
        
        Vector3 newLocalPosition = Vector3.zero;

        foreach (CameraTransforms cameraTransform in cameraTransforms)
        {
            float dotProduct = Vector3.Dot(transform.forward, cameraTransform.lookTransform.forward);
            if (dotProduct < 0)
            {
                continue;
            }
            newLocalPosition = Vector3.Lerp(newLocalPosition, cameraTransform.lookTransform.localPosition, dotProduct);
        }

        transform.localPosition = newLocalPosition; // probably don't need the offset?
    }
}
