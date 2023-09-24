using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherManager : MonoBehaviour
{
    [SerializeField] private Transform trackingTransform;
    private Vector3 offset;

    private void Start()
    {
        offset = this.transform.position - trackingTransform.position;
    }
    private void FixedUpdate()
    {
        this.transform.position = trackingTransform.position + offset;
    }
}
