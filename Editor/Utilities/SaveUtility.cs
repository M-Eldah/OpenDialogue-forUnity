using DSystem;
using System.IO;
using UnityEngine;
using UnityEditor;

public class SaveUtility
{
    private static DialogueData dialouge;

    public static void Save(string DialougeName, DSGraphView graphView)
    {
        string savefile = $"Assets/OpenDialogue/Resources/DialoguesData/{DialougeName}.json";
        string jsondata = JsonUtility.ToJson(graphView.data,true) ;
        File.WriteAllText(savefile, jsondata);
        AssetDatabase.Refresh();
    }

    public static DialogueData Load(string DialougeName)
    {
        string savefile = $"Assets/OpenDialogue/Resources/DialoguesData/{DialougeName}";
        if (File.Exists(savefile))
        {
            string JsonData = File.ReadAllText(savefile);
            dialouge = JsonUtility.FromJson<DialogueData>(JsonData);
            return dialouge;
        }
        else
        {
            return null;
        }
       
    }
}
