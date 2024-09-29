using System.Collections.Generic;

namespace OpenDialouge
{
    public class DialogueSave
    {
        public List<DialogueRecord> Choices;

        public DialogueSave()
        {
            Choices = new List<DialogueRecord>();
        }
    }
}