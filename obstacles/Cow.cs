using Godot;

public partial class Cow : RigidBody3D
{

    private AudioStreamPlayer3D Sound => GetNode<AudioStreamPlayer3D>("%sound");

    public override void _Ready()
    {
        base._Ready();
        BodyEntered += cow_body_entered;
    }

    private void cow_body_entered(Node body)
    {
        if (!Sound.IsPlaying() && GlobalCs.Current.IsInVehicleGroup(body))
        {
            Sound.Play();
            GlobalCs.Current.DoVehiclePickup(((CdVehicle)body).PlayerId, "Cow bounces", 1);
        }
    }
}
