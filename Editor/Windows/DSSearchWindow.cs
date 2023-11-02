using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DSystem.Windows
{
    public class DSSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        private DSGraphView graphView;
        public Vector2 Position;
        public PortPass port;
        public void Intialiaze(DSGraphView dSGraphView,PortPass port =null)
        {
            graphView = dSGraphView;
            this.port= port;
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            List<SearchTreeEntry> searchTreeEntries = new List<SearchTreeEntry>()
            {
                new SearchTreeGroupEntry(new GUIContent("Nodes")),
                new SearchTreeEntry(new GUIContent("Single Node"))
                {
                     level=1,userData=SubType.SingleNode
                },
                new SearchTreeEntry(new GUIContent("MultiNode"))
                {
                     level=1,userData=SubType.MultiNode
                },
                new SearchTreeEntry(new GUIContent("StageControlNode"))
                {
                     level=1,userData=SubType.StartChangeNode
                },
                new SearchTreeEntry(new GUIContent("ActionNode"))
                {
                     level=1,userData=SubType.ActionNode
                },
                new SearchTreeGroupEntry(new GUIContent("Dialouge Nodes"),1),
                new SearchTreeEntry(new GUIContent("RandomNode"))
                {
                     level=2,userData=SubType.RandomNode
                },
                new SearchTreeEntry(new GUIContent("MRandomNode"))
                {
                     level=2,userData=SubType.MRandomNode
                },
                new SearchTreeEntry(new GUIContent("ValueChoiceNode"))
                {
                     level=2,userData=SubType.ValueChoiceNode
                },
                new SearchTreeEntry(new GUIContent("ValueDirectionNode"))
                {
                     level=2,userData=SubType.ValueDirectionNode
                },
                new SearchTreeEntry(new GUIContent("ScriptNode"))
                {
                     level=2,userData=SubType.ScriptNode
                },
                new SearchTreeGroupEntry(new GUIContent("Utility Nodes"),1),
                new SearchTreeEntry(new GUIContent("ChoiceUnlockNode"))
                {
                     level=2,userData=SubType.ChoiceUnlockNode
                },
                new SearchTreeEntry(new GUIContent("ValuechangeNode"))
                {
                     level=2,userData=SubType.Valuechangenode
                },
                new SearchTreeEntry(new GUIContent("StartChangeNode"))
                {
                     level=2,userData=SubType.StartChangeNode
                },
                new SearchTreeEntry(new GUIContent("AnimationNode"))
                {
                     level=2,userData=SubType.AnimationNode
                },
                new SearchTreeEntry(new GUIContent("AudioNode"))
                {
                     level=2,userData=SubType.AudioNode
                }

            };
            return searchTreeEntries;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            Vector2 localmousepos = Position;
            switch (SearchTreeEntry.userData)
            {
                case SubType.SingleNode:
                    graphView.CreateNode(NodeType.DialogueNode, SubType.SingleNode, localmousepos,port);
                    return true;
                case SubType.RandomNode:
                    graphView.CreateNode(NodeType.DialogueNode, SubType.RandomNode, localmousepos,port);
                    return true;
                case SubType.MRandomNode:
                    graphView.CreateNode(NodeType.DialogueNode, SubType.MRandomNode, localmousepos,port);
                    return true;
                case SubType.MultiNode:
                    graphView.CreateNode(NodeType.DialogueNode, SubType.MultiNode, localmousepos, port);
                    return true;
                case SubType.ValueChoiceNode:
                    graphView.CreateNode(NodeType.DialogueNode, SubType.ValueChoiceNode, localmousepos, port);
                    return true;
                case SubType.ValueDirectionNode:
                    graphView.CreateNode(NodeType.DialogueNode, SubType.ValueDirectionNode, localmousepos, port);
                    return true;
                case SubType.ActionNode:
                    graphView.CreateNode(NodeType.UtilityNode, SubType.ActionNode, localmousepos, port);
                    return true;
                case SubType.Valuechangenode:
                    graphView.CreateNode(NodeType.UtilityNode, SubType.ValueChoiceNode, localmousepos, port);
                    return true;
                case SubType.AnimationNode:
                    graphView.CreateNode(NodeType.UtilityNode, SubType.AnimationNode, localmousepos, port);
                    return true;
                case SubType.AudioNode:
                    graphView.CreateNode(NodeType.UtilityNode, SubType.AudioNode, localmousepos, port);
                    return true;
                case SubType.StartChangeNode:
                    graphView.CreateNode(NodeType.UtilityNode, SubType.StartChangeNode, localmousepos, port);
                    return true;
                case SubType.ChoiceUnlockNode:
                    graphView.CreateNode(NodeType.UtilityNode, SubType.ChoiceUnlockNode, localmousepos, port);
                    return true;
                case SubType.ScriptNode:
                    graphView.CreateNode(NodeType.DialogueNode, SubType.ScriptNode, localmousepos, port);
                    return true;
                case SubType.StageControlNode:
                    graphView.CreateNode(NodeType.UtilityNode, SubType.StageControlNode, localmousepos, port);
                    return true;
                default:
                    return false;
            }
        }
    }
}