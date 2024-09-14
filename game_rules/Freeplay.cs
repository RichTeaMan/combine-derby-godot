using Godot;

public partial class Freeplay : Node3D
{
	public override void _EnterTree()
	{
		var arena = GD.Load<PackedScene>("res://arenas/crash.tscn");
		AddChild(arena.Instantiate<Node3D>());
	}
}
