using System;
using System.Collections.Generic;
using Godot;

public class WheelPart : VehiclePart
{

    public WheelPart(string name, string scenePath) : base(name, scenePath)
    {
        PartType = PartType.Wheels;
    }

    public static List<WheelPart> WheelPartsInit()
    {
        var parts = new List<WheelPart>();

        // basic wheel
        var basicWheel = new WheelPart("Basic Wheel", "res://assets/parts/body/basic-wheel.tscn");
        parts.Add(basicWheel);

        return parts;
    }
}
