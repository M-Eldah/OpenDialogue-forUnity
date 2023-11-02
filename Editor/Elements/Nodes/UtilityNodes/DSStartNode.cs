using UnityEngine;
using System.Collections.Generic;
namespace DSystem.Elements
{
    using System.IO;
    using System.Linq;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine.UIElements;
    using utilities;
    public class DSStartChangeNode : UtilityNode
    {

        public override void Initialize(Vector2 Pos, DSGraphView graph)
        {
            base.Initialize(Pos,graph);

            data.subType = SubType.StartChangeNode;
            AddToClassList("ActionNode");
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
            DropdownField dropdownobjects = DSElementUtilities.CreateDropDownMenu("Dialouge", v => {
                data.q_string1 = v.newValue;
            }
           );
            var Dialouges = Directory.GetFiles("Assets\\OpenDialogue\\Resources\\DialoguesData").Where(s => s.EndsWith(".json")); ;

            foreach (string d in Dialouges.ToArray())
            {
                string name = d.Split("\\")[4];
                dropdownobjects.choices.Add(name);
            }
            TextField textField = DSElementUtilities.CreateTextField("StartNode", v => { data.q_string2 = v.newValue; });
            if(data.q_string1 !=null)
            {
                dropdownobjects.value = data.q_string1;
                textField.value = data.q_string2; 
            }
            customDataContainer.Add(dropdownobjects);
            customDataContainer.Add(textField);
            extensionContainer.Add(customDataContainer);
            RefreshExpandedState();
        }

    }
}
