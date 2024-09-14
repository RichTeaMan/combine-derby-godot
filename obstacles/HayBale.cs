using Godot;

public partial class HayBale : RigidBody3D
{
    private PackedScene bale_explode_template;

    public override void _Ready()
    {
        base._Ready();
        bale_explode_template = GD.Load<PackedScene>("res://obstacles/hay_bale_explode.tscn");
        BodyEntered += hay_body_entered;
    }

    private void hay_body_entered(Node body)
    {
        if (GlobalCs.Current.IsInVehicleGroup(body))
        {
            GlobalCs.Current.DoVehiclePickup(((CdVehicle)body).PlayerId, "Hay bales", 1);

            // create hay bale particle emitter
            var explode_instance = bale_explode_template.Instantiate<Node3D>();
            var current_transform = GlobalTransform;
            explode_instance.Transform = current_transform;
            GetTree().GetRoot().AddChild(explode_instance);

            // delete hay bale
            QueueFree();
        }
    }
}
