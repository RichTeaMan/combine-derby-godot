using Godot;

public partial class Bull : RigidBody3D
{

    private AudioStreamPlayer3D Sound => GetNode<AudioStreamPlayer3D>("%sound");

    public override void _Ready()
    {
        base._Ready();
        BodyEntered += bull_body_entered;
    }

    private void bull_body_entered(Node body)
    {
        if (!Sound.IsPlaying() && GlobalCs.Current.IsInVehicleGroup(body))
        {
            Sound.Play();
            GlobalCs.Current.DoVehiclePickup(((CdVehicle)body).PlayerId, "Bull bounces", 1);
        }
    }
}
