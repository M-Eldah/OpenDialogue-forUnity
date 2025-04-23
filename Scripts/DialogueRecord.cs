using System;
using System.Collections.Generic;
using System.Linq;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

//The class for saving modification done to dialogue
/// <summary>
/// when saving changes to dialogue, if changes are applied to source dialoge directly
/// it will cause the original dialogue to be change so we save the changes to an external file
/// using this class which saves the starting dialogue, and any dialogue unlocks
/// </summary>
[System.Serializable]
public class DialogueRecord
{
    public string title;
    public bool startModified;
    public int startindex;
    public List<ModifiedRecord> changes;
    public List<Keys> Vocab;
    public List<Keys> Keys;
    public DialogueRecord(string name="", int startindex=-1)
    {
        title = name;
        changes= new List<ModifiedRecord>();
        Vocab = new List<Keys>();
        Keys = new List<Keys>();
        this.startindex = startindex;
    }
    public void UpdateRecord(DialogueRecord record)
    {
        startModified=record.startModified; 
        startindex=record.startindex;
        this.changes.Clear();
        this.changes.AddRange(record.changes);
        this.Vocab.Clear();
        this.Vocab.AddRange(record.Vocab);
        this.Keys.Clear();
        this.Keys.AddRange(record.Keys);
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
    public void AddVocab(string Key, string Value)
    {
        if (Vocab.FirstOrDefault(i => i.key == Key) == null)
        {
            Vocab.Add(new Keys(Key, Value));
        }
        else
        {
            UpdateVocab(Key, Value);
        }
    }
    public bool HasVocab(string Key)
    {
        return Vocab.FirstOrDefault(i => i.key == Key) != null;
    }
    public bool UpdateVocab(string Key, string Value)
    {
        if (HasVocab(Key))
        {
            Vocab.FirstOrDefault(i => i.key == Key).value = Value;
            return true;
        }
        else
        {
            return false;
        }
    }
    public void AddKey(string Key, string Value)
    {
        if (Keys.FirstOrDefault(i => i.key == Key) == null)
        {
            Keys.Add(new Keys(Key, Value));
        }
        else
        {
            UpdateKey(Key, Value);
        }
    }
    public bool HasKey(string Key)
    {
        return Keys.FirstOrDefault(i => i.key == Key) != null;
    }
    public bool UpdateKey(string Key, string Value)
    {
        if (HasKey(Key))
        {
            Keys.FirstOrDefault(i => i.key == Key).value = Value;
            return true;
        }
        else
        {
            return false;
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
public class Keys
{
    public string key;
    public string value;
    public Keys(string key, string value)
    {
        this.key = key;
        this.value = value;
    }
}