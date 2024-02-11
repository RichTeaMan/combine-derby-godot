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

    /// <summary>
    /// Hides all visible descendant nodes, returning ones that were changed.
    /// </summary>
    /// <returns></returns>
    public static Node[] HideVisibleNodeChildrenRecursive(this Node node)
    {
        var result = new List<Node>();

        foreach (var n in node.ChildrenRecursive())
        {
            if (n is Node3D node3d)
            {
                node3d.Visible = false;
                result.Add(n);
            }
            if (n is Node2D node2d)
            {
                node2d.Visible = false;
                result.Add(n);
            }
            if (n is Control control)
            {
                control.Visible = false;
                result.Add(n);
            }
        }
        return result.ToArray();
    }

    public static void RestoreVisibleNodes(this Node[] nodes)
    {
        foreach (var n in nodes)
        {
            if (n is Node3D node3d)
            {
                node3d.Visible = true;
            }
            if (n is Node2D node2d)
            {
                node2d.Visible = true;
            }
            if (n is Control control)
            {
                control.Visible = true;
            }
        }
    }
}
