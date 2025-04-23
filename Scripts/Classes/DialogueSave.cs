using System.Collections.Generic;
namespace OpenDialogue
{
    [System.Serializable]
    public class DialogueSave
    {
        public List<DialogueRecord> Dialogues;

        public DialogueSave()
        {
            Dialogues = new List<DialogueRecord>();
        }
    }
}