using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
namespace DSystem.Elements
{
    using UnityEngine.UIElements;
    using utilities;
    public class DSGroup : Group
    {
        public GroupsDB data;
        public DSGroup()
        {
            data=new GroupsDB();
        }
        protected override void OnGroupRenamed(string oldName, string newName)
        {
            Debug.Log(newName);
            data.GName = newName;
        }

        protected override void OnElementsAdded(IEnumerable<GraphElement> elements)
        {
            foreach (BaseNode element in elements) 
            {
                data.ContainedNodes.Add(element.data.id);
            }
        }
        
        protected override void OnElementsRemoved(IEnumerable<GraphElement> elements)
        {
            foreach (BaseNode element in elements)
            {
                data.ContainedNodes.Remove(element.data.id);
            }
        }

       
    }

}
