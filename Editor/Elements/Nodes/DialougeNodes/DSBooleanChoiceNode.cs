using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace OpenDialouge.Elements
{
    using System.Reflection;
    using utilities;
    /// <summary>
    /// BooleanChoiceNode,Checks for a Method and if that method returns a Boolean 
    /// can be programmed when Checking the Choices at Dialouge Controller
    /// </summary>
    public class DSBooleanChoiceNode : DialogueNode
    {
        private List<string> methodNames;
        private List<DropdownField> prop;

        

        public override void Initialize(Vector2 Pos, DSGraphView graph)
        {
            prop = new List<DropdownField>();
            methodNames = new List<string>();
            base.Initialize(Pos, graph);
            data.subType = SubType.BooleanChoiceNode;
            AddToClassList("MultiNode");
        }

        public override void Initialize(Vector2 Pos, DSGraphView graph, LineData db)
        {
            Initialize(Pos, graph);
            data = db;
        }

        public override void Draw()
        {
            //Main Container
            base.Draw(true, "Value");

            DropdownField dropdownobjects = ODtoolsElementUtilities.CreateDropDownMenu("Object", v =>
            {
                data.q_string1 = v.newValue;
                AddValueNames(v.newValue);
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

            textfoldout.Add(dropdownobjects);

            Button addchoice = ODtoolsElementUtilities.CreateButton("Add Choice", () =>
            {
                int x = data.choices.Count - 1;
                data.dialogueText.Add(data.dialogueText[x]);
                data.choices.Add(data.choices[x]);
                data.extraValues.Add("");
                CreateChoice(x, x, true);
                RefreshExpandedState();
            }
            );

            mainContainer.Insert(1, addchoice);

            if (data.choices.Count == 0)
            {
                data.dialogueText.Add("ChoiceText");
                data.choices.Add("AlternativeText");
                data.extraValues.Add("");
                CreateChoice(0, 0,true);
                RefreshExpandedState();
            }
            else
            {
                dropdownobjects.value = data.q_string1;
                for (int i = 0; i < data.choices.Count; i++)
                {
                    CreateChoice(i, i,false);
                }
            }
            ToggleCollapse();ToggleCollapse();
        }

        private void AddValueNames(string v)
        {
            GameObject gameObject = GameObject.Find(v);
            if (gameObject != null)
            {
                methodNames.Clear();

                methodNames.AddRange(UtilityFunctions.GetMethodsNames(gameObject));
            }
            //Clearing the choices of the drop down field not releated to our choices which is the alternative text for each method
            prop.ForEach(m => { m.choices.Clear(); m.choices.AddRange(methodNames); });
        }

        #region Choice Element Creation

        private void CreateChoice(int id, int eid, bool newport)
        {
            //the port
            Port Choice = this.CreatePort("", Orientation.Horizontal, Direction.Output, Port.Capacity.Single);
            output.Add(Choice);
            Choice.RegisterCallback<MouseUpEvent, PortPass>(Portcheck, new PortPass(Choice, Getindex(Choice), data.id));
            Choice.portName = $"Output"; 
            if (newport)
            { data.ConnectedNodes.Add(-1); }
            //Choice Text
            TextField choiceTextfield = ODtoolsElementUtilities.CreateTextField("Choice Text", evt =>
            {
                int indeX = Getindex(Choice);
                data.dialogueText[indeX] = evt.newValue;
            }, KeyboardCombo);
            //Choice Alternative
            TextField alternative = ODtoolsElementUtilities.CreateTextField("Alternative Text", evt =>
            {
                int indeX = Getindex(Choice);
                data.choices[indeX] = evt.newValue;
            }, KeyboardCombo);
            DropdownField dropdownmethods = ODtoolsElementUtilities.CreateDropDownMenu("",
            evt =>
            {
                int indeX = Getindex(Choice);
                data.extraValues[indeX] = evt.newValue;
            });
            prop.Add(dropdownmethods);
            if (methodNames.Count != 0)
            {
                dropdownmethods.choices.AddRange(methodNames);
            }

            //Adding the values arranged by length of name, very useless and confusing as it's not assigned by assigment order
            //but damn it looks good

            alternative.value = data.choices[id];
            choiceTextfield.value = data.dialogueText[id];
            dropdownmethods.value = data.extraValues[eid];

            //TheFirstContainer that contain all the choice shit
            VisualElement container = new VisualElement();
            //The Second container which contains every thing that ain't a variable
            VisualElement container2 = new VisualElement();
            //The Third container which contains every thing that ain't a port
            VisualElement container3 = new VisualElement();
            container.AddToClassList("multiContainer");
            container2.AddToClassList("secondaryContainer");
            container3.AddToClassList("rowContainer");

            //the fetus deletus
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
                int indeX = Getindex(Choice);
                Debug.Log(indeX);
                data.ConnectedNodes.RemoveAt(indeX);
                data.dialogueText.RemoveAt(indeX);
                data.choices.RemoveAt(indeX);
                data.extraValues.RemoveAt((indeX));
                outputContainer.Remove(container);
            });

            container3.Add(DeleteChoice);
            container3.Add(dropdownmethods);
            container2.Add(container3);
            container2.Add(Choice);
            container.Add(container2);
            container.Add(choiceTextfield);
            container.Add(alternative);
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
                int x = data.choices.Count - 1;
                data.dialogueText.Add(data.dialogueText[x]);
                data.choices.Add(data.choices[x]);
                data.extraValues.Add("");
                CreateChoice(x, x, true);
                RefreshExpandedState();
            }
        }

        #endregion Choice Element Creation
    }
}