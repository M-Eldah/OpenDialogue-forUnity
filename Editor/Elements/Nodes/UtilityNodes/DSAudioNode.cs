using UnityEngine;
namespace OpenDialouge.Elements
{
    using UnityEditor.UIElements;
    using UnityEngine.UIElements;
    using utilities;

    public class DSAudioNode : UtilityNode
    {

        public override void Initialize(Vector2 Pos, DSGraphView graph)
        {
            base.Initialize(Pos, graph);

            data.subType = SubType.AudioNode;
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

            ObjectField objectField = ODtoolsElementUtilities.Objectfield("Audio Clip:",evt=>
            {
                data.q_string1 =evt.newValue.name;
            });
            Toggle pauseToggle = ODtoolsElementUtilities.CreateToggle("Pause", evt => data.q_bool1 =evt.newValue);
            objectField.objectType = typeof(AudioClip);  
            if (data.q_string1 != null)
            {
                objectField.value = Resources.Load(data.q_string1);
                pauseToggle.value = data.q_bool1;
            }
            customDataContainer.Add(objectField);
            customDataContainer.Add(pauseToggle);
            extensionContainer.Add(customDataContainer);
            RefreshExpandedState();
        }

    }
}
