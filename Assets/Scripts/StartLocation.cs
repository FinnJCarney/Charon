using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartLocation : MonoBehaviour
{

    [SerializeField] private Passenger passenger;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // enter vehicle
            passenger.BeginTrip();
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
