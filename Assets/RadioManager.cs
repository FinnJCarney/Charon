using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using TMPro;
using UnityEngine.InputSystem;

public class RadioManager : MonoBehaviour
{
    [Header("Audio Elements")]
    [SerializeField] private FMODUnity.EventReference radioRef;
    [SerializeField] private Transform radioTransform;
    private FMOD.Studio.EventInstance radioInst;
    private FMOD.Studio.PARAMETER_ID radioFreq;

    [Header("Visual Elements")]
    [SerializeField] Transform radioNeedle;
    [SerializeField] Vector3 maxExtents;
    [SerializeField] [Range(0f, 10f)] private float freqValue;
    [SerializeField] TextMeshPro tMP;
    [SerializeField] private float freqChangeSpeed = .2f;
    private Vector3 startingPos;
    private float freqChange = 0;

    void Start()
    {
        startingPos = radioNeedle.transform.localPosition;

        radioInst = FMODManager.i.CreateAttachedInstance(radioRef, radioTransform);
    }

    void FixedUpdate()
    {
        float newFreqValue = freqValue + freqChange;
        freqValue = Mathf.Clamp(newFreqValue, 0, 10f);
        
        FMODManager.i.SetInstanceParam(radioInst, "radioFreq", freqValue);
        radioNeedle.localPosition = startingPos + (maxExtents *(freqValue / 10f));

        tMP.text = (70f + (freqValue * 18.32f)).ToString("0.0");
    }
    
    public void RadioFrequencyInput(InputAction.CallbackContext context)
    {
        freqChange = context.ReadValue<float>() * Time.deltaTime * freqChangeSpeed;
    }
}
