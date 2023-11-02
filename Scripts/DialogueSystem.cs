using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Custom class for encapsulation of CharacterData
/// </summary>
public class characterId
{
    public int id, expression;

    public characterId()
    {
    }

    public characterId(int id, int expresion)
    {
        this.id = id;
        this.expression = expresion;
    }
}

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

/// <summary>
/// Data encapsulator used to send the Dialogue data to the player Dialogue Controller in a friedlier and more abstract format
/// </summary>
public class nodeData
{
    public Dialogue dialogue;
    public List<Dialogue> Choices;
    public characterId character;
    public bool Pause;
    public TextType type;
    public string Tag;
    public AudioClip clip;

    //for utility nodes that have nothing to display
    public nodeData(bool pause = false, string tag = "", AudioClip _clip = null, TextType TextType = TextType.EmptyNode)
    {
        type = TextType;
        clip = _clip;
        Tag = tag;
        Pause = pause;
    }

    //for single Dialogue Node
    public nodeData(Dialogue dialogue = null, characterId ch = null, string tag = "")
    {
        character = ch;
        Tag = tag;
        this.dialogue = dialogue;
        type = TextType.SingleNode;
    }

    //for nodes with multiple choices
    public nodeData(List<Dialogue> dialogue = null, SubType subType = SubType.MultiNode, string tag = "")
    {
        Tag = tag;
        Choices = dialogue;
        type = subType == SubType.MultiNode ? TextType.MultiNode : TextType.MultiAltNode;
    }

    public nodeData(Dialogue dialogue, List<Dialogue> choices, characterId character, bool pause, TextType type, string tag, AudioClip clip)
    {
        this.dialogue = dialogue;
        Choices = choices;
        this.character = character;
        Pause = pause;
        this.type = type;
        Tag = tag;
        this.clip = clip;
    }
}

/// <summary>
/// The Main Dialogue System resposible for Analyzing the dialogue data and sending it to the controller
/// </summary>

public class DialogueSave
{
    public List<DialogueRecord> Choices;
}

public static class DialogueSystem
{
    public delegate void DialogueEnd();

    public static event DialogueEnd Dialogueend;

    public delegate void DialogueNext();

    public static event DialogueNext Dialoguenext;

    private static DialogueValues data;
    private static Dictionary<int, NodeDB> Nodes;
    public static bool InDialogue;
    public static int currentindex;
    private static DialogueRecord Drecord;
    private static DialogueSave dialogueSave;
    public static int CommentIndex;

    /// <summary>
    /// Calls the DialogueNext Event which is used to force the Next Dialogue line to be loaded
    /// </summary>
    public static void Next()
    {
        Dialoguenext();
    }

    /// <summary>
    /// Calls the Dialogueend Event which notify the Dialogue Controller that the Dialogue has ended,
    /// and to do all releated methods
    /// </summary>
    public static void End()
    {
        Dialogueend();
    }

    /// <summary>
    /// Load the DialogueData
    /// </summary>
    /// <param name="_data">The Dualouge Data</param>
    /// <param name="startnode">The starting Node Id</param>
    /// <returns></returns>
    public static nodeData DStart(DialogueValues _data, int startnode = -1)
    {
        data = _data;
        CommentIndex = 0;
        //turn Node list to dictionary
        Nodes = NodeDictionary(data.nodes);
        //change teh static bool to indicate we are in Dialogue
        InDialogue = true;
        //Load save data
        Drecord = LoadRecord(data.Name);
        //check the starting index
        if (startnode == -1)
        {
            //Check if the Dia
            currentindex = Drecord.startModified ? Drecord.startindex : data.startIndex;
        }
        else
        {
            currentindex = startnode;
        }
        //return the new node data
        return NodeDataReturn();
    }

    public static nodeData DStart(string dialogueName, int startnode = -1)
    {
        data = new DialogueValues(loadDialogue(dialogueName));
        CommentIndex = 0;
        //turn Node list to dictionary
        Nodes = NodeDictionary(data.nodes);
        //change teh static bool to indicate we are in Dialogue
        InDialogue = true;
        //Load save data
        Drecord = LoadRecord(data.Name);
        //check the starting index
        if (startnode == -1)
        {
            //Check if the Dia
            currentindex = Drecord.startModified ? Drecord.startindex : data.startIndex;
        }
        else
        {
            currentindex = startnode;
        }
        //return the new node data
        return NodeDataReturn();
    }

    /// <summary>
    /// A Method which return node data
    /// and then return the appropriate data
    /// </summary>
    /// <param name="index">An Index, which points to which connected Node should the Dialogue go to next</param>
    /// <returns></returns>
    ///
    ///Basically we uses this first to check which node we currently on using currentIndex and commentIndex
    ///then we send the data returned by the private method NodeDataReturn
    ///they are divided into two methods because it looks nicer
    public static nodeData DNext(int index = 0)
    {
        if (Nodes == null)
        {
            Debug.Log("ZerosLength");
        }
        //get the next node index or in case of single nodes check if all Lines are have been read
        if (Nodes[currentindex].subType == SubType.SingleNode)
        {
            if (CommentIndex == Nodes[currentindex].dialogueText.Count - 1)
            {
                currentindex = Nodes[currentindex].ConnectedNodes[index];
                CommentIndex = 0;
            }
            else
            {
                CommentIndex++;
            }
        }
        else
        {
            currentindex = Nodes[currentindex].ConnectedNodes[index];
        }

        if (currentindex == -1)
        {
            Dialogueend();
            InDialogue = false;
            return null;
        }
        return NodeDataReturn();
    }

    /// <summary>
    /// Returns the nodeData, How Surprising
    /// </summary>
    /// <returns></returns>
    private static nodeData NodeDataReturn()
    {
        bool pause = NodeAction();
        //for all utility and direction node
        if (Nodes[currentindex].NodeType == NodeType.UtilityNode || Nodes[currentindex].subType == SubType.ValueDirectionNode)
        {
            AudioClip clip = null;
            if (Nodes[currentindex].subType == SubType.AudioNode)
            {
                clip = (AudioClip)Resources.Load(Nodes[currentindex].q_string1);
                return new nodeData(pause, Nodes[currentindex].Tag, clip, TextType.AudioNode);
            }
            else if (Nodes[currentindex].subType == SubType.InputNode)
            {
                return new nodeData(pause, Nodes[currentindex].Tag, clip, TextType.InputNode);
            }
            return new nodeData(pause, Nodes[currentindex].Tag, clip);
        }
        //for single,random and Modified random
        if (Nodes[currentindex].subType == SubType.SingleNode || Nodes[currentindex].subType == SubType.RandomNode || Nodes[currentindex].subType == SubType.MRandomNode || Nodes[currentindex].subType == SubType.ScriptNode)
        {
            CheckModifiedData();
            nodeData singeNode = SingleNodeData();
            return singeNode;
        }
        //for multinode and value choice node
        return new nodeData(UnlockedList(Nodes[currentindex].subType, Nodes[currentindex].choices, Nodes[currentindex].extraValues, Nodes[currentindex].dialogueText), Nodes[currentindex].subType, Nodes[currentindex].Tag);
    }

    /// <summary>
    /// The Node data of nodes that returns a single line, Like Single, Random, ModifiedRandom and ScriptedNode
    /// </summary>
    /// <returns></returns>
    private static nodeData SingleNodeData()
    {
        int Extraid = CommentIndex;
        int ExtraGuide = Extraid * 3;
        string ChDiaoluge = "";
        bool locked = false;
        characterId id = new characterId();
        switch (Nodes[currentindex].subType)
        {
            case SubType.SingleNode:
                locked = bool.Parse(Nodes[currentindex].extraValues[ExtraGuide + 2]);
                ChDiaoluge = Nodes[currentindex].dialogueText[Extraid];
                id = new characterId(int.Parse(Nodes[currentindex].extraValues[ExtraGuide]), int.Parse(Nodes[currentindex].extraValues[ExtraGuide + 1]));

                break;

            case SubType.RandomNode:
                Extraid = UnityEngine.Random.Range(0, Nodes[currentindex].dialogueText.Count);
                ExtraGuide = Extraid * 2;
                ChDiaoluge = Nodes[currentindex].dialogueText[Extraid];
                id = new characterId(int.Parse(Nodes[currentindex].extraValues[ExtraGuide]), int.Parse(Nodes[currentindex].extraValues[ExtraGuide + 1]));
                break;

            case SubType.MRandomNode:

                bool valuetype = Nodes[currentindex].q_bool1;
                bool greater = Nodes[currentindex].q_bool2;
                GameObject gameObject = GameObject.Find(Nodes[currentindex].q_string1);
                Extraid = (int)Convert.ChangeType(Value(gameObject, Nodes[currentindex].q_string2, valuetype), typeof(int));

                if (Extraid < 0 || Extraid >= Nodes[currentindex].dialogueText.Count)
                {
                    if (Extraid < 0)
                    {
                        Extraid = 0;
                    }
                    else
                    {
                        Extraid = Nodes[currentindex].dialogueText.Count;
                    }
                }
                else
                {
                    if (greater)
                    {
                        if (Extraid == Nodes[currentindex].dialogueText.Count)
                        {
                            Extraid = Nodes[currentindex].dialogueText.Count - 1;
                        }
                        Extraid = UnityEngine.Random.Range(Mathf.FloorToInt(Extraid), Nodes[currentindex].dialogueText.Count);
                    }
                    else
                    {
                        if (Extraid == 0)
                        {
                            Extraid = 1;
                        }
                        Extraid = UnityEngine.Random.Range(0, Mathf.FloorToInt((float)Extraid));
                    }
                }
                ChDiaoluge = Nodes[currentindex].dialogueText[Extraid];
                ExtraGuide = Extraid * 2;
                id = new characterId(int.Parse(Nodes[currentindex].extraValues[ExtraGuide]), int.Parse(Nodes[currentindex].extraValues[ExtraGuide + 1]));
                break;

            case SubType.ScriptNode:
                Debug.Log(1);
                GameObject gameObject1 = GameObject.Find(Nodes[currentindex].q_string1);
                MethodInfo m = GetMethod(gameObject1, Nodes[currentindex].q_string2);
                Debug.Log(m.Name);
                ParameterInfo[] ps = m.GetParameters();
                if (ps.Length == 0)
                {
                    ChDiaoluge = (string)m.Invoke(GetComponent(gameObject1, m), new object[] { });
                    Debug.Log(ChDiaoluge);
                }
                else if (ps.Length == 1)
                {
                    var paramter = Convert.ChangeType(Nodes[currentindex].extraValues[2], ps[0].ParameterType);
                    ChDiaoluge = (string)m.Invoke(GetComponent(gameObject1, m), new object[] { paramter });
                    Debug.Log(ChDiaoluge);
                }
                else
                {
                    object[] objects = new object[ps.Count()];
                    string[] inputs = Nodes[currentindex].extraValues[2].Split(",");
                    for (int i = 0; i < inputs.Length; i++)
                    {
                        objects[i] = Convert.ChangeType(inputs[i], ps[i].ParameterType);
                    }
                    ChDiaoluge = (string)m.Invoke(GetComponent(gameObject1, m), objects);
                    Debug.Log(ChDiaoluge);
                }
                id = new characterId(int.Parse(Nodes[currentindex].extraValues[0]), int.Parse(Nodes[currentindex].extraValues[1]));
                break;
        }
        nodeData nodedata = new nodeData(new Dialogue(ChDiaoluge, locked), id, Nodes[currentindex].Tag);

        return nodedata;
    }

    /// <summary>
    /// Perform non-dialogue function
    /// </summary>
    /// <returns></returns>
    private static bool NodeAction()
    {
        bool pause = false;
        switch (Nodes[currentindex].subType)
        {
            #region Valuechangenode

            case SubType.Valuechangenode:
                if (!Nodes[currentindex].q_bool1)
                {
                    GameObject gameObject3 = GameObject.Find(Nodes[currentindex].q_string1);
                    List<PropertyInfo> Properties = UtilityFunctions.GetProperties(gameObject3);
                    foreach (PropertyInfo _m in Properties)
                    {
                        if (_m.Name == Nodes[currentindex].q_string2)
                        {
                            if (Nodes[currentindex].q_bool2)
                            {
                                if (_m.PropertyType == typeof(bool))
                                {
                                    Debug.LogError("You can't add Booleans");
                                }
                                else if (_m.PropertyType == typeof(int))
                                {
                                    _m.SetValue(gameObject3.GetComponent(UtilityFunctions.Type(gameObject3, _m)), (int)Value(gameObject3, _m.Name, Nodes[currentindex].q_bool1) + (int)Convert.ChangeType(Nodes[currentindex].extraValues[0], _m.PropertyType));
                                }
                                else if (_m.PropertyType == typeof(float))
                                {
                                    _m.SetValue(gameObject3.GetComponent(UtilityFunctions.Type(gameObject3, _m)), (float)Value(gameObject3, _m.Name, Nodes[currentindex].q_bool1) + (float)Convert.ChangeType(Nodes[currentindex].extraValues[0], _m.PropertyType));
                                }
                                else if (_m.PropertyType == typeof(string))
                                {
                                    _m.SetValue(gameObject3.GetComponent(UtilityFunctions.Type(gameObject3, _m)), (string)Value(gameObject3, _m.Name, Nodes[currentindex].q_bool1) + (string)Convert.ChangeType(Nodes[currentindex].extraValues[0], _m.PropertyType));
                                }
                            }
                            else
                            {
                                _m.SetValue(gameObject3.GetComponent(UtilityFunctions.Type(gameObject3, _m)), Convert.ChangeType(Nodes[currentindex].extraValues[0], _m.PropertyType));
                            }
                        }
                    }
                }
                else
                {
                    GameObject gameObject1 = GameObject.Find(Nodes[currentindex].q_string1);
                    List<FieldInfo> Properties = UtilityFunctions.GetFields(gameObject1);
                    foreach (FieldInfo _m in Properties)
                    {
                        if (_m.Name == Nodes[currentindex].q_string2)
                        {
                            if (Nodes[currentindex].q_bool2)
                            {
                                var nas = Value(gameObject1, _m.Name, Nodes[currentindex].q_bool1);
                                if (_m.FieldType == typeof(bool))
                                {
                                    Debug.LogError("You can't add Booleans");
                                }
                                else if (_m.FieldType == typeof(int))
                                {
                                    _m.SetValue(gameObject1.GetComponent(UtilityFunctions.Type(gameObject1, _m)), (int)nas + (int)Convert.ChangeType(Nodes[currentindex].extraValues[0], _m.FieldType));
                                }
                                else if (_m.FieldType == typeof(float))
                                {
                                    _m.SetValue(gameObject1.GetComponent(UtilityFunctions.Type(gameObject1, _m)), (float)nas + (float)Convert.ChangeType(Nodes[currentindex].extraValues[0], _m.FieldType));
                                }
                                else if (_m.FieldType == typeof(string))
                                {
                                    _m.SetValue(gameObject1.GetComponent(UtilityFunctions.Type(gameObject1, _m)), (string)nas + (string)Convert.ChangeType(Nodes[currentindex].extraValues[0], _m.FieldType));
                                }
                            }
                            else
                            {
                                _m.SetValue(gameObject1.GetComponent(UtilityFunctions.Type(gameObject1, _m)), Convert.ChangeType(Nodes[currentindex].extraValues[0], _m.FieldType));
                            }
                        }
                    }
                }
                break;

            #endregion Valuechangenode

            #region ActionNode

            case SubType.ActionNode:
                //if there are no para
                if (!Nodes[currentindex].q_bool2)
                {
                    GameObject gameObject2 = GameObject.Find(Nodes[currentindex].q_string1);
                    gameObject2.SendMessage(Nodes[currentindex].q_string2);
                }
                else //if there is para
                {
                    GameObject gameObject2 = GameObject.Find(Nodes[currentindex].q_string1);
                    MethodInfo m = GetMethod(gameObject2, Nodes[currentindex].q_string2);
                    ParameterInfo[] ps = m.GetParameters();
                    string[] inputs = Nodes[currentindex].extraValues[0].Split(",");
                    if (inputs.Length == 1)
                    {
                        var paramter = Convert.ChangeType(Nodes[currentindex].extraValues[0], ps[0].ParameterType);
                        m.Invoke(GetComponent(gameObject2, m), new object[] { paramter });
                    }
                    else
                    {
                        object[] objects = new object[ps.Count()];
                        inputs = Nodes[currentindex].extraValues[0].Split(",");
                        for (int i = 0; i < inputs.Length; i++)
                        {
                            objects[i] = Convert.ChangeType(inputs[i], ps[i].ParameterType);
                        }
                        m.Invoke(GetComponent(gameObject2, m), objects);
                    }
                }
                pause = Nodes[currentindex].q_bool1;
                break;

            #endregion ActionNode

            #region ChoiceUnlockNode

            case SubType.ChoiceUnlockNode:
                int node = int.Parse(Nodes[currentindex].q_string2);

                for (int i = int.Parse(Nodes[currentindex].extraValues[0]); i < int.Parse(Nodes[currentindex].extraValues[1]); i++)
                {
                    int choice = i;

                    Drecord.SetRecord(node, choice, Nodes[currentindex].q_bool2);
                }

                Save(data.Name);
                break;

            #endregion ChoiceUnlockNode

            #region StartChangeNode

            case SubType.StartChangeNode:
                Drecord.startindex = int.Parse(Nodes[currentindex].q_string2);
                Drecord.startModified = true;
                Save(data.Name);
                break;

            #endregion StartChangeNode

            #region ValueDirectionNode

            case SubType.ValueDirectionNode:
                Debug.Log("Here");
                bool check = bool.Parse(Nodes[currentindex].dialogueText[0]);
                if (check)
                {
                    float value;
                    GameObject gameObject1 = GameObject.Find(Nodes[currentindex].extraValues[0]);
                    MethodInfo m = GetMethod(gameObject1, Nodes[currentindex].extraValues[1]);
                    ParameterInfo[] ps = m.GetParameters();
                    if (ps.Length == 1)
                    {
                        var paramter = Convert.ChangeType(Nodes[currentindex].extraValues[3], ps[0].ParameterType);
                        value = (float)m.Invoke(GetComponent(gameObject1, m), new object[] { paramter });
                    }
                    else
                    {
                        Debug.Log(m.Name);
                        object[] objects = new object[ps.Count()];
                        string[] inputs = Nodes[currentindex].extraValues[3].Split(",");
                        for (int i = 0; i < inputs.Length; i++)
                        {
                            objects[i] = Convert.ChangeType(inputs[i], ps[i].ParameterType);
                        }
                        value = (float)m.Invoke(GetComponent(gameObject1, m), objects);
                        Debug.Log("The choice value is:" + value.ToString());
                    }
                    bool direction = bool.Parse(Nodes[currentindex].extraValues[2]);
                    for (int i = 0; i < Nodes[currentindex].choices.Count; i++)
                    {
                        if (direction)
                        {
                            if (float.Parse(Nodes[currentindex].choices[i]) <= value)
                            {
                                Nodes[currentindex].ConnectedNodes[0] = Nodes[currentindex].ConnectedNodes[i];
                                break;
                            }
                        }
                        else
                        {
                            if (float.Parse(Nodes[currentindex].choices[i]) >= value)
                            {
                                Nodes[currentindex].ConnectedNodes[0] = Nodes[currentindex].ConnectedNodes[i];
                                break;
                            }
                        }
                    }
                }
                else
                {
                    bool greater1 = Nodes[currentindex].q_bool2;
                    bool valuetype1 = Nodes[currentindex].q_bool1;
                    GameObject q_string1ect2 = GameObject.Find(Nodes[currentindex].q_string1);
                    var Tan = (int)Value(q_string1ect2, Nodes[currentindex].q_string2, valuetype1);

                    for (int i = 0; i < Nodes[currentindex].choices.Count; i++)
                    {
                        if (greater1)
                        {
                            if (float.Parse(Nodes[currentindex].choices[i]) <= Tan)
                            {
                                Nodes[currentindex].ConnectedNodes[0] = Nodes[currentindex].ConnectedNodes[i];
                                break;
                            }
                        }
                        else
                        {
                            if (float.Parse(Nodes[currentindex].choices[i]) >= Tan)
                            {
                                Nodes[currentindex].ConnectedNodes[0] = Nodes[currentindex].ConnectedNodes[i];
                                break;
                            }
                        }
                    }
                }

                break;

            #endregion ValueDirectionNode

            #region AnimationNode

            case SubType.AnimationNode:
                Animator animation = GameObject.Find(Nodes[currentindex].extraValues[0]).GetComponent<Animator>();
                if (Nodes[currentindex].extraValues[1] == "")
                {
                    animation.Play(Nodes[currentindex].extraValues[1]);
                }
                else
                {
                    if (Nodes[currentindex].q_bool2)
                    {
                        animation.SetTrigger(Nodes[currentindex].q_string1);
                    }
                    else
                    {
                        AnimatorControllerParameter Apara = null;
                        foreach (AnimatorControllerParameter a in animation.parameters)
                        {
                            if (a.name == Nodes[currentindex].q_string1)
                            {
                                Apara = a;
                                break;
                            }
                        }
                        switch (Apara.type)
                        {
                            case AnimatorControllerParameterType.Bool:
                                animation.SetBool(Nodes[currentindex].q_string1, bool.Parse(Nodes[currentindex].q_string2));
                                break;

                            case AnimatorControllerParameterType.Int:
                                animation.SetInteger(Nodes[currentindex].q_string1, int.Parse(Nodes[currentindex].q_string2));
                                break;

                            case AnimatorControllerParameterType.Float:
                                animation.SetFloat(Nodes[currentindex].q_string1, float.Parse(Nodes[currentindex].q_string2));
                                break;
                        }
                    }
                }

                pause = Nodes[currentindex].q_bool1;
                break;

            #endregion AnimationNode

            #region AudioNode

            case SubType.AudioNode:
                pause = Nodes[currentindex].q_bool1;
                break;

            #endregion AudioNode

            #region Stage control

            case SubType.StageControlNode:
                switch (Nodes[currentindex].extraValues[0])
                {
                    case "Spawn":
                        stageDirection.instance.spawnActors(
                            int.Parse(Nodes[currentindex].q_string1)
                            , Nodes[currentindex].q_bool1,
                            Nodes[currentindex].q_bool2
                            );
                        break;

                    case "FlipSide":
                        stageDirection.instance.FlipSide(int.Parse(Nodes[currentindex].q_string1));
                        break;

                    case "Move":
                        stageDirection.instance.MoveActor(
                            int.Parse(Nodes[currentindex].q_string1), float.Parse(Nodes[currentindex].q_string2)
                            );
                        break;

                    case "Flip":
                        stageDirection.instance.FlipActor(int.Parse(Nodes[currentindex].q_string1));
                        break;

                    case "SetLevel":
                        stageDirection.instance.SetLvl(
                            int.Parse(Nodes[currentindex].q_string1), int.Parse(Nodes[currentindex].q_string2)
                            );
                        break;
                }
                pause = bool.Parse(Nodes[currentindex].extraValues[1]);
                break;

            #endregion Stage control

            #region InputNode

            case SubType.InputNode:

                pause = true;
                break;

                #endregion InputNode
        }
        return pause;
    }

    public static bool InputValue(string Input)
    {
        GameObject gameObject = GameObject.Find(Nodes[currentindex].q_string1);
        string ValueName = Nodes[currentindex].q_string2;
        bool field = Nodes[currentindex].q_bool1;
        Regex regex = new Regex(Nodes[currentindex].extraValues[1]);
        if (!regex.IsMatch(Input))
        {
            return false;
        }
        try
        {
            if (Nodes[currentindex].q_bool1)
            {
                FieldInfo ValueObj = UtilityFunctions.GetFields(gameObject).FirstOrDefault(i => i.Name == ValueName);
                Debug.Log(ValueObj.FieldType);
                switch (Nodes[currentindex].extraValues[0])
                {
                    case "int":
                        ValueObj.SetValue(gameObject.GetComponent(UtilityFunctions.Type(gameObject, ValueObj)), (int)Convert.ChangeType(Input, ValueObj.FieldType));
                        break;

                    case "float":
                        ValueObj.SetValue(gameObject.GetComponent(UtilityFunctions.Type(gameObject, ValueObj)), (float)Convert.ChangeType(Input, ValueObj.FieldType));
                        break;

                    case "bool":
                        ValueObj.SetValue(gameObject.GetComponent(UtilityFunctions.Type(gameObject, ValueObj)), (bool)Convert.ChangeType(Input, ValueObj.FieldType));
                        break;

                    case "string":
                        ValueObj.SetValue(gameObject.GetComponent(UtilityFunctions.Type(gameObject, ValueObj)), Input);
                        break;
                }
            }
            else
            {
                PropertyInfo ValueObj = UtilityFunctions.GetProperties(gameObject).FirstOrDefault(i => i.Name == ValueName);
                switch (Nodes[currentindex].extraValues[0])
                {
                    case "int":
                        ValueObj.SetValue(ValueObj, (int)Convert.ChangeType(Input, ValueObj.PropertyType));
                        break;

                    case "float":
                        ValueObj.SetValue(ValueObj, (float)Convert.ChangeType(Input, ValueObj.PropertyType));
                        break;

                    case "bool":
                        ValueObj.SetValue(ValueObj, (bool)Convert.ChangeType(Input, ValueObj.PropertyType));
                        break;

                    case "string":
                        ValueObj.SetValue(ValueObj, Input);
                        break;
                }
            }

            return true;
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
            return false;
        }
    }

    private static MethodInfo GetMethod(GameObject gameObject, string method)
    {
        List<MethodInfo> methods = UtilityFunctions.GetMethods(gameObject);
        MethodInfo m = methods[0];
        foreach (MethodInfo _m in methods)
        {
            if (_m.Name == method)
            {
                m = _m;
            }
        }
        return m;
    }

    private static MonoBehaviour GetComponent(GameObject obj, MethodInfo m)
    {
        var component = new object();
        var components = UtilityFunctions.GetCompoents(obj);
        foreach (var comp in components)
        {
            if (comp.GetType().GetMethods().Contains(m))
            {
                component = comp;
                break;
            }
        }
        return (MonoBehaviour)component;
    }

    /// <summary>
    /// Used to retrive the value of needed by some of the ActionNodes
    /// </summary>
    /// <param name="GameObject"></param>
    /// <param name="valuename"></param>
    /// <returns></returns>
    private static object Value(GameObject gameObject, string valuename, bool type)
    {
        var value = new object();

        if (type)
        {
            List<FieldInfo> Fields = UtilityFunctions.GetFields(gameObject);
            foreach (FieldInfo _m in Fields)
            {
                //Debug.Log(_m.Name);
                if (_m.Name == valuename)
                {
                    value = _m.GetValue(gameObject.GetComponent(UtilityFunctions.Type(gameObject, _m)));
                }
            }
        }
        else
        {
            List<PropertyInfo> Properties = UtilityFunctions.GetProperties(gameObject);
            foreach (PropertyInfo _m in Properties)
            {
                //Debug.Log(_m.Name);
                if (_m.Name == valuename)
                {
                    value = _m.GetValue(gameObject.GetComponent(UtilityFunctions.Type(gameObject, _m)));
                }
            }
        }
        return value;
    }

    /// <summary>
    /// Creating the boolean list used for MultiChoiceNode which have a looked state or a condition
    /// </summary>
    private static List<Dialogue> UnlockedList(SubType type, List<string> choices, List<string> Extra, List<string> dialogue)
    {
        List<Dialogue> Unlocks = new List<Dialogue>();
        switch (type)
        {
            case SubType.MultiNode:
                CheckModifiedData();
                for (int i = 0; i < Extra.Count; i++)
                {
                    Unlocks.Add(new(choices[i], bool.Parse(Extra[i])));
                }
                break;

            case SubType.ValueChoiceNode:
                Debug.Log("Here2");
                CheckModifiedData();
                bool vtype = Nodes[currentindex].q_bool1;
                GameObject gameObject = GameObject.Find(Nodes[currentindex].q_string1);
                for (int i = 0; i < Nodes[currentindex].dialogueText.Count; i++)
                {
                    int eid = i * 4;
                    //The value from the object
                    var condition = Convert.ChangeType(Value(gameObject, Extra[eid], vtype), typeof(float));
                    // the value from the choice
                    var contest = Convert.ChangeType(Extra[eid + 3], typeof(float));
                    bool direction = bool.Parse(Extra[eid + 1]);
                    if (contest.GetType() == typeof(bool))
                    {
                        if ((float)condition == (float)contest)
                        {
                            Unlocks.Add(new(dialogue[i], bool.Parse(Extra[eid + 2])));
                        }
                        else
                        {
                            Unlocks.Add(new(choices[i], bool.Parse(Extra[eid + 2])));
                        }
                    }
                    else
                    {
                        if (direction)
                        {
                            if ((float)contest <= (float)condition)
                            {
                                Unlocks.Add(new(dialogue[i], bool.Parse(Extra[eid + 2])));
                            }
                            else
                            {
                                Unlocks.Add(new(choices[i], bool.Parse(Extra[eid + 2]), true));
                            }
                        }
                        else
                        {
                            if ((float)contest > (float)condition)
                            {
                                Unlocks.Add(new(dialogue[i], bool.Parse(Extra[eid + 2])));
                            }
                            else
                            {
                                Unlocks.Add(new(choices[i], bool.Parse(Extra[eid + 2])));
                            }
                        }
                    }
                }

                break;
        }
        return Unlocks;
    }
    private static void CheckModifiedData()
    {
        switch (Nodes[currentindex].subType)
        {
            case SubType.MultiNode:
                for (int i = 0; i < Nodes[currentindex].extraValues.Count; i++)
                {
                    if (Drecord.ContainsRecord(currentindex, i))
                    {
                        Nodes[currentindex].extraValues[i] = Drecord.GetrecordValue(currentindex, i).ToString();
                    }
                }
                break;

            case SubType.ValueChoiceNode:
                for (int i = 0; i < Nodes[currentindex].choices.Count; i++)
                {
                    if (Drecord.ContainsRecord(currentindex, i))
                    {
                        Nodes[currentindex].extraValues[(i * 4) + 2] = Nodes[currentindex].extraValues[i] = Drecord.GetrecordValue(currentindex, i).ToString();
                    }
                }
                break;

            case SubType.SingleNode:
                int nodeid = (CommentIndex * 3) + 2;
                if (Drecord.ContainsRecord(currentindex, CommentIndex))
                {
                    Nodes[currentindex].extraValues[nodeid] = Nodes[currentindex].extraValues[nodeid] = Drecord.GetrecordValue(currentindex, CommentIndex).ToString();
                }
                break;
        }
    }
    public static DialogueData loadDialogue(string name)
    {
        var savefile = Resources.Load<TextAsset>($"DialoguesData/{name}");
        if (savefile != null)
        {
            Debug.Log("dataLoadded");
            return JsonUtility.FromJson<DialogueData>(savefile.text);
        }
        else
        {
            Debug.LogError("This Dialogue doesn't exist");
            return null;
        }
    }
    public static string SaveName { get; set; }
    public static void Save(string saveName)
    {
        SaveName = saveName;
        string savefile = $"{Application.persistentDataPath}/{SaveName}.json";
#if UNITY_EDITOR
        savefile = $"Assets/OpenDialogue/DevSave/{SaveName}.json";
#endif
        dialogueSave.Choices.FirstOrDefault(i=>i.title==data.Name).UpdateRecord(Drecord);
        string jsondata = JsonUtility.ToJson(dialogueSave);
        File.WriteAllText(savefile, jsondata);
    }
    //Load Dialogue Records to handle saves
    public static DialogueRecord LoadRecord(string DialougeName)
    {
        DialogueRecord record = dialogueSave.Choices.FirstOrDefault(i => i.title == DialougeName);
        if(record==null)
        {
            record = new DialogueRecord(data.Name, data.startIndex);
        }
        return record;
    }
    public static void Load(string saveName)
    {
        SaveName = saveName;
        string saveLocation = $"{Application.persistentDataPath}/{SaveName}.json";
#if UNITY_EDITOR
        saveLocation = $"Assets/OpenDialogue/DevSave/{SaveName}.json";
#endif
        //if the save file already exists in savefile Location
        if (File.Exists(saveLocation))
        {
            //Debug.Log("FileLoaded");
            string JsonData = File.ReadAllText(saveLocation);
            dialogueSave = JsonUtility.FromJson<DialogueSave>(JsonData);
        }
    }
    /// <summary>
    /// Turning List to Dictionary
    /// </summary>
    /// <param name="nodes"></param>
    /// <returns></returns>
    public static Dictionary<int, NodeDB> NodeDictionary(NodeDB[] nodes)
    {
        Dictionary<int, NodeDB> nodeD = new Dictionary<int, NodeDB>();
        foreach (NodeDB n in nodes)
        {
            nodeD.Add(n.id, n);
        }
        return nodeD;
    }
}