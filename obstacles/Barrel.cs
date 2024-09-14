using Godot;

public partial class Barrel : RigidBody3D
{

    private CpuParticles3D ExplosionParticles => GetNode<CpuParticles3D>("%explosion_particles");
    private AudioStreamPlayer3D ExplosionSound => GetNode<AudioStreamPlayer3D>("%explosion_sound");

    private Node3D Model => GetNode<Node3D>("%model");

    private CpuParticles3D SmokeParticles => GetNode<CpuParticles3D>("%smoke_particles");


    private bool exploded = false;

    public override void _Ready()
    {
        base._Ready();
        BodyEntered += barrel_body_entered;
    }

    private void barrel_body_entered(Node body)
    {
        if (!exploded && GlobalCs.Current.IsInVehicleGroup(body))
        {
            GlobalCs.Current.DoVehiclePickup(((CdVehicle)body).PlayerId, "Barrel booms", 1);
            exploded = true;

            // start explosion particle emitter
            ExplosionParticles.OneShot = true;
            ExplosionParticles.Emitting = true;
            ExplosionSound.Playing = true;
            Model.Visible = false;
            SmokeParticles.Emitting = false;
        }
    }

    public override void _Process(double _delta)
    {
        if (exploded && !ExplosionParticles.Emitting && !ExplosionSound.Playing)
        {
            // delete barrel
            QueueFree();
        }
    }
}
