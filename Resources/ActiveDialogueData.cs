using OpenDialogue;
using UnityEngine;

[CreateAssetMenu(fileName = "ActiveDialogueData", menuName = "Scriptable Objects/ActiveDialogueData")]
[System.Serializable]
public class ActiveDialogueData : ScriptableObject
{
    public DialogueSave DialogueSave;
    private void Awake()
    {
        DialogueSave = new DialogueSave();
    }
}
