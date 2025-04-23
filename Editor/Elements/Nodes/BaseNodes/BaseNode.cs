using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace OpenDialogue.Elements
{
    using UnityEngine.UIElements;
    using utilities;

    public abstract class BaseNode : Node
    {
        public Port inputport;
        public List<Port> output = new List<Port>();

        public LineData data;

        protected DSGraphView GraphView;
        //Important when inhertee initializee they need this so they get the NodeData Initialized otherwise error
        public BaseNode()
        {
            data = new LineData();
        }
        //Important Initializing the node
        public virtual void Initialize(Vector2 Pos, DSGraphView graph)
        {
          
            GraphView = graph;
            data.pos = Pos;
            SetPosition(new Rect(Pos, Vector2.zero));
        }
      /*  public override void OnSelected()
        {
            AddToClassList("infocus");
            base.OnSelected();
        }
        public override void OnUnselected()
        {
            AddToClassList("outfocus");
            base.OnUnselected();
        }*/
        //importnat Draw into the gtaph
        public virtual void Draw()
        {

            inputport = this.CreatePort("Input", Orientation.Horizontal, Direction.Input, Port.Capacity.Multi);
            inputport.portName = "Input";
            inputContainer.Add(inputport);
            TextField tag = ODtoolsElementUtilities.CreateTextField("Tag", evt => data.Tag = evt.newValue);
            if(data.Tag!=null)
            {
                tag.value = data.Tag;
            }
            tag.label = "Tag";
            extensionContainer.Add(tag);
        }
        //Return a list of edges to used to disconnect the node
        public List<Edge> Edges()
        {
            List<Edge> edges = new List<Edge>();
            foreach (Port port in output)
            {
                edges.AddRange(port.connections);
            }
            foreach (Port port in inputContainer.Children())
            {
                edges.AddRange(port.connections);
            }
            return edges;
        }
        /// <summary>
        /// Connected the node to diffrent node and return a list of edges connected 
        /// </summary>
        /// <param name="Nodes"></param>
        /// <returns></returns>
        public List<Edge> nodeConnect(Dictionary<int, BaseNode> Nodes)
        {
            List<Edge> edge = new List<Edge>();
            for (int i = 0; i < data.ConnectedNodes.Count; i++)
            {
                if (data.ConnectedNodes[i] != -1)
                {
                    if (Nodes.ContainsKey(data.ConnectedNodes[i]))
                    { edge.Add(output[i].ConnectTo(Nodes[data.ConnectedNodes[i]].inputport)); }
                }
            }
            return edge;
        }
    }
}