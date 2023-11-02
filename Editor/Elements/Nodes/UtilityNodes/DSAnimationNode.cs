using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DSystem.Elements
{
    using DSystem.utilities;
    using System;

    public class DSAnimationNode : UtilityNode
    {
        public override void Initialize(Vector2 Pos, DSGraphView graph)
        {
            base.Initialize(Pos, graph,2);
            data.subType = SubType.AnimationNode;
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
            DropdownField Parameters = DSElementUtilities.CreateDropDownMenu("Parameters", v =>
            {
                data.q_string1 = v.newValue;
            }
            );
            DropdownField dropdownobjects = DSElementUtilities.CreateDropDownMenu("Objects", v =>
            {
                data.extraValues[0] = v.newValue;
                Parameters.choices.Clear();
                Animator animation = GameObject.Find(data.extraValues[0]).GetComponent<Animator>();
                foreach(AnimatorControllerParameter a in animation.parameters)
                {
                    Parameters.choices.Add(a.name);
                }
            }
            );
            TextField textField = DSElementUtilities.CreateTextField("Clip Name", v => {
                    data.extraValues[1] = v.newValue;
            });
            var objects = Resources.FindObjectsOfTypeAll<GameObject>();
            foreach (GameObject obj in objects)
            {
                if (obj.GetComponent<Animator>() != null)
                { dropdownobjects.choices.Add(obj.name); }
            }
            Toggle toggle = DSElementUtilities.CreateToggle("Pause Here", v =>
            {
                data.q_bool1 = v.newValue;
            });
            Toggle toggle2 = DSElementUtilities.CreateToggle("Is Trigger", v =>
            {
               data.q_bool2 = v.newValue;
            });
            TextField textField2 = DSElementUtilities.CreateTextField("Parameter Value", v => { data.q_string2 = v.newValue; });
            Foldout textfoldout = DSElementUtilities.CreateFoldout("Data", false);
            if (data.extraValues[0] != "")
            {
                dropdownobjects.value = data.extraValues[0];
                Parameters.value = data.q_string1;
                textField2.value = data.q_string1 == "" ? "Parameter Value" : data.q_string2;
                Animator animation = GameObject.Find(data.extraValues[0]).GetComponent<Animator>();
                foreach (AnimatorControllerParameter a in animation.parameters)
                {
                    Parameters.choices.Add(a.name);
                }
                toggle.value = data.q_bool1;
                toggle2.value = data.q_bool2;
            }


            textfoldout.Add(dropdownobjects);
            textfoldout.Add(textField);
            textfoldout.Add(Parameters);
            textfoldout.Add(toggle2);
            textfoldout.Add(textField2);
            textfoldout.Add(toggle);
            customDataContainer.Add(textfoldout);
            extensionContainer.Add(customDataContainer);
            RefreshExpandedState();
        }
    }
}