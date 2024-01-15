using System;
using Godot;

public partial class VehicleCameraPivot : Node3D
{

    [Export(PropertyHint.Range, "1,50,0.1")]
    public float SmoothSpeed { get; set; } = 7.0f;

    [Export]
    public Vector3 CameraRotation { get; set; } = new(Mathf.DegToRad(-10.2f), Mathf.DegToRad(-180.0f), Mathf.DegToRad(0.0f));

    [Export]
    public Vector3 CameraPosition { get; set; } = new(0.0f, 5.0f, -8.0f);

    private VehicleBody3D parentVehicle;

    private Vector3 direction;

    public override void _Ready()
    {
        direction = GetParentNode3D().Rotation;
        parentVehicle = FindParentVehicle();
        AddChild(CreateCamera());
    }

    public override void _PhysicsProcess(double delta)
    {

        var vehicle_basis = parentVehicle.GlobalTransform.Basis;
        var current_velocity = parentVehicle.LinearVelocity;
        current_velocity.Y = 0.0f;
        if (current_velocity.LengthSquared() > 1.0f)
        {
            direction = direction.Lerp(-current_velocity.Normalized(), SmoothSpeed * (float)delta);
            // math hack to prevent camera from looking at the front of the vehicle when reversing
            // there must be a better way but I don't know what it is.
            // mostly works, but camera does shudder with crashes or other sudden movements
            var newBasis = FetchRotationFromDirection(direction, vehicle_basis.Tdotz(-direction.Normalized()) < 0.0);
            var newGlobalTransform = GlobalTransform;
            newGlobalTransform.Basis = newBasis;
            GlobalTransform = newGlobalTransform;
        }
    }

    private VehicleBody3D FindParentVehicle()
    {
        Node vehicle = GetParentNode3D();
        while (vehicle is not VehicleBody3D)
        {
            vehicle = vehicle.GetParent();
        }
        return (VehicleBody3D)vehicle;
    }

    private Basis FetchRotationFromDirection(Vector3 lookDirection, bool isReversing)
    {
        lookDirection = lookDirection.Normalized();
        if (isReversing)
        {
            lookDirection = -lookDirection;
        }
        var x_axis = lookDirection.Cross(Vector3.Up);
        return new Basis(x_axis, Vector3.Up, -lookDirection);
    }

    private Camera3D CreateCamera()
    {

        var camera = new Camera3D
        {
            Position = CameraPosition,
            Rotation = CameraRotation,
        };
        return camera;
    }
}
