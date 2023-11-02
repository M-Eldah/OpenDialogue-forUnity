using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace DSystem.Elements
{
    using UnityEngine.UIElements;
    using utilities;

    public abstract class BaseNode : Node
    {
        public Port inputport;
        public List<Port> output = new List<Port>();

        public NodeDB data;


        protected DSGraphView GraphView;
        public BaseNode()
        {
            data = new NodeDB();
        }

        public virtual void Initialize(Vector2 Pos, DSGraphView graph)
        {
          
            GraphView = graph;
            data.pos = Pos;
            SetPosition(new Rect(Pos, Vector2.zero));
        }
        public override void OnSelected()
        {
            AddToClassList("infocus");
            base.OnSelected();
        }
        public override void OnUnselected()
        {
            AddToClassList("outfocus");
            base.OnUnselected();
        }

        public virtual void Draw()
        {

            inputport = this.CreatePort("Input", Orientation.Horizontal, Direction.Input, Port.Capacity.Multi);
            inputport.portName = $"Input";
            inputContainer.Add(inputport);
            TextField tag = DSElementUtilities.CreateTextField("Tag", evt => data.Tag = evt.newValue);
            if(data.Tag!=null)
            {
                tag.value = data.Tag;
            }
            tag.label = "Tag";
            extensionContainer.Add(tag);
        }

        public List<Edge> DisconnectPorts()
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