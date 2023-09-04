using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
using TMPro;

public class DialogueController : MonoBehaviour
{
    
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
    
    // Start is called before the first frame update
    void Start()
    {
        LineView lv;
        OptionsListView olv;
        
        
        // cannot press continue while options active - disable it.
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnNodeStart()
    {
        Debug.Log("Line!");
    }
}
