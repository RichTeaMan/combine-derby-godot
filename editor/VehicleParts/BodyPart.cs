using System;
using System.Collections.Generic;
using Godot;

public class BodyPart : VehiclePart
{

    public IReadOnlyList<WheelAnchor> WheelAnchors => _wheelAnchors.AsReadOnly();

    private List<WheelAnchor> _wheelAnchors = new();

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
        silo.AddWheelAnchor(new Vector3(1.5f, -0.8f, 3.0f), true, true);
        silo.AddWheelAnchor(new Vector3(1.5f, -0.8f, -3.0f), true, true);
        silo.AddWheelAnchor(new Vector3(-1.5f, -0.8f, 3.0f), true, true);
        silo.AddWheelAnchor(new Vector3(-1.5f, -0.8f, -3.0f), true, true);
        parts.Add(silo);

        // box
        var box = new BodyPart("Box", "res://assets/parts/body/box-body.tscn");
        box.AddWheelAnchor(new Vector3(1.8f, -0.9f, 3.5f), true, true);
        box.AddWheelAnchor(new Vector3(1.8f, -0.9f, -3.5f), true, true);
        box.AddWheelAnchor(new Vector3(-1.8f, -0.9f, 3.5f), true, true);
        box.AddWheelAnchor(new Vector3(-1.8f, -0.9f, -3.5f), true, true);
        parts.Add(box);

        return parts;
    }
}
