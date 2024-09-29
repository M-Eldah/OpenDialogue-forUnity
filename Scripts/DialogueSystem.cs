using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

namespace OpenDialouge
{
    /// <summary>
    /// The Main Dialogue System resposible for Analyzing the dialogue data and sending it to the controller
    /// </summary>
    public static class DialogueSystem
    {
        public delegate void DialogueEnd();

        public static event DialogueEnd Dialogueend;

        public delegate void DialogueNext();

        public static event DialogueNext Dialoguenext;

        private static DialogueValues data;
        private static Dictionary<int, LineData> nodes;
        public static bool inDialogue;
        public static int currentIndex;
        private static int nextIndex;
        private static DialogueRecord dRecord;
        public static int lineIndex;

        /// <summary>
        /// Calls the DialogueNext Event which Makes the Next Dialogue line to be loaded
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
        /// <param name="_data">The Dialogue Data</param>
        /// <param name="startnode">The starting Node Id</param>
        /// <returns></returns>
        public static NodeData DStart(DialogueValues _data, int startnode = -1)
        {
            data = _data;
            lineIndex = 0;
            //turn Node list to dictionary
            nodes = NodeDictionary(data.nodes);
            //Indicates That we are in Dialogue
            inDialogue = true;
            //Load save data ??
            dRecord = LoadRecord(data.Name);
            //check the starting index
            if (startnode == -1)
            {
                //Check if the Dia
                currentIndex = dRecord.startModified ? dRecord.startindex : data.startIndex;
            }
            else
            {
                currentIndex = startnode;
            }
            //return the new node data
            return NodeDataReturn();
        }
        /// <summary>
        /// Load the DialogueData
        /// </summary>
        /// <param name="dialogueName">The Dialogue name</param>
        /// <param name="startnode">The starting Node Id</param>
        /// <returns></returns>
        public static NodeData DStart(string dialogueName, int startnode = -1)
        {
            data = new DialogueValues(LoadDialogue(dialogueName));
            lineIndex = 0;
            //turn Node list to dictionary
            nodes = NodeDictionary(data.nodes);
            //change teh static bool to indicate we are in Dialogue
            inDialogue = true;
            // ?? Load save data
            dRecord = LoadRecord(data.Name);
            //check the starting index
            if (startnode == -1)
            {
                //Check if the Dialogue starting Index was changed
                currentIndex = dRecord.startModified ? dRecord.startindex : data.startIndex;
            }
            else
            {
                currentIndex = startnode;
            }
            //return the new node data
            return NodeDataReturn();
        }

        /// <summary>
        /// A Method which returns node data
        ///
        /// </summary>
        /// <param name="index">An Index, which points to which connected Node should the Dialogue go to next</param>
        /// <returns></returns>
        ///
        ///Basically we uses this first to check which node we currently on using currentIndex and commentIndex
        ///then we send the data returned by the private method NodeDataReturn
        ///they are divided into two methods because it looks nicer
        public static NodeData DNext(int index = 0)
        {
            if (nodes == null)
            {
                Debug.LogError("NoNodeDataPresnt");
            }
            ///if in a single node it check if all the lines has been read
            ///as indexed by the Comment Index
            if (nodes[currentIndex].subType == SubType.SingleNode)
            {
                if (lineIndex == nodes[currentIndex].dialogueText.Count - 1)
                {
                    nextIndex = nodes[currentIndex].ConnectedNodes[index];
                    lineIndex = 0;
                }
                else
                {
                    lineIndex++;
                }
            }
            else
            {
                ///is this a value direction Node if not continue as normal
                if (nodes[currentIndex].subType != SubType.ValueDirectionNode)
                {
                    nextIndex = nodes[currentIndex].ConnectedNodes[index];
                }

            }
            currentIndex = nextIndex;
            if (currentIndex == -1)
            {
                Dialogueend();
                inDialogue = false;
                return null;
            }
            return NodeDataReturn();
        }

        /// <summary>
        /// Returns the nodeData, How Surprising
        /// </summary>
        /// <returns></returns>
        private static NodeData NodeDataReturn()
        {
            //Preform Utility function actions and check if they pauses the dialogue
            bool pause = NodeAction();

            //for all utility and direction node
            if (nodes[currentIndex].NodeType == NodeType.UtilityNode)
            {
                AudioClip clip = null;
                if (nodes[currentIndex].subType == SubType.AudioNode)
                {
                    clip = (AudioClip)Resources.Load(nodes[currentIndex].q_string1);
                    return new NodeData(pause, nodes[currentIndex].Tag, clip, TextType.AudioNode);
                }
                if (nodes[currentIndex].subType == SubType.InputNode)
                {
                    return new NodeData(pause, nodes[currentIndex].Tag, clip, TextType.InputNode);
                }
                return new NodeData(pause, nodes[currentIndex].Tag, clip);
            }

            //for SingleNodes
            if (nodes[currentIndex].subType == SubType.SingleNode  || 
                nodes[currentIndex].subType == SubType.RandomNode  || 
                nodes[currentIndex].subType == SubType.MRandomNode || 
                nodes[currentIndex].subType == SubType.ScriptNode)
            {
                CheckModifiedData();
                NodeData singeNode = SingleNodeData();
                return singeNode;
            }

            //for multinode and value choice node
            return new NodeData
                (UnlockedList(nodes[currentIndex].subType, 
                nodes[currentIndex].choices,        nodes[currentIndex].extraValues, 
                nodes[currentIndex].dialogueText),  nodes[currentIndex].subType, 
                nodes[currentIndex].Tag);
        }

        /// <summary>
        /// The Node data of nodes that returns a single line, Like Single, Random, ModifiedRandom and ScriptedNode
        /// </summary>
        /// <returns></returns>
        private static NodeData SingleNodeData()
        {
            int Extraid = lineIndex;
            int ExtraGuide = Extraid * 3;
            string ChDiaoluge = "";
            bool locked = false;
            CharacterId id = new CharacterId();
            switch (nodes[currentIndex].subType)
            {
                case SubType.SingleNode:
                    locked = bool.Parse(nodes[currentIndex].extraValues[ExtraGuide + 2]);
                    ChDiaoluge = nodes[currentIndex].dialogueText[Extraid];
                    id = new CharacterId(
                        int.Parse(nodes[currentIndex].extraValues[ExtraGuide]), 
                        int.Parse(nodes[currentIndex].extraValues[ExtraGuide + 1]));

                    break;

                case SubType.RandomNode:
                    Extraid = UnityEngine.Random.Range(0, nodes[currentIndex].dialogueText.Count);
                    ExtraGuide = Extraid * 2;
                    ChDiaoluge = nodes[currentIndex].dialogueText[Extraid];
                    id = new CharacterId(int.Parse(nodes[currentIndex].extraValues[ExtraGuide]), int.Parse(nodes[currentIndex].extraValues[ExtraGuide + 1]));
                    break;

                case SubType.MRandomNode:

                    bool valuetype = nodes[currentIndex].q_bool1;
                    bool greater = nodes[currentIndex].q_bool2;
                    GameObject gameObject = GameObject.Find(nodes[currentIndex].q_string1);
                    Extraid = (int)Convert.ChangeType(GetValue(gameObject, nodes[currentIndex].q_string2, valuetype), typeof(int));

                    if (Extraid < 0 || Extraid >= nodes[currentIndex].dialogueText.Count)
                    {
                        if (Extraid < 0)
                        {
                            Extraid = 0;
                        }
                        else
                        {
                            Extraid = nodes[currentIndex].dialogueText.Count;
                        }
                    }
                    else
                    {
                        if (greater)
                        {
                            if (Extraid == nodes[currentIndex].dialogueText.Count)
                            {
                                Extraid = nodes[currentIndex].dialogueText.Count - 1;
                            }
                            Extraid = UnityEngine.Random.Range(Mathf.FloorToInt(Extraid), nodes[currentIndex].dialogueText.Count);
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
                    ChDiaoluge = nodes[currentIndex].dialogueText[Extraid];
                    ExtraGuide = Extraid * 2;
                    id = new CharacterId(int.Parse(nodes[currentIndex].extraValues[ExtraGuide]), int.Parse(nodes[currentIndex].extraValues[ExtraGuide + 1]));
                    break;

                case SubType.ScriptNode:
                    GameObject gameObject1 = GameObject.Find(nodes[currentIndex].q_string1);
                    MethodInfo m = GetMethod(gameObject1, nodes[currentIndex].q_string2);
                    ParameterInfo[] ps = m.GetParameters();
                    if (ps.Length == 0)
                    {
                        ChDiaoluge = (string)m.Invoke(GetComponent(gameObject1, m), new object[] { });                    }
                    else if (ps.Length == 1)
                    {
                        var paramter = Convert.ChangeType(nodes[currentIndex].extraValues[2], ps[0].ParameterType);
                        ChDiaoluge = (string)m.Invoke(GetComponent(gameObject1, m), new object[] { paramter });
                    }
                    else
                    {
                        object[] objects = new object[ps.Count()];
                        string[] inputs = nodes[currentIndex].extraValues[2].Split(",");
                        for (int i = 0; i < inputs.Length; i++)
                        {
                            objects[i] = Convert.ChangeType(inputs[i], ps[i].ParameterType);
                        }
                        ChDiaoluge = (string)m.Invoke(GetComponent(gameObject1, m), objects);
                    }
                    id = new CharacterId(int.Parse(nodes[currentIndex].extraValues[0]), int.Parse(nodes[currentIndex].extraValues[1]));
                    break;
            }
            ChDiaoluge = ReplaceVocab(ChDiaoluge);
            NodeData nodedata = new NodeData(new Dialogue(ChDiaoluge, locked), id, nodes[currentIndex].Tag);

            return nodedata;
        }

        private static string ReplaceVocab(string text)
        {
            foreach (Vocab v in dRecord.Variables)
            {
                if (text.Contains(v.key))
                {
                    text = text.Replace(v.key, v.value);
                }
            }
            return text;
        }

        /// <summary>
        /// Perform non-dialogue function
        /// </summary>
        /// <returns></returns>
        private static bool NodeAction()
        {
            bool pause = false;
            switch (nodes[currentIndex].subType)
            {
                #region Valuechangenode

                case SubType.Valuechangenode:
                    if (!nodes[currentIndex].q_bool1)
                    {
                        GameObject gameObject3 = GameObject.Find(nodes[currentIndex].q_string1);
                        List<PropertyInfo> Properties = UtilityFunctions.GetProperties(gameObject3);
                        foreach (PropertyInfo _m in Properties)
                        {
                            if (_m.Name == nodes[currentIndex].q_string2)
                            {
                                if (nodes[currentIndex].q_bool2)
                                {
                                    if (_m.PropertyType == typeof(bool))
                                    {
                                        Debug.LogError("You can't add Booleans");
                                    }
                                    else if (_m.PropertyType == typeof(int))
                                    {
                                        _m.SetValue(gameObject3.GetComponent(UtilityFunctions.Type(gameObject3, _m)), (int)GetValue(gameObject3, _m.Name, nodes[currentIndex].q_bool1) + (int)Convert.ChangeType(nodes[currentIndex].extraValues[0], _m.PropertyType));
                                    }
                                    else if (_m.PropertyType == typeof(float))
                                    {
                                        _m.SetValue(gameObject3.GetComponent(UtilityFunctions.Type(gameObject3, _m)), (float)GetValue(gameObject3, _m.Name, nodes[currentIndex].q_bool1) + (float)Convert.ChangeType(nodes[currentIndex].extraValues[0], _m.PropertyType));
                                    }
                                    else if (_m.PropertyType == typeof(string))
                                    {
                                        _m.SetValue(gameObject3.GetComponent(UtilityFunctions.Type(gameObject3, _m)), (string)GetValue(gameObject3, _m.Name, nodes[currentIndex].q_bool1) + (string)Convert.ChangeType(nodes[currentIndex].extraValues[0], _m.PropertyType));
                                    }
                                }
                                else
                                {
                                    _m.SetValue(gameObject3.GetComponent(UtilityFunctions.Type(gameObject3, _m)), Convert.ChangeType(nodes[currentIndex].extraValues[0], _m.PropertyType));
                                }
                            }
                        }
                    }
                    else
                    {
                        GameObject gameObject1 = GameObject.Find(nodes[currentIndex].q_string1);
                        List<FieldInfo> Properties = UtilityFunctions.GetFields(gameObject1);
                        foreach (FieldInfo _m in Properties)
                        {
                            if (_m.Name == nodes[currentIndex].q_string2)
                            {
                                if (nodes[currentIndex].q_bool2)
                                {
                                    var nas = GetValue(gameObject1, _m.Name, nodes[currentIndex].q_bool1);
                                    if (_m.FieldType == typeof(bool))
                                    {
                                        Debug.LogError("You can't add Booleans");
                                    }
                                    else if (_m.FieldType == typeof(int))
                                    {
                                        _m.SetValue(gameObject1.GetComponent(UtilityFunctions.Type(gameObject1, _m)), (int)nas + (int)Convert.ChangeType(nodes[currentIndex].extraValues[0], _m.FieldType));
                                    }
                                    else if (_m.FieldType == typeof(float))
                                    {
                                        _m.SetValue(gameObject1.GetComponent(UtilityFunctions.Type(gameObject1, _m)), (float)nas + (float)Convert.ChangeType(nodes[currentIndex].extraValues[0], _m.FieldType));
                                    }
                                    else if (_m.FieldType == typeof(string))
                                    {
                                        _m.SetValue(gameObject1.GetComponent(UtilityFunctions.Type(gameObject1, _m)), (string)nas + (string)Convert.ChangeType(nodes[currentIndex].extraValues[0], _m.FieldType));
                                    }
                                }
                                else
                                {
                                    _m.SetValue(gameObject1.GetComponent(UtilityFunctions.Type(gameObject1, _m)), Convert.ChangeType(nodes[currentIndex].extraValues[0], _m.FieldType));
                                }
                            }
                        }
                    }
                    break;

                #endregion Valuechangenode

                #region ActionNode

                case SubType.ActionNode:
                    //if there are no para
                    if (!nodes[currentIndex].q_bool2)
                    {
                        GameObject gameObject2 = GameObject.Find(nodes[currentIndex].q_string1);
                        gameObject2.SendMessage(nodes[currentIndex].q_string2);
                    }
                    else //if there is para
                    {
                        GameObject gameObject2 = GameObject.Find(nodes[currentIndex].q_string1);
                        MethodInfo m = GetMethod(gameObject2, nodes[currentIndex].q_string2);
                        ParameterInfo[] ps = m.GetParameters();
                        string[] inputs = nodes[currentIndex].extraValues[0].Split(",");
                        if (inputs.Length == 1)
                        {
                            var paramter = Convert.ChangeType(nodes[currentIndex].extraValues[0], ps[0].ParameterType);
                            m.Invoke(GetComponent(gameObject2, m), new object[] { paramter });
                        }
                        else
                        {
                            object[] objects = new object[ps.Count()];
                            inputs = nodes[currentIndex].extraValues[0].Split(",");
                            for (int i = 0; i < inputs.Length; i++)
                            {
                                objects[i] = Convert.ChangeType(inputs[i], ps[i].ParameterType);
                            }
                            m.Invoke(GetComponent(gameObject2, m), objects);
                        }
                    }
                    pause = nodes[currentIndex].q_bool1;
                    break;

                #endregion ActionNode

                #region ChoiceUnlockNode

                case SubType.ChoiceUnlockNode:
                    int node = int.Parse(nodes[currentIndex].q_string2);

                    for (int i = int.Parse(nodes[currentIndex].extraValues[0]); i < int.Parse(nodes[currentIndex].extraValues[1]); i++)
                    {
                        int choice = i;

                        dRecord.SetRecord(node, choice, nodes[currentIndex].q_bool2);
                    }

                    Save(SaveName);
                    break;

                #endregion ChoiceUnlockNode

                #region StartChangeNode

                case SubType.StartChangeNode:
                    dRecord.startindex = int.Parse(nodes[currentIndex].q_string2);
                    dRecord.startModified = true;
                    Save(SaveName);
                    break;

                #endregion StartChangeNode


                #region ValueDirectionNode

                case SubType.ValueDirectionNode:
                    //Checks if method or value
                    bool check = bool.Parse(nodes[currentIndex].dialogueText[0]);
                    if (check)
                    {
                        float value;
                        GameObject gameObject1 = GameObject.Find(nodes[currentIndex].q_string1);
                        MethodInfo m = GetMethod(gameObject1, nodes[currentIndex].q_string2);
                        ParameterInfo[] ps = m.GetParameters();
                        if (ps.Length == 1)
                        {
                            var paramter = Convert.ChangeType(nodes[currentIndex].extraValues[0], ps[0].ParameterType);
                            var o = m.Invoke(GetComponent(gameObject1, m), new object[] { paramter });
                            value = Convert.ToSingle(o);
                        }
                        else
                        {
                            object[] objects = new object[ps.Count()];
                            string[] inputs = nodes[currentIndex].extraValues[0].Split(",");
                            for (int i = 0; i < inputs.Length; i++)
                            {
                                objects[i] = Convert.ChangeType(inputs[i], ps[i].ParameterType);
                            }
                            var o = m.Invoke(GetComponent(gameObject1, m), objects);
                            value = Convert.ToSingle(o);

                        }
                        bool direction = nodes[currentIndex].q_bool2;
                        for (int i = 0; i < nodes[currentIndex].choices.Count; i++)
                        {
                            if (direction)
                            {
                                if (float.Parse(nodes[currentIndex].choices[i]) <= value)
                                {
                                    nextIndex = i;
                                    break;
                                }
                            }
                            else
                            {
                                if (float.Parse(nodes[currentIndex].choices[i]) >= value)
                                {
                                    nextIndex = i;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        bool greater1 = nodes[currentIndex].q_bool2;
                        bool valuetype1 = nodes[currentIndex].q_bool1;
                        GameObject q_string1ect2 = GameObject.Find(nodes[currentIndex].q_string1);
                        var Tan = GetValue(q_string1ect2, nodes[currentIndex].q_string2, valuetype1);
                        if (Tan.GetType() == typeof(bool))
                        {
                            bool v = (bool)Tan;
                            for (int i = 0; i < nodes[currentIndex].choices.Count; i++)
                            {
                                if (bool.Parse(nodes[currentIndex].choices[i]) == v)
                                {
                                    nodes[currentIndex].Tag = i.ToString();
                                    break;
                                }
                            }
                        }
                        else
                        {
                            float v = BooleanConvert(Tan);
                            for (int i = 0; i < nodes[currentIndex].choices.Count; i++)
                            {
                                if (greater1)
                                {
                                    if (float.Parse(nodes[currentIndex].choices[i]) <= v)
                                    {
                                        nextIndex = i;
                                        break;
                                    }
                                }
                                else
                                {
                                    if (float.Parse(nodes[currentIndex].choices[i]) >= v)
                                    {
                                        nextIndex = i;
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    break;


                #endregion ValueDirectionNode

                #region AnimationNode

                case SubType.AnimationNode:
                    Animator animation = GameObject.Find(nodes[currentIndex].extraValues[0]).GetComponent<Animator>();
                    if (nodes[currentIndex].extraValues[1] == "")
                    {
                        animation.Play(nodes[currentIndex].extraValues[1]);
                    }
                    else
                    {
                        if (nodes[currentIndex].q_bool2)
                        {
                            animation.SetTrigger(nodes[currentIndex].q_string1);
                        }
                        else
                        {
                            AnimatorControllerParameter Apara = null;
                            foreach (AnimatorControllerParameter a in animation.parameters)
                            {
                                if (a.name == nodes[currentIndex].q_string1)
                                {
                                    Apara = a;
                                    break;
                                }
                            }
                            switch (Apara.type)
                            {
                                case AnimatorControllerParameterType.Bool:
                                    animation.SetBool(nodes[currentIndex].q_string1, bool.Parse(nodes[currentIndex].q_string2));
                                    break;

                                case AnimatorControllerParameterType.Int:
                                    animation.SetInteger(nodes[currentIndex].q_string1, int.Parse(nodes[currentIndex].q_string2));
                                    break;

                                case AnimatorControllerParameterType.Float:
                                    animation.SetFloat(nodes[currentIndex].q_string1, float.Parse(nodes[currentIndex].q_string2));
                                    break;
                            }
                        }
                    }

                    pause = nodes[currentIndex].q_bool1;
                    break;

                #endregion AnimationNode

                #region AudioNode

                case SubType.AudioNode:
                    pause = nodes[currentIndex].q_bool1;
                    break;

                #endregion AudioNode

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
            GameObject gameObject = GameObject.Find(nodes[currentIndex].q_string1);
            string ValueName = nodes[currentIndex].q_string2;
            bool field = nodes[currentIndex].q_bool1;
            Regex regex = new Regex(nodes[currentIndex].extraValues[1]);
            if (!regex.IsMatch(Input))
            {
                return false;
            }
            try
            {
                if (nodes[currentIndex].q_bool1)
                {
                    FieldInfo ValueObj = UtilityFunctions.GetFields(gameObject).FirstOrDefault(i => i.Name == ValueName);
                    switch (nodes[currentIndex].extraValues[0])
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
                    switch (nodes[currentIndex].extraValues[0])
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
        /// Used to retrive the value of needed by some of the UtilityNodes
        /// </summary>
        /// <param name="GameObject"></param>
        /// <param name="valuename"></param>
        /// <returns></returns>
        private static object GetValue(GameObject gameObject, string valuename, bool type)
        {
            var value = new object();

            if (type)
            {
                List<FieldInfo> Fields = UtilityFunctions.GetFields(gameObject);
                foreach (FieldInfo _m in Fields)
                {
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
                    CheckModifiedData();
                    bool vtype = nodes[currentIndex].q_bool1;
                    GameObject gameObject = GameObject.Find(nodes[currentIndex].q_string1);
                    for (int i = 0; i < nodes[currentIndex].dialogueText.Count; i++)
                    {
                        int eid = i * 4;
                        //The value from the object
                        var condition = Convert.ChangeType(GetValue(gameObject, Extra[eid], vtype), typeof(float));
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

                case SubType.BooleanChoiceNode:
                    GameObject gameObject1 = GameObject.Find(nodes[currentIndex].q_string1);
                    for (int i = 0; i < nodes[currentIndex].choices.Count; i++)
                    {
                        bool check;
                        MethodInfo m = GetMethod(gameObject1, nodes[currentIndex].extraValues[i]);
                        ParameterInfo[] ps = m.GetParameters();
                        string[] inputs = nodes[currentIndex].choices[i].Split(",");
                        if (ps.Length == 0)
                        {
                            check = (bool)m.Invoke(GetComponent(gameObject1, m), new object[] { });
                        }
                        else if (ps.Length == 1)
                        {
                            var paramter = Convert.ChangeType(inputs[0], ps[0].ParameterType);
                            check = (bool)m.Invoke(GetComponent(gameObject1, m), new object[] { paramter });
                        }
                        else
                        {
                            object[] objects = new object[ps.Count()];
                            for (int v = 0; v < inputs.Length; v++)
                            {
                                objects[v] = Convert.ChangeType(inputs[v], ps[v].ParameterType);
                            }
                            check = (bool)m.Invoke(GetComponent(gameObject1, m), objects);
                        }
                        Unlocks.Add(new(nodes[currentIndex].dialogueText[i], !check));
                    }

                    break;
            }
            return Unlocks;
        }

        public static void UpdateVocab(string Key, string Value)
        {
            if (dRecord.Variables.FirstOrDefault(i => i.key == Key) == null)
            {
                dRecord.Variables.Add(new Vocab(Key, Value));
            }
            else
            {
                dRecord.Variables.FirstOrDefault(i => i.key == Key).value = Value;
            }
        }

        private static void CheckModifiedData()
        {
            switch (nodes[currentIndex].subType)
            {
                //ForMultiNodes the locked state is kept. in the extra Values we are here checking if we got a key for it 
                // in our DialogueRecord
                #region MultiNodes Check
                case SubType.MultiNode:
                    for (int i = 0; i < nodes[currentIndex].extraValues.Count; i++)
                    {
                        if (dRecord.ContainsRecord(currentIndex, i))
                        {
                            nodes[currentIndex].extraValues[i] = dRecord.GetrecordValue(currentIndex, i).ToString();
                        }
                    }
                    break;
                case SubType.ValueChoiceNode:
                    for (int i = 0; i < nodes[currentIndex].choices.Count; i++)
                    {
                        if (dRecord.ContainsRecord(currentIndex, i))
                        {
                            nodes[currentIndex].extraValues[(i * 4) + 2] = nodes[currentIndex].extraValues[i] = dRecord.GetrecordValue(currentIndex, i).ToString();
                        }
                    }
                    break;
                #endregion
                #region SingleNodes Check
                // the locked check for single nodes is releated to the lineIndex
                case SubType.SingleNode:
                    int nodeid = (lineIndex * 3) + 2;
                    if (dRecord.ContainsRecord(currentIndex, lineIndex))
                    {
                        nodes[currentIndex].extraValues[nodeid] = nodes[currentIndex].extraValues[nodeid] = dRecord.GetrecordValue(currentIndex, lineIndex).ToString();
                    }
                    break;
                #endregion
            }
        }

        public static DialogueData LoadDialogue(string name)
        {
            var savefile = Resources.Load<TextAsset>($"DialoguesData/{name}");
            if (savefile != null)
            {
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
            DialogueSave s = Load(SaveName);
            DialogueRecord r = s.Choices.FirstOrDefault(i => i.title == data.Name);
            if (r == null)
            {
                s.Choices.Add(dRecord);
            }
            else
            {
                r.UpdateRecord(dRecord);
            }
            string jsondata = JsonUtility.ToJson(s);
            File.WriteAllText(savefile, jsondata);
        }

        //Load Dialogue Records to handle saves
        public static DialogueRecord LoadRecord(string DialougeName)
        {
            DialogueRecord record = Load(SaveName).Choices.FirstOrDefault(i => i.title == DialougeName);
            record ??= new DialogueRecord(data.Name, data.startIndex);
            return record;
        }

        public static DialogueSave Load(string saveName)
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
                return JsonUtility.FromJson<DialogueSave>(JsonData);
            }
            else
            {
                return new DialogueSave();
            }
        }

        /// <summary>
        /// Turning List to Dictionary
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        public static Dictionary<int, LineData> NodeDictionary(LineData[] nodes)
        {
            Dictionary<int, LineData> nodeD = new Dictionary<int, LineData>();
            foreach (LineData n in nodes)
            {
                nodeD.Add(n.id, n);
            }
            return nodeD;
        }
        private static float BooleanConvert(object o)
        {
            float value;
            if (o.GetType() == typeof(bool))
            {
                value = (bool)o ? 1 : 0;
            }
            else
            {
                value = Convert.ToSingle(o);
            }
            return value;
        }
    }
}