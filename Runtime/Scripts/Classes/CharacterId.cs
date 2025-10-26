namespace OpenDialogue
{
    /// <summary>
    /// Custom class for encapsulation of CharacterData
    /// </summary>
    public class CharacterId
    {
        public int id, expression;

        public CharacterId()
        {
        }

        public CharacterId(int id, int expresion)
        {
            this.id = id;
            this.expression = expresion;
        }
    }
}