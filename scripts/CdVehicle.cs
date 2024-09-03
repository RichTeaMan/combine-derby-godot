using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using Godot;

public partial class CdVehicle : VehicleBody3D
{

    private const float UPSIDE_DOWN_ANGLE = (float)Math.PI * 0.75f;
    private static float UPSIDE_DOWN_FRAMES_LIMIT = Engine.PhysicsTicksPerSecond * 2.0f;

    public CdVehicleModel Model { get; set; } = new CdVehicleModel();

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

    public Camera3D Camera => this.ChildrenRecursive().FirstOrDefault(c => c is Camera3D && c != FreeCamera) as Camera3D;

    private int UpsideDownFrames = 0;

    private string SteeringLeftInput;
    private string SteeringRightInput;
    private string ForwardInput;
    private string BackInput;

    private VehicleWheel3D[] TractionWheels = Array.Empty<VehicleWheel3D>();

    private VehicleWheel3D[] SteeringWheels = Array.Empty<VehicleWheel3D>();

    private Node3D CameraGimbal;

    private Camera3D FreeCamera;

    private bool rotating = false;


    private Vector2 prevMousePosition;
    private Vector2 nextMousePosition;
    private float rotation_constant = 0.5f;
    private float zoom_constant = 0.2f;

    public enum VehicleCameraMode
    {
        FREE_ROTATE,
        CHASE
    }

    private VehicleCameraMode _cameraMode = VehicleCameraMode.CHASE;
    public VehicleCameraMode CameraMode
    {
        get
        {
            return _cameraMode;
        }
        set
        {
            _cameraMode = value;
            if (_cameraMode == VehicleCameraMode.FREE_ROTATE)
            {
                FreeCamera.Current = true;
            }
            else
            {
                Camera.Current = true;
            }
        }
    }

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
        CameraGimbal = new Node3D();
        AddChild(CameraGimbal);
        FreeCamera = new Camera3D();
        CameraGimbal.AddChild(FreeCamera);
        FreeCamera.Position = new Vector3(0.0f, 0.0f, 20.0f);
        CameraGimbal.RotateY(Mathf.DegToRad(180.0f));
    }

    public override void _Ready()
    {
        SteeringLeftInput = $"player{PlayerId}_left";
        SteeringRightInput = $"player{PlayerId}_right";
        ForwardInput = $"player{PlayerId}_forward";
        BackInput = $"player{PlayerId}_back";


        //$sound.play()
    }

    public override void _UnhandledInput(InputEvent _inputEvent)
    {
        if (Input.IsActionJustPressed("rotate"))
        {
            rotating = true;
            prevMousePosition = GetViewport().GetMousePosition();
        }
        if (Input.IsActionJustReleased("rotate"))
        {
            rotating = false;
        }
        if (Input.IsActionJustPressed("change_camera_mode"))
        {
            GD.Print("Camera Mode");
            if (CameraMode == VehicleCameraMode.CHASE)
            {
                CameraMode = VehicleCameraMode.FREE_ROTATE;
            }
            else
            {
                CameraMode = VehicleCameraMode.CHASE;
            }
        }
    }

    public override void _Process(double delta)
    {
        if (rotating && CameraGimbal != null)
        {
            nextMousePosition = GetViewport().GetMousePosition();
            CameraGimbal.RotateY((nextMousePosition.X - prevMousePosition.X) * rotation_constant * (float)delta);
            CameraGimbal.RotateX((nextMousePosition.Y - prevMousePosition.Y) * rotation_constant * (float)delta);
            prevMousePosition = nextMousePosition;
        }

        if (FreeCamera != null)
        {
            if (Input.IsActionJustPressed("zoom_in"))
            {
                FreeCamera.Position = new Vector3(FreeCamera.Position.X, FreeCamera.Position.Y, FreeCamera.Position.Z - zoom_constant);
            }

            if (Input.IsActionJustPressed("zoom_out"))
            {
                FreeCamera.Position = new Vector3(FreeCamera.Position.X, FreeCamera.Position.Y, FreeCamera.Position.Z + zoom_constant);
            }
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        Steering = Mathf.Lerp(Steering, Input.GetAxis(SteeringLeftInput, SteeringRightInput) * -0.4f, 2.0f * (float)delta);
        var acceleration = Input.GetAxis(BackInput, ForwardInput);

        foreach (var wheel in TractionWheels)
        {
            if (!IsInstanceValid(wheel) || wheel.IsQueuedForDeletion())
            {
                continue;
            }
            var rpm = Math.Abs(wheel.GetRpm());
            wheel.EngineForce = acceleration * MaxTorque * (1.0f - rpm / MaxRpm);
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

    public void RebuildFromParts(VehiclePart[] parts)
    {

        GetChildren().ToList().ForEach(c => c.Free());
        float mass = 0;

        BodyPart body = null;
        if (Model.BodyId != null)
        {
            body = (BodyPart)parts.Single(p => p.Name == Model.BodyId);
            mass += body.Mass;
            AddChild(body.InstantiateScene());


            if (Model.WheelsId != null)
            {
                var wheelPart = (WheelPart)parts.Single(p => p.Name == Model.WheelsId);
                foreach (var wheelAnchor in body.WheelAnchors)
                {
                    var wheel = new VehicleWheel3D();
                    var instance = wheelPart.InstantiateScene();
                    wheel.AddChild(instance);
                    wheel.Position = wheelAnchor.AttachmentPoint;
                    instance.Rotation = wheelAnchor.BaseRotation;
                    wheel.UseAsSteering = wheelAnchor.IsSteering;
                    wheel.UseAsTraction = wheelAnchor.IsTraction;
                    wheel.SuspensionStiffness = 50.0f;
                    wheel.WheelRadius = wheelPart.Radius;

                    AddChild(wheel);
                    mass += wheelPart.Mass;
                }
            }
        }
        foreach (var accessoryPartTransform in Model.Accessories)
        {
            var accessoryTransform = (AccessoryPart)parts.Single(p => p.Name == accessoryPartTransform.PartId);
            var instance = accessoryTransform.InstantiateScene();
            AddChild(instance);
            instance.Transform = accessoryPartTransform.Transform;
            mass += accessoryTransform.Mass;
        }

        RebuildWheels();
        CenterOfMassMode = CenterOfMassModeEnum.Custom;
        CenterOfMass = body.CalculateCenterOfMass();
        Mass = mass;
        MaxRpm = body.BaseRpm;
        MaxTorque = body.BaseTorque;
    }

    public class CdVehicleModel
    {
        public string Name { get; set; }
        public string BodyId { get; set; }
        public string WheelsId { get; set; }
        public List<CdVehicleModelTransform> Accessories { get; set; } = new List<CdVehicleModelTransform>();
    }

    public class CdVehicleModelTransform
    {
        public string PartId { get; set; }
        public Transform3D Transform { get; set; }
    }

}