using System;
using System.Collections.Generic;
using System.Linq;

//The class for saving modification done to dialogue
/// <summary>
/// when saving changes to dialogue, if changes are applied to source dialoge directly
/// it will cause the original dialogue to be change so we save the changes to an external file
/// using this class which saves the starting dialogue, and any dialogue unlocks
/// </summary>
[Serializable]
public class DialogueRecord
{
    public string title;
    public bool startModified;
    public int startindex;
    public List<ModifiedRecord> changes;
    public List<Vocab> Variables;
    public DialogueRecord(string name, int startindex)
    {
        title = name;
        changes= new List<ModifiedRecord>();
        Variables = new List<Vocab>();
        this.startindex = startindex;
    }
    public void UpdateRecord(DialogueRecord record)
    {
        startModified=record.startModified; 
        startindex=record.startindex;
        changes.Clear();
        changes.AddRange(record.changes);
        Variables.Clear();
        Variables.AddRange(record.Variables);
    }
    public bool ContainsRecord(int node, int choice)
    {
        return changes.Where(x => x.node == node && x.choice == choice).Count() != 0;
    }
    public bool GetrecordValue(int node, int choice)
    {
        return changes.Where(x => x.node == node && x.choice == choice).FirstOrDefault().value;
    }
    public void SetRecord(int node, int choice,bool value)
    {
        if (ContainsRecord(node, choice))
        {
            changes.Where(x => x.node == node && x.choice == choice).FirstOrDefault().value = value;
        }
        else
        {
            changes.Add(new ModifiedRecord(node, choice, value));
        }
    }
}
[Serializable]
public class ModifiedRecord
{
    public int node;
    public int choice;
    public bool value;

    public ModifiedRecord(int node, int choice, bool value)
    {
        this.node = node;
        this.choice = choice;
        this.value = value;
    }
   
}
[Serializable]
public class Vocab
{
    public string key;
    public string value;
    public Vocab(string key, string value)
    {
        this.key = key;
        this.value = value;
    }
}