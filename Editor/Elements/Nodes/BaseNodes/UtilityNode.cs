using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace DSystem.Elements
{
    using System.Linq;
    using UnityEditor.Experimental.GraphView;
    using utilities;
    public class UtilityNode : BaseNode
    {
        public override void Initialize(Vector2 Pos, DSGraphView graph)
        {
            base.Initialize(Pos,graph);
            data.NodeType = NodeType.UtilityNode;
  
        }
        public virtual void Initialize(Vector2 Pos, DSGraphView graph, int evCount)
        {
            base.Initialize(Pos, graph);
            data.NodeType = NodeType.UtilityNode;
            while(data.extraValues.Count<evCount)
            {
                data.extraValues.Add("");
            }

        }
        public virtual void Initialize(Vector2 Pos, DSGraphView graph, NodeDB dB)
        {
            Debug.Log("Intializae");
            Initialize(Pos, graph);
            data = dB;
        }
        public override void Draw()
        {
            //input container
            base.Draw();
            //output container
            Port Choice = this.CreatePort("Output");
            Choice.RegisterCallback<MouseUpEvent, PortPass>(portcheck, new PortPass(Choice,0,data.id));
            Choice.portName = $"Output";
            if (data.ConnectedNodes.Count == 0)
            { data.ConnectedNodes.Add(-1); }
            output.Add(Choice);
            outputContainer.Add(Choice);
        }
        public void portcheck(MouseUpEvent evt, PortPass port)
        {

            if (port.port.connections.Count() == 0)
            {
                GraphView.OpenSearchMenu(GraphView.GetLocalMousePosition(evt.mousePosition), port);
            }
            else
            {
                List<Edge> edges = new List<Edge>(port.port.connections);
                BaseNode node = (BaseNode)edges[0].input.node;
                data.ConnectedNodes[port.index] = node.data.id;
            }
        }
    }
}


