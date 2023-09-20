using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;
using FMODUnity;

public class FMODManager : MonoBehaviour
{

    public static FMODManager i;

    private void Awake()
    {
        if(i == null)
        {
            FMODManager.i = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void PlayOneShotAttached(EventReference eventRef, Transform attachObj)
    {
        FMODUnity.RuntimeManager.PlayOneShotAttached(eventRef, attachObj.gameObject);

    }

    public FMOD.Studio.EventInstance CreateAttachedInstance(EventReference eventRef, Transform attachObj)
    {
        FMOD.Studio.EventInstance eventInst = FMODUnity.RuntimeManager.CreateInstance(eventRef);
        eventInst.start();
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(eventInst, attachObj);
        return eventInst;
    }

    public FMOD.Studio.PARAMETER_ID GetParameter(FMOD.Studio.EventInstance eventInst, string paramName)
    {
        FMOD.Studio.EventDescription eventDescription;
        eventInst.getDescription(out eventDescription);

        FMOD.Studio.PARAMETER_DESCRIPTION parameterDescription;
        eventDescription.getParameterDescriptionByName(paramName, out parameterDescription);

        UnityEngine.Debug.Log(parameterDescription.id);
        return parameterDescription.id;
    }
   
    public void SetInstanceParam(FMOD.Studio.EventInstance eventInst, string param, float value)
    {
        eventInst.setParameterByName(param, value, false);
    }
}
