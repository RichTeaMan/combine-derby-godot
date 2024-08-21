using System;
using System.Collections.Generic;
using Godot;

public class WheelPart : VehiclePart
{

    public float Radius { get; protected set; } = 0.5f;

    public WheelPart(string name, string scenePath) : base(name, scenePath)
    {
        PartType = PartType.Wheels;
    }

    public static List<WheelPart> WheelPartsInit()
    {
        var parts = new List<WheelPart>();

        // basic wheel
        var basicWheel = new WheelPart("Basic Wheel", "res://assets/parts/wheels/basic-wheel.tscn")
        {
            Description = "Yup. They're round.",
            Mass = 20.0f,
            Radius = 0.45f,
        };
        parts.Add(basicWheel);

        // disc wheel
        var discWheel = new WheelPart("Debug Wheel", "res://assets/parts/wheels/debug-wheel.tscn")
        {
            Description = "Only for debugging. Hopefully.",
            Mass = 10.0f,
            Radius = 1.0f,
        };
        parts.Add(discWheel);

        return parts;
    }
}
