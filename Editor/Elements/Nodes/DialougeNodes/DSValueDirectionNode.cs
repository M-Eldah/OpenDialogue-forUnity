using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DSystem.Elements
{
    using System;
    using System.Linq;
    using System.Reflection;
    using utilities;

    public class DSValueDirectionNode : DialogueNode
    {
        public override void Initialize(Vector2 Pos, DSGraphView graph)
        {
            base.Initialize(Pos, graph);
            data.subType = SubType.ValueDirectionNode;
            AddToClassList("MultiNode");
        }


        public override void Initialize(Vector2 Pos, DSGraphView graph, NodeDB db)
        {
            Initialize(Pos, graph);
            data = db;
        }

        public override void Draw()
        {
            //Main Container
            base.Draw(true, "Value");
            DropdownField Mode = DSElementUtilities.CreateDropDownMenu("Mode",
            evt =>
            {
                var c=textfoldout.Children().ToList();
                for (int i = 1; i < c.Count; i++)
                {
                    textfoldout.Remove(c[i]);
                }
                
                if (evt.newValue == "Method")
                {
                    data.dialogueText[0] = "true";
                    MethodMode();
                }
                else
                {
                    data.dialogueText[0] = "false";
                    ValueMode();
                }
            }, new string[] { "Method", "Value" });
            textfoldout.Add(Mode);
            Button addchoice = DSElementUtilities.CreateButton("Add Directions", () =>
            {
                data.choices.Add($"Value{data.choices.Count}");
                CreateChoice(data.choices.Count - 1,true);
                RefreshExpandedState();
            }
            );
            mainContainer.Insert(1, addchoice);

            //Port Container
            if (data.choices.Count == 0)
            {
                data.choices.Add($"Value{data.choices.Count}");
                CreateChoice(data.choices.Count - 1, true);

                RefreshExpandedState();
            }
            else
            {
                for (int i = 0; i < data.choices.Count; i++)
                {
                    CreateChoice(i,false);
                }
            }
            if (data.dialogueText.Count == 0)
            {
                data.dialogueText.Add("True");
            }
            else
            {
               

                bool check = bool.Parse(data.dialogueText[0]);
                if (check)
                {
                    Mode.value = "Method";
                    MethodMode();
                }
                else
                {
                    Mode.value = "Value";
                    ValueMode();
                }
            }
        }

        private void MethodMode()
        {
            if (data.extraValues.Count == 0)
            {
                data.extraValues.AddRange(new string[] {""});
            }
            DropdownField methods = DSElementUtilities.CreateDropDownMenu("Methods", v =>
            {
                data.q_string2 = v.newValue;
            }
            );

            DropdownField dropdownobjects = DSElementUtilities.CreateDropDownMenu("Objects", v =>
            {
                data.q_string1 = v.newValue;
                methods.choices.Clear();
                UtilityFunctions.GetMethods(GameObject.Find(data.extraValues[0])).ForEach(m => { methods.choices.Add(m.Name); });
            }
            );

            var objects = Resources.FindObjectsOfTypeAll<GameObject>();

            foreach (GameObject obj in objects)
            {
                if (UtilityFunctions.GetMethods(obj).Count != 0)
                {
                    dropdownobjects.choices.Add(obj.name);
                }
            }
            Toggle Direction = DSElementUtilities.CreateToggle("Greater");
            Direction.tooltip = "Type of value you want to change";
            Direction.RegisterValueChangedCallback(evt =>
            {
                data.q_bool2 = evt.newValue;
            }
            );
            TextField Values = DSElementUtilities.CreateTextField("Parameters", evt =>
            {
                data.extraValues[0] = evt.newValue;
            });
            Values.label = "Parameters";
            textfoldout.Add(dropdownobjects);
            textfoldout.Add(methods);
            textfoldout.Add(Direction);
            textfoldout.Add(Values);
            RefreshExpandedState();
            dropdownobjects.value = data.q_string1;
            methods.value = data.q_string2;
            Direction.value = data.q_bool2;
            Values.value= data.extraValues[0];
        }

        private void ValueMode()
        {
            DropdownField dropdownmethods, dropdownobjects, DataType;
            Toggle Direction;
            dropdownmethods = DSElementUtilities.CreateDropDownMenu("Properties",
            evt =>
            {
                data.q_string2 = evt.newValue;
            });
            dropdownobjects = DSElementUtilities.CreateDropDownMenu("Objects", v =>
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
            DataType = DSElementUtilities.CreateDropDownMenu("DataType", v =>
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
            }, new string[] { "Field", "Property" }
            );

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
            Direction = DSElementUtilities.CreateToggle("Greater");
            Direction.tooltip = "should the values be test to see if they are greater than or equal";
            Direction.RegisterValueChangedCallback(evt =>
            {
                data.q_bool2 = evt.newValue;
            }
            );
            textfoldout.Add(DataType);
            textfoldout.Add(Direction);
            textfoldout.Add(dropdownobjects);
            textfoldout.Add(dropdownmethods);
            RefreshExpandedState();

            {
                dropdownobjects.value = data.q_string1;
                dropdownmethods.value = data.q_string2;
                Direction.value = data.q_bool2;
                DataType.value = data.q_bool1 ? "Field" : "Property";
            }
        }

        #region Choice Element Creation

        private void CreateChoice(int id,bool newport)
        {
            Port Choice = this.CreatePort("", Orientation.Horizontal, Direction.Output, Port.Capacity.Single);
            output.Add(Choice);
            Choice.RegisterCallback<MouseUpEvent, PortPass>(portcheck, new PortPass(Choice, Getindex(Choice), data.id));
            Choice.portName = $"Output";
            if (newport)
            { data.ConnectedNodes.Add(-1); }
            TextField Value = DSElementUtilities.CreateTextField(data.choices[id], evt =>
            {
                int index = Getindex(Choice);
                data.choices[index] = evt.newValue.ToString();
            }, KeyboardCombo);
            Button DeleteChoice = DSElementUtilities.CreateButton("X", () =>
            {
                if (data.choices.Count == 1)
                {
                    return;
                }
                if (Choice.connected)
                {
                    GraphView.DeleteElements(Choice.connections);
                }
                int x =Getindex(Choice);
                data.ConnectedNodes .RemoveAt(x);
                data.choices        .RemoveAt(x);
                output.Remove(Choice);
                GraphView.RemoveElement(Choice);
            });

            DeleteChoice.AddToClassList("DeleteButton");
            Choice.Add(DeleteChoice);
            Choice.Add(Value);
            outputContainer.Add(Choice);
            Value.Focus();
        }
        private int Getindex(Port port)
        {
            return output.FindIndex(x => x == port);
        }
        private void KeyboardCombo(KeyDownEvent e)
        {
            if (e.altKey && e.keyCode == KeyCode.N)
            {
                data.choices.Add($"Value{data.choices.Count}");
                CreateChoice(data.choices.Count - 1,true);
            }
        }
        #endregion Choice Element Creation
    }
}