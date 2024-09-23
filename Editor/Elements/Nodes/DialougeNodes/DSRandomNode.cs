using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
namespace OpenDialouge.Elements
{
    using utilities;
    /// <summary>
    /// Random dialouge return
    /// </summary>
    public class DSRandomNode : DialogueNode
    {
        public Port Choice;
        public override void Initialize(Vector2 Pos, DSGraphView graph)
        {

            base.Initialize(Pos,graph);
            data.subType = SubType.RandomNode;
            AddToClassList("SingleNode");
            data.choices.Add("Dialouge");
        }

        public override void Initialize(Vector2 Pos,DSGraphView graph, NodeDB db)
        {
            Initialize(Pos,graph);
            data=db;
        }
        public override void DrawSingle()
        {
            base.DrawSingle();
            RefreshExpandedState();
        }


    }
}

