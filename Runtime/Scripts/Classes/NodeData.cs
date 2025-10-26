using System.Collections.Generic;
using UnityEngine;

namespace OpenDialogue
{
    /*public interface INodeData
    {
        public TextType DialogueType { get; set; }
        public string DialogueTag { get; set; }

       
    }
    public class AbstractNode : INodeData
    {
        public TextType DialogueType { get; set; }
        public string DialogueTag { get; set; }
        public AbstractNode(TextType t, string tag) 
        {
            DialogueType = t;
            DialogueTag = tag;
        }

    }
    public class SingleDialogue : AbstractNode
    {
        public string Text{ get; set; }
        public TextType Type { get { return DialogueType; } }
        public string Tag { get { return DialogueTag; } }
        public SingleDialogue(TextType t, string tag, string DialogueText) : base(t, tag)
        {
            Text = DialogueText;
        }
    }
    public class MultiDialogue : AbstractNode
    {
        public List<string> Choices { get; set; }
        public TextType Type { get { return DialogueType; } }
        public string Tag { get { return DialogueTag; } }
        public MultiDialogue(TextType t, string tag, List<string> ChoicesText) : base(t, tag)
        {
            Choices = ChoicesText;
        }
    }*/
    /// <summary>
    /// Data encapsulator used to send the Dialogue data to the player Dialogue Controller in a friedlier and more abstract format
    /// </summary>
    public class NodeData
    {
        public Dialogue dialogue;
        public List<Dialogue> Choices;
        public CharacterId character;
        public bool Pause;
        public TextType type;
        public string Tag;
        public AudioClip clip;

        //for utility nodes that have nothing to display
        public NodeData(bool pause = false, string tag = "", AudioClip _clip = null, TextType TextType = TextType.EmptyNode)
        {
            type = TextType;
            clip = _clip;
            Tag = tag;
            Pause = pause;
        }

        //for single Dialogue Node
        public NodeData(Dialogue dialogue = null, CharacterId ch = null, string tag = "")
        {
            character = ch;
            Tag = tag;
            this.dialogue = dialogue;
            type = TextType.SingleNode;
        }

        //for nodes with multiple choices
        public NodeData(List<Dialogue> dialogue = null, SubType subType = SubType.MultiNode, string tag = "")
        {
            Tag = tag;
            Choices = dialogue;
            type = subType == SubType.MultiNode ? TextType.MultiNode : TextType.MultiAltNode;
        }

        public NodeData(Dialogue dialogue, List<Dialogue> choices, CharacterId character, bool pause, TextType type, string tag, AudioClip clip)
        {
            this.dialogue = dialogue;
            Choices = choices;
            this.character = character;
            Pause = pause;
            this.type = type;
            Tag = tag;
            this.clip = clip;
        }
        public T Dialogue<T>() where T : class
        {
            if (typeof(T) == typeof(Dialogue))
            {
                return dialogue as T;
            }
            else if (typeof(T) == typeof(List<Dialogue>))
            {
                return Choices as T;
            }
            else
            {
                return null;
            }
        }
    }
}