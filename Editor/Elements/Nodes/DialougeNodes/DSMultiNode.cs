using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DSystem.Elements
{
    using System;
    using System.Linq;
    using utilities;

    public class DSMultiNode : DialogueNode
    {
        private TextField t;
        public override void Initialize(Vector2 Pos, DSGraphView graph)
        {
            base.Initialize(Pos, graph);
            data.subType = SubType.MultiNode;
            AddToClassList("MultiNode");
        }
        public override void Initialize(Vector2 Pos, DSGraphView graph, NodeDB db)
        {
            Initialize(Pos, graph);
            data = db;
        }

        public override void Draw()
        {
            base.Draw();
            Button addchoice = DSElementUtilities.CreateButton("Add Choice", () =>
            {
                data.choices.Add($"New Choice{data.choices.Count}");
                data.extraValues.Add("False");
                CreateChoice(data.choices.Count - 1);
                RefreshExpandedState();
            }
            );
            mainContainer.Insert(1, addchoice);

            if (data.choices.Count == 0)
            {
                data.choices.Add($"New Choice{data.choices.Count}");
                data.extraValues.Add("False");
                CreateChoice(data.choices.Count - 1);
                RefreshExpandedState();
            }
            else
            {
                for (int i = 0; i < data.choices.Count; i++)
                {
                    CreateChoice(i);
                }
            }
            ToggleCollapse(); ToggleCollapse();
        }

        #region Choice Element Creation

        private void CreateChoice(int id)
        {
            VisualElement container = new VisualElement();
            container.AddToClassList("multiContainer");
            Port Choice = this.CreatePort("", Orientation.Horizontal, Direction.Output, Port.Capacity.Single);
            output.Add(Choice); 
            Choice.RegisterCallback<MouseUpEvent, PortPass>(portcheck, new PortPass(Choice, Getindex(Choice),data.id));
            Choice.portName = $"Output";
            if (data.ConnectedNodes.Count < output.Count)
            { data.ConnectedNodes.Add(-1); }
            TextField choiceTextfield = DSElementUtilities.CreateTextField(data.choices[id], evt =>
            {
                int indeX = Getindex(Choice);
                data.choices[indeX] = evt.newValue;
            }, KeyboardCombo);
            Toggle ElementToggle = DSElementUtilities.CreateToggle("locked", evt =>
            {
                int indeX = Getindex(Choice);
                data.extraValues[indeX] = (string)Convert.ChangeType(evt.newValue, typeof(string));
            });

            ElementToggle.value = (Boolean)Convert.ChangeType(data.extraValues[id], typeof(Boolean));
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
                data.ConnectedNodes .RemoveAt(indeX);
                data.choices        .RemoveAt(indeX);
                data.extraValues    .RemoveAt(indeX); 
                output              .Remove(Choice);
                outputContainer     .Remove(container);
            });
            VisualElement container2 = new VisualElement();
            VisualElement container3 = new VisualElement();
            container3      .AddToClassList("rowContainer");
            container2      .AddToClassList("secondaryContainer");
            DeleteChoice    .AddToClassList("DeleteButton");
            choiceTextfield .AddToClassList("ChoiceText");
            container3      .Add(DeleteChoice);
            container3      .Add(ElementToggle);
            container2      .Add(container3);
            container2      .Add(Choice);
            container       .Add(container2);
            container       .Add(choiceTextfield);           
            outputContainer .Add(container);
            choiceTextfield .Focus();
        }

        private int Getindex(Port text)
        {
            return output.FindIndex(x => x == text);
        }

        #endregion Choice Element Creation

        private void KeyboardCombo(KeyDownEvent e)
        {
            if (e.altKey && e.keyCode == KeyCode.N)
            {
                data.choices.Add($"New Choice{data.choices.Count}");
                data.extraValues.Add("False");
                CreateChoice(data.choices.Count - 1);
                
            }
        }
       
    }
}