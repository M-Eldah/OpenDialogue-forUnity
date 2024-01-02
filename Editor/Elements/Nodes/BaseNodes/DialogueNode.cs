using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DSystem.Elements
{
    using System.Linq;
    using utilities;
    public class DialogueNode : BaseNode
    {
        public bool skipable=false;
        public Foldout textfoldout;
        delegate void Portdel(Port id);
        public override void Initialize(Vector2 Pos,DSGraphView graph)
        {
            base.Initialize(Pos,graph);

            data.NodeType = NodeType.DialogueNode;
        }

        public virtual void Initialize(Vector2 Pos,DSGraphView graph,NodeDB dB)
        {
            Initialize(Pos,graph);
            data = dB;
        }

        public void Draw(bool extension,string drawName)
        {
            base.Draw();

            if(extension) 
            {
                VisualElement customDataContainer = new VisualElement();
                textfoldout = DSElementUtilities.CreateFoldout(drawName, false);
                customDataContainer.Add(textfoldout);
                extensionContainer.Add(customDataContainer);
            }
        }
        public virtual void DrawSingle()
        {

            base.Draw();
            Port Choice = this.CreatePort("Output");
            //Here
            Choice.RegisterCallback<MouseUpEvent, PortPass>(portcheck,new PortPass(Choice,0,data.id));
            output.Add(Choice);
            Choice.portName = $"Output";
            outputContainer.Add(Choice);
            if (data.ConnectedNodes.Count == 0)
            { data.ConnectedNodes.Add(-1); }
            //extension Container
            VisualElement customDataContainer = new VisualElement();
            textfoldout = DSElementUtilities.CreateFoldout("Dialogue", false);
            if (data.dialogueText.Count == 0)
            {
                if (skipable)
                {
                    data.dialogueText.Add($"Dialogue{data.dialogueText.Count}");
                    data.extraValues.Add("0"); data.extraValues.Add("0"); data.extraValues.Add("False");
                    CreateDialogueContainer(data.dialogueText[data.dialogueText.Count - 1], "0", "0", "False");
                }
                else
                {
                    data.dialogueText.Add($"Dialogue{data.dialogueText.Count}");
                    data.extraValues.Add("0"); data.extraValues.Add("0"); 
                    CreateDialougeContainer(data.dialogueText[data.dialogueText.Count - 1], "0", "0");
                }
            }
            else
            {
                for (int i = 0; i < data.dialogueText.Count; i++)
                {
                    if (skipable)
                    {
                        CreateDialogueContainer(data.dialogueText[i], data.extraValues[(i * 3)], data.extraValues[(i * 3) + 1], data.extraValues[(i * 3) + 2]);
                    }
                    else
                    {
                        CreateDialougeContainer(data.dialogueText[i], data.extraValues[(i * 2)], data.extraValues[(i * 2) + 1]);
                    }
                }
            }
            Button addchoice = DSElementUtilities.CreateButton("Add Dialogue", () =>
            {
                AddChoice();
            }
            );
            customDataContainer.Add(addchoice);
            customDataContainer.Add(textfoldout);
            extensionContainer.Add(customDataContainer);
        }

        private void AddChoice()
        {
            if (skipable)
            {
                data.dialogueText.Add($"Dialogue{data.dialogueText.Count}");
                data.extraValues.Add(data.extraValues[data.extraValues.Count - 3]); data.extraValues.Add(data.extraValues[data.extraValues.Count - 3]); data.extraValues.Add("False");
                CreateDialogueContainer(data.dialogueText[data.dialogueText.Count - 1], data.extraValues[data.extraValues.Count - 3], data.extraValues[data.extraValues.Count - 2], "False");
            }
            else
            {
                data.dialogueText.Add($"Dialogue{data.dialogueText.Count}");
                data.extraValues.Add(data.extraValues[data.extraValues.Count - 2]); data.extraValues.Add(data.extraValues[data.extraValues.Count - 2]); ;
                CreateDialougeContainer(data.dialogueText[data.dialogueText.Count - 1], data.extraValues[data.extraValues.Count - 2], data.extraValues[data.extraValues.Count - 1]);
            }
        }

        //for skipable
        private void CreateDialogueContainer(string text, string extra, string extra2, string extra3)
        {
            VisualElement cont = new VisualElement();
            TextField textField = DSElementUtilities.CreateTextArea(text, evt => { int index = Getindex(cont); data.dialogueText[index] = evt.newValue; }, KeyboardCombo);
            textField.RegisterCallback<KeyDownEvent, VisualElement>(KeyboardCombo2, cont);
            textField.AddToClassList("Speachdial    ougeText");
            Foldout Extra = DSElementUtilities.CreateFoldout("Extra", true);
            TextField Actor = DSElementUtilities.CreateTextField(extra, evt => { int index = Getindex(cont); data.extraValues[(index * 3)] = evt.newValue; });
            Actor.label = "Actor";
            TextField id = DSElementUtilities.CreateTextField(extra2, evt => { int index = Getindex(cont); data.extraValues[(index * 3) + 1] = evt.newValue; });
            id.label = "FaceID";

            Toggle toggle = DSElementUtilities.CreateToggle("Skip", evt => { int index = Getindex(cont); data.extraValues[(index * 3) + 2] = evt.newValue.ToString(); });
            toggle.value = bool.Parse(extra3);
            Button Delte = DSElementUtilities.CreateButton("Remove Dialogue", () =>
            {
                DeleteEntry(cont);
            });

            Button SwapUp = DSElementUtilities.CreateButton("SwapUp", () =>
            {
                Swap(cont,-1);
            });
            Button SwapDown = DSElementUtilities.CreateButton("SwapDown", () =>
            {
                Swap(cont, +1);
            });

            Extra.Add(Actor);
            Extra.Add(id);
            Extra.Add(toggle);
            Extra.Add(Delte);
            cont.Add(textField);
            cont.Add(Extra);
            cont.Add(SwapUp);
            cont.Add(SwapDown);
            cont.AddToClassList("singledialougeholder");
            textfoldout.Add(cont);
            textField.Focus();
        }
        private void Swap(VisualElement cont,int d)
        {
            int index = Getindex(cont);
            int index2 = index + d;

            for (int i = 0; i < 3; i++)
            {
                string temp="";
                temp = data.extraValues[(index * 3) + i];
                data.extraValues[(index * 3) + i] = data.extraValues[(index2 * 3) + i];
                data.extraValues[(index2 * 3) + i] = temp;
            }
            string tempD = data.dialogueText[index];
            data.dialogueText[index] = data.dialogueText[index2];
            data.dialogueText[index2] = tempD;


            TextField DField= (TextField)cont.Children().ToList()[0];
            DField.value = data.dialogueText[index];
            Foldout foldout = (Foldout)cont.Children().ToList()[1];
            TextField actorField= (TextField)foldout.Children().ToList()[0];
            actorField.value = data.extraValues[(index * 3)];
            TextField FaceField = (TextField)foldout.Children().ToList()[1];
            FaceField.value = data.extraValues[(index * 3)+1];
            Toggle toggle = (Toggle)foldout.Children().ToList()[2];

            toggle.value =bool.Parse(data.extraValues[(index * 3) + 2]);

            cont = textfoldout.Children().ToList()[index2];
            DField = (TextField)cont.Children().ToList()[0];
            DField.value = data.dialogueText[index2];
            foldout = (Foldout)cont.Children().ToList()[1];
            actorField = (TextField)foldout.Children().ToList()[0];
            actorField.value = data.extraValues[(index2 * 3)];
            FaceField = (TextField)foldout.Children().ToList()[1];
            FaceField.value = data.extraValues[(index2 * 3) + 1];
            toggle = (Toggle)foldout.Children().ToList()[2];
            toggle.value = bool.Parse(data.extraValues[(index2 * 3) + 2] );


        }
        private void DeleteEntry(VisualElement cont)
        {
            int index = Getindex(cont);

            data.dialogueText.RemoveAt(index);
            data.extraValues.RemoveAt((index * 3) + 2);
            data.extraValues.RemoveAt((index * 3) + 1);
            data.extraValues.RemoveAt((index * 3));
            textfoldout.Remove(cont);
        }

        //for unskipable
        private void CreateDialougeContainer(string text, string extra, string extra2)
        {
            VisualElement cont = new VisualElement();
            TextField textField = DSElementUtilities.CreateTextArea(text, evt => { int index = Getindex(cont); data.dialogueText[index] = evt.newValue; }, KeyboardCombo );
            textField.RegisterCallback<KeyDownEvent, VisualElement>(KeyboardCombo2, cont);
            textField.AddToClassList("SpeachdialougeText");
            textField.AddToClassList("SpeachdialougeText");
            Foldout Extra = DSElementUtilities.CreateFoldout("Extra", true);
            TextField Actor = DSElementUtilities.CreateTextField(extra, evt => { int index = Getindex(cont); data.extraValues[(index * 2)] = evt.newValue; });
            Actor.label = "Actor";
            TextField id = DSElementUtilities.CreateTextField(extra2, evt => { int index = Getindex(cont); data.extraValues[(index * 2) + 1] = evt.newValue; });
            id.label = "Face";

            Button Delte = DSElementUtilities.CreateButton("Remove Dialogue", () =>
            {
                int index = Getindex(cont);
                Debug.Log(index);

                data.dialogueText.RemoveAt(index);
                data.extraValues.RemoveAt((index * 2) + 1);
                data.extraValues.RemoveAt((index * 2));
                textfoldout.Remove(cont);
            });
            Extra.Add(Actor);
            Extra.Add(id);
            cont.Add(textField);
            cont.Add(Extra);
            Extra.Add(Delte);
            cont.AddToClassList("singledialougeholder");
            textfoldout.Add(cont);
            textField.Focus();
        }

        private int Getindex(VisualElement text)
        {
            int index = textfoldout.Children().ToList().IndexOf(text);
            return index;
        }
        void KeyboardCombo(KeyDownEvent e)
        {
            
            if (e.altKey && e.keyCode == KeyCode.N)
            {
                AddChoice();
            }
        }
        void KeyboardCombo2(KeyDownEvent e,VisualElement x)
        {
            if (e.altKey && e.keyCode == KeyCode.X)
            {
                DeleteEntry(x);
            }
        }
        public void portcheck(MouseUpEvent evt,PortPass port)
        {
            BaseNode n = (BaseNode)port.port.node;
            if(port.port.connections.Count()==0)
            {
                GraphView.OpenSearchMenu(GraphView.GetLocalMousePosition(evt.mousePosition), port);
            }
            else
            {
                List<Edge> edges = new List<Edge>(port.port.connections);
                BaseNode node = (BaseNode)edges[0].input.node;
                Debug.Log(port.index);
                data.ConnectedNodes[port.index]=node.data.id;
            }
        }
    }
}