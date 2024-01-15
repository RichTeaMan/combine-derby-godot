using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class CdVehicle : VehicleBody3D
{

    private const float UPSIDE_DOWN_ANGLE = (float)Math.PI * 0.75f;
    private static float UPSIDE_DOWN_FRAMES_LIMIT = Engine.PhysicsTicksPerSecond * 2.0f;

    [Export]
    public int PlayerId { get; set; } = 1;

    [Export]
    public float MaxRpm { get; set; } = 500.0f;

    [Export]
    public float MaxTorque { get; set; } = 2000.0f;

    [Export]
    public float IdleSoundDb { get; set; } = 0.0f;

    [Export]
    public float AcceleratingSoundDb { get; set; } = 6.0f;

    public Camera3D Camera => this.ChildrenRecursive().FirstOrDefault(c => c is Camera3D) as Camera3D;

    private int UpsideDownFrames = 0;

    private string SteeringLeftInput;
    private string SteeringRightInput;
    private string ForwardInput;
    private string BackInput;

    private VehicleWheel3D[] TractionWheels = Array.Empty<VehicleWheel3D>();

    private VehicleWheel3D[] SteeringWheels = Array.Empty<VehicleWheel3D>();

    /// <summary>
    /// Changes rotation so the combine is on its wheels. 
    /// </summary>
    public void RollToWheels()
    {
        Rotation = new Vector3(Rotation.X, Rotation.Y, 0.0f);
    }

    /// <summary>
    /// Rebuild wheel positions using VehicleWheel3D that are children of this node.
    /// To be used by the editor.
    /// </summary>
    public void RebuildWheels()
    {
        var tractionWheels = new List<VehicleWheel3D>();
        var steeringWheels = new List<VehicleWheel3D>();

        var childNodes = Utils.ChildrenRecursive(this);
        foreach (var node in childNodes)
        {
            GD.Print(node);
            if (node is VehicleWheel3D)
            {
                var wheel = node as VehicleWheel3D;
                if (wheel.UseAsTraction)
                {
                    tractionWheels.Add(wheel);
                }
                if (wheel.UseAsSteering)
                {
                    steeringWheels.Add(wheel);
                }
            }
        }
        TractionWheels = tractionWheels.ToArray();
        SteeringWheels = steeringWheels.ToArray();
        GD.Print($"Found {steeringWheels.Count} steering wheels, {tractionWheels.Count} traction wheels.");
    }

    public void RebuildCamera()
    {
        foreach (var node in this.ChildrenRecursive())
        {
            if (node is VehicleCameraPivot)
            {
                node.QueueFree();
            }
        }
        AddChild(new VehicleCameraPivot());
    }

    public override void _Ready()
    {
        SteeringLeftInput = $"player{PlayerId}_left";
        SteeringRightInput = $"player{PlayerId}_right";
        ForwardInput = $"player{PlayerId}_forward";
        BackInput = $"player{PlayerId}_back";
        //$sound.play()
    }

    public override void _PhysicsProcess(double delta)
    {
        Steering = Mathf.Lerp(Steering, Input.GetAxis(SteeringRightInput, SteeringLeftInput) * -0.4f, 2.0f * (float)delta);
        var acceleration = Input.GetAxis(BackInput, ForwardInput);

        foreach (var wheel in TractionWheels)
        {
            if (!IsInstanceValid(wheel) || wheel.IsQueuedForDeletion())
            {
                continue;
            }
            var rpm = Math.Abs(wheel.GetRpm());
            wheel.EngineForce = acceleration * MaxTorque * (1 - rpm / MaxRpm);
        }

        // TODO
        // Global.update_speed(player_id, basis.tdotz(get_linear_velocity()))

        //if acceleration != 0:
        //	$sound.volume_db = accelerating_sound_db
        //else:
        //	$sound.volume_db = idle_sound_db

        if (Rotation.Z > UPSIDE_DOWN_ANGLE || Rotation.Z < -UPSIDE_DOWN_ANGLE)
        {

            UpsideDownFrames += 1;
            if (UpsideDownFrames >= UPSIDE_DOWN_FRAMES_LIMIT)
            {
                RollToWheels();
            }
        }
        else
        {
            UpsideDownFrames = 0;
        }
        if (Input.IsActionJustPressed("reset"))
        {
            RollToWheels();
        }
    }

    public bool IsReversing()
    {
        // speed that will go negative if reversing
        var speed = Basis.Tdotz(LinearVelocity);
        return speed < 0.0f;
    }

    public override void _IntegrateForces(PhysicsDirectBodyState3D state)
    {
        if (state.GetContactCount() == 0)
        {
            return;
        }
        var collisionForce = Vector3.Zero;
        foreach (var i in Enumerable.Range(0, state.GetContactCount()))
        {
            collisionForce += state.GetContactImpulse(i) * state.GetContactLocalNormal(i);
        }
        if (collisionForce != Vector3.Zero)
        {
            //GD.Print($"combine collsion force {collisionForce.length_squared()}");
        }
        //if (collision_force.length_squared() > 30000.0)
        //	crash_sounds.play_big_sound()
        //if (collision_force.length_squared() > 100.0)
        //	crash_sounds.play_small_sound()
    }


    //public override void _on_vehicle_body_shape_entered(Rid _body_rid, Node body, int _body_shape_index,int _local_shape_index){

    // integrate forces seem to miss some collision (usually static bodies, but not always)
    // this seems to find the rest of them. big crashes are assumed
    //crash_sounds.play_big_sound()
    //Global.do_vehicle_body_shape_entered(player_id, body)
    //}

}