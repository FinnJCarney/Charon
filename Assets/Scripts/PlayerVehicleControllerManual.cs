using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleControllerManual : MonoBehaviour
{

    [Header("Driving Mechanics Vars")]
    [SerializeField] private float maxForwardSpeed = 10f;
    [SerializeField] private float minForwardOrBackSpeed = .5f;
    [SerializeField] private float maxReverseSpeed = 2f;
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
    private float _handbrakeInput = 0f;

    private bool _reverseOn = false;

    private CharacterController _characterController;
    
    
    
    void Start()
    {
        if (!TryGetComponent(out _characterController))
        {
            Debug.Log(name + " does not contain a Character Controller.");
        }
    }

    void Update()
    {
        _steeringInput = Input.GetAxis("Horizontal");
        _gasPedalInput = Input.GetAxis("Gas");
        _brakePedalInput = Input.GetAxis("Brake");
        _handbrakeInput = Input.GetAxis("Handbrake");
        if (Input.GetButtonDown("ToggleReverse"))
        {
            _reverseOn = !_reverseOn;
        }
    }

    private void FixedUpdate()
    {
        Debug.Log("Horizontal: " + _steeringInput + " Gas: " + _gasPedalInput + " Brake: " + _brakePedalInput + " HandBrake: " + _handbrakeInput);
        
        // Ideal Momentum
        bool isMovingForward = Vector3.Dot(_characterController.velocity, transform.forward) > 0;
        
        // Calculate direction/turning first!
        // first do it instantly
        Quaternion newRotation = CalculateNewForwardDirection();
        //_characterController.attachedRigidbody.MoveRotation(newRotation);
        transform.rotation = newRotation;
        
        
        // Calculate speed
        
        float idealSpeedFromGas = _gasPedalInput * rateForwardAcceleration * Time.fixedDeltaTime;// * forwardVelocityLag;
        
        if (isMovingForward)
        {
            
        }
        else
        {
            
        }

        Vector3 newVelocity = CalculateNewForwardSpeed() * Time.fixedDeltaTime * transform.forward;
        
        Debug.Log("newVelocity: " + newVelocity);
        _characterController.Move(newVelocity);
    }

    Quaternion CalculateNewForwardDirection()
    {
        // todo: add grippp
        
        float steeringAmount = _steeringInput * maxSteeringAngle;
        // if is moving backward, reverse steering
        if (Vector3.Dot(_characterController.velocity, transform.forward) < 0)
        {
            steeringAmount *= -1f;
        }

        float currentAngle = Vector3.SignedAngle(Vector3.forward, transform.forward, transform.up); // todo: could add lag to this
        float desiredAngle = currentAngle + steeringAmount;
        float angleChangeThisFixedUpdate = _characterController.velocity.magnitude * rateTurningSpeed * Time.fixedDeltaTime; // todo: add a curve to this - higher speed should mean slower turning
        float newAngle = Mathf.Lerp(currentAngle, desiredAngle, angleChangeThisFixedUpdate);
        Debug.Log("newAngle " + newAngle);
        return Quaternion.AngleAxis(newAngle, transform.up);
    }

    float CalculateNewForwardSpeed()
    {
        // todo: add grippp
        
        float currentSpeed = _characterController.velocity.magnitude;
        // if is moving backward, currentSpeed is negative
        if (Vector3.Dot(_characterController.velocity, transform.forward) < 0)
        {
            currentSpeed *= -1f;
        }
        // 50% press = 50% of max speed
        // cars have a minimum non-brake speed
        float desiredSpeed = GetDesiredSpeedFromGas();
        float accelerationThisFixedUpdate = 0f;
        
        // Natural Deceleration
        if (_gasPedalInput > .05f)
        {
            if (_reverseOn)
            {
                accelerationThisFixedUpdate = _gasPedalInput * rateReverseAcceleration * Time.fixedDeltaTime; // less gas = less acceleration
            }
            else
            {
                accelerationThisFixedUpdate = _gasPedalInput * rateForwardAcceleration * Time.fixedDeltaTime; // less gas = less acceleration
            }
        }
        else
        {
            if (_characterController.velocity.magnitude > minForwardOrBackSpeed)
            {
                accelerationThisFixedUpdate = rateNaturalDeceleration * Time.fixedDeltaTime; // if gas pedal is 0, desired Velocity will be 0, so we will lerp DOWN
            }
            else
            {
                accelerationThisFixedUpdate = (1 - _brakePedalInput) * rateForwardAcceleration * Time.fixedDeltaTime; // if gas pedal is 0, desired Velocity will be 0, so we will lerp DOWN
            }
        }

        accelerationThisFixedUpdate = Mathf.Clamp01(accelerationThisFixedUpdate);
        float newSpeedFromGas = Mathf.Lerp(currentSpeed, desiredSpeed, accelerationThisFixedUpdate);
        
        Debug.Log("currentSpeed: " + currentSpeed + " desiredSpeed: " + desiredSpeed + " accelerationThisFixedUpdate: " + accelerationThisFixedUpdate + " newSpeedFromGas: " + newSpeedFromGas);
        
        // Brake Input
        float desiredSpeedFromBrake = 0f;
        float brakeDecelerationThisFixedUpdate = _brakePedalInput * rateBrakeDeceleration * Time.fixedDeltaTime;
        float newSpeedFromGasAndBrake = Mathf.Lerp(newSpeedFromGas, desiredSpeedFromBrake, brakeDecelerationThisFixedUpdate);
        
        Debug.Log("brakeDecelerationThisFixedUpdate: " + brakeDecelerationThisFixedUpdate + " newSpeedFromGasAndBrake: " + newSpeedFromGasAndBrake);
        
        return newSpeedFromGasAndBrake;
    }

    float GetDesiredSpeedFromGas()
    {
        float toReturn = 0f;

        if (_reverseOn)
        {
            toReturn = Mathf.Min(-minForwardOrBackSpeed, _gasPedalInput * -maxReverseSpeed);
        }
        else
        {
            toReturn = Mathf.Max(minForwardOrBackSpeed, _gasPedalInput * maxForwardSpeed);
        }

        return toReturn;
    }
}
