using OpenDialogue;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveStates : MonoBehaviour
{
    public GameObject container;
    public TMP_InputField inputField;
    public GameObject loadDefault;
    private List<GameObject> Loads;
    private void OnEnable()
    {
        Loads = new List<GameObject>();
        ReloadSaveStates();
    }

    private void ReloadSaveStates()
    {
        List<string> Saves = DialogueSystem.Dialoguelist();
        if(Loads.Count>0)
        {
            foreach (GameObject load in Loads)
            {
                Destroy(load);
            }
            Loads.Clear();
        }
        foreach (string save in Saves)
        {
            GameObject load = Instantiate(loadDefault, container.transform);
            load.GetComponentInChildren<TextMeshProUGUI>().text = save;
            load.GetComponentInChildren<Button>().onClick.AddListener(() => LoadDialogueState(save));
            Loads.Add(load);
        }
    }

    public void Save()
    {
        SaveDialogueState(inputField.text);
        ReloadSaveStates();
    }

    void SaveDialogueState(string name)
    {
        DialogueSystem.Save(name);
    }

    // Update is called once per frame
    void LoadDialogueState(string name)
    {
        DialogueSystem.Load(name);
    }
}
