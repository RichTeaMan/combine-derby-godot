using System;
using System.Collections.Generic;
using Godot;

public static class Utils
{
    public static List<Node> ChildrenRecursive(this Node node)
    {

        var children = new List<Node>();
        foreach (var c in node.GetChildren())
        {
            children.Add(c);
            children.AddRange(c.ChildrenRecursive());
        }
        return children;
    }
}
