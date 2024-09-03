using Godot;

public class PartTransform<T> where T : VehiclePart
{
    public T VehiclePart { get; set; }
    public Transform3D Transform { get; set; }
}
