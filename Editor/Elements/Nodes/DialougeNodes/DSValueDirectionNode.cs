using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace OpenDialouge.Elements
{
    using System.Linq;
    using utilities;
    /// <summary>
    /// Compares a list of value with a value returned from a method and moves through with the first path that returns the condition true
    /// </summary>
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
            DropdownField Mode = ODtoolsElementUtilities.CreateDropDownMenu("Mode",
            evt =>
            {
                var c=textfoldout.Children().ToList();
                for (int i = 1; i < c.Count; i++)
                {
                    textfoldout.Remove(c[i]);
                }
                //Setting if the value should be gotten from a method
                if (evt.newValue == "Method")
                {
                    data.dialogueText[0] = "true";
                    MethodMode();
                }
                //or from a property/Field
                else
                {
                    data.dialogueText[0] = "false";
                    ValueMode();
                }
            }, new string[] { "Method", "Value" });
            textfoldout.Add(Mode);
            //add choices and when add choices we add value that we will compare our Main Value to
            Button addchoice = ODtoolsElementUtilities.CreateButton("Add Directions", () =>
            {
                data.choices.Add($"Value{data.choices.Count}");
                CreateChoice(data.choices.Count - 1,true);
                RefreshExpandedState();
            }
            );
            mainContainer.Insert(1, addchoice);

            //Port Container which contains the out port and our value
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
        // if we are using a method
        private void MethodMode()
        {
            if (data.extraValues.Count == 0)
            {
                data.extraValues.AddRange(new string[] {""});
            }
            //Setting the method name
            DropdownField methods = ODtoolsElementUtilities.CreateDropDownMenu("Methods", v =>
            {
                data.q_string2 = v.newValue;
            }
            );
            //Getting a list of methods in the object
            DropdownField dropDownObjects = ODtoolsElementUtilities.CreateDropDownMenu("Objects", v =>
            {
                data.q_string1 = v.newValue;
                methods.choices.Clear();
                methods.choices.AddRange(UtilityFunctions.GetMethodsNames(GameObject.Find(data.q_string1)));
            }
            );
            //Getting a list of gameobject names
            var objects = Resources.FindObjectsOfTypeAll<GameObject>();

            foreach (GameObject obj in objects)
            {
                if (UtilityFunctions.GetMethods(obj).Count != 0)
                {
                    dropDownObjects.choices.Add(obj.name);
                }
            }
            Toggle Direction = ODtoolsElementUtilities.CreateToggle("Greater");
            Direction.tooltip = "Type of value you want to change";
            Direction.RegisterValueChangedCallback(evt =>
            {
                data.q_bool2 = evt.newValue;
            }
            );
            TextField Values = ODtoolsElementUtilities.CreateTextField("Parameters", evt =>
            {
                data.extraValues[0] = evt.newValue;
            });
            Values.label = "Parameters";
            textfoldout.Add(dropDownObjects);
            textfoldout.Add(methods);
            textfoldout.Add(Direction);
            textfoldout.Add(Values);
            RefreshExpandedState();
            dropDownObjects.value = data.q_string1;
            methods.value = data.q_string2;
            Direction.value = data.q_bool2;
            Values.value= data.extraValues[0];
        }

        private void ValueMode()
        {
            DropdownField dropDownMethods, dropDownObjects, dataType;
            Toggle direction;
            dropDownMethods = ODtoolsElementUtilities.CreateDropDownMenu("Properties",
            evt =>
            {
                data.q_string2 = evt.newValue;
            });
            dropDownObjects = ODtoolsElementUtilities.CreateDropDownMenu("Objects", v =>
            {
                data.q_string1 = v.newValue;
                GameObject gameObject = GameObject.Find(v.newValue);
                if (gameObject != null)
                {
                    dropDownMethods.choices.Clear();
                    if (!data.q_bool1)
                    {
                        dropDownMethods.choices.AddRange(UtilityFunctions.GetPropertiesNames(gameObject));
                    }
                    else
                    {
                        dropDownMethods.choices.AddRange(UtilityFunctions.GetFieldNames(gameObject));
                    }
                }
            }
            );
            dataType = ODtoolsElementUtilities.CreateDropDownMenu("DataType", v =>
            {
                data.q_bool1 = v.newValue == "Field";
                var objects = Resources.FindObjectsOfTypeAll<GameObject>();
                dropDownObjects.choices.Clear();
                dropDownMethods.choices.Clear();
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
                    dropDownMethods.value = "";
                    GameObject gameObject = GameObject.Find(dropDownObjects.value);
                    if (gameObject != null)
                    {
                        dropDownMethods.choices.Clear();
                        if (!data.q_bool1)
                        {
                            dropDownMethods.choices.AddRange(UtilityFunctions.GetPropertiesNames(gameObject));
                        }
                        else
                        {
                            dropDownMethods.choices.AddRange(UtilityFunctions.GetFieldNames(gameObject));
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
            direction = ODtoolsElementUtilities.CreateToggle("Greater");
            direction.tooltip = "should the values be test to see if they are greater than or equal";
            direction.RegisterValueChangedCallback(evt =>
            {
                data.q_bool2 = evt.newValue;
            }
            );
            textfoldout.Add(dataType);
            textfoldout.Add(direction);
            textfoldout.Add(dropDownObjects);
            textfoldout.Add(dropDownMethods);
            RefreshExpandedState();

            {
                dropDownObjects.value = data.q_string1;
                dropDownMethods.value = data.q_string2;
                direction.value = data.q_bool2;
                dataType.value = data.q_bool1 ? "Field" : "Property";
            }
        }

        #region Choice Element Creation

        private void CreateChoice(int id,bool newport)
        {
            Port Choice = this.CreatePort("", Orientation.Horizontal, Direction.Output, Port.Capacity.Single);
            output.Add(Choice);
            Choice.RegisterCallback<MouseUpEvent, PortPass>(Portcheck, new PortPass(Choice, Getindex(Choice), data.id));
            Choice.portName = $"Output";
            if (newport)
            { data.ConnectedNodes.Add(-1); }
            TextField Value = ODtoolsElementUtilities.CreateTextField(data.choices[id], evt =>
            {
                int index = Getindex(Choice);
                data.choices[index] = evt.newValue.ToString();
            }, KeyboardCombo);
            Button DeleteChoice = ODtoolsElementUtilities.CreateButton("X", () =>
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