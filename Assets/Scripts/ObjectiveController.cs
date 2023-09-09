using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveController : MonoBehaviour
{
    public static ObjectiveController Instance;
    
    public Passenger currentPassenger;

    private void Awake()
    {
        // Ensure that there is only one instance of the ObjectiveController.
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }

    public void PickUpPassenger(Passenger passenger)
    {
        currentPassenger = passenger;
        DialogueController.Instance.StartDialogue(passenger.yarnStartNode);
    }
    
    public void DropOffPassenger()
    {
        DialogueController.Instance.StopConversationDialogue();
        DialogueController.Instance.InterruptLineForArriveAtDestination();
        currentPassenger = null;
    }
}
