using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace DSystem.Elements
{
    using Unity.VisualScripting;
    using UnityEditor.Experimental.GraphView;
    using utilities;

    public class DSScriptNode : DialogueNode
    {
        public override void Initialize(Vector2 Pos, DSGraphView graph)
        {
            base.Initialize(Pos, graph);
            data.subType = SubType.ScriptNode;
            AddToClassList("MultiNode");
        }

        public override void Initialize(Vector2 Pos, DSGraphView graph, NodeDB db)
        {
            Initialize(Pos, graph);
            data = db;
        }

        public override void Draw()
        {
            //Input Container
            inputport = this.CreatePort("Input", Orientation.Horizontal, Direction.Input, Port.Capacity.Multi);
            inputport.portName = $"Input";
            inputContainer.Add(inputport);
            //Output Container
            Port outport = this.CreatePort("Output");
            VisualElement visualElement= new VisualElement();
            visualElement.AddToClassList("secondaryContainer");
            outport.RegisterCallback<MouseUpEvent, PortPass>(base.portcheck, new PortPass(outport,0,data.id));
            output.Add(outport);
            data.ConnectedNodes.Add(-1);
            DropdownField methods = DSElementUtilities.CreateDropDownMenu("Objects", v =>
            {
                data.q_string2 = v.newValue;
            }
            );

            DropdownField dropdownobjects = DSElementUtilities.CreateDropDownMenu("Objects", v =>
            {
                data.q_string1 = v.newValue;
                UtilityFunctions.GetMethods(GameObject.Find(data.q_string1)).ForEach(m => { methods.choices.Add(m.Name); });

                
            }
            );
            visualElement.Add(dropdownobjects);
            visualElement.Add(methods);
            visualElement.Add(outport);
            outputContainer.Add(visualElement);
            var objects = Resources.FindObjectsOfTypeAll<GameObject>();
            foreach ( GameObject obj in objects )
            { 
                if(UtilityFunctions.GetMethods(obj).Count!=0)
                {
                    dropdownobjects.choices.Add(obj.name);
                }
            }
            
            TextField actor = DSElementUtilities.CreateTextField("Actor", evt =>
            {
                data.extraValues[0]=evt.newValue;
            });
            actor.label = "Actor";
            TextField Expression = DSElementUtilities.CreateTextField("Face", evt =>
            {
                data.extraValues[1] = evt.newValue;
            });
            Expression.label = "Face";
            TextField Values = DSElementUtilities.CreateTextField("Parameters", evt =>
            {
                data.extraValues[2] = evt.newValue;
            });
            Values.label = "Parameters";
            extensionContainer.Add(actor);
            extensionContainer.Add(Expression);
            extensionContainer.Add(Values);
            RefreshExpandedState();
            if (data.q_string1 !=null)
            {
                dropdownobjects.value = data.q_string1;
                methods.value= data.q_string2;
                actor.value = data.extraValues[0];
                Expression.value = data.extraValues[1];
                Values.value = data.extraValues[2];
            }
            else
            {
                data.extraValues.AddRange(new string[] { "0", "0", "Value" });
            }
        }
    }
}