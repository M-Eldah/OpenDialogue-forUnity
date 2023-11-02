using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The MainClass Which defines if our node will carry a txt or preform a function
public enum NodeType
{
    DialogueNode,
    UtilityNode
}
//The SubClass Which kind of node out node is, is it a cute small node does
//here is a quick explaination, there should be a clearer explaination in the document 
//if not then lol
public enum SubType
{
    //For Simple just Read Dialogue
    SingleNode,
    //For Choices
    MultiNode,
    //For activating Methods
    ActionNode,
    //for change the changing the value of Property or Fields
    Valuechangenode,
    //like Simple but returns a random line from several choices 
    RandomNode,
    //Multinode and Singlenode might have locked text/choice you can unlock them with this
    ChoiceUnlockNode,
    //Change the starting node of this dialogue
    StartChangeNode,
    //Play a certain audio
    AudioNode,
    //like Multinode but it uses a value to choose which choices to hide and show
    ValueChoiceNode,
    //Redirect the dialogue 
    ValueDirectionNode,
    //Like Random node but uses a value to modify which string will be returned 
    MRandomNode,
    AnimationNode,
    ScriptNode,
    StageControlNode,
    InputNode
}
//How the node is displayed
public enum TextType
{
    SingleNode,
    MultiNode,
    EmptyNode,
    AudioNode,
    MultiAltNode,
    InputNode
}
