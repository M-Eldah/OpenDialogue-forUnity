using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class DialogueHandeler : MonoBehaviour
{
    [SerializeField]
    private int OverRideStartNode = -1;
    [SerializeField]
    public int ORSNode { get { return OverRideStartNode; } set { OverRideStartNode = value; }}
    [SerializeField]
    private string DialogueName;
    [SerializeField]
    public string Dname { get { return DialogueName; } set { DialogueName = value; } }
    [SerializeField]
    private int index;
    [SerializeField]
    public int Dindex { get { return index; } set { index = value; } }
    [SerializeField]
    public DialogueValues data;
    [SerializeField]
    public NodeDB[] nodes;
    public DialogueValues DialogueData 
    { 
        get {
            data.nodes = nodes;
            return data; 
        } 
        set {
            data = value; 
        } 
    }

    //the array of actors we choose from
    public Actor[] actors;
    private void Awake()
    {
        if(data.Name=="")
        {
            LoadData(Dname);
        }
    }
    public void LoadData(string dialogueName)
    {
            if (dialogueName == data.Name)
            {
                Debug.Log("Data already loaded");
            }
            else
            {
                var savefile = Resources.Load<TextAsset>($"DialoguesData/{dialogueName}");
                if (savefile != null)
                {
                    data=new DialogueValues(JsonUtility.FromJson<DialogueData>(savefile.text));
                    nodes = data.nodes;
                }

            }

      
    }

    public void Cleardata()
    {
        data =new DialogueValues();
        nodes= new NodeDB[0];
    }
}