using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace DSystem.Elements
{
    using System.Reflection;
    using UnityEngine.UIElements;
    using utilities;

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

            DropdownField dropdownmethods = DSElementUtilities.CreateDropDownMenu("Properties",
            evt =>
            {
                data.q_string2 = evt.newValue;
            });
            DropdownField dropdownobjects = DSElementUtilities.CreateDropDownMenu("Objects", v =>
            {
                data.q_string1 = v.newValue;
                GameObject gameObject = GameObject.Find(v.newValue);
                if (gameObject != null)
                {
                    dropdownmethods.choices.Clear();
                    if (!data.q_bool1)
                    {
                        List<PropertyInfo> methodz = UtilityFunctions.GetProperties(gameObject);
                        foreach (PropertyInfo method in methodz)
                        {
                            dropdownmethods.choices.Add(method.Name);
                        }
                    }
                    else
                    {
                        List<FieldInfo> methodz = UtilityFunctions.GetFields(gameObject);
                        foreach (FieldInfo method in methodz)
                        {
                            dropdownmethods.choices.Add(method.Name);
                        }
                    }
                }
            }
            );
            DropdownField DataType = DSElementUtilities.CreateDropDownMenu("DataType", v =>
            {
                data.q_bool1 = v.newValue == "Field";
                var objects = Resources.FindObjectsOfTypeAll<GameObject>();
                dropdownobjects.choices.Clear();
                dropdownmethods.choices.Clear();
                if (data.q_bool1)
                {
                    foreach (GameObject obj in objects)
                    {
                        if (UtilityFunctions.GetFields(obj).Count != 0)
                        {
                            dropdownobjects.choices.Add(obj.name);
                        }
                    }
                }
                else
                {
                    foreach (GameObject obj in objects)
                    {
                        if (UtilityFunctions.GetProperties(obj).Count != 0)
                        {
                            dropdownobjects.choices.Add(obj.name);
                        }
                    }
                }
                if (dropdownobjects.value != "")
                {
                    dropdownmethods.value = "";
                    GameObject gameObject = GameObject.Find(dropdownobjects.value);
                    if (gameObject != null)
                    {
                        dropdownmethods.choices.Clear();
                        if (!data.q_bool1)
                        {
                            List<PropertyInfo> methodz = UtilityFunctions.GetProperties(gameObject);
                            foreach (PropertyInfo method in methodz)
                            {
                                dropdownmethods.choices.Add(method.Name);
                            }
                        }
                        else
                        {
                            List<FieldInfo> methodz = UtilityFunctions.GetFields(gameObject);
                            foreach (FieldInfo method in methodz)
                            {
                                dropdownmethods.choices.Add(method.Name);
                            }
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
                        dropdownobjects.choices.Add(obj.name);
                    }
                }
            }
            else
            {
                foreach (GameObject obj in objects)
                {
                    if (UtilityFunctions.GetProperties(obj).Count != 0)
                    {
                        dropdownobjects.choices.Add(obj.name);
                    }
                }
            }
            Toggle Direction = DSElementUtilities.CreateToggle("Greater");
            Direction.tooltip = "Do you want to pass values that are greater than or equal ?";
            Direction.RegisterValueChangedCallback(evt =>
            {
                data.q_bool2 = evt.newValue;
            }
            );

            Foldout extradata = DSElementUtilities.CreateFoldout("ValueData", false);
            extensionContainer.Insert(0, extradata);
            extradata.Insert(0, DataType);
            extradata.Insert(1, Direction);
            extradata.Insert(2, dropdownobjects);
            extradata.Insert(3, dropdownmethods);
            //Checking Data to fill Ui
            if (data.q_string1 != null)
            {
                DataType.value = data.q_bool1 ? "Field" : "Property";
                Direction.value = data.q_bool2;
                dropdownobjects.value = data.q_string1;
                dropdownmethods.value = data.q_string2;
            }
            RefreshExpandedState();
        }

    }
}