using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// borrowing heavily from https://www.youtube.com/watch?v=Z4HA8zJhGEk
public class VehicleControllerPhysics : MonoBehaviour
{

    [Header("Driving Mechanics Vars")]
    [SerializeField] private float maxForwardTorque = 500f;
    [SerializeField] private float minForwardOrBackTorque = 50f;
    [SerializeField] private float maxReverseTorque = 200f;
    [SerializeField] private float brakeTorque = 2000f;
    [SerializeField] private float handbrakeTorque = 4000f;
    [SerializeField] private float rateForwardAcceleration = 1f;
    [SerializeField] private float rateReverseAcceleration = 1f;
    [SerializeField] private float rateBrakeDeceleration = 1f;
    [SerializeField] private float rateNaturalDeceleration = .1f;
    [SerializeField] private float rateTurningSpeed = .1f;
    [SerializeField] private float brakeGripModifier = 1f;
    [SerializeField] private float handbrakeGripModifier = 5f;
    [SerializeField] private float rateHandbrakeDeceleration = 10f;
    [SerializeField] private float maxSteeringAngle = 30f;
    [SerializeField] private AnimationCurve gasInputCurve;
    [SerializeField] private AnimationCurve gripCurve;

    private float _steeringInput = 0f;
    private float _gasPedalInput = 0f;
    private float _brakePedalInput = 0f;
    private bool _handbrakeInput = false;
    private bool _reverseOn = false;
    
    [SerializeField] private WheelCollider frontLeftWheelCollider;
    [SerializeField] private WheelCollider frontRightWheelCollider;
    [SerializeField] private WheelCollider backLeftWheelCollider;
    [SerializeField] private WheelCollider backRightWheelCollider;
    
    [SerializeField] private Transform frontLeftWheelTransform;
    [SerializeField] private Transform frontRightWheelTransform;
    [SerializeField] private Transform backLeftWheelTransform;
    [SerializeField] private Transform backRightWheelTransform;

    void Start()
    {
        
    }

    void Update()
    {
        GetInput();
    }

    private void GetInput()
    {
        _steeringInput = Input.GetAxis("Steering");
        _gasPedalInput = Input.GetAxis("Gas");
        _brakePedalInput = Input.GetAxis("Brake");
        _handbrakeInput = Input.GetButton("Handbrake");
        if (Input.GetButtonDown("ToggleReverse"))
        {
            _reverseOn = !_reverseOn;
        }
    }

    private void FixedUpdate()
    {
        //Debug.Log("Horizontal: " + _steeringInput + " Gas: " + _gasPedalInput + " Brake: " + _brakePedalInput + " HandBrake: " + _handbrakeInput);

        CalculateTorque();
        CalculateSteering();
    }

    private void CalculateTorque()
    {
        float motorTorque = 0f;

        if (!_handbrakeInput)
        {
            if (!_reverseOn)
            {
                motorTorque = Mathf.Max(minForwardOrBackTorque, _gasPedalInput * maxForwardTorque);
            }
            else
            {
                motorTorque = Mathf.Min(-minForwardOrBackTorque, _gasPedalInput * -maxReverseTorque);
            }
        }
        
        frontLeftWheelCollider.motorTorque = motorTorque;
        frontRightWheelCollider.motorTorque = motorTorque;
        
        // braking

        float currentBrakeTorque = _handbrakeInput ? handbrakeTorque : _brakePedalInput * brakeTorque;
        
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
    }
    
    private void UpdateWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        wheelCollider.GetWorldPose(out Vector3 pos, out Quaternion rot);
        wheelTransform.rotation = rot;
        wheelTransform.position = pos;
    }
}
