using Godot;

public partial class Chicken : RigidBody3D
{

    private AudioStreamPlayer3D Sound => GetNode<AudioStreamPlayer3D>("%sound");

    public override void _Ready()
    {
        base._Ready();
        BodyEntered += chicken_body_entered;
    }

    private void chicken_body_entered(Node body)
    {
        if (!Sound.IsPlaying() && GlobalCs.Current.IsInVehicleGroup(body))
        {
            Sound.Play();
            GlobalCs.Current.DoVehiclePickup(((CdVehicle)body).PlayerId, "Chickens", 1);
        }
    }
}
