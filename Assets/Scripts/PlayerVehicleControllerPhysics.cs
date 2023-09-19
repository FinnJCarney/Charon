using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


// borrowing heavily from https://www.youtube.com/watch?v=Z4HA8zJhGEk
public class VehicleControllerPhysics : MonoBehaviour
{

    [Header("Driving Mechanics Vars")]
    [SerializeField] private float maxForwardTorque = 500f;
    [SerializeField] private float minForwardOrBackTorque = 50f;
    [SerializeField] private float maxReverseTorque = 200f;
    [SerializeField] private float brakeTorque = 2000f;
    [SerializeField] private float handbrakeTorque = 4000f;
    [SerializeField] private float maxSteeringAngle = 30f;
    [SerializeField] private AnimationCurve gasInputCurve;

    private float _steeringInput = 0f;
    private float _gasPedalInput = 0f;
    private float _brakePedalInput = 0f;
    private bool _handbrakeOn = true;
    private bool _reverseOn = false;
    
    [SerializeField] private WheelCollider frontLeftWheelCollider;
    [SerializeField] private WheelCollider frontRightWheelCollider;
    [SerializeField] private WheelCollider backLeftWheelCollider;
    [SerializeField] private WheelCollider backRightWheelCollider;
    
    [SerializeField] private Transform frontLeftWheelTransform;
    [SerializeField] private Transform frontRightWheelTransform;
    [SerializeField] private Transform backLeftWheelTransform;
    [SerializeField] private Transform backRightWheelTransform;
    
    
    [SerializeField] private Transform steeringWheelTransform;
    [SerializeField] private float steeringWheelModelRotationMultiplier = 4f;

    [SerializeField] private Transform SpeedNeedleTransform;
    [SerializeField] [Range(0, 360)] private float SpeedNeedleMaxAngle;
    [SerializeField] private Vector3 SpeedNeedleStartRot;
    private Vector3 lastPos;
    
    public void SteeringInput(InputAction.CallbackContext context)
    {
        SteeringInput(context.ReadValue<float>());
    }
    
    public void SteeringInput(float value)
    {
        _steeringInput = value;
    }
    
    public void GasPedalInput(InputAction.CallbackContext context)
    {
        GasPedalInput(context.ReadValue<float>());
    }
    
    public void GasPedalInput(float value)
    {
        _gasPedalInput = value;
    }
    
    public void BrakePedalInput(InputAction.CallbackContext context)
    {
        BrakePedalInput(context.ReadValue<float>());
    }
    
    public void BrakePedalInput(float value)
    {
        _brakePedalInput = value;
    }
    
    public void HandbrakeInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            HandbrakeInputToggle();
        }
    }
    
    public void HandbrakeInputToggle()
    {
        _handbrakeOn = !_handbrakeOn;
    }
    
    public void ReverseInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _reverseOn = !_reverseOn;
        }
    }

    private void FixedUpdate()
    {
        //Debug.Log("Horizontal: " + _steeringInput + " Gas: " + _gasPedalInput + " Brake: " + _brakePedalInput + " HandBrake: " + _handbrakeInput);

        CalculateTorque();
        CalculateSteering();
        UpdateNeedle();
    }

    private void CalculateTorque()
    {
        float motorTorque = 0f;

        if (!_handbrakeOn)
        {
            float adjustedGasPedalInput = gasInputCurve.Evaluate(_gasPedalInput);
            if (!_reverseOn)
            {
                motorTorque = Mathf.Max(minForwardOrBackTorque, adjustedGasPedalInput * maxForwardTorque);
            }
            else
            {
                motorTorque = Mathf.Min(-minForwardOrBackTorque, adjustedGasPedalInput * -maxReverseTorque);
            }
        }
        
        frontLeftWheelCollider.motorTorque = motorTorque;
        frontRightWheelCollider.motorTorque = motorTorque;
        
        // braking

        float currentBrakeTorque = _handbrakeOn ? handbrakeTorque : _brakePedalInput * brakeTorque;
        
        frontLeftWheelCollider.brakeTorque = currentBrakeTorque;
        frontRightWheelCollider.brakeTorque = currentBrakeTorque;
        backLeftWheelCollider.brakeTorque = currentBrakeTorque;
        backRightWheelCollider.brakeTorque = currentBrakeTorque;
    }

    private void CalculateSteering()
    {
        float currentSteeringAngle = maxSteeringAngle * _steeringInput;
        frontLeftWheelCollider.steerAngle = currentSteeringAngle;
        frontRightWheelCollider.steerAngle = currentSteeringAngle;
        
        UpdateWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateWheel(frontRightWheelCollider, frontRightWheelTransform);
        UpdateWheel(backLeftWheelCollider, backLeftWheelTransform);
        UpdateWheel(backRightWheelCollider, backRightWheelTransform);

        steeringWheelTransform.localEulerAngles = new Vector3(0, currentSteeringAngle * steeringWheelModelRotationMultiplier, 0f);
    }
    
    private void UpdateWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        wheelCollider.GetWorldPose(out Vector3 pos, out Quaternion rot);
        wheelTransform.rotation = rot;
        wheelTransform.position = pos;
    }

    private void UpdateNeedle()
    {
        float currentSpeed = (this.transform.position - lastPos).magnitude / Time.fixedDeltaTime;
        SpeedNeedleTransform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(new Vector3(SpeedNeedleStartRot.x, SpeedNeedleStartRot.y + (currentSpeed * SpeedNeedleMaxAngle), SpeedNeedleStartRot.z)));
        lastPos = this.transform.position;
    }
}
