using Godot;
using System;
using System.Linq;

public partial class DisplayPodium : Node3D
{

    [Export]
    public float AngularSpeed { get; set; } = 0.5f;

    private Node3D gimbalNode => GetNode<Node3D>("%gimbal");

    private Node3D displayNode => GetNode<Node3D>("%display_node");

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        gimbalNode.RotateY((float)(AngularSpeed * delta));
    }

    public void AddNodeToPodium(Node3D node)
    {
        var position = node.Position;
        // TODO: set this better so small vehicles don't float and large ones don't intersect the stage
        position.Y = 1.0f;
        node.Position = position;
        displayNode.AddChild(node);
    }

    public void ClearPodium()
    {
        displayNode.GetChildren().ToList().ForEach(n => n.QueueFree());
    }
}
