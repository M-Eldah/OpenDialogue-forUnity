using System;
using System.Collections.Generic;
using UnityEngine;
public class DialogueContainer:MonoBehaviour
{
    public DialogueData Dialogue;
}
[System.Serializable]
public struct DialogueValues
{
    public string Name;
    public int startIndex;
    public NodeDB[] nodes;
    public DialogueValues(int leaveEmpty=-1)
    {
        Name= string.Empty;
        startIndex = -1;
        nodes = new NodeDB[0];
    }
    public DialogueValues(DialogueData data)
    {
        Name = data.name;
        startIndex=data.startIndex;
        
        nodes=data.Nodes.ToArray();
    }
}
//The class used to save the Dialogue
[System.Serializable]
public class DialogueData
{
    public string name;
    public int id;
    public int startIndex;
    [SerializeField]
    public List<NodeDB> Nodes;
    [SerializeField]
    public List<GroupsDB> Group;
    public int groudid;
}
//The class used to save the Nodes
[System.Serializable]
public class NodeDB
{
    public string name;

    public int id;

    public List<string> dialogueText;

    public List<string> extraValues;

    public List<string> choices;

    public Vector2 pos;

    public SubType subType;
    public NodeType NodeType;

    public List<int> ConnectedNodes;

    public string q_string1;

    public string q_string2;
 
    public bool q_bool1;

    public bool q_bool2;

    public string Tag;
    public NodeDB()
    {
        dialogueText = new List<string>();
        extraValues = new List<string>();
        choices = new List<string>();
        ConnectedNodes = new List<int>();

    }

    public NodeDB(string name, int id, List<string> dialogueText, List<string> extraValues, List<string> choices, Vector2 pos, NodeType nodeType, List<int> connectedNodes, SubType subType, string q_string1, string q_string2, bool q_bool1, bool q_bool2, string tag)
    {
        this.name = name;
        this.id = id;
        this.dialogueText = dialogueText;
        this.extraValues = extraValues;
        this.choices = choices;
        this.pos = pos;
        this.NodeType = nodeType;
        this.ConnectedNodes = connectedNodes;
        this.subType = subType;
        this.q_string1 = q_string1;
        this.q_string2 = q_string2;
        this.q_bool1 = q_bool1;
        this.q_bool2 = q_bool2;
        this.Tag = tag;
    }

    public NodeDB Clone(int id)
    {
        NodeDB node = new NodeDB();
        node.id = id;
        node.name= $"{subType}-ID:{id}";
        node.dialogueText.AddRange(dialogueText);
        node.extraValues.AddRange(extraValues);
        node.choices.AddRange(choices);
        node.ConnectedNodes.AddRange(ConnectedNodes);
        node.pos = pos+new Vector2(100,100);
        node.NodeType= NodeType;
        node.subType= subType;
        node.q_string1 = q_string1;
        node.q_string2 = q_string2;
        node.q_bool1= q_bool1;
        node.q_bool2= q_bool2;
        node.Tag = Tag;

        return node;
    }

    public void reMapConnections(Dictionary<int,int> mapper)
    {
        for (int i = 0; i < ConnectedNodes.Count; i++)
        {
            if (ConnectedNodes[i] != -1)
            { ConnectedNodes[i] = mapper[ConnectedNodes[i]]; }
        }
    }
}

//this is used to save Groups
//have you been using Groups to make your dialogue easier to navigate, you should
[Serializable]
public class GroupsDB
{

    public string GroupName;

    public string GName
    {
        get { return GroupName; }
        set { Debug.Log(value);GroupName= value; }
    }
                                         
    public List<int> ContainedNodes;

    public Vector2 Position;
    public GroupsDB()
    {
        ContainedNodes = new List<int>();
    }
    public GroupsDB(string name, List<int> _Cnodes, Vector2 pos)
    {
        GroupName = name;
        ContainedNodes = _Cnodes;
        Position = pos;
    }
}
