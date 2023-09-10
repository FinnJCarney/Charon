using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCollisionManager : MonoBehaviour
{
    [SerializeField] private float minSpeedForCrash = 2f;
    [SerializeField] private float crashInterruptDialogueCooldown = 5f;
    private float _timeAtLastCrashInterruptDialogue = 0f;
    
    private void OnCollisionEnter(Collision other)
    {
        if (other.relativeVelocity.sqrMagnitude > minSpeedForCrash * minSpeedForCrash)
        {
            // Crash occured
            
            if (Time.time > _timeAtLastCrashInterruptDialogue + crashInterruptDialogueCooldown)
            {
                _timeAtLastCrashInterruptDialogue = Time.time;
                DialogueController.Instance.InterruptLineForCrash();
            }
        }
    }
}
