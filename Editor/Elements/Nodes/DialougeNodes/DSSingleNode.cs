using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
namespace DSystem.Elements
{
    using UnityEngine.UIElements;
    using utilities;
    public class DSSingleNode : DialogueNode
    {
        public Port Choice;
        public override void Initialize(Vector2 Pos, DSGraphView graph)
        {
            base.Initialize(Pos,graph);
            data.subType = SubType.SingleNode;
            AddToClassList("SingleNode");
            data.choices.Add("Dialouge");
        }

        public override void Initialize(Vector2 Pos, DSGraphView graph,NodeDB Data)
        {
            Initialize(Pos, graph);
            data = Data;
        }
        public override void DrawSingle()
        {
            skipable = true;
            base.DrawSingle();
            RefreshExpandedState();

        }
        
      
    }
}

