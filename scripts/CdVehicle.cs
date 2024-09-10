using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class CdVehicle : VehicleBody3D
{

    private const float UPSIDE_DOWN_ANGLE = (float)Math.PI * 0.75f;
    private static float UPSIDE_DOWN_FRAMES_LIMIT = Engine.PhysicsTicksPerSecond * 2.0f;

    public CdVehicleModel Model { get; set; } = new CdVehicleModel();

    [Export]
    public int PlayerId { get; set; } = 1;

    /// <summary>
    /// Compaitibilty for older GD script that still uses pre C# code.
    /// </summary>
    [Obsolete]
    public int player_id => PlayerId;

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

    private float SteeringAngle = 0.0f;

    private VehicleWheel3D[] TractionWheels = Array.Empty<VehicleWheel3D>();

    private VehicleWheel3D[] SteeringWheels = Array.Empty<VehicleWheel3D>();

    private ISoundPartInstance SoundPart = null;

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
        base._Ready();
        AddToGroup("vehicle");
        SteeringLeftInput = $"player{PlayerId}_left";
        SteeringRightInput = $"player{PlayerId}_right";
        ForwardInput = $"player{PlayerId}_forward";
        BackInput = $"player{PlayerId}_back";

        ContactMonitor = true;
        MaxContactsReported = 4;

        BodyShapeEntered += _onVehicleBodyShapeEntered;

        UnpackBodyScene();
        SoundPart?.StartEngineNoise();
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
        SteeringAngle = Mathf.Lerp(SteeringAngle, Input.GetAxis(SteeringLeftInput, SteeringRightInput) * -0.4f, 2.0f * (float)delta);
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
        foreach (var wheel in SteeringWheels)
        {
            if (wheel.Position.Z > 0.0f)
            {
                wheel.Steering = SteeringAngle;
            }
            else
            {
                wheel.Steering = -SteeringAngle;
            }
        }
        // TODO
        // Global.update_speed(player_id, basis.tdotz(get_linear_velocity()))

        if (SoundPart != null)
        {
            SoundPart.Acceleration = acceleration;
        }

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
        SoundPart?.PlayCollisionNoise(collisionForce);
    }

    public void _onVehicleBodyShapeEntered(Rid _body_rid, Node body, long _body_shape_index, long _local_shape_index)
    {
        // integrate forces seem to miss some collision (usually static bodies, but not always)
        // this seems to find the rest of them. big crashes are assumed
        SoundPart?.PlayLargeCollisionNoise();
        //Global.do_vehicle_body_shape_entered(player_id, body)
    }

    public void RebuildFromParts(Dictionary<string, VehiclePart> parts)
    {

        GetChildren().ToList().ForEach(c => c.Free());
        float mass = 0;

        BodyPart body = null;
        SoundPart = null;
        if (Model.BodyId != null)
        {
            body = (BodyPart)parts[Model.BodyId];
            mass += body.Mass;
            var bodyScene = body.InstantiateScene();
            SoundPart = body.SoundPart?.Instantiate();
            AddChild(bodyScene);
            UnpackBodyScene();

            if (Model.WheelsId != null)
            {
                var wheelPart = (WheelPart)parts[Model.WheelsId];
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
            var accessoryTransform = (AccessoryPart)parts[accessoryPartTransform.PartId];
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

    private void UnpackBodyScene()
    {
        if (!IsInsideTree())
        {
            return;
        }
        // colliders only work when they're a direct child of a character node, so move contents of 'collisions' up
        var collisionNode = GetChild(0)?.FindChild("collisions");
        if (collisionNode != null)
        {
            GD.Print("collisions node found.");
            int reparents = 0;
            foreach (var sub in collisionNode.GetChildren())
            {
                // TODO: causes warnings if the vehicle isn't yet in the scene tree.
                sub.Reparent(this);
                reparents++;
            }
            GD.Print($"Reparented {reparents} nodes.");
        }
        else
        {
            GD.Print($"collisions node not found. {GetChild(0)?.Name}/collisions");
        }

        if (SoundPart != null)
        {
            SoundPart.AddSoundNode(this);
        }
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