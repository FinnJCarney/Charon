using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCollisionManager : MonoBehaviour
{
    [SerializeField] private float minSpeedForCrash = 2f;
    
    private void OnCollisionEnter(Collision other)
    {
        if (other.relativeVelocity.sqrMagnitude > minSpeedForCrash * minSpeedForCrash)
        {
            DialogueController.Instance.InterruptLineForCrash();
        }
    }
}
