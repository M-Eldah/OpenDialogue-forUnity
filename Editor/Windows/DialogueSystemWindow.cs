using OpenDialogue.utilities;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace OpenDialogue.Windows
{
    
    public class DialogueSystemWindow : EditorWindow
    {
        private DSGraphView dSGraphView;
        private Button save;
        private TextField index;
        [SerializeField]
        public static string Dname;
        [MenuItem("Window/Dialouge Graph")]
        public static void Open()
        {
            GetWindow<DialogueSystemWindow>("Dialouge Graph");
        }

        private void CreateGUI()
        {
            AddGraphView();
            AddToolbar();
        }
        
        private void AddToolbar()
        {
            Toolbar toolbar = new Toolbar();
            TextField FileNameTextField = ODtoolsElementUtilities.CreateTextField("", null);
            if (Dname != null)
            { 
                FileNameTextField.value = Dname.Split(".")[0]; 
            }
            Button SaveButton = ODtoolsElementUtilities.CreateButton("Save", () => {
                dSGraphView.Save(FileNameTextField.text);
                save.style.backgroundColor= new Color(0.345098f, 0.345098f, 0.345098f,1);
                save.style.color=Color.white;
            });
            save = SaveButton;
            DropdownField LoadMenu = ODtoolsElementUtilities.CreateDropDownMenu("SelectDialogue");
            var Dialouges = Directory.GetFiles("Assets\\OpenDialogue\\Resources\\DialoguesData").Where(s => s.EndsWith(".json")); ;

            foreach(string d in Dialouges.ToArray())
            {
                string name = d.Split("\\")[4];
                LoadMenu.choices.Add(name);
            }

            TextField StartingIndex = ODtoolsElementUtilities.CreateTextField("0", evt => {
                dSGraphView.data.startIndex = int.Parse(evt.newValue);
                if (!dSGraphView.Nodes.ContainsKey(int.Parse(index.value)))
                {
                    index.style.backgroundColor = Color.red;
                }
                else
                {
                    index.style.backgroundColor =new Color(1,0,0,0);
                    index.style.color = Color.white;
                }
            });
            index = StartingIndex;
            StartingIndex.label = "StartingIndex";
            dSGraphView.startindex = StartingIndex;
            Button LoadButton = ODtoolsElementUtilities.CreateButton("Load",()=> {
                Dname = LoadMenu.text;
                FileNameTextField.value = LoadMenu.text.Split(".")[0];
                dSGraphView.ClearGraph();
                StartingIndex.value = dSGraphView.LoadGraph(LoadMenu.text).ToString() ;
            });
            Button ClearGraph = ODtoolsElementUtilities.CreateButton("Clear Graph", () => {
                dSGraphView.ClearGraph();
            });
            Button FocusOrigin = ODtoolsElementUtilities.CreateButton("Focus Origin", () => {
                dSGraphView.FrameOrigin();
            });

            dSGraphView.Change.AddListener(delegate () { checkgraph(); });
            toolbar.Add(FileNameTextField);
            toolbar.Add(SaveButton);
            toolbar.Add(LoadMenu);
            toolbar.Add(LoadButton);
            toolbar.Add(StartingIndex);
            toolbar.Add(ClearGraph);
            toolbar.Add(FocusOrigin);
            rootVisualElement.Add(toolbar);
        }

        #region Element Addition
        public void checkgraph()
        {
            if (dSGraphView.saved == false)
            {
                save.style.backgroundColor = Color.yellow;
                save.style.color = Color.black;
            }
            else
            {
                save.style.backgroundColor = new Color(0.345098f, 0.345098f, 0.345098f, 1);
                save.style.color = Color.white;
            }
            if(!dSGraphView.Nodes.ContainsKey(int.Parse(index.value)))
            {
                index.style.backgroundColor= Color.red;
                index.style.color = Color.black;
            }
            else
            {
                index.style.backgroundColor = new Color(1, 0, 0, 0);
                index.style.color = Color.white;
            }

        }
        private void AddGraphView()
        {
            //throw new NotImplementedException();
            DSGraphView graphView = new DSGraphView(this);
            graphView.StretchToParentSize();
            rootVisualElement.Add(graphView);
            dSGraphView = graphView;
            if(Dname!=null&&Dname!="")
            {
                dSGraphView.LoadGraph(Dname);
            }
        }
      
        
        #endregion Element Addition
    }
}