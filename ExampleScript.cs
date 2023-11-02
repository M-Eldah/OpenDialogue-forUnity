
using UnityEngine;
using TMPro;
public class ExampleScript : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;
    private float    floatfieldEx;
    public bool     boolfieldEx;
    public int      intfieldEx;
    public int      intfieldEx2;
    public int      intfieldEx3;
    public string   sname;
    public float floatpropertyEx 
    { 
        get { return floatfieldEx; } 
        set { floatfieldEx = value; } 
    }
    private void Update()
    {

    }
    public void Start()
    {
        //textMeshPro.autoSizeTextContainer = true;
    }
    public void TestMethod()
    {
        Debug.Log("Method with no parameters");
    }
    public void TestMethod(string n)
    {
        Debug.Log($"Method which takes a parameter, with an example of {n}");
    }
    public void TestMethod(float n,string v,int b,bool z)
    {
        Debug.Log($"Method which takes a parameter, with an example of {n},{b},{v},{z}");
    }

    public string TestReturn(string v,int b)
    {
        return $"yes i know where {v} is he is at {getlocation(b)}";
    }
    public float TestValue(int v)
    {
        return v*0.5f;
    }
    string getlocation(int b)
    {
        return "the observatory";
    }
}
