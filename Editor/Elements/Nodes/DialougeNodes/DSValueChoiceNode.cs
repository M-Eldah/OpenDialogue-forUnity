using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DSystem.Elements
{
    using System.Linq;
    using System.Reflection;
    using utilities;

    public class DSValueChoiceNode : DialogueNode
    {
        private List<string> valueName;
        private List<DropdownField> prop;
        public override void Initialize(Vector2 Pos, DSGraphView graph)
        {
            prop= new List<DropdownField>();
            valueName = new List<string>();
            base.Initialize(Pos, graph);
            data.subType = SubType.ValueChoiceNode;
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
            
            DropdownField dropdownobjects = DSElementUtilities.CreateDropDownMenu("Objects", v =>
            {
                data.q_string1 = v.newValue;
                AddValueNames(v.newValue);
            }
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
            

            DropdownField DataType = DSElementUtilities.CreateDropDownMenu("DataType",
                v =>{
                    data.q_bool1 = v.newValue == "Field";
                    dropdownobjects.choices.Clear();
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

                }
                ,new string[] {"Field","Property"});
            textfoldout.Add(DataType);
            textfoldout.Add(dropdownobjects);
            

            Button addchoice = DSElementUtilities.CreateButton("Add Choice", () =>
            {
                int X = data.choices.Count - 1;
                data.dialogueText.Add(data.dialogueText[X]);
                data.choices.Add(data.choices[X]);
                data.extraValues.AddRange(new string[] { data.extraValues[X*4], data.extraValues[(X*4)+1], data.extraValues[(X*4)+2], data.extraValues[(X*4)+3] });
                CreateChoice((data.choices.Count - 1), (data.choices.Count - 1)*4);
                RefreshExpandedState();
            }
            );

            mainContainer.Insert(1, addchoice);

            if (data.choices.Count == 0)
            {
                data.dialogueText.Add("ChoiceText");
                data.choices.Add("AlternativeText");
                data.extraValues.AddRange(new string[] { "", "False", "False", "Value" });
                CreateChoice(0,0);
                RefreshExpandedState();
            }
            else
            {
                DataType.value = data.q_bool1 ? "Field" : "Property";
                dropdownobjects.value = data.q_string1;
                for (int i = 0; i < data.choices.Count; i++)
                {
                    CreateChoice(i,i*4);
                }
                AddValueNames(data.q_string1);
            }
            ToggleCollapse(); ToggleCollapse();
        }

        private void AddValueNames(string v)
        {
            GameObject gameObject = GameObject.Find(v);
            if (gameObject != null)
            {
                valueName.Clear();
                if (!data.q_bool1)
                {
                    List<PropertyInfo> methodz = UtilityFunctions.GetProperties(gameObject);
                    foreach (PropertyInfo method in methodz)
                    {
                        valueName.Add(method.Name);
                    }
                }
                else
                {
                    List<FieldInfo> methodz = UtilityFunctions.GetFields(gameObject);
                    foreach (FieldInfo method in methodz)
                    {
                        valueName.Add(method.Name);
                    }
                }
            }
            prop.ForEach(m => { m.choices.Clear(); m.choices.AddRange(valueName); });
        }

        #region Choice Element Creation

        private void CreateChoice(int id,int eid)
        {
           
            //the port
            Port Choice = this.CreatePort("", Orientation.Horizontal, Direction.Output, Port.Capacity.Single);
            Choice.RegisterCallback<MouseUpEvent, PortPass>(portcheck, new PortPass(Choice, Getindex(Choice), data.id));
            Choice.portName = $"Output"; output.Add(Choice);
            data.ConnectedNodes.Add(-1);
            //Choice Text
            TextField choiceTextfield = DSElementUtilities.CreateTextField("Choice Text", evt =>
            {
                int indeX = Getindex(Choice);
                data.dialogueText[indeX] = evt.newValue;
            },  KeyboardCombo); 
            //Choice Alternative
            TextField alternative = DSElementUtilities.CreateTextField("Alternative", evt =>
            {
                int indeX = Getindex(Choice);
                data.choices[indeX] = evt.newValue;
            },  KeyboardCombo);
            //Greater Than toggle
            Toggle direction = DSElementUtilities.CreateToggle("");
            direction.tooltip = "GreaterThan or equal to value";
            direction.RegisterValueChangedCallback(evt =>
            {
                int indeX = Getindex(Choice);
                data.extraValues[eid + 1] = evt.newValue.ToString();
            }
            );
            //SkipToggle
            Toggle skip = DSElementUtilities.CreateToggle("");
            skip.tooltip = "Skip this choice";
            skip.RegisterValueChangedCallback(evt =>
            {
                int indeX = Getindex(Choice);
                data.extraValues[eid + 2] = evt.newValue.ToString();
            }
            );
            //PropertyName
            DropdownField dropdownmethods = DSElementUtilities.CreateDropDownMenu("     ",
            evt =>
            {
                int indeX = Getindex(Choice);
                data.extraValues[eid] = evt.newValue;
            });
            prop.Add(dropdownmethods);
            if(valueName.Count!=0)
            {
                dropdownmethods.choices.AddRange(valueName);
            }
            //The Value
            TextField ValueText = DSElementUtilities.CreateTextField(data.extraValues[id], evt =>
            {
                int indeX = Getindex(Choice);
                data.extraValues[eid + 3] = evt.newValue;

            });
            ValueText.AddToClassList("value");
            //Adding the values arranged by length of name, very useless and confusing as it's not assigned by assigment order
            //but damn it looks good
           
            skip.value              = bool.Parse(data.extraValues[eid + 2]);
            direction.value         = bool.Parse(data.extraValues[eid + 1]);
            ValueText.value         = data.extraValues[eid + 3];
            alternative.value       = data.choices[id];
            choiceTextfield.value   = data.dialogueText[id];
            dropdownmethods.value   = data.extraValues[eid];

            //TheFirstContainer that contain all the choice shit
            VisualElement container = new VisualElement();
            //The Second container which contains every thing that ain't a variable
            VisualElement container2 = new VisualElement();
            //The Third container which contains every thing that ain't a port
            VisualElement container3 = new VisualElement();
            container.  AddToClassList("multiContainer");
            container2. AddToClassList("secondaryContainer");
            container3. AddToClassList("rowContainer");
           

            //the fetus deletus
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
                int indeX = Getindex(Choice);
                Debug.Log(indeX);
                data.ConnectedNodes.RemoveAt(indeX);
                data.dialogueText.  RemoveAt(indeX);
                data.choices.       RemoveAt(indeX);
                data.extraValues.   RemoveAt((indeX * 4) + 3);
                data.extraValues.   RemoveAt((indeX * 4) + 2);
                data.extraValues.   RemoveAt((indeX * 4) + 1);
                data.extraValues.   RemoveAt((indeX*4));
                outputContainer.    Remove(container);
            });



            container3. Add(DeleteChoice);
            container3. Add(dropdownmethods);
            container3. Add(direction);
            container3. Add(skip);
            container2. Add(container3);
            container2. Add(Choice);
            container3. Add(ValueText);
            container.  Add(container2);
            container.  Add(choiceTextfield);
            container.  Add(alternative);
            outputContainer.Add(container);
            choiceTextfield.Focus();
        }

        private int Getindex(Port text)
        {
            return output.FindIndex(x => x == text);
        }

        private void KeyboardCombo(KeyDownEvent e)
        {
            if (e.altKey && e.keyCode == KeyCode.N)
            {
                int X = data.choices.Count - 1;
                data.dialogueText.Add(data.dialogueText[X]);
                data.choices.Add(data.choices[X]);
                data.extraValues.AddRange(new string[] { data.extraValues[X * 4], data.extraValues[(X * 4) + 1], data.extraValues[(X * 4) + 2], data.extraValues[(X * 4) + 3] });
                CreateChoice((data.choices.Count - 1), (data.choices.Count - 1) * 4);
                RefreshExpandedState();
            }
        }
        #endregion Choice Element Creation
    }
}