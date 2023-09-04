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

    private DialogueRunner _yarnDialogueRunner;
    public DialogueRunner DialogueRunner
    {
        get
        {
            Debug.Assert(_yarnDialogueRunner != null);
            return _yarnDialogueRunner;
        }
        private set => _yarnDialogueRunner = value;
    }

    private void Awake()
    {
        // Ensure that there is only one instance of the DialogueManager.
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
        
        DialogueRunner = GetComponent<DialogueRunner>();
    }

    public void StartDialogue(string yarnStartNode)
    {
        if (_yarnDialogueRunner.IsDialogueRunning)
        {
            Debug.Log($"Dialogue node {_yarnDialogueRunner.CurrentNodeName} currently running while trying to start {yarnStartNode}!");
            _yarnDialogueRunner.Stop();
        }
        _yarnDialogueRunner.StartDialogue(yarnStartNode);
    }

    public void OnNodeStart()
    {
        Debug.Log("Line!");
    }

    public void StopDialogue()
    {
        _yarnDialogueRunner.Stop();
    }
}
