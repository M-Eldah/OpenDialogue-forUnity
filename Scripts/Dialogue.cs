namespace OpenDialogue
{
    /// <summary>
    /// Custom class for encapsulation of DialogueLines
    /// </summary>
    public class Dialogue
    {
        public string Text;
        public bool locked;
        public bool alt;
        public Dialogue(string text, bool locked)
        {
            Text = text;
            this.locked = locked;
        }

        public Dialogue(string text, bool locked, bool alt) : this(text, locked)
        {
            this.alt = alt;
        }

    }
}