using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValueTestScript : MonoBehaviour
{

    public int SetValue;

    public string Value;
    

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
        DialogueSystem.UpdateVocab($"{key}", Value);
    }

    public bool CheckTest(int i)
    {
        return i == 2;
        
    }
}
