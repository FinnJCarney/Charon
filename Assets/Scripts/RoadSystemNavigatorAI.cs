using System;
using System.Collections;
using System.Collections.Generic;
using Barmetler;
using Barmetler.RoadSystem;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.InputSystem;

public class RoadSystemNavigatorAI : MonoBehaviour
{

    [SerializeField] private Transform startTransform;
    [SerializeField] private Transform goalTransform;
    [SerializeField] private Transform tempGoalTransform;
    [SerializeField] private Transform tempGoalTransform1;
    [SerializeField] private Transform tempGoalTransform2;
    [SerializeField] private Transform tempGoalTransform3;
    [SerializeField] private Transform tempGoalTransform4;
    [SerializeField] private Transform tempGoalTransform5;
    
    [SerializeField] private float navigationSpeed = 1f;

    private bool _bAIActive = true;

    private RoadSystemNavigator _navigator;
    private Rigidbody _rb;
    private VehicleControllerPhysics _vehicleController;

    private void Awake()
    {
        _navigator = GetComponent<RoadSystemNavigator>();
        _rb = GetComponent<Rigidbody>();
        _vehicleController = GetComponent<VehicleControllerPhysics>();
    }

    private void Start()
    {
        StartCoroutine(AILoopCo());
        _navigator.Goal = goalTransform.position;
        _vehicleController.HandbrakeInputToggle();
    }

    private IEnumerator AILoopCo()
    {

        while (_bAIActive)
        {
            yield return new WaitForFixedUpdate();
            if (_navigator.CurrentPoints.Count > 5)
            {
                //transform.position = Vector3.Lerp(transform.position, _navigator.CurrentPoints[10].position, navigationSpeed * Time.fixedDeltaTime);
                Debug.Log($"Before debug squares: num = {_navigator.CurrentPoints.Count}");
                tempGoalTransform.transform.localPosition = _navigator.CurrentPoints[0].position;
                tempGoalTransform1.transform.localPosition = _navigator.CurrentPoints[1].position;
                tempGoalTransform2.transform.localPosition = _navigator.CurrentPoints[2].position;
                tempGoalTransform3.transform.localPosition = _navigator.CurrentPoints[3].position;
                tempGoalTransform4.transform.localPosition = _navigator.CurrentPoints[4].position;
                tempGoalTransform5.transform.localPosition = _navigator.CurrentPoints[5].position;
                Debug.Log($"AFTER debug squares: num = {_navigator.CurrentPoints.Count}");
                CalculateMovement(_navigator.CurrentPoints[5].ToLocalSpace(transform).position);
            }
        }
    }

    void CalculateMovement(Vector3 nextNodeLocalPosition)
    {
        //tempGoalTransform.transform.localPosition = nextNodeLocalPosition;
        //bool bNextNodeInFront = 
        //if (nextNodeLocalPosition)
        float turnAmount = Mathf.Lerp(-1, 1, .5f + nextNodeLocalPosition.x);
        _vehicleController.SteeringInput(turnAmount);
        bool bShouldTurnLeft = nextNodeLocalPosition.x < -.2f;
        bool bShouldTurnRight = nextNodeLocalPosition.x > .2f;
        if (bShouldTurnLeft)
        {
            //_vehicleController.SteeringInput(-1f);
        }
        else if (bShouldTurnRight)
        {
            //_vehicleController.SteeringInput(1f);
        }

        bool bShouldGoForward = nextNodeLocalPosition.z > nextNodeLocalPosition.x;
        bool bShouldBrake = IsApproachingTurn() && _rb.velocity.sqrMagnitude > 4;
        if (bShouldBrake)
        {
            _vehicleController.GasPedalInput(0f);
            _vehicleController.BrakePedalInput(1f);
        }
        else
        {
            _vehicleController.BrakePedalInput(0f);
            if (bShouldGoForward)
            {
                _vehicleController.GasPedalInput(1f);
            }
            else
            {
                _vehicleController.GasPedalInput(0f);
            }
        }


        if (false)//Physics.BoxCast(transform.position, new Vector3(.5f, .5f, .5f), transform.forward, transform.rotation, 1f))
        {
            // don't move!
        }
        else
        {
            // turn first
            //RB.rota
            // move!
            Debug.Log($"Target: {nextNodeLocalPosition}");
            Vector3 targetPosition = transform.position + transform.forward * (navigationSpeed * Time.fixedDeltaTime);
            //RB.MovePosition(targetPosition);
            //transform.position = Vector3.Lerp(transform.position, nextNodeLocalPosition, navigationSpeed * Time.fixedDeltaTime);
        }
    }

    bool IsApproachingTurn()
    {
        
        Debug.Log($"Before IsApproachingTurn: num = {_navigator.CurrentPoints.Count}");
        for (int i = 1; i < _navigator.CurrentPoints.Count - 1 && i < 10; i++)
        {
            Vector3 prevNodePos = _navigator.CurrentPoints[i - 1].position;
            Vector3 curNodePos = _navigator.CurrentPoints[i].position;
            Vector3 nextNodePos = _navigator.CurrentPoints[i + 1].position;

            Vector3 currentHeading = (curNodePos - prevNodePos).normalized;
            Vector3 nextHeading = (nextNodePos - curNodePos).normalized;

            if (currentHeading == Vector3.zero || nextHeading == Vector3.zero)
            {
                continue;
            }

            float dot = Vector3.Dot(currentHeading, nextHeading);
            if (dot < .5f)
            {
                Debug.Log($"Should brake! i {i} prevNodePos {prevNodePos} curNodePos {curNodePos} nextNodePos {nextNodePos} currentHeading {currentHeading} nextHeading {nextHeading} dot {dot}");
                return true;
            }
        }

        return false;
    }

}
