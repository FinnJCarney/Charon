using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using FMODUnity;


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

    [Header("Visual Elements")]

    [SerializeField] private Transform steeringWheelTransform;
    [SerializeField] private float steeringWheelModelRotationMultiplier = 4f;

    [SerializeField] private Transform speedNeedleTransform;
    [SerializeField] [Range(0.1f, 20)] private float speedNeedleAngleMultiplier;
     private Vector3 _speedNeedleStartRot;
    private Vector3 _lastPos;

    [Header("Audio Elements")]
    [SerializeField] private FMODUnity.EventReference engineRef;
    [SerializeField] private Transform engineTransform;
    [SerializeField] private FMODUnity.EventReference gearRef;
    [SerializeField] private Transform gearTransform;
    private FMOD.Studio.EventInstance _engineInst;
    private FMOD.Studio.EventInstance _gearInst;
    private FMOD.Studio.PARAMETER_ID _torque;

    private float _currentSpeed;
    private float _currentForwardWheelSlipAvg = 0f;
    private float _currentSidewaysWheelSlipAvg = 0f;


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

        if (_handbrakeOn)
        {
            PlayGearShiftAudio(1);
        }
        else
        {
            PlayGearShiftAudio(2);
        }
    }

    public void ReverseInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _reverseOn = !_reverseOn;
        }
        
        if(_reverseOn)
        {
            PlayGearShiftAudio(4);
        }
        else
        {
            PlayGearShiftAudio(3);
        }
    }

    private void Start()
    {
        SetNeedle();
        CreateAudioEvents();
    }

    private void SetNeedle()
    {
        if (speedNeedleTransform != null)
        {
            _speedNeedleStartRot = speedNeedleTransform.rotation.eulerAngles;
        }
    }

    private void CreateAudioEvents()
    {
        _engineInst = FMODManager.i.CreateAttachedInstance(engineRef, engineTransform);
        _gearInst = FMODManager.i.CreateAttachedInstance(gearRef, gearTransform);

    }

    private void FixedUpdate()
    {
        //Debug.Log("Horizontal: " + _steeringInput + " Gas: " + _gasPedalInput + " Brake: " + _brakePedalInput + " HandBrake: " + _handbrakeInput);

        CalculateTorque();
        CalculateSteering();
        CalculateSpeed();
        CalculateSkidding();
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

        UpdateAudio(motorTorque / maxForwardTorque * (_currentSpeed / 10));
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

    private void CalculateSpeed()
    {
        _currentSpeed = (this.transform.position - _lastPos).magnitude / Time.fixedDeltaTime;
        _lastPos = this.transform.position;
    }
    
    private void CalculateSkidding()
    {
        frontLeftWheelCollider.GetGroundHit(out WheelHit hitFL);
        frontRightWheelCollider.GetGroundHit(out WheelHit hitFR);
        backLeftWheelCollider.GetGroundHit(out WheelHit hitBL);
        backRightWheelCollider.GetGroundHit(out WheelHit hitBR);
        _currentForwardWheelSlipAvg = 0f;
        _currentForwardWheelSlipAvg =+ hitFL.forwardSlip;
        _currentForwardWheelSlipAvg =+ hitFR.forwardSlip;
        _currentForwardWheelSlipAvg /= 2;
        
        _currentSidewaysWheelSlipAvg = 0f;
        _currentSidewaysWheelSlipAvg =+ hitFL.sidewaysSlip;
        _currentSidewaysWheelSlipAvg =+ hitFR.sidewaysSlip;
        _currentSidewaysWheelSlipAvg =+ hitBL.sidewaysSlip;
        _currentSidewaysWheelSlipAvg =+ hitBR.sidewaysSlip;
        _currentSidewaysWheelSlipAvg /= 4;
        
        //Debug.Log($"ForwardSlip: {_currentForwardWheelSlipAvg}, SidewaysSlip: {_currentSidewaysWheelSlipAvg}");
    }

    //Visual Elements

    private void UpdateWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        wheelCollider.GetWorldPose(out Vector3 pos, out Quaternion rot);
        wheelTransform.rotation = rot;
        wheelTransform.position = pos;
    }

    private void UpdateNeedle()
    {
        if (speedNeedleTransform != null)
        {
            speedNeedleTransform.SetLocalPositionAndRotation(Vector3.zero,
                Quaternion.Euler(new Vector3(_speedNeedleStartRot.x,
                    _speedNeedleStartRot.y + (_currentSpeed * speedNeedleAngleMultiplier), _speedNeedleStartRot.z)));
        }
    }

    //Audio Elements
    private void PlayGearShiftAudio(int value)
    {
        FMODManager.i.SetInstanceParam(_gearInst, "gearShift", value);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(_gearInst, gearTransform);
        _gearInst.start();
    }


    private void UpdateAudio(float motorTorque)
    {
        FMODManager.i.SetInstanceParam(_engineInst, "torque", motorTorque);
    }
}
