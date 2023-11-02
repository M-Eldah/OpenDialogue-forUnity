using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using UnityEngine.UIElements;

namespace DSystem.Elements
{
    using utilities;


    public class DSValuechangenode : UtilityNode
    {
        private DropdownField methods;

        public override void Initialize(Vector2 Pos, DSGraphView graph)
        {
            base.Initialize(Pos, graph, 1);

            data.subType = SubType.Valuechangenode;
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

            methods = DSElementUtilities.CreateDropDownMenu("Properties", v =>
            {
                data.q_string2 = v.newValue;
            });
            DropdownField dropdownobjects = DSElementUtilities.CreateDropDownMenu("Objects", v =>
            {
                data.q_string1 = v.newValue;
                FieldMenu();
            }
            );
            var objects = Resources.FindObjectsOfTypeAll<GameObject>();

            DropdownField type = DSElementUtilities.CreateDropDownMenu("Type", evt =>
            {
                data.q_bool1 = evt.newValue == "Field";
                FillGameObjectMenu(dropdownobjects, objects);
                FieldMenu();
            }, new string[] { "Field", "Property" }
            );
            DropdownField OpertationType = DSElementUtilities.CreateDropDownMenu("OperationType", evt =>
            {
                data.q_bool2 = evt.newValue == "Modify";
            }, new string[] { "Modify", "Set" }
            );

            TextField textField = DSElementUtilities.CreateTextField("Value", v => { data.extraValues[0] = v.newValue; });
            Foldout textfoldout = DSElementUtilities.CreateFoldout("Data", false);
            if (data.q_string1 != null)
            {
                dropdownobjects.value = data.q_string1;
                methods.value = data.q_string2;
                textField.value = data.extraValues[0] == "" ? "Parameters" : data.extraValues[0];
                type.value = data.q_bool1 ? "Field" : "Property";
                OpertationType.value = data.q_bool2 ? "Modify" : "Set";
                FillGameObjectMenu(dropdownobjects, objects);
                FieldMenu();
            }
            textfoldout.Add(type);
            textfoldout.Add(OpertationType);
            textfoldout.Add(dropdownobjects);
            textfoldout.Add(methods);
            textfoldout.Add(textField);
            customDataContainer.Add(textfoldout);
            extensionContainer.Add(customDataContainer);
            RefreshExpandedState();
        }

        private void FillGameObjectMenu(DropdownField dropdownobjects, GameObject[] objects)
        {
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

        private void FieldMenu()
        {
            GameObject q_string1ect = GameObject.Find(data.q_string1);
            if (q_string1ect != null)
            {
                methods.choices.Clear();
                if (!data.q_bool1)
                {
                    List<PropertyInfo> methodz = UtilityFunctions.GetProperties(q_string1ect);
                    foreach (PropertyInfo method in methodz)
                    {
                        methods.choices.Add(method.Name);
                    }
                }
                else
                {
                    List<FieldInfo> methodz = UtilityFunctions.GetFields(q_string1ect);
                    foreach (FieldInfo method in methodz)
                    {
                        methods.choices.Add(method.Name);
                    }
                }
            }
        }
    }
}