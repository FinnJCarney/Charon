using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Passenger : MonoBehaviour
{
    public string yarnStartNode;
    [SerializeField] private Transform model;

    private void Start()
    {
        model.gameObject.SetActive(false);
    }

    public void BeginTrip()
    {
        model.gameObject.SetActive(true);
        ObjectiveController.Instance.PickUpPassenger(this);
    }

    public void EndTrip()
    {
        model.gameObject.SetActive(false);
        ObjectiveController.Instance.DropOffPassenger();
    }
}
