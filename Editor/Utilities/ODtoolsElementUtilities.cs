using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
namespace OpenDialogue.utilities
{
    using Elements;
    using UnityEditor.UIElements;

    public static class ODtoolsElementUtilities
    {
        #region Toggle
        public static Toggle CreateToggle(string text, EventCallback<ChangeEvent<bool>> onValueChanged = null)
        {
            Toggle toggle = new Toggle() { label = text};
            if (onValueChanged != null)
            {
                toggle.RegisterValueChangedCallback(onValueChanged);
            }
            return toggle;

        }
        #endregion
        #region DropDown
        public static DropdownField CreateDropDownMenu(string text)
        {
            DropdownField dropdownMenu = new DropdownField() { label = text };
          
            return dropdownMenu;

        }
        public static DropdownField CreateDropDownMenu(string text, EventCallback<ChangeEvent<string>> onValueChanged = null, string[] ListItem = null)
        {
            DropdownField dropdownMenu = new DropdownField() { label = text };
            foreach (string item in ListItem)
            {
                dropdownMenu.choices.Add(item);
            }
            if (onValueChanged != null)
            {
                dropdownMenu.RegisterValueChangedCallback(onValueChanged);
            }
            return dropdownMenu;

        }
        public static DropdownField CreateDropDownMenu(string text ,EventCallback<ChangeEvent<string>> onValueChanged = null)
        {
            DropdownField dropdownMenu = new DropdownField() { label = text };
            if (onValueChanged != null)
            {
                dropdownMenu.RegisterValueChangedCallback(onValueChanged);
            }
            return dropdownMenu;

        }
        #endregion
        #region Port
        public static Port CreatePort(this BaseNode node, string Portname = "", Orientation orientation=Orientation.Horizontal,Direction direction = Direction.Output,Port.Capacity capacity = Port.Capacity.Single)
        {
            Port port = node.InstantiatePort(orientation, direction, capacity, typeof(bool));
            port.portName = Portname;
            return port;

        }
        #endregion
        #region Button
        public static Button CreateButton(string buttontext, Action onClick = null)
        {
            Button button = new Button(onClick)
            {
                text = buttontext
            };
            return button;
        }
        #endregion
        #region Foldout
        public static Foldout CreateFoldout(string Title, bool collapsed)
        {
            Foldout foldout = new Foldout()
            {
                text = Title, value = !collapsed
            };
            return foldout;
        }
        #endregion
        #region TextField & textArea
        public static TextField CreateTextField(string value = null, EventCallback<ChangeEvent<string>> onValueChanged = null)
        {
            TextField textField = new TextField()
            {
                value = value

            };
            textField.multiline = true;
            if (onValueChanged != null)
            {
                textField.RegisterValueChangedCallback(onValueChanged);
            }

            return textField;
        }
        public static TextField CreateTextArea(string value = null, EventCallback<ChangeEvent<string>> onValueChanged = null)
        {
            TextField textArea = CreateTextField(value, onValueChanged);

            return textArea;
        }
        public static TextField CreateTextField(string value=null,EventCallback<ChangeEvent<string>> onValueChanged = null, EventCallback<KeyUpEvent> onKeyUpEvent = null)
        {
            TextField textField = new TextField()
            {
                value = value
                
            };
            textField.multiline= true;
            if (onValueChanged != null)
            {
                textField.RegisterValueChangedCallback(onValueChanged);
            }
            if(onKeyUpEvent!=null)
            {
                textField.RegisterCallback<KeyUpEvent>(onKeyUpEvent);
            }
            
            return textField;
        }
        public static TextField CreateTextArea(string value = null, EventCallback<ChangeEvent<string>> onValueChanged = null, EventCallback<KeyUpEvent> onKeyUpEvent=null)
        {
            TextField textArea = CreateTextField(value, onValueChanged,onKeyUpEvent);

            return textArea;
        }
        public static TextField CreateTextField(string value = null, EventCallback<ChangeEvent<string>> onValueChanged = null, EventCallback<KeyUpEvent>[] onKeyUpEvent = null)
        {
            TextField textField = new TextField()
            {
                value = value

            };
            textField.multiline = true;
            if (onValueChanged != null)
            {
                textField.RegisterValueChangedCallback(onValueChanged);
            }
            if (onKeyUpEvent != null)
            {
                foreach (EventCallback<KeyUpEvent> keydown in onKeyUpEvent)
                { textField.RegisterCallback<KeyUpEvent>(keydown); }
            }

            return textField;
        }
        public static TextField CreateTextArea(string value = null, EventCallback<ChangeEvent<string>> onValueChanged = null, EventCallback<KeyUpEvent>[] onKeyUpEvent = null)
        {
            TextField textArea = CreateTextField(value, onValueChanged, onKeyUpEvent);

            return textArea;
        }
        #endregion
        #region ObjectField
        public static ObjectField Objectfield(string value = null, EventCallback<ChangeEvent<UnityEngine.Object>> onValueChanged = null)
        {
            ObjectField Object = new ObjectField(value);
            if (onValueChanged != null)
            {
                Object.RegisterValueChangedCallback(onValueChanged);
            }
            return Object;
        }
        #endregion
    }

}
