using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace OpenDialouge.Elements
{
    using System.Linq;
    using utilities;
    /// <summary>
    /// The parent class for Dialogue containing Lines
    /// </summary>
    public class DialogueNode : BaseNode
    {
        public bool skipable=false;
        public Foldout textfoldout;
        //delegate void Portdel(Port id);

        /// <summary>
        /// Starting the node
        /// </summary>
        /// <param name="Pos"></param>
        /// <param name="graph"></param>
        public override void Initialize(Vector2 Pos,DSGraphView graph)
        {
            base.Initialize(Pos,graph);

            data.NodeType = NodeType.DialogueNode;
        }
        /// <summary>
        /// Starting the node but with data
        /// </summary>
        /// <param name="Pos"></param>
        /// <param name="graph"></param>
        public virtual void Initialize(Vector2 Pos,DSGraphView graph,LineData dB)
        {
            Initialize(Pos,graph);
            data = dB;
        }
        /// <summary>
        /// Drawing the node onto the graph
        /// </summary>
        /// <param name="Pos"></param>
        /// <param name="graph"></param>
        public void Draw(bool extension,string drawName)
        {
            base.Draw();

            if(extension) 
            {
                VisualElement customDataContainer = new VisualElement();
                textfoldout = ODtoolsElementUtilities.CreateFoldout(drawName, false);
                customDataContainer.Add(textfoldout);
                extensionContainer.Add(customDataContainer);
            }
        }
        /// <summary>
        /// For Single, Random and Modified Random nodes
        /// </summary>
        public virtual void DrawSingle()
        {

            base.Draw();
            Port outPort = this.CreatePort("Output");
            //Here
            outPort.RegisterCallback<MouseUpEvent, PortPass>(Portcheck,new PortPass(outPort,0,data.id));
            output.Add(outPort);
            outPort.portName = $"Output";
            outputContainer.Add(outPort);
            if (data.ConnectedNodes.Count == 0)
            { data.ConnectedNodes.Add(-1); }
            //extension Container
            VisualElement customDataContainer = new VisualElement();
            textfoldout = ODtoolsElementUtilities.CreateFoldout("Dialogue", false);
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
            Button addchoice = ODtoolsElementUtilities.CreateButton("Add Dialogue", () =>
            {
                AddChoice();
            }
            );
            customDataContainer.Add(addchoice);
            customDataContainer.Add(textfoldout);
            extensionContainer.Add(customDataContainer);
        }
        //ForNodes whith choices
        private void AddChoice()
        {
            if (skipable)
            {
                data.dialogueText.Add($"Dialogue{data.dialogueText.Count}");
                data.extraValues.Add(data.extraValues[data.extraValues.Count - 3]); 
                data.extraValues.Add(data.extraValues[data.extraValues.Count - 3]); 
                data.extraValues.Add("False");
                CreateDialogueContainer(data.dialogueText[data.dialogueText.Count - 1], data.extraValues[data.extraValues.Count - 3], data.extraValues[data.extraValues.Count - 2], "False");
            }
            else
            {
                data.dialogueText.Add($"Dialogue{data.dialogueText.Count}");
                data.extraValues.Add(data.extraValues[data.extraValues.Count - 2]); 
                data.extraValues.Add(data.extraValues[data.extraValues.Count - 2]);
                CreateDialougeContainer(data.dialogueText[data.dialogueText.Count - 1], data.extraValues[data.extraValues.Count - 2], data.extraValues[data.extraValues.Count - 1]);
            }
        }

        //For skipable

        //**Check what the fuck skip does in Dialogue system Main file
        private void CreateDialogueContainer(string text, string extra, string extra2, string extra3)
        {
            VisualElement cont = new VisualElement();
            TextField textField = ODtoolsElementUtilities.CreateTextArea(text, evt => { int index = Getindex(cont); data.dialogueText[index] = evt.newValue; }, KeyboardCombo);
            textField.RegisterCallback<KeyDownEvent, VisualElement>(KeyboardCombo2, cont);
            textField.AddToClassList("Speachdial    ougeText");
            Foldout Extra = ODtoolsElementUtilities.CreateFoldout("Extra", true);
            TextField Actor = ODtoolsElementUtilities.CreateTextField(extra, evt => { int index = Getindex(cont); data.extraValues[(index * 3)] = evt.newValue; });
            Actor.label = "Actor";
            TextField id = ODtoolsElementUtilities.CreateTextField(extra2, evt => { int index = Getindex(cont); data.extraValues[(index * 3) + 1] = evt.newValue; });
            id.label = "FaceID";

            Toggle toggle = ODtoolsElementUtilities.CreateToggle("Skip", evt => { int index = Getindex(cont); data.extraValues[(index * 3) + 2] = evt.newValue.ToString(); });
            toggle.value = bool.Parse(extra3);
            Button Delte = ODtoolsElementUtilities.CreateButton("Remove Dialogue", () =>
            {
                DeleteEntry(cont);
            });

            Button SwapUp = ODtoolsElementUtilities.CreateButton("SwapUp", () =>
            {
                MoveEntry(cont,-1);
            });
            Button SwapDown = ODtoolsElementUtilities.CreateButton("SwapDown", () =>
            {
                MoveEntry(cont, +1);
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
        /// <summary>
        /// Rearange Dialogue order
        /// </summary>
        /// <param name="cont"></param>
        /// <param name="id"></param>
        private void MoveEntry(VisualElement cont,int id)
        {
            int index = Getindex(cont);
            int index2 = index + id;

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

        /// <summary>
        /// Self Explanatory 
        /// </summary>
        /// <param name="cont"></param>
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
        // Dialogue which doesn't have the option to be skipped
        private void CreateDialougeContainer(string text, string extra, string extra2)
        {
            VisualElement cont = new VisualElement();
            TextField textField = ODtoolsElementUtilities.CreateTextArea(text, evt => { int index = Getindex(cont); data.dialogueText[index] = evt.newValue; }, KeyboardCombo );
            textField.RegisterCallback<KeyDownEvent, VisualElement>(KeyboardCombo2, cont);
            textField.AddToClassList("SpeachdialougeText");
            textField.AddToClassList("SpeachdialougeText");
            Foldout Extra = ODtoolsElementUtilities.CreateFoldout("Extra", true);
            TextField Actor = ODtoolsElementUtilities.CreateTextField(extra, evt => { int index = Getindex(cont); data.extraValues[(index * 2)] = evt.newValue; });
            Actor.label = "Actor";
            TextField id = ODtoolsElementUtilities.CreateTextField(extra2, evt => { int index = Getindex(cont); data.extraValues[(index * 2) + 1] = evt.newValue; });
            id.label = "Face";

            Button Delte = ODtoolsElementUtilities.CreateButton("Remove Dialogue", () =>
            {
                int index = Getindex(cont);
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
        //Get index of a certain used in deleting Dialogue and reanranging 
        private int Getindex(VisualElement text)
        {
            int index = textfoldout.Children().ToList().IndexOf(text);
            return index;
        }
        /// <summary>
        /// Adding a new choice when pressing alt and e
        /// </summary>
        /// <param name="e"></param>
        void KeyboardCombo(KeyDownEvent e)
        {
            
            if (e.altKey && e.keyCode == KeyCode.N)
            {
                AddChoice();
            }
        }
        /// <summary>
        /// Deleting entry when pressing alt and x 
        /// </summary>
        /// <param name="e"></param>
        /// <param name="x"></param>
        void KeyboardCombo2(KeyDownEvent e,VisualElement x)
        {
            if (e.altKey && e.keyCode == KeyCode.X)
            {
                DeleteEntry(x);
            }
        }
        /// <summary>
        /// Check Ports to see if we are currently connect to other ports 
        /// </summary>
        /// <param name="evt"></param>
        /// <param name="port"></param>
        public void Portcheck(MouseUpEvent evt,PortPass port)
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
                data.ConnectedNodes[port.index]=node.data.id;
            }
        }
    }
}