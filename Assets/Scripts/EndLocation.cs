using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndLocation : MonoBehaviour
{
    
    [SerializeField] private Passenger passenger;

    private bool _collisionTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (ObjectiveController.Instance.currentPassenger == passenger)
            {
                passenger.EndTrip();
                _collisionTriggered = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && _collisionTriggered)
        {
            gameObject.SetActive(false);
        }
    }
}
