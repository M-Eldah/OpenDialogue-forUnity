using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Actor",menuName ="Actor")]
public class Actor :ScriptableObject
{
    public bool ally;
    public float size;

    public Sprite[] expression;
    public AudioClip voice;
}
