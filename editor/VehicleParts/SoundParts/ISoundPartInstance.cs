using Godot;

public interface ISoundPartInstance
{
    float Acceleration { get; set; }

    void StartEngineNoise();

    void StopEngineNoise();

    void PlayCollisionNoise(Vector3 collisionForce);

    void PlayLargeCollisionNoise();

    public void AddSoundNode(Node3D parentNode);
}