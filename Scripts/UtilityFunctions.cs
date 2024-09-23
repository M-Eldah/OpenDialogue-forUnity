using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
//A class of functions that returns commonly used functinos that returns lists
public static class UtilityFunctions
{
    public static List<MonoBehaviour> GetCompoents(GameObject obj)
    {
        var Components = new List<MonoBehaviour>();
        if (obj == null) { return Components; }

        Components.AddRange(obj.GetComponents<MonoBehaviour>());

        
        return Components;
    }
    //returns a list of methods that are attached to a q_string1
    #region Returns list of methods
    public static List<MethodInfo> GetMethods(GameObject obj)
    {
        var methods = new List<MethodInfo>();
        if (obj == null) { return methods; }

        var mbs = obj.GetComponents<MonoBehaviour>();

        var publicFlags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public;

        foreach (MonoBehaviour mb in mbs)
        {
            if(mb!=null)
            {
                methods.AddRange(mb.GetType().GetMethods(publicFlags));
            }
        }
        return methods;
    }
    public static List<string> GetMethodsNames(GameObject obj)
    {
        var methodNames = new List<string>();
        if (obj == null) { return methodNames; }

        var mbs = obj.GetComponents<MonoBehaviour>();

        var publicFlags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public;

        foreach (MonoBehaviour mb in mbs)
        {
            if (mb != null)
            {
                foreach (MethodInfo method in mb.GetType().GetMethods(publicFlags))
                {
                    //if (field.GetType() == typeof(int)) ;
                    methodNames.Add(method.Name);
                };
            }
        }
        return methodNames;
    }
    #endregion
    //Same as Above but with fields and properties
    #region fieldsandproperties
    public static List<FieldInfo> GetFields(GameObject obj)
    {
        var fields = new List<FieldInfo>();
        if (obj == null) { return fields; }

        var mbs = obj.GetComponents<MonoBehaviour>();
        var publicFlags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public;

        foreach (MonoBehaviour mb in mbs)
        {
            if (mb != null)
            {
                fields.AddRange(mb.GetType().GetFields(publicFlags));
            }
        }
        return fields;
    }
    public static List<string> GetFieldNames(GameObject obj)
    {
        var fieldNames = new List<string>();
        if (obj == null) { return fieldNames; }

        var mbs = obj.GetComponents<MonoBehaviour>();
        var publicFlags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public;

        foreach (MonoBehaviour mb in mbs)
        {
            if (mb != null)
            {
                foreach(FieldInfo field in mb.GetType().GetFields(publicFlags))
                {
                    //if (field.GetType() == typeof(int)) ;
                    fieldNames.Add(field.Name);
                };
            }
        }
        return fieldNames;
    }

    public static List<PropertyInfo> GetProperties(GameObject obj)
    {
        var properties = new List<PropertyInfo>();
        if (obj == null) { return properties; }

        var mbs = obj.GetComponents<MonoBehaviour>();
        var publicFlags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

        foreach (MonoBehaviour mb in mbs)
        {
            if (mb != null)
            {
                properties.AddRange(mb.GetType().GetProperties(publicFlags));
            }
        }
        return properties;
    }

    public static List<string> GetPropertiesNames(GameObject obj)
    {
        var propertiesNames = new List<string>();
        if (obj == null) { return propertiesNames; }

        var mbs = obj.GetComponents<MonoBehaviour>();
        var publicFlags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

        foreach (MonoBehaviour mb in mbs)
        {
            if (mb != null)
            {
                foreach (PropertyInfo field in mb.GetType().GetProperties(publicFlags))
                {
                    propertiesNames.Add(field.Name);
                };
            }
        }
        return propertiesNames;
    }
    #endregion
    //Return Type Ironically doesn't return type as in bool and such, what it actually does it return the script name of
    //that the field or property is attached too, it's helpful when we are try to set the value of a property or a field
    #region types
    public static string Type(GameObject obj,FieldInfo field)
    {
        string ty = "";
        var mbs = obj.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour mb in mbs)
        {
            if(mb.GetType().GetField(field.Name)!=null)
            {
                ty = mb.GetType().Name;
            }
        }
        return ty;
    }
    public static string Type(GameObject obj, PropertyInfo property)
    {
        string ty = "";
        var mbs = obj.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour mb in mbs)
        {
            if (mb.GetType().GetProperty(property.Name) != null)
            {
                ty = mb.GetType().Name;
            }
        }
        return ty;
    }
    #endregion
}