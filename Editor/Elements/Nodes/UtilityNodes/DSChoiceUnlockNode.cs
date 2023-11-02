using UnityEngine;
using System.Collections.Generic;
namespace DSystem.Elements
{
    using System.IO;
    using System.Linq;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine.UIElements;
    using utilities;
    public class DSChoiceUnlockNode : UtilityNode
    {

        public override void Initialize(Vector2 Pos, DSGraphView graph)
        {
            base.Initialize(Pos, graph,2);

            data.subType = SubType.ChoiceUnlockNode;
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
            VisualElement customDataContainer = new VisualElement();
            if (data.extraValues[0]=="")
            {
                data.extraValues.Add("");
            }
            DropdownField dropdownobjects = DSElementUtilities.CreateDropDownMenu("Objects", v => {
                data.q_string1 = v.newValue;
            }
            );
            var Dialouges = Directory.GetFiles("Assets\\OpenDialogue\\Resources\\DialoguesData").Where(s => s.EndsWith(".json")); ;

            foreach (string d in Dialouges.ToArray())
            {
                string name = d.Split("\\")[4];
                dropdownobjects.choices.Add(name);
            }
            Toggle toggle = DSElementUtilities.CreateToggle("Lock", v =>
            {
                data.q_bool2 = v.newValue;
            });
            Toggle Type = DSElementUtilities.CreateToggle("Multinode", v =>
            {
                data.q_bool1 = v.newValue;
            });
            TextField textField = DSElementUtilities.CreateTextField("Node", v =>     { data.q_string2 = v.newValue;});
            TextField textField2 = DSElementUtilities.CreateTextField("Value", v => { data.extraValues[0] = v.newValue; });
            textField2.label = "From [Inclusive]";
            TextField textField3 = DSElementUtilities.CreateTextField("Value", v => { data.extraValues[1] = v.newValue; });
            textField3.label = "To [Exclusive]";
            Foldout textfoldout = DSElementUtilities.CreateFoldout("Data", false);
            if (data.q_string1 != null)
            {
                dropdownobjects.value = data.q_string1;
                textField.value = data.q_string2 == "" ? "Node" : data.q_string2;
                textField2.value = data.extraValues[0] == "" ? "Value" : data.extraValues[0];
                textField3.value = data.extraValues[1] == "" ? "Value" : data.extraValues[1];
                toggle.value = data.q_bool2;
                Type.value = data.q_bool1;
            }
            textfoldout.Add(Type);
            textfoldout.Add(dropdownobjects);
            textfoldout.Add(textField);
            textfoldout.Add(textField2);
            textfoldout.Add(textField3);
            textfoldout.Add(toggle);
            customDataContainer.Add(textfoldout);
            extensionContainer.Add(customDataContainer);
            RefreshExpandedState();
        }

    }
}
