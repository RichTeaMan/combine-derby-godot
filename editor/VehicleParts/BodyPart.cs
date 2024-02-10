using System;
using System.Collections.Generic;
using Godot;

public class BodyPart : VehiclePart
{

    public IReadOnlyList<WheelAnchor> WheelAnchors => _wheelAnchors.AsReadOnly();

    private List<WheelAnchor> _wheelAnchors = new();

    /// <summary>
    /// The mass, in kg.
    /// </summary>
    public float Mass { get; set; } = 100.0f;

    public BodyPart(string name, string scenePath) : base(name, scenePath)
    {
        PartType = PartType.Body;
    }

    public void AddWheelAnchor(Vector3 anchorPoint, bool isTraction, bool isSteering)
    {
        var anchor = new WheelAnchor(anchorPoint, isTraction, isSteering);
        _wheelAnchors.Add(anchor);
    }

    public static List<BodyPart> BodyPartsInit()
    {
        var parts = new List<BodyPart>();

        // silo
        var silo = new BodyPart("Silo", "res://assets/parts/body/silo-body.tscn");
        silo.AddWheelAnchor(new Vector3(1.5f, -0.8f, 3.0f), false, true);
        silo.AddWheelAnchor(new Vector3(1.5f, -0.8f, -3.0f), true, false);
        silo.AddWheelAnchor(new Vector3(-1.5f, -0.8f, 3.0f), false, true);
        silo.AddWheelAnchor(new Vector3(-1.5f, -0.8f, -3.0f), true, false);
        parts.Add(silo);

        // box
        var box = new BodyPart("Box", "res://assets/parts/body/box-body.tscn");
        box.AddWheelAnchor(new Vector3(1.8f, -0.9f, 3.5f), false, true);
        box.AddWheelAnchor(new Vector3(1.8f, -0.9f, -3.5f), true, false);
        box.AddWheelAnchor(new Vector3(-1.8f, -0.9f, 3.5f), false, true);
        box.AddWheelAnchor(new Vector3(-1.8f, -0.9f, -3.5f), true, false);
        parts.Add(box);

        return parts;
    }
}
