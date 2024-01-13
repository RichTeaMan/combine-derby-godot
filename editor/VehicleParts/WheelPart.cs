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
        var basicWheel = new WheelPart("Basic Wheel", "res://assets/parts/wheels/basic-wheel.tscn");
        parts.Add(basicWheel);

        // disc wheel
        var discWheel = new WheelPart("Disc Wheel", "res://assets/parts/wheels/disc-wheel.tscn");
        parts.Add(discWheel);

        return parts;
    }
}
