using Godot;

public partial class HayBaleExplode : Node3D
{

    private CpuParticles3D CPUParticles3D => GetNode<CpuParticles3D>("CPUParticles3D");

    public override void _Ready()
    {
        CPUParticles3D.OneShot = true;
    }

    public override void _PhysicsProcess(double _delta)
    {
        if (!CPUParticles3D.Emitting)
        {
            QueueFree();
        }
    }
}
