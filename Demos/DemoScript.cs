using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if(!DialogueSystem.InDialogue)
            {
                OpenDialogueController.instance.StartDialogue();
            }
            else
            {
                if(!OpenDialogueController.instance.field.gameObject.activeInHierarchy)
                OpenDialogueController.instance.ContinueDialogue();
            }

        }
    }
}
