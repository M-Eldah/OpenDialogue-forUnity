using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenDialogue;

public class ValueTestScript : MonoBehaviour
{

    public int SetValue;

    public string Value;
    
    public int PropertyINT
    { get; }

    public string Greeting()
    {
        return $"Hello {Value} I hope you have a wonderful day";
    }
    public void Greeting1(string Name)
    {
        Debug.Log($"Hello {Name} I hope you have a wonderful day");
    }
    public void AddKey(string key)
    {
        DialogueSystem.AddKey($"{key}", Value);
    }

    public bool CheckTest(int i)
    {
        return i == 2;
        
    }
}
