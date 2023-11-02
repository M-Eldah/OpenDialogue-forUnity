using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DSystem
{
    using DSystem.Windows;
    using Elements;
    using UnityEngine.Events;

    [System.Serializable]
    public class DSGraphView : GraphView
    {
        public UnityEvent Change;
        private DialogueSystemWindow editorWindow;
        private DSSearchWindow searchWindow;
        public TextField startindex;
        public Dictionary<int, BaseNode> Nodes;
        public bool saved;
        private GameObject holder;
        public DialogueData data;
        public DialogueContainer container;

        public DSGraphView(DialogueSystemWindow window)
        {
            if (Change == null)
                Change = new UnityEvent();
            holder = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/OpenDialogue/Editor/DB.prefab", typeof(GameObject));
            container = holder.GetComponent<DialogueContainer>();
            data = container.Dialogue;
            editorWindow = window;
            this.RegisterCallback<KeyDownEvent>(saveattempt);
            this.RegisterCallback<KeyDownEvent>(copyNodes);
            RegisterCallback<MouseUpEvent>(Updatepos);
            AddManipulators();
            AddGridBackground();
            AddSearchwindow();
            OndeleteElements();
            addStyles();
            Nodes = new Dictionary<int, BaseNode>();
            Undo.undoRedoPerformed += ReloadGraph;
        }

        private void copyNodes(KeyDownEvent evt)
        {
            if (evt.ctrlKey && evt.keyCode == KeyCode.C)
            {
                copySelection();
            }
        }

        private void ReloadGraph()
        {
            ClearGraph(false);
            DrawGraph(data, true);
        }

        private void Updatepos(MouseUpEvent e)
        {
            bool posUpdated = false;
            foreach (GraphElement element in selection)
            {
                if (element is BaseNode node)
                {
                    if (!posUpdated)
                    {
                        posUpdated = node.data.pos != node.GetPosition().position;
                        if (posUpdated)
                        {
                            Undo.RecordObject(container, $"Position Update at {node.GetPosition().position}");
                        }
                    }
                    if (posUpdated)
                    {
                        node.data.pos = node.GetPosition().position;
                    }
                }
            }
        }

        private void saveattempt(KeyDownEvent evt)
        {
            if (evt.ctrlKey && evt.keyCode == KeyCode.S)
            {
                save(data.name);
            }
        }

        #region OverRide Methods

        /// <summary>
        /// Making ports able to connect to other ports of diffrent direction
        /// </summary>
        /// <param name="startPort"></param>
        /// <param name="nodeAdapter"></param>
        /// <returns></returns>
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> Combatiableport = new List<Port>();

            ports.ForEach(Port =>
            {
                if (startPort == Port)
                {
                    return;
                }
                if (startPort.node == Port.node)
                {
                    return;
                }
                if (startPort.direction == Port.direction)
                {
                    return;
                }

                Combatiableport.Add(Port);
            }
            );
            return Combatiableport;
        }
 
        #endregion OverRide Methods

        #region Elements,Manipulators and Keyboard callbacks

        /// <summary>
        /// Manipulators and callbacks used for zooming adding nodes, and registering keyboard Shortcuts
        /// </summary>
        private void AddManipulators()
        {
            SetupZoom(0.1f, 3, 1, 0.5f);
            this.AddManipulator(CreatecopyNode());
            this.AddManipulator(CreateNodeContextualmenu("Add (SingleNode)", NodeType.DialogueNode, SubType.SingleNode));
            this.AddManipulator(CreateNodeContextualmenu("Add (MultiNode)", NodeType.DialogueNode, SubType.MultiNode));
            this.AddManipulator(CreateNodeContextualmenu("Add (RandomNode)", NodeType.DialogueNode, SubType.RandomNode));
            this.AddManipulator(CreateNodeContextualmenu("Add (ModifiedRandomNode)", NodeType.DialogueNode, SubType.MRandomNode));
            this.AddManipulator(CreateNodeContextualmenu("Add (ValueChoiceNode)", NodeType.DialogueNode, SubType.ValueChoiceNode));
            this.AddManipulator(CreateNodeContextualmenu("Add (ValueDirectionNode)", NodeType.DialogueNode, SubType.ValueDirectionNode));
            this.AddManipulator(CreateNodeContextualmenu("Add (ScriptNode)", NodeType.DialogueNode, SubType.ScriptNode));
            this.AddManipulator(CreateNodeContextualmenu("Add (ActionNode)", NodeType.UtilityNode, SubType.ActionNode));
            this.AddManipulator(CreateNodeContextualmenu("Add (ChoiceUnlock)", NodeType.UtilityNode, SubType.ChoiceUnlockNode));
            this.AddManipulator(CreateNodeContextualmenu("Add (ValueChangeNode)", NodeType.UtilityNode, SubType.Valuechangenode));
            this.AddManipulator(CreateNodeContextualmenu("Add (OverwriteStartNode)", NodeType.UtilityNode, SubType.StartChangeNode));
            this.AddManipulator(CreateNodeContextualmenu("Add (AudioNode)", NodeType.UtilityNode, SubType.AudioNode));
            this.AddManipulator(CreateNodeContextualmenu("Add (Animation)", NodeType.UtilityNode, SubType.AnimationNode));
            this.AddManipulator(CreateNodeContextualmenu("Add (StageControlNode)", NodeType.UtilityNode, SubType.StageControlNode));
            this.AddManipulator(CreateNodeContextualmenu("Add (InputNode)", NodeType.UtilityNode, SubType.InputNode));
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(CreateGropContextualMenu());
        }

        #region AddElements

        private IManipulator CreatecopyNode()
        {
            ContextualMenuManipulator contextualMenu = new ContextualMenuManipulator
            (
                menuEvent => menuEvent.menu.AppendAction("Copynode", actionEvent => copySelection())
            );
            return contextualMenu;
        }

        private IManipulator CreateGropContextualMenu()
        {
            ContextualMenuManipulator contextualMenu = new ContextualMenuManipulator
            (
                menuEvent => menuEvent.menu.AppendAction("Add Group", actionEvent => AddElement(Creategroup("DialogueGroup", GetLocalMousePosition(actionEvent.eventInfo.mousePosition))))
            );
            return contextualMenu;
        }

        public Group Creategroup(string Title, Vector2 vector2)
        {
            DSGroup group = new DSGroup()
            {
                title = Title
                
            };
            group.data.GName = Title;
            group.SetPosition(new Rect(vector2, Vector2.zero));
            foreach (GraphElement selected in selection)
            {
                if (!(selected is BaseNode))
                {
                    continue;
                }
                BaseNode node = (BaseNode)selected;
                group.AddElement(node);
            }
            data.Group.Add(group.data);
            Debug.Log(group.data.GroupName);
            return group;
        }

        private IManipulator CreateNodeContextualmenu(string actionTile, NodeType dialogueType, SubType subType)
        {
            ContextualMenuManipulator contextualMenu = new ContextualMenuManipulator
            (
                menuEvent => menuEvent.menu.AppendAction(actionTile, actionEvent => CreateNode(dialogueType, subType, GetLocalMousePosition(actionEvent.eventInfo.mousePosition)))
            );
            return contextualMenu;
        }

        /// <summary>
        /// For new Dialogue
        /// </summary>
        /// <param name="dialogueType"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public void CreateNode(NodeType dialogueType, SubType subType, Vector2 pos, PortPass port = null)
        {
            saved = false;
            switch (dialogueType)
            {
                case NodeType.DialogueNode:
                    Undo.RecordObject(container, $"Creating {subType}");
                    Type nodeType = Type.GetType($"DSystem.Elements.DS{subType}");
                    DialogueNode node = (DialogueNode)Activator.CreateInstance(nodeType);
                    node.title = $"{subType}-ID:{data.id}";
                    node.data.name = node.title;
                    node.data.id = data.id;
                    data.id++;
                    node.Initialize(pos, this);
                    if (subType == SubType.SingleNode || subType == SubType.RandomNode || subType == SubType.MRandomNode)
                    {
                        node.DrawSingle();
                    }
                    else
                    {
                        node.Draw();
                    }
                    AddElement(node);
                    if (selection.Count == 1)
                    {
                        if (selection[0] is DSGroup group)
                        {
                            group.AddElement(node);
                        }
                    }

                    Nodes.Add(node.data.id, node);
                    data.Nodes.Add(node.data);
                    if (port != null)
                    {
                        AddElement(port.port.ConnectTo(node.inputport));
                        Debug.Log(port.NodeId);
                        BaseNode oldNode = Nodes[port.NodeId];
                       
                        oldNode.data.ConnectedNodes[port.index]=node.data.id;
                    }
                    break;

                case NodeType.UtilityNode:
                    Undo.RecordObject(container, $"Creating {subType}");
                    Type UtilitynodeType = Type.GetType($"DSystem.Elements.DS{subType}");
                    UtilityNode Utilitynode = (UtilityNode)Activator.CreateInstance(UtilitynodeType);
                    Utilitynode.title = $"{subType}-ID:{data.id}";
                    Utilitynode.data.name = Utilitynode.title;
                    Utilitynode.data.id = data.id;
                    data.id++;
                    Utilitynode.Initialize(pos, this);
                    Utilitynode.Draw();
                    AddElement(Utilitynode);
                    if (selection.Count == 1)
                    {
                        if (selection[0] is DSGroup group)
                        {
                            group.AddElement(Utilitynode);
                        }
                    }
                    data.Nodes.Add(Utilitynode.data);
                    Nodes.Add(Utilitynode.data.id, Utilitynode);

                    if (port != null)
                    {
                        AddElement(port.port.ConnectTo(Utilitynode.inputport));
                        BaseNode oldNode = Nodes[port.NodeId];
                        oldNode.data.ConnectedNodes[port.index] = Utilitynode.data.id;
                    }
                    break;

                default:
                    break;
            }
            Change.Invoke();
        }

        public void loadNode(Vector2 _pos, NodeDB db, bool addtodata = false)
        {
            SubType subType = db.subType;
            Type nodeType = Type.GetType($"DSystem.Elements.DS{subType}");
            switch (db.NodeType)
            {
                case NodeType.DialogueNode:
                    DialogueNode dNode = (DialogueNode)Activator.CreateInstance(nodeType);
                    dNode.title = db.name;
                    dNode.Initialize(_pos, this, db);
                    if (subType == SubType.SingleNode || subType == SubType.RandomNode || subType == SubType.MRandomNode)
                    {
                        dNode.DrawSingle();
                    }
                    else
                    {
                        dNode.Draw();
                    }

                    AddElement(dNode);
                    Nodes.Add(dNode.data.id, dNode);
                    if (addtodata)
                        data.Nodes.Add(dNode.data);
                    break;

                case NodeType.UtilityNode:
                    UtilityNode UNode = (UtilityNode)Activator.CreateInstance(nodeType);
                    UNode.title = db.name;
                    UNode.Initialize(_pos, this, db);
                    UNode.Draw();
                    AddElement(UNode);
                    Nodes.Add(UNode.data.id, UNode);
                    if (addtodata)
                        data.Nodes.Add(UNode.data);
                    break;
            }
        }

        //Still Untested but should be faster than the Casting used in the normal Load
        public void QuickLoad(Vector2 _pos, NodeDB db, bool addtodata = false)
        {
            SubType subType = db.subType;
            Type nodeType = Type.GetType($"DSystem.Elements.DS{subType}");

            switch (subType)
            {
                case SubType.SingleNode:
                    DSSingleNode snode = new DSSingleNode();

                    snode.Initialize(_pos, this, db);
                    snode.DrawSingle();
                    AddElement(snode);
                    Nodes.Add(snode.data.id, snode);
                    if (addtodata)
                        data.Nodes.Add(snode.data);
                    break;

                case SubType.RandomNode:
                    DSRandomNode Rnode = new DSRandomNode();
                    Rnode.Initialize(_pos, this, db);
                    Rnode.DrawSingle();
                    AddElement(Rnode);
                    Nodes.Add(Rnode.data.id, Rnode);
                    if (addtodata)
                        data.Nodes.Add(Rnode.data);
                    break;

                case SubType.MRandomNode:
                    DSMRandomNode MRnode = new DSMRandomNode();
                    MRnode.Initialize(_pos, this, db);
                    MRnode.DrawSingle();
                    AddElement(MRnode);
                    Nodes.Add(MRnode.data.id, MRnode);
                    if (addtodata)
                        data.Nodes.Add(MRnode.data);
                    break;

                case SubType.MultiNode:
                    DSMultiNode lcnode = new DSMultiNode();
                    lcnode.Initialize(_pos, this, db);
                    lcnode.Draw();
                    AddElement(lcnode);
                    Nodes.Add(lcnode.data.id, lcnode);
                    if (addtodata)
                        data.Nodes.Add(lcnode.data);
                    break;

                case SubType.ValueChoiceNode:
                    DSValueChoiceNode vcnode = new DSValueChoiceNode();
                    vcnode.Initialize(_pos, this, db);
                    vcnode.Draw();
                    AddElement(vcnode);
                    Nodes.Add(vcnode.data.id, vcnode);
                    if (addtodata)
                        data.Nodes.Add(vcnode.data);
                    break;

                case SubType.ScriptNode:
                    DSScriptNode scriptnode = new DSScriptNode();
                    scriptnode.Initialize(_pos, this, db);
                    scriptnode.Draw();
                    AddElement(scriptnode);
                    Nodes.Add(scriptnode.data.id, scriptnode);
                    if (addtodata)
                        data.Nodes.Add(scriptnode.data);
                    break;

                case SubType.ValueDirectionNode:
                    DSValueDirectionNode vdnode = new DSValueDirectionNode();
                    vdnode.Initialize(_pos, this, db);
                    vdnode.Draw();
                    AddElement(vdnode);
                    Nodes.Add(vdnode.data.id, vdnode);
                    if (addtodata)
                        data.Nodes.Add(vdnode.data);
                    break;
                //Action
                case SubType.ActionNode:
                    DSActionNode Anode = new DSActionNode();
                    Anode.Initialize(_pos, this, db);
                    Anode.Draw();
                    AddElement(Anode);
                    Nodes.Add(Anode.data.id, Anode);
                    if (addtodata)
                        data.Nodes.Add(Anode.data);
                    break;

                case SubType.Valuechangenode:
                    DSValuechangenode pnode = new DSValuechangenode();
                    pnode.Initialize(_pos, this, db);
                    pnode.Draw();
                    AddElement(pnode);
                    Nodes.Add(pnode.data.id, pnode);
                    if (addtodata)
                        data.Nodes.Add(pnode.data);
                    break;

                case SubType.ChoiceUnlockNode:
                    DSChoiceUnlockNode cunode = new DSChoiceUnlockNode();
                    cunode.Initialize(_pos, this, db);
                    cunode.Draw();
                    AddElement(cunode);
                    Nodes.Add(cunode.data.id, cunode);
                    if (addtodata)
                        data.Nodes.Add(cunode.data);
                    break;

                case SubType.StartChangeNode:
                    DSStartChangeNode scnode = new DSStartChangeNode();
                    scnode.Initialize(_pos, this, db);
                    scnode.Draw();
                    AddElement(scnode);
                    Nodes.Add(scnode.data.id, scnode);
                    if (addtodata)
                        data.Nodes.Add(scnode.data);
                    break;
                //Audio
                case SubType.AudioNode:
                    DSAudioNode Aunode = new DSAudioNode();
                    Aunode.Initialize(_pos, this, db);
                    Aunode.Draw();
                    AddElement(Aunode);
                    Nodes.Add(Aunode.data.id, Aunode);
                    if (addtodata)
                        data.Nodes.Add(Aunode.data);
                    break;

                case SubType.AnimationNode:
                    DSAnimationNode Aniode = new DSAnimationNode();
                    Debug.Log("LoadNode");
                    Aniode.Initialize(_pos, this, db);
                    Aniode.Draw();
                    AddElement(Aniode);
                    Nodes.Add(Aniode.data.id, Aniode);
                    if (addtodata)
                        data.Nodes.Add(Aniode.data);
                    break;

                case SubType.StageControlNode:
                    DSStageControlNode dscontrolnode = new DSStageControlNode();
                    dscontrolnode.Initialize(_pos, this, db);
                    dscontrolnode.Draw();
                    AddElement(dscontrolnode);
                    Nodes.Add(dscontrolnode.data.id, dscontrolnode);
                    if (addtodata)
                        data.Nodes.Add(dscontrolnode.data);
                    break;
            }
        }

        private void loadGroup(string name, Vector2 pos, List<int> nodes)
        {
            DSGroup group = new DSGroup()
            {
                title = name
            };
            group.data.GName = name;
            group.SetPosition(new Rect(pos, Vector2.zero));
            foreach (int node in nodes)
            {
                group.AddElement(Nodes[node]);
            }
            data.Group.Add(group.data);
            AddElement(group);
        }

        private void AddSearchwindow()
        {
            nodeCreationRequest = context => OpenSearchMenu(GetLocalMousePosition(context.screenMousePosition));
        }

        public bool OpenSearchMenu(Vector2 pos)
        {
            if (searchWindow == null)
            {
                searchWindow = ScriptableObject.CreateInstance<DSSearchWindow>();
                searchWindow.Intialiaze(this);
            }
            searchWindow.Position = pos;
            return SearchWindow.Open(new SearchWindowContext(pos), searchWindow);
        }

        public bool OpenSearchMenu(Vector2 pos, PortPass port)
        {
            if (searchWindow == null)
            {
                searchWindow = ScriptableObject.CreateInstance<DSSearchWindow>();
                searchWindow.Intialiaze(this, port);
            }
            searchWindow.Position = pos;
            searchWindow.port=port;
            return SearchWindow.Open(new SearchWindowContext(pos), searchWindow);
        }

        #endregion AddElements

        #endregion Elements,Manipulators and Keyboard callbacks

        #region Utilities

        public void ClearGraph(bool empty = true)
        {
            if (empty)
            {
                data.id = 0;
                data.startIndex = 0;
                data.groudid = 0;
                data.Nodes.Clear();
                data.Group.Clear();
            }
            Nodes.Clear();
            DeleteElements(graphElements);
        }

        public Vector2 GetLocalMousePosition(Vector2 mousePosition, bool issearchwindow = false)
        {
            Vector2 worldMousePosition = mousePosition;
            if (issearchwindow)
            {
                worldMousePosition -= editorWindow.position.position;
            }
            Vector2 LocalMousepos = contentViewContainer.WorldToLocal(worldMousePosition);
            return LocalMousepos;
        }

        private void copySelection()
        {
            Undo.RecordObject(container, "Copy Nodes");
            Dictionary<int, int> Mapper = new Dictionary<int, int>();
            int offset = 0;
            List<BaseNode> b = new List<BaseNode>();
            //pick out nodes out of the selected grap elements
            foreach (GraphElement ele in selection)
            {
                if (ele is BaseNode node)
                {
                    b.Add(node);
                    continue;
                }
            }
            //Create a mapper to change the connections for the new node
            foreach (BaseNode node in b)
            {
                RemoveFromSelection(node);
                Mapper.Add(node.data.id, data.id + offset);
                offset++; Debug.Log(offset);
            }

            //create the new nodes
            List<int> id= new List<int>();
            foreach (BaseNode node in b)
            {
                Vector2 Cpos = new Vector2(100, 100) + node.data.pos;
                NodeDB nd = node.data.Clone(data.id);
                id.Add(nd.id);
                nd.reMapConnections(Mapper);
                loadNode(Cpos, nd, true);
                data.id++;
            }

            //connected their edges
            selection.Clear();
            foreach (BaseNode node in nodes.ToList())
            {
                if (id.Contains(node.data.id))
                {
                    selection.Add(node);
                    node.AddToClassList("infocus");
                    List<Edge> edges = node.nodeConnect(Nodes);
                    foreach (Edge edge in edges)
                    {
                        AddElement(edge);
                    }
                }
            }
            Change.Invoke();
        }

        private void OndeleteElements()
        {
            List<BaseNode> DeletedNodes = new List<BaseNode>();
            List<DSGroup> DeletedGroups = new List<DSGroup>();
            List<Edge> DeletedEdges = new List<Edge>();
            deleteSelection = (operationName, AskUser) =>
            {
                foreach (GraphElement ele in selection)
                {
                    if (ele is BaseNode node)
                    {
                        DeletedNodes.Add(node);
                        continue;
                    }
                    if (ele is Edge edge)
                    {
                        BaseNode edgenode = (BaseNode)edge.output.node;
                        edgenode.data.ConnectedNodes[edgenode.output.IndexOf(edge.output)] = -1;

                        DeletedEdges.Add(edge);
                        continue;
                    }
                    if (ele is DSGroup group)
                    {
                        DeletedGroups.Add(group);
                    }
                }
                foreach (DSGroup group in DeletedGroups)
                {
                    foreach (int id in group.data.ContainedNodes)
                    {
                        group.RemoveElement(Nodes[id]);
                    }
                    data.Group.Remove(group.data);
                }
                Undo.RecordObject(container, "Delete Nodes");
                foreach (BaseNode Node in DeletedNodes)
                {
                    data.Nodes.Remove(Node.data);
                    Nodes.Remove(Node.data.id);
                    DeletedEdges.AddRange(Node.DisconnectPorts());
                }
                DeleteElements(DeletedNodes);
                DeleteElements(DeletedGroups);
                DeleteElements(DeletedEdges);
                DeletedNodes.Clear();
                DeletedGroups.Clear();
                DeletedEdges.Clear();
                if (nodes.Count() == 0)
                {
                    data.id = 0;
                }
                else
                {
                    int max = 0;
                    foreach (BaseNode node in nodes)
                    {
                        if (node.data.id >= max)
                        {
                            data.id = node.data.id + 1;
                        }
                    }
                }

                startindex.value = data.startIndex.ToString();
                saved = false;
                Change.Invoke();
            };
        }

        public void save(string dialogueName)
        {
            saved = true;
            data.name = dialogueName;
            SaveUtility.Save(dialogueName, this);
            Change.Invoke();
        }

        public int LoadGraph(string dialogueName)
        {
            Nodes.Clear();
            DialogueData dialogue = SaveUtility.Load(dialogueName);
            DrawGraph(dialogue, true);
            data.id = dialogue.id;
            data.startIndex = dialogue.startIndex;
            data.name = dialogue.name;
            return data.startIndex;
        }

        private void DrawGraph(DialogueData dialogue, bool addtodata)
        {
            List<NodeDB> dnodes = new List<NodeDB>();
            dnodes.AddRange(dialogue.Nodes);
            data.Nodes.Clear();
            data.Group.Clear();
            foreach (NodeDB node in dnodes)
            {
                loadNode(node.pos, node, addtodata);
            }

            foreach (BaseNode node in nodes.ToList())
            {
                List<Edge> edges = node.nodeConnect(Nodes);
                foreach (Edge edge in edges)
                {
                    AddElement(edge);
                }
            }
            List<GroupsDB> groups = new List<GroupsDB>();
            groups.AddRange(dialogue.Group);
            foreach (GroupsDB groupDB in groups)
            {
                loadGroup(groupDB.GName, groupDB.Position, groupDB.ContainedNodes);
            }
        }

        #endregion Utilities

        // the refrence to where the styles files are
        private void addStyles()
        {
            StyleSheet GraphstyleSheet = (StyleSheet)EditorGUIUtility.Load("Assets/OpenDialogue/Editor/StyleSheet/DsGraphStyle.uss");
            StyleSheet NodestyleSheet = (StyleSheet)EditorGUIUtility.Load("Assets/OpenDialogue/Editor/StyleSheet/DsNodeStyleSheet.uss");
            styleSheets.Add(GraphstyleSheet);
            styleSheets.Add(NodestyleSheet);
        }

        private void AddGridBackground()
        {
            GridBackground gridBackground = new GridBackground();
            gridBackground.StretchToParentSize();
            Insert(0, gridBackground);
        }
    }
}