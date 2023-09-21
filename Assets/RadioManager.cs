using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using TMPro;

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
    private Vector3 startingPos;

    void Start()
    {
        startingPos = radioNeedle.transform.localPosition;

        radioInst = FMODManager.i.CreateAttachedInstance(radioRef, radioTransform);
    }

    void FixedUpdate()
    {
        FMODManager.i.SetInstanceParam(radioInst, "radioFreq", freqValue);
        radioNeedle.localPosition = startingPos + (maxExtents *(freqValue / 10f));

        tMP.text = (70f + (freqValue * 18.32f)).ToString();
    }
}
