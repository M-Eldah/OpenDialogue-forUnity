using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenDialouge;
public class DemoScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DialogueSystem.Load("Dialogue");   
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if(!DialogueSystem.inDialogue)
            {
                OpenDialogueController.instance.StartDialogue();
            }
            else
            {
                if(!OpenDialogueController.instance.Inputmode)
                OpenDialogueController.instance.ContinueDialogue();
            }

        }
    }
}
