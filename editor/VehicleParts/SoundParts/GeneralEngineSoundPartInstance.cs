using System;
using System.Linq;
using Godot;

public abstract partial class GeneralEngineSoundPartInstance : Node3D, ISoundPartInstance
{
    private Random random = new Random();

    private AudioStreamPlayer3D[] smallNoises = null;

    private AudioStreamPlayer3D[] largeNoises = null;

    private AudioStreamPlayer3D engineNoise = null;

    private bool smallSoundPlaying = false;

    private bool largeSoundPlaying = false;

    public float IdleSoundDb { get; set; } = 0.0f;
    public float AcceleratingSoundDb { get; set; } = 6.0f;

    private float acceleration;

    public float Acceleration
    {
        get => acceleration;
        set
        {
            acceleration = value;
            if (!IsInsideTree() || engineNoise == null)
            {
                return;
            }
            if (acceleration != 0.0f)
            {
                engineNoise.VolumeDb = AcceleratingSoundDb;
            }
            else
            {
                engineNoise.VolumeDb = IdleSoundDb;
            }
        }
    }

    public float LargeCollisionNoiseForceLimitSquared { get; set; } = 30_000.0f;
    public float SmallCollisionNoiseForceLimitSquared { get; set; } = 100.0f;

    public abstract string[] FetchSmallNoiseResourcePaths();

    public abstract string[] FetchLargeNoiseResourcePaths();

    public abstract string FetchEngineNoiseResourcePath();

    public override void _Ready()
    {
        base._Ready();

        smallNoises = CreateAudioStreamPlayerNodes(FetchSmallNoiseResourcePaths());
        largeNoises = CreateAudioStreamPlayerNodes(FetchLargeNoiseResourcePaths());
        engineNoise = CreateAudioStreamPlayerFromResourcePath(FetchEngineNoiseResourcePath());

        foreach (var node in smallNoises)
        {
            AddChild(node);
            node.UnitSize = 1;
            node.Finished += () => { smallSoundPlaying = false; };
        }
        foreach (var node in largeNoises)
        {
            AddChild(node);
            node.UnitSize = 1;
            node.Finished += () => { largeSoundPlaying = false; };
        }
        AddChild(engineNoise);

    }

    private AudioStreamPlayer3D CreateAudioStreamPlayerFromResourcePath(string resourcePath)
    {
        var node = new AudioStreamPlayer3D()
        {
            Stream = GD.Load<AudioStream>(resourcePath),
            Bus = "sfx"
        };
        return node;
    }

    private void PlayRandomSoundNode(AudioStreamPlayer3D[] nodeArray)
    {
        if (nodeArray == null)
        {
            return;
        }
        var nodeId = random.Next() % nodeArray.Count();
        nodeArray[nodeId]?.Play();
    }

    private AudioStreamPlayer3D[] CreateAudioStreamPlayerNodes(string[] resourcePaths)
    {
        return resourcePaths.Select(path => CreateAudioStreamPlayerFromResourcePath(path)).ToArray();
    }

    public void PlayCollisionNoise(Vector3 collisionForce)
    {
        if (!IsInsideTree())
        {
            return;
        }
        if (collisionForce.LengthSquared() > LargeCollisionNoiseForceLimitSquared)
        {
            if (!largeSoundPlaying)
            {
                PlayRandomSoundNode(largeNoises);
                largeSoundPlaying = true;
            }
        }

        else if (collisionForce.LengthSquared() > SmallCollisionNoiseForceLimitSquared)
        {
            if (!smallSoundPlaying)
            {
                PlayRandomSoundNode(smallNoises);
                smallSoundPlaying = true;
            }
        }
    }

    public void PlayLargeCollisionNoise()
    {
        if (!IsInsideTree())
        {
            return;
        }
        if (!largeSoundPlaying)
        {
            PlayRandomSoundNode(largeNoises);
            largeSoundPlaying = true;
        }
    }

    public void StartEngineNoise()
    {
        if (!IsInsideTree())
        {
            return;
        }
        engineNoise?.Play();
    }

    public void StopEngineNoise()
    {
        if (!IsInsideTree())
        {
            return;
        }
        engineNoise?.Stop();
    }

    public void AddSoundNode(Node3D parentNode)
    {
        GD.Print("ENGINE 1 SOUNDS ADDED");
        parentNode.AddChild(this);
    }
}
