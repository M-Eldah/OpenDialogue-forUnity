using System.Collections.Generic;
using UnityEngine;

namespace DSystem.Elements
{
    using UnityEngine.UIElements;
    using utilities;

    public class DSStageControlNode : UtilityNode
    {
        public override void Initialize(Vector2 Pos, DSGraphView graph)
        {
            base.Initialize(Pos, graph, 1);
            data.subType = SubType.StageControlNode;
            AddToClassList("ActionNode");
            data.choices.Add("Dialouge");
        }

        public override void Initialize(Vector2 Pos, DSGraphView graph, NodeDB db)
        {
            Initialize(Pos, graph);
            data = db;
        }

        public override void Draw()
        {
            base.Draw();
            if (data.extraValues.Count == 1)
            {
                data.extraValues.Add("false");
            }
            VisualElement customDataContainer = new VisualElement();

            DropdownField dropdownmethods = DSElementUtilities.CreateDropDownMenu("Action Type", v =>
            {
                data.extraValues[0] = v.newValue;
            }, new string[] { "Spawn", "FlipSide", "Move", "Flip", "SetLevel" });

            Toggle toggle = DSElementUtilities.CreateToggle("Pause Here", v =>
            {
                data.extraValues[1] = v.newValue.ToString();
            });

            TextField textField = DSElementUtilities.CreateTextField("ID", v =>     { data.q_string1 = v.newValue; });
            TextField textField2 = DSElementUtilities.CreateTextField("Pos/Lvl", v =>    { data.q_string2 = v.newValue; });
            Toggle toggle2 = DSElementUtilities.CreateToggle("Left Side", v =>     { data.q_bool1 = v.newValue; });
            Toggle toggle3 = DSElementUtilities.CreateToggle("Flip all ", v =>     { data.q_bool2 = v.newValue; });
            Foldout textfoldout = DSElementUtilities.CreateFoldout("Data", false);



            if (data.q_string1 != null)
            {
                dropdownmethods.value = data.extraValues[0];
                toggle.value =bool.Parse(data.extraValues[1]);
                textField.value = data.q_string1;
                textField2.value=data.q_string2;
                toggle2.value = data.q_bool1;
                toggle3.value = data.q_bool2;
            }

            textfoldout.Add(dropdownmethods);
            textfoldout.Add(textField);
            textfoldout.Add(textField2);
            textfoldout.Add(toggle2); 
            textfoldout.Add(toggle3);
            textfoldout.Add(toggle);
            customDataContainer.Add(textfoldout);
            extensionContainer.Add(customDataContainer);
            RefreshExpandedState();
        }
    }
}