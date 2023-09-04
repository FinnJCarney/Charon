using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndLocation : MonoBehaviour
{
    
    [SerializeField] private Passenger passenger;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // enter vehicle
            passenger.EndTrip();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // enter vehicle
            gameObject.SetActive(false);
        }
    }
}
