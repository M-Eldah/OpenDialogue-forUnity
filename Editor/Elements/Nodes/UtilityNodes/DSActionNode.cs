using UnityEngine;
using System.Collections.Generic;
namespace DSystem.Elements
{
    using System.Reflection;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine.UIElements;
    using utilities;

    public class DSActionNode : UtilityNode
    {
        
        public override void Initialize(Vector2 Pos, DSGraphView graph)
        {
            base.Initialize(Pos,graph,1);
            data.subType = SubType.ActionNode;
            AddToClassList("ActionNode");
            data.choices.Add("Dialogue");
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


            DropdownField dropdownmethods = DSElementUtilities.CreateDropDownMenu("Methods", v =>
            {
                data.q_string2 = v.newValue;
            });
            DropdownField dropdownobjects = DSElementUtilities.CreateDropDownMenu("Objects", v => {
                data.q_string1 = v.newValue;
                GameObject gameObject = GameObject.Find(v.newValue);
                if (gameObject != null)
                {
                    dropdownmethods.choices.Clear();
                    List<MethodInfo> methodz = UtilityFunctions.GetMethods(gameObject);
                    foreach (MethodInfo method in methodz)
                    {
                        dropdownmethods.choices.Add(method.Name);
                    }
                }
            }
            );
            var objects = Resources.FindObjectsOfTypeAll<GameObject>();
            foreach (GameObject obj in objects)
            {
                List<MethodInfo> methodz = UtilityFunctions.GetMethods(obj);
                if (methodz.Count != 0)
                { dropdownobjects.choices.Add(obj.name); }
            }
            Toggle toggle = DSElementUtilities.CreateToggle("Pause Here", v =>
            {
                data.q_bool1 = v.newValue;
            });
            TextField textField = DSElementUtilities.CreateTextField("Parameters", v => { data.extraValues[0] = v.newValue; data.q_bool2 = data.extraValues[0] == "" ? false : true; }) ;
            Foldout textfoldout = DSElementUtilities.CreateFoldout("Data", false);
            if(data.q_string1 !=null)
            {
                dropdownobjects.value= data.q_string1;
                dropdownmethods.value = data.q_string2;
                textField.value = data.extraValues[0]==""? "Parameters": data.extraValues[0];
                //Checking if it has extra Porperties
                data.q_bool2 = data.extraValues[0] == "" ? false : true;
                toggle.value = data.q_bool1;
            }
        
            textfoldout.Add(dropdownobjects);
            textfoldout.Add(dropdownmethods);
            textfoldout.Add(textField);
            textfoldout.Add(toggle);
            customDataContainer.Add(textfoldout);
            extensionContainer.Add(customDataContainer);
            RefreshExpandedState();
        }
        
    }
}