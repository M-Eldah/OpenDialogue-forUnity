using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace OpenDialouge.Elements
{
    using Unity.VisualScripting;
    using UnityEditor.Experimental.GraphView;
    using utilities;
    /// <summary>
    /// Returns a customer string from a method 
    /// </summary>
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
            outport.RegisterCallback<MouseUpEvent, PortPass>(base.Portcheck, new PortPass(outport,0,data.id));
            output.Add(outport);
            data.ConnectedNodes.Add(-1);
            DropdownField methods = ODtoolsElementUtilities.CreateDropDownMenu("Objects", v =>
            {
                data.q_string2 = v.newValue;
            }
            );

            DropdownField dropDownObjects = ODtoolsElementUtilities.CreateDropDownMenu("Objects", v =>
            {
                data.q_string1 = v.newValue;
                methods.choices.AddRange(UtilityFunctions.GetMethodsNames(GameObject.Find(data.q_string1)));
                
            }
            );
            visualElement.Add(dropDownObjects);
            visualElement.Add(methods);
            visualElement.Add(outport);
            outputContainer.Add(visualElement);
            var objects = Resources.FindObjectsOfTypeAll<GameObject>();
            foreach ( GameObject obj in objects )
            { 
                if(UtilityFunctions.GetMethods(obj).Count!=0)
                {
                    dropDownObjects.choices.Add(obj.name);
                }
            }
            
            TextField actor = ODtoolsElementUtilities.CreateTextField("Actor", evt =>
            {
                data.extraValues[0]=evt.newValue;
            });
            actor.label = "Actor";
            TextField expression = ODtoolsElementUtilities.CreateTextField("Face", evt =>
            {
                data.extraValues[1] = evt.newValue;
            });
            expression.label = "Face";
            TextField parameters = ODtoolsElementUtilities.CreateTextField("Parameters", evt =>
            {
                data.extraValues[2] = evt.newValue;
            });
            parameters.label = "Parameters";
            extensionContainer.Add(actor);
            extensionContainer.Add(expression);
            extensionContainer.Add(parameters);
            RefreshExpandedState();
            if (data.q_string1 !=null)
            {
                dropDownObjects.value = data.q_string1;
                methods.value= data.q_string2;
                actor.value = data.extraValues[0];
                expression.value = data.extraValues[1];
                parameters.value = data.extraValues[2];
            }
            else
            {
                data.extraValues.AddRange(new string[] { "0", "0", "Value" });
            }
        }
    }
}