using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValueTestScript : MonoBehaviour
{
    [SerializeField]
    int randomValue;
    public int RandomValue { get { return randomValue; } set { randomValue = value; } }

    public int SetValue;

    public string Value;
    

    public string Greeting()
    {
        return $"Hello {Value} I hope you have a wonderful day";
    }
    public void AddKey(string key)
    {
        DialogueSystem.UpdateVocab($"{key}", Value);
    }
}
