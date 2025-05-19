namespace OpenDialogue
{
    /// <summary>
    /// Custom class for encapsulation of DialogueLines
    /// </summary>
    public class Dialogue
    {
        public string Text;
        public string altText;
        public bool locked;
        public bool check;
        public Dialogue(string text, bool locked)
        {
            Text = text;
            this.locked = locked;
        }

        public Dialogue(string text,bool locked, bool alt, string altText) : this(text, locked)
        {

            this.check = alt;
            this.altText = altText;
        }

    }
}