using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PortPass 
{
    public Port port;
    public int index;
    public int NodeId;
    public PortPass(Port port, int index, int nodeId)
    {
        this.port = port;
        this.index = index;
        NodeId = nodeId;    
    }
}
