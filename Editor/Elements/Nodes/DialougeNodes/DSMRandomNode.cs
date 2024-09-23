using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace OpenDialouge.Elements
{
    using System.Reflection;
    using UnityEngine.UIElements;
    using utilities;
    /// <summary>
    /// Random dialouge return, however the random range can be modified by a value
    /// </summary>
    public class DSMRandomNode : DialogueNode
    {
        public Port Choice;

        public override void Initialize(Vector2 Pos, DSGraphView graph)
        {
            base.Initialize(Pos, graph);
            data.subType = SubType.MRandomNode;
            AddToClassList("SingleNode");
            data.choices.Add("Dialouge");
        }
        public override void Initialize(Vector2 Pos, DSGraphView graph, NodeDB db)
        {
            Initialize(Pos, graph);
            data = db;
        }

        public override void DrawSingle()
        {
            base.DrawSingle();

            DropdownField dropDownIdnetifier = ODtoolsElementUtilities.CreateDropDownMenu("Value Name",
            evt =>
            {
                data.q_string2 = evt.newValue;
            });
            DropdownField dropDownObjects = ODtoolsElementUtilities.CreateDropDownMenu("Objects", v =>
            {
                data.q_string1 = v.newValue;
                GameObject gameObject = GameObject.Find(v.newValue);
                if (gameObject != null)
                {
                    dropDownIdnetifier.choices.Clear();
                    if (!data.q_bool1)
                    {
                        dropDownIdnetifier.choices.AddRange(UtilityFunctions.GetPropertiesNames(gameObject));
                    }
                    else
                    {
                        dropDownIdnetifier.choices.AddRange(UtilityFunctions.GetFieldNames(gameObject));
                    }
                }
            }
            );
            DropdownField DataType = ODtoolsElementUtilities.CreateDropDownMenu("DataType", v =>
            {
                data.q_bool1 = v.newValue == "Field";
                var objects = Resources.FindObjectsOfTypeAll<GameObject>();
                dropDownObjects.choices.Clear();
                dropDownIdnetifier.choices.Clear();
                if (data.q_bool1)
                {
                    foreach (GameObject obj in objects)
                    {
                        if (UtilityFunctions.GetFields(obj).Count != 0)
                        {
                            dropDownObjects.choices.Add(obj.name);
                        }
                    }
                }
                else
                {
                    foreach (GameObject obj in objects)
                    {
                        if (UtilityFunctions.GetProperties(obj).Count != 0)
                        {
                            dropDownObjects.choices.Add(obj.name);
                        }
                    }
                }
                if (dropDownObjects.value != "")
                {
                    dropDownIdnetifier.value = "";
                    GameObject gameObject = GameObject.Find(dropDownObjects.value);
                    if (gameObject != null)
                    {
                        dropDownIdnetifier.choices.Clear();
                        if (!data.q_bool1)
                        {
                            dropDownIdnetifier.choices.AddRange(UtilityFunctions.GetPropertiesNames(gameObject));
                        }
                        else
                        {
                            dropDownIdnetifier.choices.AddRange(UtilityFunctions.GetFieldNames(gameObject));
                        }
                    }
                }
            }
           );
            DataType.choices.Add("Field");
            DataType.choices.Add("Property");

            var objects = Resources.FindObjectsOfTypeAll<GameObject>();

            if (data.q_bool1)
            {
                foreach (GameObject obj in objects)
                {
                    if (UtilityFunctions.GetFields(obj).Count != 0)
                    {
                        dropDownObjects.choices.Add(obj.name);
                    }
                }
            }
            else
            {
                foreach (GameObject obj in objects)
                {
                    if (UtilityFunctions.GetProperties(obj).Count != 0)
                    {
                        dropDownObjects.choices.Add(obj.name);
                    }
                }
            }
            Toggle Direction = ODtoolsElementUtilities.CreateToggle("Greater");
            Direction.tooltip = "Do you want to pass values that are greater than or equal ?";
            Direction.RegisterValueChangedCallback(evt =>
            {
                data.q_bool2 = evt.newValue;
            }
            );

            Foldout extradata = ODtoolsElementUtilities.CreateFoldout("ValueData", false);
            extensionContainer.Insert(0, extradata);
            extradata.Insert(0, DataType);
            extradata.Insert(1, Direction);
            extradata.Insert(2, dropDownObjects);
            extradata.Insert(3, dropDownIdnetifier);
            //Checking Data to fill Ui
            if (data.q_string1 != null)
            {
                DataType.value = data.q_bool1 ? "Field" : "Property";
                Direction.value = data.q_bool2;
                dropDownObjects.value = data.q_string1;
                dropDownIdnetifier.value = data.q_string2;
            }
            RefreshExpandedState();
        }

    }
}