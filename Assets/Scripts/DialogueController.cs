using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
using TMPro;

public class DialogueController : MonoBehaviour
{
    public static DialogueController Instance;

    private LineView _yarnLineView;
    public LineView YarnLineView
    {
        get
        {
            Debug.Assert(_yarnLineView != null);
            return _yarnLineView;
        }
        private set => _yarnLineView = value;
    }

    [SerializeField] private DialogueRunner DialogueRunnerMain;
    [SerializeField] private DialogueRunner DialogueRunnerInterrupts;
    [SerializeField] private CanvasGroup MainDialogueListView;

    private void Awake()
    {
        // Ensure that there is only one instance of the DialogueManager.
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
        
        DialogueRunnerMain.AddCommandHandler<float>("waitTime", WaitTime);
        
        DialogueRunnerInterrupts.onNodeComplete.AddListener(OnInterruptComplete);
        DialogueRunnerInterrupts.gameObject.SetActive(false);
    }

    public void StartDialogue(string yarnStartNode)
    {
        if (DialogueRunnerMain.IsDialogueRunning)
        {
            Debug.Log($"Dialogue node {DialogueRunnerMain.CurrentNodeName} currently running while trying to start {yarnStartNode}!");
            DialogueRunnerMain.Stop();
            MainDialogueListView.alpha = 0f;
        }
        DialogueRunnerMain.StartDialogue(yarnStartNode);
    }

    public void OnNodeStart()
    {
        Debug.Log("Line!");
    }

    public void StopAllDialogue()
    {
        DialogueRunnerMain.Stop();
        DialogueRunnerInterrupts.Stop();
    }
    
    public void StopConversationDialogue()
    {
        DialogueRunnerMain.Stop();
    }
    
    // This is a default function and is not needed...
    private Coroutine WaitTime(float time = 1f)
    {
        return StartCoroutine(WaitTimeCO(time));
    }

    private IEnumerator WaitTimeCO(float time)
    {
        yield return new WaitForSeconds(time);
    }

    private void SetAnimation(string animationName)
    {
        // Set animation of current passenger(s)
    }
    
    [SerializeField] private string crashInterruptionNodeBase = "Crash";
    public void InterruptLineForCrash()
    {
        InterruptConversation(crashInterruptionNodeBase);
    }
    
    [SerializeField] private string arriveAtDestinationInterruptionNodeBase = "ArriveAtDestination";
    public void InterruptLineForArriveAtDestination()
    {
        InterruptConversation(arriveAtDestinationInterruptionNodeBase);
    }

    private void InterruptConversation(string reason)
    {
        if (DialogueRunnerInterrupts.IsDialogueRunning)
        {
            return;
        }
        string interruptionNodeName;
        if (ObjectiveController.Instance.currentPassenger != null)
        {
            interruptionNodeName = ObjectiveController.Instance.currentPassenger.yarnStartNode + reason;
        }
        else
        {
            interruptionNodeName = "Start" + reason;
        }
        
        Debug.Log($"Interrupting dialogue! interruptionNode: {interruptionNodeName}");
        if (DialogueRunnerInterrupts.NodeExists(interruptionNodeName))
        {
            Debug.Log("Node exists!");
            DialogueRunnerMain.gameObject.SetActive(false);
            DialogueRunnerInterrupts.gameObject.SetActive(true);
            DialogueRunnerInterrupts.StartDialogue(interruptionNodeName);
        }
        else
        {
            Debug.Log("Node does not exist!");
        }
    }

    private void OnInterruptComplete(string arg0)
    {
        Debug.Log("Interrupt complete!");
        DialogueRunnerInterrupts.gameObject.SetActive(false);
        DialogueRunnerMain.gameObject.SetActive(true);
    }
}
