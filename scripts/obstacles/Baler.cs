using System.Linq;
using Godot;

public partial class Baler : StaticBody3D
{

	private const int BALE_LIMIT = 1000;

	private int StoredWheat = 0;

	private float BaleRotation = Mathf.Pi * 0.5f;
	private float BaleHeight = 1.20f;
	private int SpawnHeight = 16;

	private PackedScene BaleTemplate;

	public override void _Ready()
	{
		BaleTemplate = GD.Load<PackedScene>("res://obstacles/hay_bale.tscn");
	}

	public void DepositWheat(int wheat)
	{
		StoredWheat += wheat;
		var bales_to_spawn = 0;
		while (StoredWheat >= BALE_LIMIT)
		{
			bales_to_spawn += 1;
			StoredWheat -= BALE_LIMIT;
		}
		GD.Print($"Created {bales_to_spawn} bales");
		var spawn_point = Position + new Vector3(0, SpawnHeight + BaleHeight, 0);
		foreach (var i in Enumerable.Range(0, bales_to_spawn))
		{
			var baleInstance = BaleTemplate.Instantiate<Node3D>();
			//bale_instance.rotate_z(bale_rotation)
			baleInstance.Position = spawn_point;
			GetParentNode3D().AddChild(baleInstance);
			GD.Print(baleInstance.Position);
			spawn_point += new Vector3(0, BaleHeight * 1.25f, 0);
		}
	}
}
