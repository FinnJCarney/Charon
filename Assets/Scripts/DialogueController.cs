using System.Collections;
using UnityEngine;
using Yarn.Unity;

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

        if (ObjectiveController.Instance == null)
        {
            Debug.LogWarning("ObjectiveController not present in scene! Dialogue will not work!");
        }
        DialogueRunnerInterrupts.AddCommandHandler("waitForCarToStop", WaitForCarToStop);
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
    
    private Coroutine WaitForCarToStop()
    {
        return StartCoroutine(WaitForCarToStopCO());
    }

    private IEnumerator WaitForCarToStopCO()
    {
        WaitForSeconds timeStep = new WaitForSeconds(.2f);
        Rigidbody vehicleRB = GetComponentInParent<Rigidbody>();
        float minSpeedForCompletion = .2f;
        
        while (vehicleRB != null && vehicleRB.velocity.sqrMagnitude > minSpeedForCompletion * minSpeedForCompletion)
        {
            yield return timeStep;
        }
    }
    
    [SerializeField] private string crashInterruptionNodeBase = "Crash";
    [SerializeField] private int crashInterruptionPriority = 10;
    public void InterruptLineForCrash()
    {
        InterruptConversation(crashInterruptionNodeBase, crashInterruptionPriority);
    }
    
    [SerializeField] private string arriveAtDestinationInterruptionNodeBase = "ArriveAtDestination";
    [SerializeField] private int arrivalInterruptionPriority = 50;
    public void InterruptLineForArriveAtDestination()
    {
        InterruptConversation(arriveAtDestinationInterruptionNodeBase, arrivalInterruptionPriority);
    }
    
    private string _currentInterruptReason = "";
    private int _currentInterruptPriority = 0;

    private void InterruptConversation(string reason, int priority)
    {
        //Debug.Log($"Is dialogue running: {DialogueRunnerInterrupts.IsDialogueRunning}, reason: {reason}, current reason: {_currentInterruptReason}, priority: {priority}, current priority: {_currentInterruptPriority}");
        // don't interrupt dialogue if a more important or same interrupt is currently occurring.
        if (DialogueRunnerInterrupts.IsDialogueRunning)
        {
            if (reason.Equals(_currentInterruptReason) || _currentInterruptPriority >= priority)
            {
                return;
            }
            else
            {
                DialogueRunnerInterrupts.Stop();
            }
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
            if (_currentInterruptReason.Equals(arriveAtDestinationInterruptionNodeBase))
            {
                DialogueRunnerMain.Stop();
            }
            DialogueRunnerMain.gameObject.SetActive(false);
            DialogueRunnerInterrupts.gameObject.SetActive(true);
            DialogueRunnerInterrupts.StartDialogue(interruptionNodeName);
            _currentInterruptReason = reason;
            _currentInterruptPriority = priority;
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
        Debug.Log($"Interrupt reason: {_currentInterruptReason}, does passenger exist? {(ObjectiveController.Instance.currentPassenger == null ? "no" : "yes")}");
        if (_currentInterruptReason.Equals(arriveAtDestinationInterruptionNodeBase) && ObjectiveController.Instance.currentPassenger == null)
        {
            DialogueRunnerMain.Stop();
        }
        _currentInterruptReason = "";
        _currentInterruptPriority = 0;
    }
}
