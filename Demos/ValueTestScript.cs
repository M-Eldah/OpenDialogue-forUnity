using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValueTestScript : MonoBehaviour
{
    [SerializeField]
    int randomValue;
    public int RandomValue { get { return randomValue; } set { randomValue = value; } }

    public int SetValue;

    public string CharacterName;
    

    public string Greeting()
    {
        return $"Hello {CharacterName} I hope you have a wonderful day";
    }
}
