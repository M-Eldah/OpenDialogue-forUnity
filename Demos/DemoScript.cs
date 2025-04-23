using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenDialogue;
using UnityEngine.SceneManagement;
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
    public void AddKey()
    {
        DialogueSystem.AddKey("MetDamos", "Didn't MetHim");
    }
    public void SwitchScene(int i)
    {
        SceneManager.LoadScene(i);
    }
}
